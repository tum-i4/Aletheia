using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aletheia.WorksheetParser.export
{
    /// <summary>
    /// CsvSheetWriter is a inherited class which holds necessary methods and variables
    /// for writing data table to csv format
    /// </summary>
    public class CsvSheetWriter : AWorksheetWriter
    {
        private char separator;
        private string dest;

        private DataTable target;
        /// <summary>
        /// the constructor sets the value of some member variables
        /// </summary>
        /// <param name="dest">Where to save csv</param>
        /// <param name="separator">The csv data separator</param>
        /// <param name="target">Datatable to be saved as csv</param>
        public CsvSheetWriter(string dest, char separator, DataTable target)
        {
            this.separator = separator;
            this.dest = dest;
            this.target = target;
        }
        /// <summary>
        /// It reads each row from datatable and writes using streamwriter
        /// </summary>
        public void writeToWorkSheet()
        {
            StreamWriter streamWriter = new StreamWriter(dest, false);

            streamWriter.Write(target.Columns[0].ToString());
            for (int i = 1; i < target.Columns.Count; i++)
            {
                streamWriter.Write(separator + target.Columns[i].ToString());
            }

            foreach (DataRow dr in target.Rows)
            {
                streamWriter.Write("\n" + dr[0]);

                for (int i = 1; i < target.Columns.Count; i++)
                {
                    streamWriter.Write(separator + dr[i].ToString());
                }
            }

            streamWriter.Close();
        }
    }
}
