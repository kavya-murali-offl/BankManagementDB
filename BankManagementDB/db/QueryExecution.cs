using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SQLite;
using System.Reflection;
using System.Runtime.Remoting.Messaging;

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
                            //if (reader.HasRows)

                            //    for (int i = 0; i < reader.FieldCount; i++)
                            //    {
                            //        result.Columns.Add(new DataColumn(reader.GetName(i)));
                            //    }

                            //int j = 0;
                            //while (reader.Read())
                            //{
                            //    DataRow row = result.NewRow();
                            //    result.Rows.Add(row);

                            //    for (int i = 0; i < reader.FieldCount; i++)
                            //    {
                            //        Console.WriteLine(reader.GetValue(i));
                            //        result.Rows[j][i] = (reader.GetValue(i));
                            //    }

                            //    j++;
                            //}
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
                                com.Parameters.AddWithValue(entry.Key, entry.Value);
                        }
                        com.ExecuteNonQuery();
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
