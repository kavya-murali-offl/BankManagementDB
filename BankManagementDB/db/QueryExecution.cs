using BankManagementDB.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Threading.Tasks;

namespace BankManagementDB.db
{
    public class QueryExecution
    {
        public async Task<DataTable> ExecuteQuery(string query, IDictionary<string, object> parameters)
        {
            DataTable result = null;
            try
            {
                result = new DataTable();
                using (SQLiteConnection conn = new SQLiteConnection("Data source=database.sqlite3"))
                {
                    await conn.OpenAsync();
                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                    {
                        cmd.CommandText = query;
                        if(parameters != null && parameters.Count>0)
                        {
                         foreach (var entry in parameters)
                                cmd.Parameters.AddWithValue(entry.Key, entry.Value);
                        }
                        await cmd.ExecuteNonQueryAsync();
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            result.Load(reader);    
                        }
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return result;
        }

        public async Task ExecuteNonQuery(string query, IDictionary<string, object> parameters)
        {
            try
            {
                using (SQLiteConnection con = new SQLiteConnection("Data source=database.sqlite3"))
                {
                    using (SQLiteCommand com = new SQLiteCommand(con))
                    {
                        await con.OpenAsync();
                        com.CommandText = query;
                        if (parameters != null && parameters.Count > 0)
                        {
                            foreach (var entry in parameters)
                                com.Parameters.AddWithValue("@" + entry.Key, entry.Value);
                        }
                        await com.ExecuteNonQueryAsync();
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
