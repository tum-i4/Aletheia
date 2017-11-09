using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectralizer.Clustering.ClusterComparison
{
    public interface IComparingStrategy
    {
        double calculateSimilarityofTwoCluster(int unionOfTwoSets, int intersectionOfTwoSets);
    }
}
