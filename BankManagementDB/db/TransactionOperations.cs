using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Threading.Tasks;
using BankManagement.Model;
using BankManagementDB.Utility;
using BankManagement.Enums;
using BankManagementDB.Interface;
//;
namespace BankManagementDB.db
{
    public class TransactionOperations : IQueryOperations<Transaction>
    {

        private static readonly string connectionString = "Data source=database.sqlite3";

        public async Task<bool> UpdateOrInsert(Transaction transaction)
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
                        cmd.Parameters.AddWithValue("@RecordedOn", DateTimeHelper.GetEpoch(transaction.RecordedOn));

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch (Exception err)
            { Console.WriteLine(err.Message); }
            return result;
        }

        public async Task<IList<Transaction>> Get(string accountID)
        {
            IList<Transaction> transactions = new List<Transaction>();
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
                        {
                            while(reader.Read())
                            {
                                var transaction = new Transaction()
                                {
                                    ID = new Guid(reader.GetString(0)),
                                    Balance= reader.GetDecimal(1),
                                    Amount = reader.GetDecimal(2),    
                                    TransactionType = (TransactionTypes)Enum.Parse(typeof(TransactionTypes),reader.GetString(3)),
                                    Description = reader.GetString(4),  
                                    AccountID = new Guid(reader.GetString(5)),  
                                    RecordedOn = DateTimeHelper.ConvertEpochToDateTime(reader.GetInt64(6)),
                                };
                                transactions.Add(transaction);
                            }
                        }
                    }
                }
            }
            catch (Exception err)
            { Console.WriteLine(err); }
            return transactions;
        }
    }
}
