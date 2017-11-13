using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aletheia.WorksheetParser.import
{
    public abstract class AWorksheetReader
    {
        protected string path;

        public AWorksheetReader(string path)
        {
            this.path = path;
        }

        public abstract void parseSheet();
        public abstract DataTable getDataTable();
    }
}
