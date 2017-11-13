using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aletheia.HitSpectra
{
    /// <summary>
    /// Enumeration of possible results of a test-run
    /// </summary>
    public enum RunResult
    {
        UNDEFINED,
        PASSED,
        FAILED,
        TIMEOUT,
        ERROREXIT
    }
}
