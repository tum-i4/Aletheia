using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aletheia.WorksheetParser.import
{
    /// <summary>
    /// Base class for CsvSheetReader. Further improvement possible
    /// </summary>
    public abstract class AWorksheetReader
    {
        protected string path;
        /// <summary>
        /// constructor initializes the member variable
        /// </summary>
        /// <param name="path">Location of the CSV to be read</param>
        public AWorksheetReader(string path)
        {
            this.path = path;
        }
        /// <summary>
        /// abstract ParseSheet() to be implemented in inherited class
        /// </summary>
        public abstract void parseSheet();
        /// <summary>
        /// abstract getDataTable() to be implemented in inherited class
        /// </summary>
        /// <returns></returns>
        public abstract DataTable getDataTable();
    }
}
