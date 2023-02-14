using BankManagement.Models;
using BankManagementCipher.Model;
using BankManagementDB.Interface;
using SQLite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankManagementDB.db
{
    
    public class AccountOperations : IQueryServices<AccountDTO>
    {
        private readonly SQLiteConnectionString Options = new SQLiteConnectionString(@"C:\Users\kavya-pt6688\source\repos\BankManagementDB\BankManagementDB\Database.sqlite3", true, key: "pass");

        public async Task<IList<AccountDTO>> Get(Guid userID)
        {
            IList<AccountDTO> accounts = new List<AccountDTO>();

            try
            {
                SQLiteAsyncConnection connection = new SQLiteAsyncConnection(Options);
                {

                    accounts = await connection.QueryAsync<AccountDTO>("Select * from Account where UserID = ? ORDER BY CreatedOn Desc", userID);
                    await connection.CloseAsync();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return accounts;

        }

        public async Task<bool> InsertOrReplace(AccountDTO accountDTO)
        {
            SQLiteAsyncConnection connection = new SQLiteAsyncConnection(Options);
            int rowsModified = await connection.InsertOrReplaceAsync(accountDTO);
            await connection.CloseAsync();
            if (rowsModified > 0) return true;
            return false;
        }
    }
}
