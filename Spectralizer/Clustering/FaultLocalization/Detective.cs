using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectralizer.Clustering.FaultLocalization.SimilarityMetrics;
using System.Data;
using Spectralizer.Toolbox;
using Spectralizer.CommandLine.input;

namespace Spectralizer.Clustering.FaultLocalization
{
    class Detective
    {
        private EStrategy strategy;
        private DataTable hitSpectraData;
        private int[,] hitSpectraMatrixArray;
        private Dictionary<int, string> idList;
        private List<Item> suspiciousnessList;
        private Dictionary<string, CommandLineArgument> commandLineArguments;
        private char separator;
        private string parentPath;

        public Detective(DataTable T, EStrategy es, Dictionary<string, CommandLineArgument> arguments, string path)
        {
            strategy = es;
            hitSpectraData = T;
            commandLineArguments = arguments;
            parentPath = path;
        } 
        public void DetectFault()
        {
            hitSpectraMatrixArray = Tools.buildTwoDimensionalIntArray(hitSpectraData);
            idList = Tools.buildIdList(hitSpectraData);
            suspiciousnessList = new FaultLocalizer(hitSpectraMatrixArray, Tools.generateFunctionNamesArray(hitSpectraData), strategy).calculateSuspiciousnessRanking();
            exportSuspiciousnessRanking();
        }
        private void exportSuspiciousnessRanking()
        {
            string filenameTemplate = "fault";

            if (!commandLineArguments.ContainsKey(PossibleCommandLineArguments.SEPARATOR))
                separator = ' ';
            else
                separator = commandLineArguments[PossibleCommandLineArguments.SEPARATOR].Value.Trim()[0];

            if (suspiciousnessList.Count > 0)
                {
                    string output = "Rank" + separator + "Block_name" + separator + "Suspiciousness Value(" + strategy.ToString() + ")\n";
                    if (!System.IO.Directory.Exists(parentPath))
                        System.IO.Directory.CreateDirectory(parentPath);
                    string path = parentPath + "\\" + filenameTemplate  + ".csv";
                    List<Item> tmpList = suspiciousnessList;

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

                }
            
        }
    }
}
