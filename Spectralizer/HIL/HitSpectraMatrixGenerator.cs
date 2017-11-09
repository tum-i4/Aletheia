using Spectralizer.CommandLine.input;
using Spectralizer.HIL.Tools;
using Spectralizer.WorksheetParser.export;
using Spectralizer.WorksheetParser.import;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectralizer.HIL
{
    public class HitSpectraMatrixGenerator
    {
        private Dictionary<string, CommandLineArgument> commandLineArguments;

        private string rootPath;
        private string outputDirectory;
        private char separator;
        private int limiter = 5;

        private DataTable hitSpectraMatrix;
        private Dictionary<string, DataTable> testcases;

        private ICollection<string> functionNameHashSet = new HashSet<string>();

        public HitSpectraMatrixGenerator(string rootPath, char separator)
        {
            this.rootPath = rootPath;
            this.separator = separator;
            hitSpectraMatrix = new DataTable();
            testcases = new Dictionary<string, DataTable>();
        }

        public HitSpectraMatrixGenerator(Dictionary<string, CommandLineArgument> commandLineArguments, string outputDirectory)
        {
            this.commandLineArguments = commandLineArguments;
            this.outputDirectory = outputDirectory;

            readCommandLineInputParameters();

            hitSpectraMatrix = new DataTable();
            testcases = new Dictionary<string, DataTable>();
        }

        public DataTable HitSpectraMatrix
        {
            get { return hitSpectraMatrix; }
        }

        public void buildHitSpectraMatrix()
        {
            Dictionary<string, string> allFilePaths = getListWithAllTestcaseFiles();

            //Console.Write("Running.");
            int countAllFilePathsAtTheBeginning = allFilePaths.Count;
            int countAllFilePaths = allFilePaths.Count;

            while (countAllFilePaths > 0)
            {
                Console.WriteLine((countAllFilePathsAtTheBeginning - allFilePaths.Count) + "/" + countAllFilePathsAtTheBeginning);
                testcases = new Dictionary<string, DataTable>();

                Dictionary<string, string> tmpFilePaths = new Dictionary<string, string>();
                if (allFilePaths.Count > limiter)
                {
                    for (int i = 0; i < limiter; i++)
                    {
                        string key = allFilePaths.Keys.ElementAt(0);
                        tmpFilePaths.Add(key, allFilePaths[key]);
                        allFilePaths.Remove(key);
                    }
                }
                else
                {
                    for (int i = 0; i < allFilePaths.Count; i++)
                    {
                        string key = allFilePaths.Keys.ElementAt(0);
                        tmpFilePaths.Add(key, allFilePaths[key]);
                        allFilePaths.Remove(key);
                    }
                }

                extractTestcasesFromWorkSheet(tmpFilePaths);

                generateFunctionNameHashSet();
                generateHitSpectraMatrixStructure();
                populateHitSpectraMatrix();

                countAllFilePaths = allFilePaths.Count;
            }
        }

        public void exportHitSpectraMatrix()
        {
            if (hitSpectraMatrix.Columns.Contains("Result"))
            {
                hitSpectraMatrix.Columns["Result"].SetOrdinal(hitSpectraMatrix.Columns.Count - 1);
            }

            CsvSheetWriter csvSheetWriter = new CsvSheetWriter(outputDirectory + "\\" + "HitSpectraMatrix.csv", separator, hitSpectraMatrix);
            csvSheetWriter.writeToWorkSheet();
        }

        private bool readCommandLineInputParameters()
        {
            foreach (CommandLineArgument cmdArgument in commandLineArguments.Values)
            {
                try
                {
                    string arg = cmdArgument.Key;

                    if (arg.ToLower().Equals(PossibleCommandLineArguments.INPUT_DIRECTORY))
                        rootPath = cmdArgument.Value;
                    else if (arg.ToLower().Equals(PossibleCommandLineArguments.SEPARATOR))
                        separator = cmdArgument.Value[0];
                    else if (arg.ToLower().Equals(PossibleCommandLineArguments.OUTPUT_DIRECTORY))
                        outputDirectory = cmdArgument.Value;
                }
                catch (Exception e)
                {
                    Console.Write(e.Message);
                    return false;
                }
            }

            return true;
        }

        private Dictionary<string, string> getListWithAllTestcaseFiles()
        {
            return new FileExtractor(rootPath).AllFilePathsWithFileNameAsKey;
        }

        private void extractTestcasesFromWorkSheet(Dictionary<string, string> filePaths)
        {
            foreach (string key in filePaths.Keys)
            {
                AWorksheetReader csvSheetReader = new CsvSheetReader(filePaths[key], separator);
                csvSheetReader.parseSheet();
                DataTable listOfUsedFunctionsWithinTestcase = csvSheetReader.getDataTable();
                testcases.Add(key, listOfUsedFunctionsWithinTestcase);
            }
        }

        private void generateFunctionNameHashSet()
        {
            foreach (DataTable functionTable in testcases.Values)
            {
                for (int i = 0; i < functionTable.Rows.Count; i++)
                {
                    DataRow row = functionTable.Rows[i];
                    string fctName = (string)row[0];
                    functionNameHashSet.Add(fctName);
                }
            }
        }

        private void generateHitSpectraMatrixStructure()
        {
            if (!hitSpectraMatrix.Columns.Contains("Index"))
            {
                DataColumn testcaseColumn = new DataColumn();
                testcaseColumn.DataType = Type.GetType("System.String");
                testcaseColumn.ColumnName = "Index";
                testcaseColumn.DefaultValue = "No Index defined";
                hitSpectraMatrix.Columns.Add(testcaseColumn);
            }

            foreach (string fctName in functionNameHashSet)
            {
                if (!hitSpectraMatrix.Columns.Contains(fctName))
                {
                    DataColumn fctColumn = new DataColumn();
                    fctColumn.DataType = Type.GetType("System.Int32");
                    fctColumn.ColumnName = fctName;
                    fctColumn.DefaultValue = 0;
                    hitSpectraMatrix.Columns.Add(fctColumn);
                }
            }
        }

        private void populateHitSpectraMatrix()
        {
            foreach (string testCaseName in testcases.Keys)
            {
                DataTable functionsList = testcases[testCaseName];
                DataRow r = hitSpectraMatrix.NewRow();
                r["Index"] = testCaseName;

                foreach (DataRow rw in functionsList.Rows)
                {
                    string fctName = (string)rw[0];
                    int numberOInvokations = (int)rw[1];
                    r[fctName] = numberOInvokations + (int)r[fctName];
                }

                hitSpectraMatrix.Rows.Add(r);
            }
        }
    }
}
