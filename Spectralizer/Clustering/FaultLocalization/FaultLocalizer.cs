using Spectralizer.Clustering.FaultLocalization.SimilarityMetrics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectralizer.Clustering.FaultLocalization
{
    public class FaultLocalizer
    {
        private EStrategy strategy;
        private IRankingStrategy rankingStrategy;
        private List<Item> suspiciousnessRanking;

        private int[,] testcaseMatrix;
        private string[] functionNames;


        public FaultLocalizer(int[,] testcaseMatrix, string[] fctNames, EStrategy strategy)
        {
            this.testcaseMatrix = testcaseMatrix;
            this.functionNames = fctNames;
            this.strategy = strategy;
            createRankingStrategyInstance();

            suspiciousnessRanking = new List<Item>();
        }

        public List<Item> calculateSuspiciousnessRanking()
        {
            List<Item> suspiciousnessList = new List<Item>();

            int dim1 = testcaseMatrix.GetLength(0);
            int dim2 = testcaseMatrix.GetLength(1);
            int indexResult = testcaseMatrix.GetUpperBound(1);

            for (int j = 0; j < dim2 - 1; j++)
            {
                string functionName = functionNames[j];

                if (j < testcaseMatrix.GetUpperBound(1))
                {
                    int coveredFailed = 0;
                    int uncoveredFailed = 0;
                    int coveredPassed = 0;
                    int uncoveredPassed = 0;

                    for (int i = 0; i < dim1; i++)
                    {
                        if (testcaseMatrix[i, indexResult] == 0)
                        {
                            if (testcaseMatrix[i, j] == 0) uncoveredFailed++;
                            else coveredFailed++;
                        }
                        else
                        {
                            if (testcaseMatrix[i, j] == 0) uncoveredPassed++;
                            else coveredPassed++;
                        }
                    }

                    double suspiciousnessValue = rankingStrategy.calculateSuspiciousness(coveredFailed, uncoveredFailed, coveredPassed, uncoveredPassed);
                    suspiciousnessList.Add(new Item(functionName, suspiciousnessValue));
                }
            }

            suspiciousnessList.Sort();
            suspiciousnessList.Reverse();

            return suspiciousnessList;
        }

        private void createRankingStrategyInstance()
        {
            switch (strategy)
            {
                case EStrategy.Dstar:
                    rankingStrategy = new Dstar();
                    break;
                case EStrategy.Jaccard:
                    rankingStrategy = new Jaccard();
                    break;
                //case EStrategy.Hamming:
                //    rankingStrategy = new Hamming();
                //    break;
                case EStrategy.Ochiai:
                    rankingStrategy = new Ochiai();
                    break;
                case EStrategy.Tarantula:
                    rankingStrategy = new Tarantula();
                    break;
            }
        }
    }
}
