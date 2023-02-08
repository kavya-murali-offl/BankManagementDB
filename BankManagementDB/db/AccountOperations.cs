using BankManagement.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;

namespace BankManagementDB.db
{
    
    public class AccountOperations
    {
        private static string connectionString = "Data source=database.sqlite3";

        public async static Task<bool> Upsert(Account account)
        {
            bool result = false;
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(connectionString))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(con))
                    {
                        await con.OpenAsync();
                        cmd.CommandText = @"INSERT INTO Account(
                                    ID, Balance, MinimumBalance, InterestRate, Status, UserID, Type, CreatedOn) 
                                    VALUES(@ID, @Balance, @MinimumBalance, @InterestRate, @Status, @UserID, @Type, @CreatedOn)
                                    ON CONFLICT(ID)
                                    DO UPDATE SET 
                                    Balance = @Balance";

                        cmd.Parameters.AddWithValue("@ID", account.ID.ToString());
                        cmd.Parameters.AddWithValue("@Balance", account.Balance);
                        cmd.Parameters.AddWithValue("@MinimumBalance", account.MinimumBalance);
                        cmd.Parameters.AddWithValue("@InterestRate", account.InterestRate);
                        cmd.Parameters.AddWithValue("@Status", account.Status.ToString());
                        cmd.Parameters.AddWithValue("@UserID", account.UserID.ToString());
                        cmd.Parameters.AddWithValue("@Type", account.Type.ToString());
                        cmd.Parameters.AddWithValue("@CreatedOn", account.CreatedOn);
                        
                        await cmd.ExecuteNonQueryAsync();
                        cmd.Dispose();
                    }
                    con.Close();
                }
                return true;
            }
            catch (Exception err)
            { Console.WriteLine(err.Message); }
            return result;
        }

        public async static Task<DataTable> Get(string userID)
        {
            DataTable dataTable = new DataTable();
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(connectionString))
                {
                    await conn.OpenAsync();
                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                    {
                        cmd.CommandText = @"Select ID, Balance, MinimumBalance, InterestRate, Status, UserID, Type, CreatedOn FROM Account WHERE UserID = @UserID";

                        cmd.Parameters.AddWithValue("UserID", userID);

                        await cmd.ExecuteNonQueryAsync();
                        using (var reader = await cmd.ExecuteReaderAsync())
                            dataTable.Load(reader);

                        cmd.Dispose();
                    }
                    conn.Close();
                }
            }
            catch (Exception err)
            { Console.WriteLine(err.Message); }
            return dataTable;
        }

    }
}
