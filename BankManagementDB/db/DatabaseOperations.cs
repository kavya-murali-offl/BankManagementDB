using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Threading.Tasks;
using BankManagementDB.View;
using BankManagement.Models;
using System.Data.Linq;
using System.Data.SQLite;

namespace BankManagementDB.db
{
    public class DatabaseOperations
    {
        public static void CreateTableIfNotExists()
        {
            string customerQuery = @"CREATE TABLE IF NOT EXISTS Customer (
                                    'ID' VARCHAR UNIQUE PRIMARY KEY NOT NULL, 
                                    'Name' TEXT, 
                                    'Age' INTEGER,
                                    'Phone' TEXT UNIQUE,
                                    'Email' TEXT,
                                    'HashedPassword' TEXT,
                                    'LastLoggedOn' DATETIME DEFAULT CURRENT_TIMESTAMP,
                                    'CreatedOn' DATETIME DEFAULT CURRENT_TIMESTAMP);";
             
            string accountQuery = @"CREATE TABLE IF NOT EXISTS 'Account' (
                                 'ID' VARCHAR NOT NULL UNIQUE,
                                 'Balance' DECIMAL, 
                                 'InterestRate' DECIMAL, 
                                 'Status' TEXT, 
                                 'MinimumBalance' DECIMAL, 
                                 'Type' TEXT, 
                                 'CreatedOn' DATETIME DEFAULT CURRENT_TIMESTAMP, 
                                 'UserID' VARCHAR NOT NULL,
                                 PRIMARY KEY('ID'))";

            string transactionsQuery = @"CREATE TABLE IF NOT EXISTS 'Transactions' (
                                      'ID' VARCHAR NOT NULL UNIQUE,
                                      'Balance' DECIMAL,
                                      'RecordedOn' DATETIME DEFAULT CURRENT_TIMESTAMP,
                                      'Amount' DECIMAL,
                                      'TransactionType' TEXT,
                                      'Description' TEXT,
                                      'AccountID' VARCHAR,
                                      FOREIGN KEY (AccountID) REFERENCES Account(ID)
                                      );";

            QueryExecution queryExecution = new QueryExecution();
            Task task1 = queryExecution.ExecuteNonQuery(customerQuery, null);
            Task task2 = queryExecution.ExecuteNonQuery(accountQuery, null);
            Task task3 = queryExecution.ExecuteNonQuery(transactionsQuery, null);
        }

        public static bool InsertRowToTable(string tableName, IDictionary<string, dynamic> parameters)
        {
            bool result = false;
            try
            {
                string query = @"INSERT INTO " + tableName + " ('";
                for (int i = 0; i < parameters.Keys.Count(); i++)
                {
                    query += parameters.Keys.ElementAt<string>(i);
                    if (i != parameters.Keys.Count() - 1)
                        query += "', '";
                }

                query += "') ";
                query += " VALUES (";

                for (int i = 0; i < parameters.Keys.Count(); i++)
                {
                    query += " @" + parameters.Keys.ElementAt<string>(i);
                    if (i != parameters.Keys.Count() - 1)
                        query += ", ";
                }
                query += ")";

                QueryExecution queryExecution = new QueryExecution();
                if (parameters["TransactionType"] == "DEPOSIT")
                {
                    Task task = queryExecution.ExecuteNonQuery2(query, parameters);
                }
                else
                {
                    Task task = queryExecution.ExecuteNonQuery(query, parameters);
                }
                result = true;
            }
            catch (Exception err)
            {
                Console.WriteLine(err);
            }
            return result;
        }

        public static DataTable FillTable(string tableName, IDictionary<string, dynamic> parameters)
        {
            DataTable result = new DataTable();
            try
            {
                string query = "SELECT * FROM " + tableName;
                if (parameters != null && parameters.Count > 0)
                {
                    query += " WHERE ";
                    foreach (var pairs in parameters)
                        query += pairs.Key + "= @" + pairs.Key;
                }

                QueryExecution queryExecution = new QueryExecution();
                DataTable Table = queryExecution.ExecuteQuery(query, parameters).Result;
                result = Table;
            }
            catch (Exception err)
            {
                Console.WriteLine(err);
            }
            return result;
        }

        public static bool UpdateTable(string tableName, IDictionary<string, dynamic> updateFields)
        {
            bool result = false;
            try
            {
                string query = "UPDATE " + tableName + " SET ";
                int i = 0;
                foreach (var pairs in updateFields)
                {
                    i += 1;
                    if (pairs.Key != "ID")
                        query += pairs.Key + "= @" + pairs.Key;
                    if(i+1 < updateFields.Count) query+= ", "; 
                }
                query += " WHERE ID = @ID";

                QueryExecution queryExecution = new QueryExecution();
                Task task = queryExecution.ExecuteNonQuery(query, updateFields);
                return true;
            }
            catch (Exception err)
            { Console.WriteLine(err.Message); }
            return result;
        }

        public static void PrintDataTable(DataTable Table)
        {
            if (Table.Rows.Count > 0)
            {
                Console.WriteLine(" ");
                Dictionary<string, int> colWidths = new Dictionary<string, int>();

                foreach (DataColumn col in Table.Columns)
                {
                    Console.Write(col.ColumnName);
                    var maxLabelSize = Table.Rows.OfType<DataRow>()
                            .Select(m => (m.Field<dynamic>(col.ColumnName)?.ToString() ?? "").Length)
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
            else
            {
                Notification.Info("No records found.");
            }
        }

        public static void Get()
        {
            var connectionString = "Data Source=database.sqlite";
            using (var db = new DatabaseContext(connectionString))
            {
                //var people = from p in db.Customers
                //             select p;
                //Console.WriteLine(db.);
                //foreach (var person in people)
                //{
                //    Console.WriteLine(person.Name);
                //}
            }
            
        }
    }
}
