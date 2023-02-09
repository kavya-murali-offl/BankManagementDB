using BankManagement.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Threading.Tasks;
using BankManagementDB.Utility;
using BankManagement.Controller;
using BankManagement.Enums;
using BankManagementDB.Interface;

namespace BankManagementDB.db
{
    
    public class AccountOperations : IQueryOperations<Account>
    {
        private static string connectionString = "Data source=database.sqlite3";

        public async Task<bool> UpdateOrInsert(Account account)
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
                        cmd.Parameters.AddWithValue("@CreatedOn", DateTimeHelper.GetEpoch(account.CreatedOn));
                        
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch (Exception err)
            { Console.WriteLine(err.Message); }
            return result;
        }

        public async Task<IList<Account>> Get(string userID)
        {
            IList<Account> accounts = new List<Account>();
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(connectionString))
                {
                    await conn.OpenAsync();
                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                    {
                        cmd.CommandText = @"Select ID, Balance, MinimumBalance, InterestRate, Status, UserID, Type, CreatedOn FROM Account WHERE UserID = @UserID";

                        cmd.Parameters.AddWithValue("@UserID", userID);

                        await cmd.ExecuteNonQueryAsync();
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                AccountTypes type = (AccountTypes)Enum.Parse(typeof(AccountTypes), reader.GetString(6));
                                Account account = AccountFactory.GetAccountByType(type);
                                account.ID = new Guid(reader.GetString(0));
                                account.Balance = reader.GetDecimal(1);
                                account.MinimumBalance = reader.GetDecimal(2);
                                account.InterestRate = reader.GetDecimal(3);
                                account.Status = (AccountStatus)Enum.Parse(typeof(AccountStatus), reader.GetString(4));
                                account.UserID = new Guid(reader.GetString(5));
                                account.Type = type;
                                account.CreatedOn = DateTimeHelper.ConvertEpochToDateTime(reader.GetInt64(7));
                                accounts.Add(account);
                            }
                        }
                    }
                }
            }
            catch (Exception err)
            { Console.WriteLine(err.Message); }
            return accounts;
        }

    }
}
