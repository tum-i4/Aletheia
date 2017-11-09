using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectralizer.Clustering.FaultLocalization.SimilarityMetrics
{
    public class Dstar : IRankingStrategy
    {
        public double calculateSuspiciousness(int coveredFailed, int uncoveredFailed, int coveredPassed, int uncoveredPassed)
        {
            if (coveredPassed + uncoveredFailed <= 0) return Int32.MaxValue;
            double result = ((double)coveredFailed / ((double)uncoveredFailed + (double)coveredPassed));

            return result;
        }
    }
}
