using MathWorks.MATLAB.NET.Arrays;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aletheia.Toolbox
{
    /// <summary>
    /// enumeration of the test case execution result
    /// </summary>
    public enum Result
    {
        Passed, Failed
    }
    /// <summary>
    /// A class to hold some utility function
    /// </summary>
    public class Tools
    {
        /// <summary>
        /// converts the data table to jagged array
        /// </summary>
        /// <param name="dataTable">HitSpectra data table</param>
        /// <returns></returns>
        public static double[][] buildJaggedDoubleArray(DataTable dataTable)
        {
            int dimCol = dataTable.Columns.Count - 1;
            int dimRow = dataTable.Rows.Count;
            double[][] table = new double[dimRow][];

            for (int i = 0; i < dimRow; i++)
            {
                double[] tmpRow = new double[dimCol];
                DataRow row = dataTable.Rows[i];

                for (int j = 1; j <= dimCol; j++)   // starting with index 1 to leave out the first column with the testcase-names
                {
                    int value = (int)row[j];
                    tmpRow[j - 1] = value;
                }

                table[i] = tmpRow;
            }

            return table;
        }
        /// <summary>
        /// separates the test case with the given result and put them to jagged array
        /// </summary>
        /// <param name="result">Result for separation</param>
        /// <param name="testcaseList">testcase spectra</param>
        /// <returns></returns>
        public static double[][] seperateTestcasesByResultJaggedDoubleArray(Result result, double[][] testcaseList)
        {
            int res;
            if (result == Result.Passed) res = 1;
            else res = 0;

            List<int> tmpList = new List<int>();

            int dim1 = testcaseList.GetLength(0);
            int dim2 = testcaseList[0].GetLength(0);

            for (int i = 0; i < dim1; i++)
            {
                if (res == (int)testcaseList[i][dim2 - 1])
                {
                    tmpList.Add(i);
                }
            }

            int dim = tmpList.Count;

            double[][] table = new double[dim][];

            for (int i = 0; i < dim; i++)
            {
                int index = tmpList.ElementAt(i);
                double[] tmpRow = new double[dim2];

                for (int j = 0; j < dim2; j++)
                {
                    tmpRow[j] = testcaseList[index][j];
                }

                table[i] = tmpRow;
            }

            return table;
        }
        /// <summary>
        /// converts MatLab MW array to C# compatible jagged array
        /// </summary>
        /// <param name="mwArray">Matlab MW array</param>
        /// <returns></returns>
        public static double[][] buildJaggedDoubleArrayFromMWArray(MWArray mwArray)
        {
            Array tmpArray = mwArray.ToArray();

            int rank = tmpArray.Rank;
            int[] dimension = new int[rank];

            for (int i = 0; i < rank; i++)
            {
                dimension[i] = tmpArray.GetLength(i);
            }

            double[][] doubleArray = new double[dimension[0]][];

            for (int i = 0; i < dimension[0]; i++)
            {
                double[] tmpArrayLine = new double[dimension[1]];
                for (int j = 0; j < dimension[1]; j++)
                {
                    tmpArrayLine[j] = (double)tmpArray.GetValue(i, j);
                }
                doubleArray[i] = tmpArrayLine;
            }

            return doubleArray;
        }
        /// <summary>
        /// creates 2D array from hitSpectra data table
        /// </summary>
        /// <param name="dataTable">HitSpectra Data table</param>
        /// <returns></returns>
        public static double[,] buildTwoDimensionalDoubleArray(DataTable dataTable)
        {
            int dimCol = dataTable.Columns.Count - 1;
            int dimRow = dataTable.Rows.Count;
            double[,] table = new double[dimRow, dimCol];

            for (int i = 0; i < dimRow; i++)
            {
                DataRow row = dataTable.Rows[i];

                for (int j = 1; j <= dimCol; j++)   // starting with index 1 to leave out the first column with the testcase-names
                {
                    int value = (int)row[j];
                    table[i, j - 1] = value;
                }
            }

            return table;
        }
        /// <summary>
        /// Separates testcases by the given result and create 2D array from the separated test case spectra
        /// </summary>
        /// <param name="result">Result for separation</param>
        /// <param name="testcaseList">Test Case Spectra</param>
        /// <returns></returns>
        public static double[,] seperateTestcasesByResultTwoDimensionalDoubleArray(Result result, double[,] testcaseList)
        {
            int res;
            if (result == Result.Passed) res = 1;
            else res = 0;

            List<int> tmpList = new List<int>();

            int dim1 = testcaseList.GetLength(0);
            int dim2 = testcaseList.GetLength(1);

            for (int i = 0; i < dim1; i++)
            {
                if (res == (int)testcaseList[i, dim2 - 1])  // we assume, that the last column is the 'Result'-column
                {
                    tmpList.Add(i);
                }
            }

            int dim = tmpList.Count;

            double[,] table = new double[dim, dim2];

            for (int i = 0; i < dim; i++)
            {
                int index = tmpList.ElementAt(i);

                for (int j = 0; j < dim2; j++)
                {
                    table[i, j] = testcaseList[index, j];
                }
            }

            return table;
        }
        /// <summary>
        /// creates 2D array from Matlab MW array
        /// </summary>
        /// <param name="mwArray">Matlab MW array</param>
        /// <returns></returns>
        public static double[,] buildTwoDimensionalDoubleArrayFromMWArray(MWArray mwArray)
        {
            Array tmpArray = mwArray.ToArray();

            int rank = tmpArray.Rank;
            int[] dimension = new int[rank];


            for (int i = 0; i < rank; i++)
            {
                dimension[i] = tmpArray.GetLength(i);
            }

            double[,] doubleArray = new double[dimension[0], dimension[1]];

            for (int i = 0; i < dimension[0]; i++)
            {
                for (int j = 0; j < dimension[1]; j++)
                {
                    doubleArray[i, j] = (double)tmpArray.GetValue(i, j);
                }
            }

            return doubleArray;
        }
        /// <summary>
        /// creates 2D integer array from hit spectra
        /// </summary>
        /// <param name="dataTable">Hit Spectra Data Table</param>
        /// <returns></returns>
        public static int[,] buildTwoDimensionalIntArray(DataTable dataTable)
        {
            int dimCol = dataTable.Columns.Count - 1;
            int dimRow = dataTable.Rows.Count;
            int[,] table = new int[dimRow, dimCol];

            for (int i = 0; i < dimRow; i++)
            {
                DataRow row = dataTable.Rows[i];

                for (int j = 1; j <= dimCol; j++)   // starting with index 1 to leave out the first column with the testcase-names
                {
                    int value = (int)row[j];
                    table[i, j - 1] = value;
                }
            }

            return table;
        }
        /// <summary>
        /// Separates test case spectra by given result and then create 2D integer array with the
        /// separated test case spectra
        /// </summary>
        /// <param name="result">Result for separation</param>
        /// <param name="testcaseList">Test Case Spectra</param>
        /// <returns></returns>
        public static int[,] seperateTestcasesByResultTwoDimensionalIntArray(Result result, int[,] testcaseList)
        {
            int res;
            if (result == Result.Passed) res = 1;
            else res = 0;

            List<int> tmpList = new List<int>();

            int dim1 = testcaseList.GetLength(0);
            int dim2 = testcaseList.GetLength(1);

            for (int i = 0; i < dim1; i++)
            {
                if (res == (int)testcaseList[i, dim2 - 1])  // we assume, that the last column is the 'Result'-column
                {
                    tmpList.Add(i);
                }
            }

            int dim = tmpList.Count;

            int[,] table = new int[dim, dim2];

            for (int i = 0; i < dim; i++)
            {
                int index = tmpList.ElementAt(i);

                for (int j = 0; j < dim2; j++)
                {
                    table[i, j] = testcaseList[index, j];
                }
            }

            return table;
        }
        /// <summary>
        /// Merge 2 2D array into one
        /// </summary>
        /// <param name="array1">First Integer array</param>
        /// <param name="array2">Second Integer array</param>
        /// <returns></returns>
        public static int[,] mergeTwoTwoDimensionalIntArrays(int[,] array1, int[,] array2)
        {
            List<int[]> tmpList = new List<int[]>();
            tmpList.AddRange(Tools.transformTwoDimensionalArrayToJaggedArray(array1));
            tmpList.AddRange(Tools.transformTwoDimensionalArrayToJaggedArray(array2));
            int[,] mergedList = Tools.transformJaggedArrayToTwoDimensionalArray(tmpList.ToArray());

            return mergedList;
        }
        /// <summary>
        /// create a dictionary wthe the testcase name and id
        /// </summary>
        /// <param name="dataTable">HitSpectra Data Table</param>
        /// <returns></returns>
        public static Dictionary<int, string> buildIdList(DataTable dataTable)
        {
            Dictionary<int, string> idList = new Dictionary<int, string>();
            DataColumn testcaseNames = dataTable.Columns[0];

            int i = 1;

            foreach (DataRow testcase in dataTable.Rows)
            {
                string testcaseName = (string)testcase[testcaseNames];
                idList.Add(i, testcaseName);
                i++;
            }

            return idList;
        }
        /// <summary>
        /// create a dictionary wthe the testcase name and id separated by given result
        /// </summary>
        /// <param name="result">Result for Separation</param>
        /// <param name="dataTable">Hit Spectra Data table</param>
        /// <returns></returns>
        public static Dictionary<int, string> buildIdListSeperated(Result result, DataTable dataTable)
        {
            int res;
            if (result == Result.Passed) res = 1;
            else res = 0;

            Dictionary<int, string> idList = new Dictionary<int, string>();
            DataColumn testcaseNames = dataTable.Columns[0];
            DataColumn testcaseResult = dataTable.Columns[dataTable.Columns.Count - 1];

            int i = 1;

            foreach (DataRow testcase in dataTable.Rows)
            {
                if (res == (int)testcase[testcaseResult])
                {
                    string testcaseName = (string)testcase[testcaseNames];
                    idList.Add(i, testcaseName);
                    i++;
                }
            }

            return idList;
        }
        /// <summary>
        /// convert Matlab MW array to one dimentional integer array
        /// </summary>
        /// <param name="mwArray">Matlab MW array</param>
        /// <returns></returns>
        public static int[] buildOneDimensionalIntArray(MWArray mwArray)
        {
            Array tmpArray = mwArray.ToArray();

            int rank = tmpArray.Rank;
            int[] dimension = new int[rank];

            for (int i = 0; i < rank; i++)
            {
                dimension[i] = tmpArray.GetLength(i);
            }

            int[] intArray = new int[dimension[0]];

            for (int i = 0; i < dimension[0]; i++)
            {
                intArray[i] = Convert.ToInt32((double)tmpArray.GetValue(i, 0));
            }

            return intArray;
        }
        /// <summary>
        /// converts double array to integer array
        /// </summary>
        /// <param name="doubleArray">double array to be converted</param>
        /// <returns></returns>
        public static int[,] convertDoubleArrayToIntArray(double[,] doubleArray)
        {
            int dim1 = doubleArray.GetLength(0);
            int dim2 = doubleArray.GetLength(1);

            int[,] intArray = new int[dim1, dim2];

            for (int i = 0; i < dim1; i++)
            {
                for (int j = 0; j < dim2; j++)
                {
                    intArray[i, j] = Convert.ToInt32(doubleArray[i, j]);
                }
            }

            return intArray;
        }
        /// <summary>
        /// creates function names from Hit Spectra Data Table
        /// </summary>
        /// <param name="dataTable">Hit Spectra Data Table</param>
        /// <returns>array of string containing function names</returns>
        public static string[] generateFunctionNamesArray(DataTable dataTable)
        {
            int dim = dataTable.Columns.Count - 2;  // leave out testcase-name and result column
            string[] functionNames = new string[dim];

            for (int i = 1; i < (dataTable.Columns.Count - 1); i++)
            {
                functionNames[i - 1] = dataTable.Columns[i].ColumnName;
            }

            return functionNames;
        }
        /// <summary>
        /// separates the test cases using cluster id
        /// </summary>
        /// <param name="cluster">list of cluster id</param>
        /// <param name="testcaseList">List of test cases</param>
        /// <returns></returns>
        public static Dictionary<int, List<int[]>> allocateTestcasesIntoClusters(int[] cluster, int[,] testcaseList)
        {
            Dictionary<int, List<int[]>> clusters = new Dictionary<int, List<int[]>>();
            int[][] testcaseListJagged = transformTwoDimensionalArrayToJaggedArray(testcaseList);

            for (int i = 0; i < cluster.Length; i++)
            {
                int clusterNumber = cluster[i];

                if (!clusters.ContainsKey(clusterNumber))
                {
                    List<int[]> listOfTestcases = new List<int[]>();
                    clusters.Add(clusterNumber, listOfTestcases);
                }
                List<int[]> tmpList = clusters[clusterNumber];
                tmpList.Add(testcaseListJagged[i]);
            }

            return clusters;
        }
        /// <summary>
        /// convert integer array to double array
        /// </summary>
        /// <param name="a">Integer array</param>
        /// <param name="len">Lenght of the array</param>
        /// <returns></returns>
        public static double[] intArrayToDoubleArray(int []a, int len)
        {
            double []d = new double[len];
        
            for(int i=0;i<len;i++)
            {
                d[i] = a[i];
            }
            return d;
        }

        /// <summary>
        /// convert 2D array to JaggedArray
        /// </summary>
        /// <param name="twoDArray">2D array to be converted</param>
        /// <returns></returns>
        public static int[][] transformTwoDimensionalArrayToJaggedArray(int[,] twoDArray)
        {
            int[][] jaggedArray = new int[twoDArray.GetLength(0)][];

            for (int i = 0; i < twoDArray.GetLength(0); i++)
            {
                int[] tmpArray = new int[twoDArray.GetLength(1)];
                for (int j = 0; j < twoDArray.GetLength(1); j++)
                {
                    tmpArray[j] = twoDArray[i, j];
                }
                jaggedArray[i] = tmpArray;
            }

            return jaggedArray;
        }
        /// <summary>
        /// convert jagged array to 2D array
        /// </summary>
        /// <param name="jaggedArray">Jagged array to be converted</param>
        /// <returns></returns>
        public static int[,] transformJaggedArrayToTwoDimensionalArray(int[][] jaggedArray)
        {
            int[,] twoDArray = new int[jaggedArray.GetLength(0), jaggedArray[0].GetLength(0)];

            for (int i = 0; i < twoDArray.GetLength(0); i++)
            {
                int[] tmpArray = jaggedArray[i];
                for (int j = 0; j < twoDArray.GetLength(1); j++)
                {
                    twoDArray[i, j] = tmpArray[j];
                }
            }

            return twoDArray;
        }
        /// <summary>
        /// convert the hitSpectra data table to binary table
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public static DataTable buildBinaryDataTable(DataTable dataTable)
        {
            DataTable dt = new DataTable();

            foreach (DataColumn dc in dataTable.Columns)
            {
                DataColumn column = new DataColumn();
                column.DataType = dc.DataType;
                column.ColumnName = dc.ColumnName;
                column.DefaultValue = 0;

                dt.Columns.Add(column);
            }

            foreach (DataRow dr in dataTable.Rows)
            {
                DataRow dataRow = dt.NewRow();

                foreach (DataColumn dc in dataTable.Columns)
                {
                    if (dc.DataType.Equals(Type.GetType("System.Int32")))
                    {
                        int value = (int)dr[dc];

                        if (value > 0) value = 1;

                        dataRow[dc.ColumnName] = value;
                    }
                    else
                    {
                        dataRow[dc.ColumnName] = dr[dc];
                    }
                }
                dt.Rows.Add(dataRow);
            }

            return dt;
        }
    }
}
