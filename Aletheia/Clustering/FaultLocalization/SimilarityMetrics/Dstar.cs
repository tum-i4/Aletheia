using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aletheia.Clustering.FaultLocalization.SimilarityMetrics
{
    public class Dstar : IRankingStrategy
    {
        private int star = 1;
        public Dstar()
        {

        }
        public Dstar(int s)
        {
            star = s;
        }
        public double calculateSuspiciousness(int coveredFailed, int uncoveredFailed, int coveredPassed, int uncoveredPassed)
        {
            if (coveredPassed + uncoveredFailed <= 0) return Int32.MaxValue;
            double result = (Math.Pow((double)coveredFailed, star) / ((double)uncoveredFailed + (double)coveredPassed));

            return result;
        }
    }
}
