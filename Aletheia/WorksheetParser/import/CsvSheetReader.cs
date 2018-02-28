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
    /// CsvSheetReader is inherited from AWorksheetReader
    /// It implements the abstract methods defined in the base class
    /// </summary>
    public class CsvSheetReader : AWorksheetReader
    {
        private char separator;
        private bool separatorAlreadySwitched = false;

        private List<string> linesOfTheSheet;
        private DataTable table;
        /// <summary>
        /// This constructor initializes member variable
        /// </summary>
        /// <param name="path">Path to Csv file</param>
        /// <param name="separator">Data separator for csv file</param>
        public CsvSheetReader(string path, char separator) : base(path)
        {
            linesOfTheSheet = new List<string>();
            this.separator = separator;
        }
        /// <summary>
        /// reads the csv file and calls routine for generating data table
        /// </summary>
        public override void parseSheet()
        {
            if (File.Exists(path))
            {
                readFile();
            }
            else
            {
                throw new FileNotFoundException();
            }

            generateDataTable();

            foreach (string str in linesOfTheSheet)
            {
                parseLine(str);
            }
        }
        /// <summary>
        /// get method
        /// </summary>
        /// <returns>returns the member data table</returns>
        public override DataTable getDataTable()
        {
            return table;
        }
        /// <summary>
        /// method to read csv file and add the lines to linesOfTheSheet
        /// </summary>
        private void readFile()
        {
            string line;
            int counter = 0;

            StreamReader reader = new StreamReader(base.path);

            while ((line = reader.ReadLine()) != null)
            {
                linesOfTheSheet.Add(line);
                counter++;
            }

            reader.Close();

            linesOfTheSheet.RemoveAt(0);
        }
        /// <summary>
        /// parse a single line of csv file and put them to the row of datatable
        /// </summary>
        /// <param name="line">A line from CSV sheet</param>
        private void parseLine(string line)
        {
            string[] words;
            words = line.Split(separator);

            if (words.Length != 3)
            {
                switchSeparator();
                words = line.Split(separator);
            }

            DataRow row;
            row = table.NewRow();
            row[0] = words[1].Trim();
            row[1] = words[2].Trim();
            table.Rows.Add(row);
        }
        /// <summary>
        /// generates blank data table to be filled up by other methods
        /// </summary>
        private void generateDataTable()
        {
            table = new DataTable();    // Creating a new DataTable

            // Add two column objects to the table
            DataColumn fctNameColumn = new DataColumn();
            fctNameColumn.DataType = Type.GetType("System.String");
            table.Columns.Add(fctNameColumn);

            DataColumn numberOfInvocations = new DataColumn();
            numberOfInvocations.DataType = Type.GetType("System.Int32");
            table.Columns.Add(numberOfInvocations);
        }
        /// <summary>
        /// switches separator between comma and semicolon
        /// </summary>
        private void switchSeparator()
        {
            if (separatorAlreadySwitched)
            {
                throw new Exception("Excel Sheet could not be read");
            }
            else
            {
                if (separator == ',')
                {
                    separator = ';';
                }
                else if (separator == ';')
                {
                    separator = ',';
                }
                separatorAlreadySwitched = true;
            }
        }
    }
}
