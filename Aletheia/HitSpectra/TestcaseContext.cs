using Aletheia.HitSpectra.persistence;
using Aletheia.HitSpectra.persistence.cobertura;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aletheia.HitSpectra
{
    public class TestcaseContext
    {
        private string name;
        private RunResult result;
        private Coverage testCoverage;
        private Dictionary<Class, HashSet<Block>> allInThisTestcaseInvokedFunctions;
        private Dictionary<Block, bool> testcaseSpectra;

        public TestcaseContext(string name, RunResult result, Coverage testCoverage)
        {
            this.name = name;
            this.result = result;
            this.testCoverage = testCoverage;

            this.allInThisTestcaseInvokedFunctions = new Dictionary<Class, HashSet<Block>>();
            this.testcaseSpectra = new Dictionary<Block, bool>();
        }

        public string Name
        {
            get { return name; }
        }

        public RunResult Result
        {
            get { return result; }
        }

        public Coverage TestCoverage
        {
            get { return testCoverage; }
        }

        public Dictionary<Class, HashSet<Block>> AllFunctionsInThisTestcase
        {
            get { return allInThisTestcaseInvokedFunctions; }
        }

        public Dictionary<Block, bool> TestcaseSpectra
        {
            get { return testcaseSpectra; }
        }

        public void addFunction(Block function, Class srcFile)
        {
            HashSet<Block> hashSet;

            if (!this.allInThisTestcaseInvokedFunctions.Keys.Contains(srcFile))
            {
                hashSet = new HashSet<Block>();
                allInThisTestcaseInvokedFunctions[srcFile] = hashSet;
            }
            else
            {
                hashSet = this.allInThisTestcaseInvokedFunctions[srcFile];
            }

            hashSet.Add(function);
        }

        public void addTestcaseSpectra(Dictionary<Block, bool> testcaseSpectra)
        {
            this.testcaseSpectra = testcaseSpectra;
        }

        public HashSet<Block> getListOfFunctionsFromSourceFile(Class srcFile)
        {
            if (this.allInThisTestcaseInvokedFunctions.Keys.Contains(srcFile))
            {
                return this.allInThisTestcaseInvokedFunctions[srcFile];
            }

            return new HashSet<Block>();
        }
    }
}
