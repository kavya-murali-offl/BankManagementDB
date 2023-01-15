using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;
using System.Data;
using BankManagement.Models;

namespace BankManagementDB.db
{
    public static class Database
    {
        public static bool InsertRowToTable(string tableName, IDictionary<string,object> parameters)
        {
            bool result = false;
            var s = 0;
            try
            {
                string query = "INSERT INTO " + tableName + " ('";
                foreach (KeyValuePair<string, object> pair in parameters)
                {
                    query += pair.Key + "', '";
                }
                query += " VALUES (";
                foreach(KeyValuePair<string, object> pair in parameters)
                {
                    query += '@' + pair.Key;
                }
                query += ")";
                QueryExecution queryExecution = new QueryExecution();
               
                queryExecution.ExecuteNonQuery(query, parameters);
                result = true;
            }
            catch (Exception err)
            {
                Console.WriteLine(err);
            }
            return result;
        }

        public static DataTable FillTable(string tableName, IDictionary<string, object> parameters)
        {
            DataTable result = new DataTable();
            try
            {
                string query = "SELECT * FROM "+ tableName;
                if(parameters != null && parameters.Count> 0)
                {
                    query += " WHERE ";
                    foreach (KeyValuePair<string, object> pairs in parameters)
                    {
                        query += pairs.Key + "= @" + pairs.Key;
                    }
                }
                QueryExecution queryExecution = new QueryExecution();
                DataTable Table = queryExecution.ExecuteQuery(query, parameters);
                result = Table;
            }
            catch (Exception err)
            {
                Console.WriteLine(err);
            }
            finally
            {
                Console.WriteLine("FillingTable");
                PrintDataTable(result);
            }
            return result;
        }

        public static bool UpdateTable(string tableName, IDictionary<string, object> updateFields)
        {
            bool result = false;
            try
            {
                string query = "UPDATE "+ tableName + " SET ";

                foreach (KeyValuePair<string, object> pairs in updateFields)
                {
                    query += pairs.Key + "= @" + pairs.Key;
                }

                query += " WHERE ID = @ID";
                QueryExecution queryExecution = new QueryExecution();
                queryExecution.ExecuteNonQuery(query, updateFields);
                result = true;
            }
            catch (Exception err)
            { Console.WriteLine(err.Message); }
            return result;
        }

        public static void PrintDataTable(DataTable AccountsTable)
        {
            Console.WriteLine();
            Dictionary<string, int> colWidths = new Dictionary<string, int>();

            foreach (DataColumn col in AccountsTable.Columns)
            {
                Console.Write(col.ColumnName);
                var maxLabelSize = AccountsTable.Rows.OfType<DataRow>()
                        .Select(m => (m.Field<object>(col.ColumnName)?.ToString() ?? "").Length)
                        .OrderByDescending(m => m).FirstOrDefault();

                colWidths.Add(col.ColumnName, maxLabelSize);
                for (int i = 0; i < maxLabelSize - col.ColumnName.Length + 10; i++) Console.Write(" ");
            }

            Console.WriteLine();

            foreach (DataRow dataRow in AccountsTable.Rows)
            {
                for (int j = 0; j < dataRow.ItemArray.Length; j++)
                {
                    Console.Write(dataRow.ItemArray[j]);
                    for (int i = 0; i < colWidths[AccountsTable.Columns[j].ColumnName] - dataRow.ItemArray[j].ToString().Length + 10; i++) Console.Write(" ");
                }
                Console.WriteLine();
            }
        }

    }
}
