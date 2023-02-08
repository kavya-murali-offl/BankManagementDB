using BankManagement.Models;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.SQLite;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankManagement.Model;
//;
namespace BankManagementDB.db
{
    public class TransactionOperations
    {

        private static string connectionString = "Data source=database.sqlite3";


        public async static Task<bool> Upsert(Transaction transaction)
        {
            bool result = false;
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(connectionString))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(con))
                    {
                        await con.OpenAsync();
                        cmd.CommandText = @"INSERT INTO Transactions(
                                    ID, Balance, Amount, TransactionType, Description, AccountID, RecordedOn) 
                                    VALUES(@ID, @Balance, @Amount, @TransactionType, @Description, @AccountID, @RecordedOn)
                                    ON CONFLICT(ID) 
                                    DO UPDATE SET 
                                    Balance = @Balance";

                        cmd.Parameters.AddWithValue("@ID", transaction.ID.ToString());
                        cmd.Parameters.AddWithValue("@Balance", transaction.Balance);
                        cmd.Parameters.AddWithValue("@Amount", transaction.Amount);
                        cmd.Parameters.AddWithValue("@TransactionType", transaction.TransactionType.ToString());
                        cmd.Parameters.AddWithValue("@Description", transaction.Description);
                        cmd.Parameters.AddWithValue("@AccountID", transaction.AccountID.ToString());
                        cmd.Parameters.AddWithValue("@RecordedOn", transaction.RecordedOn);

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

        public async static Task<DataTable> Get(string accountID)
        {
            DataTable dataTable = new DataTable();
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(connectionString))
                {
                    await conn.OpenAsync();
                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                    {
                        string query = @"Select ID, Balance, Amount, TransactionType, Description, AccountID, RecordedOn FROM Transactions WHERE AccountID = @AccountID";
                        cmd.CommandText = query;

                        cmd.Parameters.AddWithValue("AccountID", accountID);

                        await cmd.ExecuteNonQueryAsync();
                        using (var reader = await cmd.ExecuteReaderAsync())
                            dataTable.Load(reader);

                        cmd.Dispose();
                    }
                    conn.Close();
                }
            }
            catch (Exception err)
            { Console.WriteLine(err); }
            return dataTable;
        }
    }
}
