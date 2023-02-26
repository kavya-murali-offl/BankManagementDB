using BankManagementCipher.Model;
using BankManagementDB.Interface;
using SQLite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankManagementDB.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly SQLiteConnectionString Options = new SQLiteConnectionString(Environment.GetEnvironmentVariable("DATABASE_PATH"),
   true, key: Environment.GetEnvironmentVariable("DATABASE_PASSWORD"));

        public async Task<IList<AccountDTO>> Get(Guid userID)
        {
            IList<AccountDTO> accounts = new List<AccountDTO>();

            try
            {
                SQLiteAsyncConnection connection = new SQLiteAsyncConnection(Options);
                {
                    var result = connection.Table<AccountDTO>().Where(x => x.UserID == userID).OrderByDescending(x => x.CreatedOn);
                    accounts = result.ToListAsync().Result;
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
