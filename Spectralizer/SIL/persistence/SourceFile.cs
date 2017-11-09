using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using Spectralizer.SIL.persistence.cobertura;

namespace Spectralizer.SIL.persistence
{
    public class SourceFile
    {
        private Dictionary<int, string> linesOfSourceFile;
        private Dictionary<int, Block> functions;

        private SemaphoreSlim _Semaphore;
        private static SemaphoreSlim _Semaphore2 = new SemaphoreSlim(1);

        private string pathOfSoureFile;

        public SourceFile(String baseFilePath)
        {
            this.linesOfSourceFile = new Dictionary<int, string>();
            this.functions = new Dictionary<int, Block>();
            this._Semaphore = new SemaphoreSlim(1);
            this.pathOfSoureFile = baseFilePath;

            //Read the content of the base-sourcefile and remove the comments. All lines are stored in Dictionary linesOfSourceFile
            readSourceFileAndRemoveComments();
        }

        public Dictionary<Block, bool> BuildFctSpectraFromTrace(Class executionTrace)
        {
            Dictionary<Block, bool> fctSpectra = new Dictionary<Block, bool>();
            Block lastFunction = null;

            foreach (Line line in executionTrace.LinesOfCode)
            {
                if (this.linesOfSourceFile.ContainsKey(line.number))
                {
                    if (lastFunction == null || line.number > lastFunction.Exit)
                    {
                        string lineOfCode = this.linesOfSourceFile[line.number];
                        if (this.linesOfSourceFile[line.number].Contains("{"))
                        {  //Entering a new execution-context
                            this._Semaphore.Wait();
                            if (!this.functions.ContainsKey(line.number))
                            {
                                Block fct = BuildFunctionFromEntrance(line, executionTrace, fctSpectra);
                                if (fct != null)
                                {
                                    this.functions.Add(line.number, fct);
                                }
                            }
                            this._Semaphore.Release();

                            try
                            {
                                lastFunction = this.functions[line.number];
                                fctSpectra.Add(lastFunction, Convert.ToBoolean(line.hits));
                            }
                            catch (Exception e)
                            {

                            }
                        }
                    }
                }
            }

            return fctSpectra;
        }

        public string GetLine(int lineNumber)
        {
            return this.linesOfSourceFile[lineNumber];
        }

        private void readSourceFileAndRemoveComments()
        {
            //Read the content of the base-sourcefile
            using (StreamReader streamReader = new StreamReader(this.pathOfSoureFile))
            {
                string result = string.Empty;
                bool enteredComment = false;

                int lineIndex = 1;
                string lineOfCode = null;
                while ((lineOfCode = streamReader.ReadLine()) != null)
                {
                    result = lineOfCode;
                    //Handling of normal, inline-comments (e.g. //)
                    if (!enteredComment)
                    {
                        if (result.Contains("//"))
                            result = lineOfCode.Remove(lineOfCode.IndexOf("//"), lineOfCode.Length - lineOfCode.IndexOf("//"));
                    }

                    //Handlin of mulit-line comments (e.g. /*  */ )
                    if (!enteredComment)
                    {
                        while (result.Contains("/*"))
                        {
                            if (result.Contains("*/"))
                            {
                                result = result.Remove(result.IndexOf("/*"), result.IndexOf("*/") - result.IndexOf("/*") + 2);
                            }
                            else
                            {
                                result = result.Remove(result.IndexOf("/*"), result.Length - result.IndexOf("/*"));
                                enteredComment = true;
                            }
                        }
                    }
                    else
                    {
                        if (lineOfCode.Contains("*/"))
                        {
                            result = lineOfCode.Remove(0, lineOfCode.IndexOf("*/") + 2);
                            enteredComment = false;
                        }
                        else
                        {
                            result = string.Empty;
                        }
                    }

                    this.linesOfSourceFile.Add(lineIndex++, result);
                }
            }
        }

        private Block BuildFunctionFromEntrance(Line line, Class executionTrace, Dictionary<Block, bool> fctSpectra)
        {
            Dictionary<int, string> fLoC = new Dictionary<int, string>();
            int contextDepths = 0;
            int lineNumber = line.number;
            int totalLine = this.linesOfSourceFile.Count();
            do
            {
                string lineOfCode = this.linesOfSourceFile[lineNumber];

                //Entering a new execution-context if entrance reached
                contextDepths += this.linesOfSourceFile[lineNumber].Count(c => { return c == '{'; });

                //Leaving an execution-context if exit reached
                contextDepths -= this.linesOfSourceFile[lineNumber].Count(c => { return c == '}'; });

                fLoC.Add(lineNumber, this.linesOfSourceFile[lineNumber]);

                lineNumber++;
            } while (contextDepths >= 1 && totalLine >= lineNumber);


            // string name = parseFunctionName(line.number);
            string name = parseFunctionName(line.number);

            if (name == null || name.Equals("")) name = "Fct" + (fctSpectra.Count + 1);

            return new Block(name, executionTrace.FileName, line.number, fLoC);
        }

        private string parseFunctionName(int linenumber)
        {
            bool closingCurlyBracket = false;
            bool openingCurlyBracket = false;
            bool singleQuotationMark = false;
            bool doubleQuotationMark = false;

            string sourceCode = this.linesOfSourceFile[linenumber];
            int counter = 1;

            while (counter < 20)
            {
                if (linesOfSourceFile.ContainsKey(linenumber - counter))
                {
                    sourceCode = linesOfSourceFile[linenumber - counter].Trim() + " " + sourceCode.Trim();
                }
                else
                {
                    break;
                }

                counter++;
            }

            int index = sourceCode.Length - 1;
            int indexOpeningCurlyBracket = -1;

            // Find the opening curly bracket of the function
            while (index >= 0)
            {
                char letter = sourceCode[index];

                if (letter.Equals('{') && !closingCurlyBracket && !singleQuotationMark && !doubleQuotationMark)
                {
                    indexOpeningCurlyBracket = index;
                    break;
                }

                if (letter.Equals('\'') && !singleQuotationMark)
                {
                    singleQuotationMark = true;
                }
                else if (letter.Equals('\'') && singleQuotationMark)
                {
                    singleQuotationMark = false;
                }

                if (letter.Equals('\"') && !doubleQuotationMark)
                {
                    doubleQuotationMark = true;
                }
                else if (letter.Equals('\"') && doubleQuotationMark)
                {
                    doubleQuotationMark = false;
                }

                index--;
            }

            bool inParameterList = false;
            int numberOfOpenedBrackets = 0;

            index = indexOpeningCurlyBracket;
            int indexOpeningBracketOfParameterList = -1;

            // Finding the start of the parameter list of the function
            while (index >= 0)
            {
                char letter = sourceCode[index];

                if (letter.Equals(')'))
                {
                    numberOfOpenedBrackets++;
                    inParameterList = true;
                }

                if (letter.Equals('('))
                {
                    numberOfOpenedBrackets--;
                }

                if ((numberOfOpenedBrackets == 0) && inParameterList)
                {
                    indexOpeningBracketOfParameterList = index;
                    break;
                }


                index--;
            }


            index = indexOpeningBracketOfParameterList - 1;
            int endIndex = index;
            int startIndex = -1;
            bool spaceInBetween = true;

            if (index >= 0)
            {
                char test = sourceCode[index];

                while ((index >= 0 && (Char.IsLetterOrDigit(sourceCode[index]) || sourceCode[index] == '_') && !Char.IsWhiteSpace(sourceCode[index])) || (spaceInBetween && index >= 0))
                {
                    spaceInBetween = false;
                    startIndex = index;
                    index--;
                }

                string functionName = sourceCode.Substring(startIndex, endIndex - startIndex + 1);

                return functionName;
            }
            else
            {
                return "";
            }
        }

        private bool leaveOutTestFunction(string name)
        {
            if (name.Equals("TEST_F"))
            {
                return true;
            }

            return false;
        }
    }
}
