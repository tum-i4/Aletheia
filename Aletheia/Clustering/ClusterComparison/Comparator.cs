using Aletheia.Clustering.FaultLocalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aletheia.Clustering.ClusterComparison
{
    public class Comparator
    {
        private ComparingStrategy comparingStrategy;
        private IComparingStrategy strategy;

        private double percentage;

        public Comparator(ComparingStrategy comparingStrategy, double percentage)
        {
            this.comparingStrategy = comparingStrategy;
            selectComparingStrategy();

            this.percentage = percentage;
        }

        public double compare(List<Item> suspiciousnessList1, List<Item> suspiciousnessList2)
        {
            int length = Math.Min(Convert.ToInt32((double)suspiciousnessList1.Count * percentage + 1), suspiciousnessList1.Count);

            List<Item> subList1 = suspiciousnessList1.GetRange(0, length);
            List<Item> subList2 = suspiciousnessList2.GetRange(0, length);

            List<Item> unionList = subList1.Union(subList2).ToList<Item>();
            List<Item> intersectionList = subList1.Intersect(subList2).ToList<Item>();

            double similarity = strategy.calculateSimilarityofTwoCluster(unionList.Count, intersectionList.Count);

            return similarity;
        }

        private void selectComparingStrategy()
        {
            switch (comparingStrategy)
            {
                case ComparingStrategy.JaccardTwoSets:
                    strategy = new JaccardTwoSets();
                    break;
            }
        }

        private List<Item> unionFunctionList(List<Item> list1, List<Item> list2)
        {
            HashSet<Item> unionList = new HashSet<Item>();

            foreach (Item fct1 in list1)
            {
                unionList.Add(fct1);
            }

            foreach (Item fct2 in list2)
            {
                unionList.Add(fct2);
            }

            return unionList.ToList();
        }
    }
}
