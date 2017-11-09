using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Spectralizer.SIL.persistence
{
    public class Block : IComparable
    {
        public static string keyWords = "alignas|alignof|asm|auto|bool|break|case|catch|char|char16_t|char32_t|class|const|constexpr|const_cast|continue|decltype|default|delete|do|double|dynamic_cast|else|enum|explicit|export|extern|false|float|for|friend|goto|if|inline|int|long|mutable|namespace|new|noexcept|nullptr|operator|private|protected|public|register|reinterpret_cast|return|short|signed|sizeof|static|static_assert|static_cast|struct|switch|template|this|thread_local|throw|true|try|typedef|typeid|typename|union|unsigned|using|virtual|void|volatile|wchar_t|while";

        private Dictionary<int, string> lines;
        private string name;
        private string sourceFileTheBlockBelongsTo;
        private string typeOfSourceFile = "";
        private int lineOfDefinition;    // eigt. irgendwie unnoetig, aber ich modelliere es vorerst mal so
        private Dictionary<int, List<InvokedFunction>> listOfBlocksInLine; // key is a line number, behind all these linenumbers in this Dictionary is the invokation of a function
        private static SemaphoreSlim semaphorExport = new SemaphoreSlim(1);

        public Block(string name, string sourceFile, int lineOfDefinition, Dictionary<int, string> linesOfCode)
        {
            this.lines = linesOfCode;
            this.name = name;
            this.lineOfDefinition = lineOfDefinition;
            this.sourceFileTheBlockBelongsTo = sourceFile;

            if (sourceFileTheBlockBelongsTo.IndexOf(".cpp") >= 0)
            {
                sourceFileTheBlockBelongsTo = sourceFileTheBlockBelongsTo.Substring(0, sourceFileTheBlockBelongsTo.IndexOf(".cpp"));
                typeOfSourceFile = ".cpp";
            }
            else if (sourceFileTheBlockBelongsTo.IndexOf(".h") >= 0)
            {
                sourceFileTheBlockBelongsTo = sourceFileTheBlockBelongsTo.Substring(0, sourceFileTheBlockBelongsTo.IndexOf(".h"));
                typeOfSourceFile = ".h";
            }

            this.sourceFileTheBlockBelongsTo = this.sourceFileTheBlockBelongsTo.Trim();

            this.listOfBlocksInLine = new Dictionary<int, List<InvokedFunction>>();
            foreach (int linenumber in lines.Keys)
            {
                parseLine(lines[linenumber], linenumber);
            }
        }

        public int Entrance
        {
            get { return this.lines.Keys.Last(); }
        }

        public int Exit
        {
            get { return this.lines.Keys.First(); }
        }

        public string Name
        {
            get { return this.name; }
        }

        public string SourceFileName
        {
            get { return sourceFileTheBlockBelongsTo; }
        }

        public string ConcatenatedName
        {
            get { return this.sourceFileTheBlockBelongsTo + this.typeOfSourceFile + "_" + this.name + "_" + this.lineOfDefinition; }
        }

        public bool containsLineAndIsBlockInvokation(int linenumber)
        {
            if (listOfBlocksInLine.ContainsKey(linenumber))
            {
                return true;
            }

            return false;
        }

        public List<InvokedFunction> getListOfInvokedFuncsInLine(int line)
        {
            return listOfBlocksInLine[line];
        }

        private void parseLine(string line, int linenumber)
        {
            int lastIndexOfLeftParenthesis = 0;
            int indexOfLeftParenthesisPosition = 0;
            List<InvokedFunction> blockNames = new List<InvokedFunction>();

            // check if line contains a left parenthesis
            if (line.Contains('('))
            {
                int tmpIndex;
                while (!((tmpIndex = line.IndexOf('(', indexOfLeftParenthesisPosition + 1)) < 0))
                {
                    indexOfLeftParenthesisPosition = tmpIndex;

                    // new parseFunctionName with SourceFile if available
                    string[] block = parseBlockName(line.Substring(lastIndexOfLeftParenthesis, (indexOfLeftParenthesisPosition - lastIndexOfLeftParenthesis)));

                    string func; string srcFile; string word;

                    if (block.Count() == 2)
                    {
                        srcFile = block[1];
                        func = block[0];
                        word = func;
                    }
                    else if (block.Count() == 1)
                    {
                        srcFile = "";
                        func = block[0];
                        word = func;
                    }
                    else
                    {
                        srcFile = "";
                        func = block[0];
                        word = func;
                    }

                    lastIndexOfLeftParenthesis = indexOfLeftParenthesisPosition;
                    if (!keyWords.Contains(word.ToLower()))
                    {
                        List<string> parameters = parseBlockParameters(line, indexOfLeftParenthesisPosition);
                        blockNames.Add(new InvokedFunction(func, srcFile, parameters));
                    }
                }
            }
            if (blockNames.Count > 0)
            {
                listOfBlocksInLine.Add(linenumber, blockNames);
            }
            return;
        }

        private string[] parseBlockName(string text)
        {
            text = text.Trim();
            int endIndex = text.Length - 1;
            int index = endIndex;
            string word = "";
            string[] block;

            while (index >= 0)
            {
                if (!(Char.IsLetterOrDigit(text, index) || text[index] == '_' || text[index] == ':' /*|| text[index] == '.' */))
                {
                    word = text.Substring(index + 1, endIndex - index).Trim();

                    if (word.Contains("::"))
                    {
                        block = word.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
                        if (block.Length > 1)
                        {
                            string tmp = block[0];
                            block[0] = block[1];
                            block[1] = tmp;
                        }
                    }
                    else
                    {
                        block = new string[1];
                        block[0] = word;
                    }

                    return block;
                }
                index--;
            }

            word = text.Substring(0, endIndex + 1);

            if (word.Contains("::"))
            {
                block = word.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
                string tmp = block[0];
                if (block.Length > 1)
                {
                    block[0] = block[1];  // exception occured because of splitting the string 'text'function parameter. "::memset"
                    // by splitting this string the resulting array only has one element and so we cannot access the second element of this array function[1].
                    block[1] = tmp;
                }
            }
            else
            {
                block = new string[1];
                block[0] = word;
            }

            return block;
        }

        private List<string> parseBlockParameters(string text, int position)
        {
            List<string> parameters = new List<string>();

            bool inParameterList = false;
            bool inWord = false;
            int numberOpenedBrackets = 0;

            int length = text.Length;
            int index = position;
            int tmpWordBegin = position;

            while (index < length)
            {
                char letter = text[index];

                if (letter.Equals('('))
                {
                    inParameterList = true;
                    numberOpenedBrackets++;

                    if (inWord)
                    {
                        inWord = false;
                        string word = text.Substring(tmpWordBegin, index - tmpWordBegin);
                        parameters.Add(word);
                    }
                }

                if (letter.Equals(')'))
                {
                    numberOpenedBrackets--;

                    if (inWord)
                    {
                        inWord = false;
                        string word = text.Substring(tmpWordBegin, index - tmpWordBegin);
                        parameters.Add(word);
                    }
                }

                if (!(Char.IsLetterOrDigit(letter) || letter == '_') && inWord)
                {
                    inWord = false;
                    string word = text.Substring(tmpWordBegin, index - tmpWordBegin);
                    parameters.Add(word);
                }

                if (!inWord && (Char.IsLetter(letter) || letter == '_'))
                {
                    inWord = true;
                    tmpWordBegin = index;
                }

                if (inParameterList && (numberOpenedBrackets == 0))
                {
                    if (inWord)
                    {
                        string word = text.Substring(tmpWordBegin, index - tmpWordBegin);
                        parameters.Add(word);
                    }
                    break;
                }

                index++;
            }

            return parameters;
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            Block otherBlock = obj as Block;

            if (this.name.Equals(otherBlock.Name) && this.sourceFileTheBlockBelongsTo.Equals(otherBlock.SourceFileName)) return 0;

            return 1;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            Block func = obj as Block;

            return (this.name.Equals(func.Name, StringComparison.OrdinalIgnoreCase) && this.sourceFileTheBlockBelongsTo.Equals(func.SourceFileName, StringComparison.OrdinalIgnoreCase));
        }
    }
}
