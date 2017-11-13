using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aletheia.Clustering.ClusterComparison
{
    public class JaccardTwoSets : IComparingStrategy
    {
        public double calculateSimilarityofTwoCluster(int unionOfTwoSets, int intersectionOfTwoSets)
        {
            return ((double)intersectionOfTwoSets / (double)unionOfTwoSets);
        }
    }
}
