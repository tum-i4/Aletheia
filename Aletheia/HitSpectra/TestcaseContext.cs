using Aletheia.HitSpectra.persistence;
using Aletheia.HitSpectra.persistence.cobertura;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aletheia.HitSpectra
{
    /// <summary>
    /// This class holds the contextual data for test case executions
    /// </summary>
    public class TestcaseContext
    {
        private string name;
        private RunResult result;
        private Coverage testCoverage;
        private Dictionary<Class, HashSet<Block>> allInThisTestcaseInvokedFunctions;
        private Dictionary<Block, bool> testcaseSpectra;
        /// <summary>
        /// the constructor initializes the member variables
        /// </summary>
        /// <param name="name">Name of the Test Case</param>
        /// <param name="result">Result of execution</param>
        /// <param name="testCoverage">Coverage data for the test case</param>
        public TestcaseContext(string name, RunResult result, Coverage testCoverage)
        {
            this.name = name;
            this.result = result;
            this.testCoverage = testCoverage;

            this.allInThisTestcaseInvokedFunctions = new Dictionary<Class, HashSet<Block>>();
            this.testcaseSpectra = new Dictionary<Block, bool>();
        }
        /// <summary>
        /// get method to get test case name
        /// </summary>
        public string Name
        {
            get { return name; }
        }
        /// <summary>
        /// get method to get test case result
        /// </summary>
        public RunResult Result
        {
            get { return result; }
        }
        /// <summary>
        /// get method to get test case coverage
        /// </summary>
        public Coverage TestCoverage
        {
            get { return testCoverage; }
        }
        /// <summary>
        /// get method to get all invoked functions in the test case
        /// </summary>
        public Dictionary<Class, HashSet<Block>> AllFunctionsInThisTestcase
        {
            get { return allInThisTestcaseInvokedFunctions; }
        }
        /// <summary>
        /// get method for getting the spectra
        /// </summary>
        public Dictionary<Block, bool> TestcaseSpectra
        {
            get { return testcaseSpectra; }
        }
        /// <summary>
        /// Add function name to allInThisTestCaseInvokedFunctions
        /// </summary>
        /// <param name="function">Function block</param>
        /// <param name="srcFile">Class object</param>
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
        /// <summary>
        /// kind of set method for testCase spectra
        /// </summary>
        /// <param name="testcaseSpectra">test case spectra</param>
        public void addTestcaseSpectra(Dictionary<Block, bool> testcaseSpectra)
        {
            this.testcaseSpectra = testcaseSpectra;
        }
        /// <summary>
        /// get the name of functions of a class
        /// </summary>
        /// <param name="srcFile">Class object to be searched</param>
        /// <returns></returns>
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
