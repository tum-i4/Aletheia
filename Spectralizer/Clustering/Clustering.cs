using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathWorks.MATLAB.NET.Arrays;
using MwMatlabClustering;
using System.Data;
using Spectralizer.Clustering.FaultLocalization;
using Spectralizer.Clustering.FaultLocalization.SimilarityMetrics;
using Spectralizer.Toolbox;
using Spectralizer.Clustering.ClusterComparison;
using Spectralizer.CommandLine.input;
using Spectralizer.CommandLine.output;
using Spectralizer.WorksheetParser.export;

namespace Spectralizer.Clustering
{
    public class Clustering
    {
        private Dictionary<string, CommandLineArgument> commandLineArguments;

        private char separator = ' ';
        private string parentPath;
        private EStrategy faultLocalizationStrategy;
        private string linkage_method = "average";
        private string linkage_metric = "euclidean";
        private string clustering_method = "maxclust";
        private double similarityThreshold = 0.8;
        private double comparisonRange = 0.1;
        private List<int>[] clusters;
        private MatlabClustering matlabClustering;
        private int K=3;
        private DataTable hitSpectraMatrix;
        private bool DEBUG = false;

        // Separation of hitSpectraMatrix into various arrays
        private int[,] hitSpectraMatrixArray;
        private int[,] failedTestcasesInHitSpectraMatrix;
        private int[,] passedTestcasesInHitSpectraMatrix;

        private Dictionary<int, string> idList;
        private Dictionary<int, string> idListFailed;
        private Dictionary<int, string> idClusterCenter;
        private Dictionary<int, string> idListPassed;

        // Work products during runtime
        private double[,] binaryClusterTree;
        private int numberOfClusters;
        private int[] clusterClassification;
        private Dictionary<int, List<int[]>> listOfClusters;
        private Dictionary<int, List<Item>> suspiciousnessListForEveryCluster;
        private Dictionary<int, string[]> kNNData;

        private string Do;
        private Dictionary<int, double[]> idClusterDistance;// int is the id of cluster and the double array contains the distance array generated for that cluster

        public Clustering(DataTable dataTable, string parentPath, EStrategy faultLocalizationStrategy)
        {
            this.hitSpectraMatrix = dataTable;
            this.parentPath = parentPath;
            this.faultLocalizationStrategy = faultLocalizationStrategy;

            buildHitSpectraMatrixArray();
            buildFailedTestcasesInHitSpectraMatrixArray();
            buildPassedTestcasesInHitSpectraMatrixArray();
            try
            {
                matlabClustering = new MatlabClustering();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public Clustering(DataTable dataTable, string parentPath, Dictionary<string, CommandLineArgument> cmdArguments)
        {
            this.hitSpectraMatrix = dataTable;
            this.parentPath = parentPath;
            this.commandLineArguments = cmdArguments;

            readCommandLineInputParameters();

            buildHitSpectraMatrixArray();
            buildFailedTestcasesInHitSpectraMatrixArray();
            buildPassedTestcasesInHitSpectraMatrixArray();
            try
            {

                matlabClustering = new MatlabClustering();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public double[,] BinaryClusterTree
        {
            get { return binaryClusterTree; }
        }

        public int NumberOfClusters
        {
            get { return numberOfClusters; }
        }

        public int[] ClusterClassification
        {
            get { return clusterClassification; }
        }

        public void linkage()
        {
            string output = "\nDoing Linkage\n";
            CommandLinePrinter.printToCommandLine(output);

            MWArray tmpSolution = matlabClustering.matlabLinkage(new MWNumericArray(failedTestcasesInHitSpectraMatrix), new MWCharArray(linkage_method), new MWCharArray(linkage_metric));
            binaryClusterTree = Tools.buildTwoDimensionalDoubleArrayFromMWArray(tmpSolution);

            output = "\nFinished Linkage\n";
            CommandLinePrinter.printToCommandLine(output);
        }
        public double euclideanDistance(double []vectorA, double []vectorB, int len)
        {
            double result = 0.0;
            int i = 0;
            for(i=0;i<len; i++)
            {
                result += (vectorA[i] - vectorB[i]) * (vectorA[i] - vectorB[i]);
            }
            return Math.Sqrt(result);

        }
        public double[] getRow(int rowId, int [,]matrix, int len)
        {
            double[] ret = new double[len];
            int i = 0;
            for (i = 0; i < len; i++)
            {
                ret[i] = matrix[rowId,i];
            }
            return ret;
        }
        public double AverageAnArray(double []A)
        {
            double sum = 0;
            int len = A.Length;
            for (int i = 0; i < len; i++)
                sum += A[i];
            return sum / (double)len;
        }
        public int getFailedTestId(int minIndex, int N)
        {
            int a, b;
            int counter = 0;
            for (a = 0; a < N - 1; a++)
            {
                for (b = a + 1; b < N; b++)
                {
                    if (counter == minIndex)
                        break;
                    else
                        counter++;
                }
                if (counter == minIndex)
                    break;
            }
            return a;
        }

        public void findClusterCenter()
        {

            string output = "\nFinding Cluster Center\n";
            if (Do.Equals("cluster", StringComparison.OrdinalIgnoreCase))
            {
                CommandLinePrinter.printToCommandLine(output);
            }
            int i = 0, j = 0, counter = 0;
            int[][] failedHitSpectra = Tools.transformTwoDimensionalArrayToJaggedArray(failedTestcasesInHitSpectraMatrix);
            //step 1. separate the test cases from listOfCluster per cluster
            //idListFailed contains the name of failed test cases
            //clusterClassification holds the cluster number for each test case
            //listOfClusters holds the cluster list
            int n = listOfClusters.Count();// number of clusters
            int F = idListFailed.Count();
            idClusterDistance = new Dictionary<int, double[]>();
            clusters = new List<int>[n];

            for (i=0;i<F;i++)
            {
                if (clusters[clusterClassification[i]-1] == null)
                    clusters[clusterClassification[i]-1] = new List<int>();
                clusters[clusterClassification[i] - 1].Add(i);
            }

            

            //step 2. for each cluster separate the hitSpectra from failedTestCaseInHitSpectra matrix
            idClusterCenter = new Dictionary<int, string>();
            int L = failedTestcasesInHitSpectraMatrix.GetLength(1);
            //dumpArray(failedTestcasesInHitSpectraMatrix);
            for ( i = 0;i < n; i++){//for each cluster
                double[][] failedCluster = new double[clusters[i].Count()][];
                counter = 0;
                //double D = euclideanDistance(getRow(0, failedTestcasesInHitSpectraMatrix, L), getRow(6, failedTestcasesInHitSpectraMatrix, L), L);
                foreach (int  a in clusters[i])
                {
                    //failedCluster[counter] = new double[L];
                    failedCluster[counter] =  getRow(a,failedTestcasesInHitSpectraMatrix, L);
                    counter++;
                }
                //step 3. for each set of hitSpectra run find cluster center
                string centerName = "";
                if (counter > 2)
                    centerName = ClusterCenter(failedCluster, clusters[i], i );
                else if (counter==2)
                {
                    centerName = idListFailed[clusters[i][0] + 1];
                    double[] dist2 = new double[2];
                    dist2[0] = 0.0;
                    dist2[1] = euclideanDistance(failedCluster[0], failedCluster[1], failedCluster[0].Length);
                    idClusterDistance.Add(i, dist2);
                }
                else
                {
                    centerName = idListFailed[clusters[i][0] + 1];
                    double[] dist2 = new double[1];
                    dist2[0] = 0.0;
                    idClusterDistance.Add(i, dist2);
                }
                    
                //if(Do.Equals("cluster", StringComparison.OrdinalIgnoreCase))    
                  //  Console.WriteLine("Cluster id: "+ (i+1)+" Cluster Center: "+centerName);
                idClusterCenter.Add(i, centerName);
            }
            KNNData();
            exportClusterHitSpectra();
        }
        public void exportClusterHitSpectra()
        {
            string[] columnNames = hitSpectraMatrix.Columns.Cast<DataColumn>()
                                 .Select(x => x.ColumnName)
                                 .ToArray();
            int numCluster = clusters.Count();
            for (int i=0;i<numberOfClusters; i++)
            {
                DataTable combinedDataTable = hitSpectraMatrix.Clone();
                //add rows
                //the index is located in idFailedtestcase, data is located at failedHitSpectraMatrix and the result is 0 as all the test cases are failing
                List<int> failedTestId = clusters[i];
                if (failedTestId.Count() > 0)
                {
                    for (int j = 0; j < failedTestId.Count(); j++)
                    {
                        DataRow dr = hitSpectraMatrix.AsEnumerable().SingleOrDefault(r => r.Field<string>("Index") == idListFailed[failedTestId[j]+1]);
                        combinedDataTable.ImportRow(dr);
                    }

                    for(int j=0; j < idListPassed.Count(); j++)
                    {
                        DataRow dr = hitSpectraMatrix.AsEnumerable().SingleOrDefault(r => r.Field<string>("Index") == idListPassed[j + 1]);
                        combinedDataTable.ImportRow(dr);
                    }
                    //write datatatble to csv
                    string dirPath = parentPath + "\\ClusterHitSpectra";
                    if (!System.IO.Directory.Exists(dirPath))
                        System.IO.Directory.CreateDirectory(dirPath);
                    string exportPath = dirPath + "\\Cluster_" + (i + 1) + "_HitSpectra.csv";
                    CsvSheetWriter writer= new CsvSheetWriter(exportPath, separator, combinedDataTable);
                    writer.writeToWorkSheet();
                }
            }
        }

        public void KNNData()
        {
            kNNData = new Dictionary<int, string[]>();
            //idClusterDistance contains cluster id and distances between testcases. 
            int N = listOfClusters.Count();
            for(int i = 0; i < N; i++)
            {
                if (idClusterDistance.ContainsKey(i))
                {
                    double[] dist = idClusterDistance[i];
                    int len = dist.Length;
                    if (len > 1)
                    {
                        int maxNode = clusters[i].Count();
                        //double avgDist = AverageAnArray(dist);
                        //double[] diff = new double[dist.Length];
                        //for (int m = 0; m < dist.Length; m++)
                        //{
                         //   diff[m] = Math.Abs(dist[m] - avgDist);
                        //}
                        string[] str = new string[dist.Length];
                        double maxVal = dist.Max();
                        int centerIndex = Array.IndexOf(dist, dist.Min());
                        for (int j = 0; j < dist.Length; j++)
                        {
                            int Id = Array.IndexOf(dist, dist.Min());
                            //int testIdInList = getFailedTestId(Id, maxNode);
                            int testIdInList = Id;
                            int testId = clusters[i][testIdInList];
                            string testName = idListFailed[testId + 1];
                            str[j] = testName;
                            //update array
                            dist[Id] += maxVal + 1;
                        }
                        //add knn to dictionary
                        kNNData.Add(i, str);
                    }
                    else
                    {
                        string[] str = new string[dist.Length];
                        int testId = clusters[i][0];
                        string testName = idListFailed[testId + 1];
                        str[0] = testName;
                        kNNData.Add(i, str);
                    }
                }
            }
        }
        public string ClusterCenter(double [][]cluster, List<int> testList, int clusterId)
        {
            int maxNumberOfNodes = cluster.GetLength(0);// number or rows
            int len = cluster[0].GetLength(0); // number of columns
            int i, j, counter=0;
            //double []dist = new double[maxNumberOfNodes*(maxNumberOfNodes-1)/2];
            double[] dist = new double[maxNumberOfNodes];
            /*for (i = 0; i < maxNumberOfNodes - 1; i++)
            {
                //double[] vectorA = getRow(i, failedTestcasesInHitSpectraMatrix, len);
                double[] vectorA = cluster[i];
                for (j = i + 1; j < maxNumberOfNodes; j++)
                {
                    //double[] vectorB = getRow(j, failedTestcasesInHitSpectraMatrix, len);
                    double[] vectorB = cluster[j];
                    dist[counter] = euclideanDistance(vectorA, vectorB, len);
                    counter++;
                }
            }*/
            double []AbsoluteZero = new double[len];
            for (j = 0; j < maxNumberOfNodes; j++)
            {
                //double[] vectorB = getRow(j, failedTestcasesInHitSpectraMatrix, len);
                double[] vectorB = cluster[j];
                dist[counter] = euclideanDistance(AbsoluteZero, vectorB, len);
                counter++;
            }
 
            //idClusterDistance.Add(clusterId, dist);
            double avgDist = AverageAnArray(dist);
            double[] diff = new double[dist.Length];
            //get the central test which has minimal difference from average distance
            for (i = 0; i < dist.Length; i++)
            {
                diff[i] = Math.Abs(dist[i] - avgDist);
            }
            //get the culprit testCase index
            int minIndex = Array.IndexOf(diff, diff.Min());
            //retrieve the testcase id in failedTestCseInHitspectraMatrix
            //int testIdInList = getFailedTestId(minIndex, maxNumberOfNodes);
            int testIdInList = minIndex;
            int testId = testList[testIdInList];
            string testName = idListFailed[testId + 1];
            //Console.WriteLine(testName);
            //update distance 
            double []vectorA = cluster[testIdInList];// center of cluster

            // now get the distance from center to other testcases
            counter = 0;
            double[] dist2 = new double[maxNumberOfNodes];
            for (j = 0; j < maxNumberOfNodes; j++)
            {
                //double[] vectorB = getRow(j, failedTestcasesInHitSpectraMatrix, len);
                double[] vectorB = cluster[j];
                dist2[counter] = euclideanDistance(vectorA, vectorB, len);
                counter++;
            }

            //export distance for KNN
            idClusterDistance.Add(clusterId, dist2);
            return testName;
        }
        public void doClustering()
        {
            string output = "\nStarting Clustering\n";
            if (Do.Equals("cluster", StringComparison.OrdinalIgnoreCase))
            {
                
                CommandLinePrinter.printToCommandLine(output);
            }

            int numberOfNodes = 2;
            int prevNumberOfNodes = 1;
            int maxNumberOfNodes = failedTestcasesInHitSpectraMatrix.GetLength(0);
            int[] tmpClusterClassification;
            int[] prevClusterClassification =
                Tools.buildOneDimensionalIntArray(matlabClustering.buildNCluster(new MWNumericArray(binaryClusterTree), new MWNumericArray(1), new MWCharArray(clustering_method)));
            Dictionary<int, List<int[]>> prevListOfClusters = Tools.allocateTestcasesIntoClusters(Tools.buildOneDimensionalIntArray(matlabClustering.buildNCluster(new MWNumericArray(binaryClusterTree), new MWNumericArray(1), new MWCharArray(clustering_method))), failedTestcasesInHitSpectraMatrix);

            while (numberOfNodes < maxNumberOfNodes)
            {
                

                MWArray tmpSolution = matlabClustering.buildNCluster(new MWNumericArray(binaryClusterTree), new MWNumericArray(numberOfNodes), new MWCharArray(clustering_method));
                tmpClusterClassification = Tools.buildOneDimensionalIntArray(tmpSolution);
                //MWArray tmpCenter = matlabClustering.findClusterCenter(new MWNumericArray(binaryClusterTree), 0.6);
                Dictionary<int, List<int[]>> tmpListOfClusters = Tools.allocateTestcasesIntoClusters(tmpClusterClassification, failedTestcasesInHitSpectraMatrix);

                int[,] firstCluster = Tools.transformJaggedArrayToTwoDimensionalArray(tmpListOfClusters[1].ToArray());
                firstCluster = Tools.mergeTwoTwoDimensionalIntArrays(firstCluster, passedTestcasesInHitSpectraMatrix);
                int[,] secondCluster = Tools.transformJaggedArrayToTwoDimensionalArray(tmpListOfClusters[2].ToArray());
                secondCluster = Tools.mergeTwoTwoDimensionalIntArrays(secondCluster, passedTestcasesInHitSpectraMatrix);

                List<Item> suspiciousnessList1 = new FaultLocalizer(firstCluster, Tools.generateFunctionNamesArray(hitSpectraMatrix), faultLocalizationStrategy).calculateSuspiciousnessRanking();
                List<Item> suspiciousnessList2 = new FaultLocalizer(secondCluster, Tools.generateFunctionNamesArray(hitSpectraMatrix), faultLocalizationStrategy).calculateSuspiciousnessRanking();

                Comparator comparator = new Comparator(ComparingStrategy.JaccardTwoSets, comparisonRange);
                double similarity = comparator.compare(suspiciousnessList1, suspiciousnessList2);

                if (similarity > similarityThreshold) break;

                prevNumberOfNodes = numberOfNodes;
                numberOfNodes++;
                prevClusterClassification = tmpClusterClassification;
                prevListOfClusters = tmpListOfClusters;
            }

            numberOfClusters = prevNumberOfNodes;
            clusterClassification = prevClusterClassification;
            listOfClusters = prevListOfClusters;

            buildSuspiciousnessRankingForCluster();


            if (Do.Equals("cluster", StringComparison.OrdinalIgnoreCase))
            {
                output = "\nFinished Clustering\n";
                //distance = D.getDistance(new MWNumericArray(failedTestcasesInHitSpectraMatrix), new MWCharArray(linkage_metric));
                CommandLinePrinter.printToCommandLine(output);
            }
            
        }

        public bool checkIfAnyFailedTestcasesInTestSet()
        {
            int numbOfFailedTestcases = 0;

            foreach (DataRow row in hitSpectraMatrix.Rows)
            {
                int value = (int)row[hitSpectraMatrix.Columns.Count - 1];

                if (value == 0) numbOfFailedTestcases++;
                if (numbOfFailedTestcases >= 2) return true;
            }

            return false;
        }

        private void buildHitSpectraMatrixArray()
        {
            hitSpectraMatrixArray = Tools.buildTwoDimensionalIntArray(hitSpectraMatrix);
            idList = Tools.buildIdList(hitSpectraMatrix);
        }

        private void buildFailedTestcasesInHitSpectraMatrixArray()
        {
            failedTestcasesInHitSpectraMatrix = Tools.seperateTestcasesByResultTwoDimensionalIntArray(Result.Failed, hitSpectraMatrixArray);
            idListFailed = Tools.buildIdListSeperated(Result.Failed, hitSpectraMatrix);
        }

        private void buildPassedTestcasesInHitSpectraMatrixArray()
        {
            passedTestcasesInHitSpectraMatrix = Tools.seperateTestcasesByResultTwoDimensionalIntArray(Result.Passed, hitSpectraMatrixArray);
            idListPassed = Tools.buildIdListSeperated(Result.Passed, hitSpectraMatrix);
        }

        private void buildSuspiciousnessRankingForCluster()
        {
            suspiciousnessListForEveryCluster = new Dictionary<int, List<Item>>();
            foreach (int key in listOfClusters.Keys)
            {
                List<int[]> tmpList = listOfClusters[key];
                int[][] ar = tmpList.ToArray();
                int[,] tr = Tools.transformJaggedArrayToTwoDimensionalArray(ar);
                int[,] mergedList = Tools.mergeTwoTwoDimensionalIntArrays(tr, passedTestcasesInHitSpectraMatrix);

                List<Item> suspiciousnessList = new FaultLocalizer(mergedList, Tools.generateFunctionNamesArray(hitSpectraMatrix), faultLocalizationStrategy).calculateSuspiciousnessRanking();
                suspiciousnessListForEveryCluster.Add(key, suspiciousnessList);
            }
        }

        private bool readCommandLineInputParameters()
        {
            foreach (CommandLineArgument cmdArgument in commandLineArguments.Values)
            {
                try
                {
                    string arg = cmdArgument.Key;

                    if (arg.ToLower().Equals(PossibleCommandLineArguments.FAULT_RANKING_METRIC))
                    {
                        if (!determineFaultLocalizationStrategy(cmdArgument.Value)) throw new Exception("Unknown Fault Localization Strategy");
                    }
                    else if (arg.ToLower().Equals(PossibleCommandLineArguments.SEPARATOR))
                    {
                        separator = cmdArgument.Value.Trim()[0];
                    }
                    else if (arg.ToLower().Equals(PossibleCommandLineArguments.LINKAGE_METHOD))
                    {
                        linkage_method = cmdArgument.Value.Trim();
                    }
                    else if (arg.ToLower().Equals(PossibleCommandLineArguments.LINKAGE_METRIC))
                    {
                        linkage_metric = cmdArgument.Value.Trim();
                    }
                    else if (arg.ToLower().Equals(PossibleCommandLineArguments.CLUSTERING_METHOD))
                    {
                        clustering_method = cmdArgument.Value.Trim();
                    }
                    else if (arg.ToLower().Equals(PossibleCommandLineArguments.SIMILARITY_THRESHOLD))
                    {
                        similarityThreshold = Convert.ToDouble(cmdArgument.Value.Trim());
                    }
                    else if (arg.ToLower().Equals(PossibleCommandLineArguments.COMPARISON_RANGE))
                    {
                        comparisonRange = Convert.ToDouble(cmdArgument.Value.Trim());
                    }
                    else if (arg.ToLower().Equals(PossibleCommandLineArguments.NUM_REPRESENTATIVE_TEST))
                    {
                        K = Convert.ToInt16(cmdArgument.Value.Trim());
                    }
                    else if (arg.ToLower().Equals(PossibleCommandLineArguments.OPERATION))
                    {
                        Do = cmdArgument.Value.Trim();
                    }
                    else if (arg.ToLower().Equals(PossibleCommandLineArguments.DEBUG))
                        DEBUG = Convert.ToBoolean(cmdArgument.Value);
                }
                catch (Exception e)
                {
                    Console.Write(e.Message);
                    return false;
                }
            }

            return true;
        }

        private bool determineFaultLocalizationStrategy(string cmdStrategy)
        {
            foreach (EStrategy es in Enum.GetValues(typeof(EStrategy)))
            {
                if (es.ToString().Equals(cmdStrategy, StringComparison.OrdinalIgnoreCase))
                {
                    faultLocalizationStrategy = es;
                    return true;
                }
            }

            return false;
        }

        #region Export results of Clustering
        public void exportClusteringResults()
        {
            string toPrint = "\nExporting Clustering results\n";
            if (Do.Equals("cluster", StringComparison.OrdinalIgnoreCase))
            {
                CommandLinePrinter.printToCommandLine(toPrint);
            }

            exportClustering();
            //exportSuspiciousnessRanking();
            drawDendrogram();
            if (Do.Equals("cluster", StringComparison.OrdinalIgnoreCase))
            {
                CommandLinePrinter.printToCommandLine("\nFinished Exporting Clustering results\n");
            }
        }
        private void dumpArray(int [,]A)
        {
            string filenameTemplate = "array_";
            string path = parentPath + "\\" + filenameTemplate + ".csv";
            int len = A.GetLength(0);
            int itemNum = A.GetLength(1);
            String output = "";
            for(int i = 0; i < len; i++)
            {
                for (int j=0;j < itemNum; j++)
                {
                    output += Convert.ToString(A[i,j]) + ",";
                }
                output += "\n";
            }

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(path))
            {
                file.Write(output);
            }
        }
        private void exportSuspiciousnessRanking()
        {
            string filenameTemplate = "cluster_";

            foreach (int key in suspiciousnessListForEveryCluster.Keys)
            {
                if(suspiciousnessListForEveryCluster[key].Count>0)
                { 
                    string output = "Rank" + separator + "Functionname" + separator + "Suspiciousness Value(" + faultLocalizationStrategy.ToString() + ")\n";
                    string path = parentPath + "\\" + filenameTemplate + key + ".csv";
                    List<Item> tmpList = suspiciousnessListForEveryCluster[key];

                    int rank = 1;
                    double value = tmpList.ElementAt(0).Suspiciousness;
                    foreach (Item item in tmpList)
                    {
                        if (Math.Abs(value - item.Suspiciousness) > 0.0000001) rank++;
                        output += Convert.ToString(rank) + separator + item.ItemName + separator + item.Suspiciousness + "\n";
                        value = item.Suspiciousness;
                    }

                    

                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(path))
                    {
                        file.Write(output);
                    }

                    CommandLinePrinter.printToCommandLine(".");
                }
            }
        }

        private void exportClustering()
        {
            string path = parentPath + @"\clustering.csv";
            string output = "Testcase" + separator + "Cluster" + separator + "Link\n";
            int i = 1;

            foreach (int n in clusterClassification)
            {
                output += idListFailed[i] + separator + n + separator + "=HYPERLINK(\"" + @"ClusterHitSpectra\Cluster_" + n + "HitSpectra.csv\")\n";
                i++;

                CommandLinePrinter.printToCommandLine(".");
            }
            /*output += "\n\nCluster Centers\n\n";
            int N = idClusterCenter.Count();
            for (i = 0; i < N; i++)
            {
                output += "Cluster ID: " + (i + 1) + "  Cluster Center: " + idClusterCenter[i] + "\n";
            }
            output += "\n\nkNN data for k="+K+"\n";
           */
            int len = kNNData.Count();

            for (i = 0; i < len; i++ )
            {
                if (kNNData.ContainsKey(i))
                {
                    string[] str = kNNData[i];
                    
                    if (str.Length >= K)
                    {
                        output += "For Cluster " + (i + 1) + " " + K + " representative test(s) is/are: \n\t";
                        for (int j = 0; j < K; j++)
                        {
                            if(j==K-1)
                                output += (j+1) + ") " + str[j] + "\n";
                            else
                                output += (j + 1) + ") " + str[j] + "\n\t";
                        }
                    }
                    else if(str.Length>1)
                    {
                        output += "For Cluster " + (i + 1) + " Available " + (str.Length)+ " representative test(s) is/are: \n\t";
                        for (int j = 0; j < str.Length; j++)
                        {
                            if(j==str.Length-1)
                                output += (j+1) + ") " + str[j] + "\n";
                            else
                                output += (j+1) + ") " + str[j] + "\n\t";
                        }
                    }
                    else
                    {
                        output += "Cluster " + (i + 1) + " has only one Represenntative test\n\t1) "+str[0]+"\n";
                    }
                }
            }
            output += "\n";
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(path))
            {
                file.Write(output);
            }
        }

        private void printClustering()
        {
            for (int i = 0; i < clusterClassification.Length; i++)
            {
                int key = i + 1;

                Console.WriteLine(clusterClassification[i] + " - " + idListFailed[key]);
            }
            Console.WriteLine("number of Clusters: " + numberOfClusters);
        }

        private void drawDendrogram()
        {
            matlabClustering.exportDendrogram(new MWNumericArray(binaryClusterTree), new MWCharArray(parentPath));
        }
        #endregion
    }
}
