﻿using Spectralizer.CommandLine.input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectralizer.SIL;
using Spectralizer.HIL;
using System.Data;
using Spectralizer.Clustering.FaultLocalization.SimilarityMetrics;
using Spectralizer.WorksheetParser.import;
using Spectralizer.CommandLine.output;
using MwMatlabClustering;
using Spectralizer.Clustering.FaultLocalization;

namespace Spectralizer
{
    public class Program
    {
        private static string mode;
        private static string workingDirectory;
        private static bool clustering = false;
        private static string operation;
        private static bool DEBUG = false;

        private static Dictionary<string, CommandLineArgument> commandLineArguments;

        // Datatables with HitSpectraMatrices
        private static DataTable silFunctionHitSpectraMatrix = null;
        private static DataTable silCountingFunctionInvokationsHitSpectraMatrix = null;
        private static DataTable silInvokedFunctionsHitSpectraMatrix = null;
        private static DataTable silInvokedFunctionsWithParametersHitSpectraMatrix = null;
        private static DataTable silLineCoverageHitSpectraMatrix = null;

        private static DataTable hilHitSpectraMatrix = null;
        private static string rankingMetric="Jaccard";
        public static void Main(string[] args)
        {

            #region Reading and partially evaluate commandline parameters
            // Reading command line arguments
            CommandLineReader commandLineReader = new CommandLineReader(args);
            commandLineArguments = commandLineReader.CommandLineArguments;
            if (commandLineArguments.Count() == 0)
            {
                Console.WriteLine("No Recognized parameter\n\n use Spectralizer.exe do=getHelp for help\n");
                return;
            }


            printCommandLineParameters();


            //check which operation is seleced

            if (!commandLineArguments.Keys.Contains(PossibleCommandLineArguments.OPERATION)) throw new Exception("Please select a operation: GenerateHitSpectra/Cluster/FaultLocalization!");
                operation = commandLineArguments[PossibleCommandLineArguments.OPERATION].Value;
            // Check, which mode is selected
            //if (!commandLineArguments.Keys.Contains(PossibleCommandLineArguments.MODE)) throw new Exception("Please select a operation mode!");
            //mode = commandLineArguments[PossibleCommandLineArguments.MODE].Value;
            if (!commandLineArguments.Keys.Contains(PossibleCommandLineArguments.MODE))
                mode = "sil";
            string outputDirectory = "";

            //check debug value
            if (commandLineArguments.Keys.Contains(PossibleCommandLineArguments.DEBUG))
                DEBUG = Convert.ToBoolean(commandLineArguments[PossibleCommandLineArguments.DEBUG].Value);

            // Check, if output directory is available
            if (!commandLineArguments.Keys.Contains(PossibleCommandLineArguments.OUTPUT_DIRECTORY))
                outputDirectory = "C:\\HitSpectras";
            else
                outputDirectory = commandLineArguments[PossibleCommandLineArguments.OUTPUT_DIRECTORY].Value;
            if ((workingDirectory = Program.createWorkingDirectory(outputDirectory)) == null) return;

            // Check if clustering should be done
            //if (!commandLineArguments.Keys.Contains(PossibleCommandLineArguments.CLUSTERING)) return;
            //clustering = Convert.ToBoolean(commandLineArguments[PossibleCommandLineArguments.CLUSTERING].Value);
            #endregion

            #region SIL
            // SIL Tests
            //if (mode.Equals("sil", StringComparison.OrdinalIgnoreCase))
            if (operation.Equals("GenerateHitSpectra", StringComparison.OrdinalIgnoreCase))
            {
                if (!commandLineArguments.Keys.Contains(PossibleCommandLineArguments.GTEST_PATH))
                {
                    Console.WriteLine("Gtest path is mandatory for OpenCV project\n");
                    return;

                }
                SIL.Spectralizer spectralizer = new SIL.Spectralizer(commandLineArguments, workingDirectory);
                if(DEBUG)
                    Console.WriteLine("Generation of Spectralizer object is complete\n");
                spectralizer.executeTestSuite();
                if (DEBUG)
                    Console.WriteLine("Execution of Test Suite is complete\n");
                spectralizer.exportHitSpectraMatrices();
                if (DEBUG)
                    Console.WriteLine("Exporting of HitSpectra matrix is complete\n");

                silFunctionHitSpectraMatrix = spectralizer.FunctionHitSpectraMatrix;
                silCountingFunctionInvokationsHitSpectraMatrix = spectralizer.CountingFunctionInvokationsHitSpectraMatrix;
                silInvokedFunctionsHitSpectraMatrix = spectralizer.InvokedFunctionsHitSpectraMatrix;
                silInvokedFunctionsWithParametersHitSpectraMatrix = spectralizer.InvokedFunctionsWithParametersHitSpectraMatrix;
                silLineCoverageHitSpectraMatrix = spectralizer.LineCoverageHitSpectraMatrix;
                Console.WriteLine("Spectra Matrix generated\n");
            }
            else if (operation.Equals("Cluster", StringComparison.OrdinalIgnoreCase))
            {
                string output = "\nClustering Given HitSpectraMatrix\n";
                if (operation.Equals("Cluster", StringComparison.OrdinalIgnoreCase))
                    CommandLinePrinter.printToCommandLine(output);
                else
                    Console.WriteLine("\nCreating Fault Localization with given HitSpectra");

                char separator;
                string inputPath;

                if (!commandLineArguments.ContainsKey(PossibleCommandLineArguments.SEPARATOR))
                    separator = ' ';
                else
                    separator = commandLineArguments[PossibleCommandLineArguments.SEPARATOR].Value.Trim()[0];

                if (!commandLineArguments.ContainsKey(PossibleCommandLineArguments.INPUT_PATH)) throw new Exception("No input path");
                inputPath = commandLineArguments[PossibleCommandLineArguments.INPUT_PATH].Value;

                HitSpectraCsvSheetReader reader = new HitSpectraCsvSheetReader(inputPath, separator);
                reader.parseSheet();
                DataTable dataTable = reader.getDataTable();

                string pathAdditional = "Clustering";
                string path = workingDirectory + "\\" + pathAdditional;

                doClustering(dataTable, path);
            }
            else if (operation.Equals("faultLocalization", StringComparison.OrdinalIgnoreCase))
            {

                string output = "\nRunning Fault Localization\n";

                CommandLinePrinter.printToCommandLine(output);
                char separator;
                string inputPath;

                if (!commandLineArguments.ContainsKey(PossibleCommandLineArguments.SEPARATOR))
                    separator = ' ';
                else
                    separator = commandLineArguments[PossibleCommandLineArguments.SEPARATOR].Value.Trim()[0];

                if (!commandLineArguments.ContainsKey(PossibleCommandLineArguments.INPUT_PATH)) throw new Exception("No input path");
                inputPath = commandLineArguments[PossibleCommandLineArguments.INPUT_PATH].Value;

                HitSpectraCsvSheetReader reader = new HitSpectraCsvSheetReader(inputPath, separator);
                reader.parseSheet();
                DataTable dataTable = reader.getDataTable();

                string pathAdditional = "FaultLocalization";
                string path = workingDirectory + "\\" + pathAdditional;
                if (commandLineArguments.ContainsKey(PossibleCommandLineArguments.FAULT_RANKING_METRIC))
                    rankingMetric = commandLineArguments[PossibleCommandLineArguments.FAULT_RANKING_METRIC].Value;
                EStrategy rankingStrategy = getFaultLocalizationStrategy(rankingMetric);
                Detective detective = new Clustering.FaultLocalization.Detective(dataTable, rankingStrategy, commandLineArguments, path);
                detective.DetectFault();
                CommandLinePrinter.printToCommandLine("Fault Localization compplete\n");



            }
            else if (operation.Equals("getHelp", StringComparison.OrdinalIgnoreCase))
            {
                String output = "These are accepted command parameters\nCommand should be given in key=value fashion\n\n";
                output += "do: specifies the operation to be performed\n\tPossible values={'GenerateHitSpectra', 'Cluster', 'FaultLocalization', 'GetHelp'}\n";
                output += "separator: specifies the separator character for csv file\nBy default white space is the separator\n";
                output += "output_directory: where the output will be generated, default output directory is C:\\HitSpectras\n";
                output += "project_path: it is a mandatory argument for HitSpectra Generation part, show the *.vcxproj file\n";

                output += "source_directory: show the directory where the source files are located\n";
                output += "degreeofparallelism: number of threads to run in parallel for HitSpectra generation, default value is 12\n";
                output += "gtest_path: mandatory argument for HitSpectra Generation, show the exe file of test project\n";
                output += "ranking_metric: ranking metric for fault localization, default is Jaccard";
                output += "clustering_method: default is maxclust\n";
                output+="linkage_method: default is average\n";
                output += "linkage_metric: default is euclidean\n";
                output += "similarity_threshold: default is 0.8\ncomparison_range: default is 0.1\n";
                output += "function_coverage: boolean argument, default is true\n";
                output += "invoked_function_coverage: boolean argument, default is true\n";
                output += "invoked_function_with_param_coverage: boolean argument, default is true\n";
                output += "counting_function_invokation_coverage: boolean argument, default is true\n";
                output += "line_coverage: boolean argument, default is true\n";
                CommandLinePrinter.printToCommandLine(output);
            }
                #endregion

                #region HIL
                // HIL-Tests
                if (mode.Equals("hil", StringComparison.OrdinalIgnoreCase))
            {
                HitSpectraMatrixGenerator hitSpectraMatrixGenerator = new HitSpectraMatrixGenerator(commandLineArguments, workingDirectory);
                hitSpectraMatrixGenerator.buildHitSpectraMatrix();
                hitSpectraMatrixGenerator.exportHitSpectraMatrix();
                hilHitSpectraMatrix = hitSpectraMatrixGenerator.HitSpectraMatrix;
            }
            #endregion

            #region Clustering
            // Do the clustering
            //if (clustering)
            if(operation.Equals("clustering", StringComparison.OrdinalIgnoreCase))
            {
                #region Clustering SIL
                if (mode.Equals("sil", StringComparison.OrdinalIgnoreCase))

                {

                    string output = "\nClustering SIL HitSpectraMatrices\n";
                    CommandLinePrinter.printToCommandLine(output);
                    if (silFunctionHitSpectraMatrix != null)
                    {
                        string pathAdditional = "Clustering_FunctionHitSpectraMatrix";
                        string path = workingDirectory + "\\" + pathAdditional;

                        doClustering(silFunctionHitSpectraMatrix, path);
                    }

                    if (silCountingFunctionInvokationsHitSpectraMatrix != null)
                    {
                        string pathAdditional = "Clustering_CountingFunctionInvokationsHitSpectraMatrix";
                        string path = workingDirectory + "\\" + pathAdditional;

                        doClustering(silCountingFunctionInvokationsHitSpectraMatrix, path);
                    }

                    if (silInvokedFunctionsHitSpectraMatrix != null)
                    {
                        string pathAdditional = "Clustering_InvokedFunctionsHitSpectraMatrix";
                        string path = workingDirectory + "\\" + pathAdditional;

                        doClustering(silInvokedFunctionsHitSpectraMatrix, path);
                    }

                    if (silInvokedFunctionsWithParametersHitSpectraMatrix != null)
                    {
                        string pathAdditional = "Clustering_InvokedFunctionsWithParametersHitSpectraMatrix";
                        string path = workingDirectory + "\\" + pathAdditional;

                        doClustering(silInvokedFunctionsWithParametersHitSpectraMatrix, path);
                    }

                    if (silLineCoverageHitSpectraMatrix != null)
                    {
                        string pathAdditional = "Clustering_LineCoverageHitSpectraMatrix";
                        string path = workingDirectory + "\\" + pathAdditional;

                        doClustering(silLineCoverageHitSpectraMatrix, path);
                    }
                }
                #endregion
                #region Clustering HIL
                else if (mode.Equals("hil", StringComparison.OrdinalIgnoreCase))
                {
                    string output = "\nClustering HIL HitSpectraMatrix\n";
                    CommandLinePrinter.printToCommandLine(output);
                    if (hilHitSpectraMatrix != null)
                    {
                        string pathAdditional = "Clustering_HIL";
                        string path = workingDirectory + "\\" + pathAdditional;

                        doClustering(hilHitSpectraMatrix, path);
                    }
                }
                #endregion
                #region Clustering any given HitSpectraMatrix as csv sheet
                else if (mode.Equals("none", StringComparison.OrdinalIgnoreCase))
                {
                    string output = "\nClustering any HitSpectraMatrix\n";
                    CommandLinePrinter.printToCommandLine(output);

                    char separator;
                    string inputPath;

                    if (!commandLineArguments.ContainsKey(PossibleCommandLineArguments.SEPARATOR))
                        separator = ';';
                    else
                        separator = commandLineArguments[PossibleCommandLineArguments.SEPARATOR].Value.Trim()[0];

                    if (!commandLineArguments.ContainsKey(PossibleCommandLineArguments.INPUT_PATH)) throw new Exception("No input path");
                    inputPath = commandLineArguments[PossibleCommandLineArguments.INPUT_PATH].Value;

                    HitSpectraCsvSheetReader reader = new HitSpectraCsvSheetReader(inputPath, separator);
                    reader.parseSheet();
                    DataTable dataTable = reader.getDataTable();

                    string pathAdditional = "Clustering";
                    string path = workingDirectory + "\\" + pathAdditional;

                    doClustering(dataTable, path);
                }
                #endregion
            }
            #endregion
            //Console.ReadLine();
        }

        private static void doClustering(DataTable dataTable, string path)
        {
            if (!System.IO.Directory.Exists(path)) System.IO.Directory.CreateDirectory(path);

            Clustering.Clustering clusterer = new Clustering.Clustering(dataTable, path, commandLineArguments);
            if (clusterer.checkIfAnyFailedTestcasesInTestSet())
            {
                clusterer.linkage();
                clusterer.doClustering();
                clusterer.findClusterCenter();
                clusterer.exportClusteringResults();
            }
        }

        private static string createWorkingDirectory(string outDir)
        {
            string workingDir = Program.createOutputDirectory(outDir);
            try
            {
                System.IO.Directory.CreateDirectory(workingDir);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                return null;
            }

            return workingDir;
        }

        private static string createOutputDirectory(string outDir)
        {
            try
            {
                return outDir
                + "\\Analysis_Fault_Localization_"
                + DateTime.Now.Year + "-"
                + (DateTime.Now.Month >= 10 ? DateTime.Now.Month + "-" : "0" + DateTime.Now.Month + "-")
                + (DateTime.Now.Day >= 10 ? DateTime.Now.Day + "_" : "0" + DateTime.Now.Day + "_")
                + (DateTime.Now.Hour >= 10 ? DateTime.Now.Hour + "-" : "0" + DateTime.Now.Hour + "-")
                + (DateTime.Now.Minute >= 10 ? DateTime.Now.Minute + "-" : "0" + DateTime.Now.Minute + "-")
                + (DateTime.Now.Second >= 10 ? DateTime.Now.Second + "" : "0" + DateTime.Now.Second);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                return null;
            }
        }

        private static void printCommandLineParameters()
        {
            string output = "**********************************\n"
                + "* Summary Commandline Parameters *\n"
                + "**********************************\n";

            foreach (string key in commandLineArguments.Keys)
            {
                CommandLineArgument arg = commandLineArguments[key];
                output += arg.Key + " = " + arg.Value + "\n";
            }

            CommandLinePrinter.printToCommandLine(output);
        }
        private static EStrategy getFaultLocalizationStrategy(string cmdStrategy)
        {
            foreach (EStrategy es in Enum.GetValues(typeof(EStrategy)))
            {
                if (es.ToString().Equals(cmdStrategy, StringComparison.OrdinalIgnoreCase))
                {
                    return es;
                }
              
            }
            return EStrategy.Jaccard;
        }
    }
}