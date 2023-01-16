using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SQLite;
using System.Reflection;

namespace BankManagementDB.db
{
    public class QueryExecution
    {
        public DataTable ExecuteQuery(string query, IDictionary<string, object> parameters)
        {
            DataTable result = null;
            try
            {
                result = new DataTable();
                using (SQLiteConnection con = new SQLiteConnection("Data source=database.sqlite3"))
                {
                    con.Open();
                    using (SQLiteCommand com = new SQLiteCommand(con))
                    {
                        com.CommandText = query;
                        if(parameters != null && parameters.Count>0)
                        {
                         foreach (KeyValuePair<string, object> entry in parameters)
                            com.Parameters.AddWithValue(entry.Key, entry.Value);
                        }
                        com.ExecuteNonQuery();
                        using (SQLiteDataReader reader = com.ExecuteReader())
                        {
                            result.Load(reader);    
                        }
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return result;
        }

        public void ExecuteNonQuery(string query, IDictionary<string, object> parameters)
        {
            try
            {
                using (SQLiteConnection con = new SQLiteConnection("Data source=database.sqlite3"))
                {
                    using (SQLiteCommand com = new SQLiteCommand(con))
                    {
                        con.Open();
                        com.CommandText = query;
                        if (parameters != null && parameters.Count > 0)
                        {
                            foreach (KeyValuePair<string, object> entry in parameters)
                                com.Parameters.AddWithValue("@"+entry.Key, entry.Value);
                        }
                        var result = com.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
