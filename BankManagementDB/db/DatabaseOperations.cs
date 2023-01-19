using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;

namespace BankManagementDB.db
{
    public class DatabaseOperations
    {
        public static void CreateTableIfNotExists()
        {
            string customerQuery = @"CREATE TABLE IF NOT EXISTS Customer (
                                    'ID' INTEGER NOT NULL UNIQUE, 
                                    'Name' TEXT, 
                                    'Phone' TEXT UNIQUE,
                                    'Email' TEXT,
                                    'HashedPassword' TEXT,
                                     'LastLoginOn' TEXT,
                                     'CreatedOn' TEXT,  
                                    PRIMARY KEY('ID' AUTOINCREMENT));";

            string accountQuery = @"CREATE TABLE IF NOT EXISTS 'Account' (
                                 'ID' INTEGER NOT NULL UNIQUE,
                                 'Balance' DECIMAL, 
                                 'InterestRate' DECIMAL, 
                                 'Status' TEXT, 
                                 'MinimumBalance' DECIMAL, 
                                 'Charges' DECIMAL, 
                                 'Type' TEXT, 
                                 'CreatedOn' TEXT, 
                                 'UserID' INTEGER NOT NULL,
                                 PRIMARY KEY('ID' AUTOINCREMENT))";

            string transactionsQuery = @"CREATE TABLE IF NOT EXISTS 'Transactions' (
                                      'ID' INTEGER PRIMARY KEY,
                                      'Balance' DECIMAL,
                                      'RecordedOn' TEXT,
                                      'Amount' DECIMAL,
                                      'TransactionType' TEXT,
                                      'Description' TEXT,
                                      'AccountID' INTEGER,
                                      FOREIGN KEY (AccountID) REFERENCES Account(ID)
                                      );";

            QueryExecution queryExecution = new QueryExecution();
            queryExecution.ExecuteNonQuery(customerQuery, null);
            queryExecution.ExecuteNonQuery(accountQuery, null);
            queryExecution.ExecuteNonQuery(transactionsQuery, null);
        }

        public static bool InsertRowToTable(string tableName, IDictionary<string, object> parameters)
        {
            bool result = false;
            try
            {
                string query = @"INSERT INTO " + tableName + " ('";
                for (int i = 0; i < parameters.Keys.Count(); i++)
                {
                    query += parameters.Keys.ElementAt<string>(i);
                    if (i != parameters.Keys.Count() - 1)
                    {
                        query += "', '";
                    }
                }
                query += "') ";
                query += " VALUES (";
                for (int i = 0; i < parameters.Keys.Count(); i++)
                {
                    query += " @" + parameters.Keys.ElementAt<string>(i);
                    if (i != parameters.Keys.Count() - 1)
                    {
                        query += ", ";
                    }
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
                string query = "SELECT * FROM " + tableName;
                if (parameters != null && parameters.Count > 0)
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
            return result;
        }

        public static bool UpdateTable(string tableName, IDictionary<string, object> updateFields)
        {
            bool result = false;
            try
            {
                string query = "UPDATE " + tableName + " SET ";

                foreach (KeyValuePair<string, object> pairs in updateFields)
                {
                    if (pairs.Key != "ID")
                        query += pairs.Key + "= @" + pairs.Key;
                }

                query += " WHERE ID = @ID";
                QueryExecution queryExecution = new QueryExecution();
                queryExecution.ExecuteNonQuery(query, updateFields);
                return true;
            }
            catch (Exception err)
            { Console.WriteLine(err.Message); }
            return result;
        }

        public static void PrintDataTable(DataTable Table)
        {
            Console.WriteLine(" ");
            Dictionary<string, int> colWidths = new Dictionary<string, int>();

            foreach (DataColumn col in Table.Columns)
            {
                Console.Write(col.ColumnName);
                var maxLabelSize = Table.Rows.OfType<DataRow>()
                        .Select(m => (m.Field<object>(col.ColumnName)?.ToString() ?? "").Length)
                        .OrderByDescending(m => m).FirstOrDefault();

                colWidths.Add(col.ColumnName, maxLabelSize);
                for (int i = 0; i < maxLabelSize - col.ColumnName.Length + 10; i++) Console.Write(" ");
            }

            Console.WriteLine();

            foreach (DataRow dataRow in Table.Rows)
            {
                for (int j = 0; j < dataRow.ItemArray.Length; j++)
                {
                    Console.Write(dataRow.ItemArray[j]);
                    for (int i = 0; i < colWidths[Table.Columns[j].ColumnName] - dataRow.ItemArray[j].ToString().Length + 10; i++) Console.Write(" ");
                }
                Console.WriteLine();
            }
        }

    }
}
