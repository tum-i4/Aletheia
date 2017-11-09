using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectralizer.Clustering.FaultLocalization.SimilarityMetrics
{
    public class Ochiai: IRankingStrategy
    {
        public double calculateSuspiciousness(int coveredFailed, int uncoveredFailed, int coveredPassed, int uncoveredPassed)
        {
            double totalFailed = coveredFailed + uncoveredFailed;
            double totalCovered = coveredFailed + coveredPassed;
            double numerator = (double)coveredFailed;
            double denominator = Math.Sqrt(totalFailed * totalCovered);
            double result = numerator/denominator;

            return result;
        }
    }
}
