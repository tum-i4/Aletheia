using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aletheia.WorksheetParser.import
{
    /// <summary>
    /// Little customized class to read HitSpectra
    /// This class is created as the HitSpectra file has header row to be processed separately
    /// </summary>
    public class HitSpectraCsvSheetReader
    {
        private char separator;

        private string path;

        private List<string> linesOfTheSheet;
        private DataTable table;
        private string[] columnNames;
        /// <summary>
        /// the constructor initializes member variables
        /// </summary>
        /// <param name="path">Path to HitSpectra</param>
        /// <param name="separator">Separator used in the HitSpectra</param>
        public HitSpectraCsvSheetReader(string path, char separator)
        {
            this.path = path;
            linesOfTheSheet = new List<string>();
            this.separator = separator;
        }
        /// <summary>
        /// reads the HitSpectra file and add the lines to the data row
        /// </summary>
        private void readFile()
        {
            string line;
            int counter = 0;

            StreamReader file = new StreamReader(path);

            while ((line = file.ReadLine()) != null)
            {
                linesOfTheSheet.Add(line);
                counter++;
            }

            file.Close();
        }
        /// <summary>
        /// get method to access member varible
        /// </summary>
        /// <returns>Returns data table</returns>
        public DataTable getDataTable()
        {
            return table;
        }
        /// <summary>
        /// Parse the HitSpectra
        /// use the first line to generate data table template\
        /// then parse the rest of the lines
        /// </summary>
        public void parseSheet()
        {
            if (File.Exists(path))
            {
                readFile();
            }
            else
            {
                throw new Exception();
            }

            generateDataTable(linesOfTheSheet.ElementAt(0));

            for (int i = 1; i < linesOfTheSheet.Count; i++)
            {
                parseLine(linesOfTheSheet.ElementAt(i));
            }
        }
        /// <summary>
        /// Parse a single line and put it to data row
        /// </summary>
        /// <param name="line">Line to be parsed from HitSpectra</param>
        private void parseLine(string line)
        {
            string[] values = line.Split(separator);
            DataRow row = table.NewRow();

            for (int i = 0; i < values.Length; i++)
            {
                string value = values[i];
                string columnName = columnNames[i];
      
                if (table.Columns[columnName].DataType == Type.GetType("System.Int32"))
                {
                    row[columnName] = Int32.Parse(value);
                }
                else if (table.Columns[columnName].DataType == Type.GetType("System.String"))
                {
                    row[columnName] = value;
                }
            }

            table.Rows.Add(row);
        }
        /// <summary>
        /// generates data table with the column names from the first line from HitSpectra
        /// </summary>
        /// <param name="line">Line containing column names</param>
        private void generateDataTable(string line)
        {
            table = new DataTable();

            columnNames = line.Split(separator);

            DataColumn col = new DataColumn();
            col.DataType = Type.GetType("System.String");
            col.ColumnName = columnNames[0].Trim();
            table.Columns.Add(col);

            for (int i = 1; i < columnNames.Length; i++)
            {
                DataColumn column = new DataColumn();
                column.DataType = Type.GetType("System.Int32");
                column.ColumnName = columnNames[i].Trim();
                if (table.Columns.Contains(column.ColumnName))
                {
                    column.ColumnName += "_duplicate_at_" + i;
                    columnNames[i] += "_duplicate_at_" + i;
                }
                    
                table.Columns.Add(column);
            }
        }
    }
}
