using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectralizer.Clustering.FaultLocalization.SimilarityMetrics
{
    public interface IRankingStrategy
    {
        double calculateSuspiciousness(int coveredFailed, int uncoveredFailed, int coveredPassed, int uncoveredPassed);
    }
}
