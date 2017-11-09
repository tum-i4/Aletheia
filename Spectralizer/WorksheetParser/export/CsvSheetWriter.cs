using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectralizer.WorksheetParser.export
{
    public class CsvSheetWriter : AWorksheetWriter
    {
        private char separator;
        private string dest;

        private DataTable target;

        public CsvSheetWriter(string dest, char separator, DataTable target)
        {
            this.separator = separator;
            this.dest = dest;
            this.target = target;
        }

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
