using BankManagementCipher.Model;
using BankManagementDB.Interface;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.Repository
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly SQLiteConnectionString Options = new SQLiteConnectionString(Environment.GetEnvironmentVariable("DATABASE_PATH"),
    true, key: Environment.GetEnvironmentVariable("DATABASE_PASSWORD"));

        public async Task<IList<TransactionDTO>> Get(Guid accountID)
        {

            try
            {
                SQLiteAsyncConnection connection = new SQLiteAsyncConnection(Options);
                {
                    var result = connection.Table<TransactionDTO>().Where(x => x.AccountID == accountID).OrderByDescending(x => x.RecordedOn);
                    IList<TransactionDTO> transactionDTOs = result.ToListAsync().Result;
                    await connection.CloseAsync();

                    return transactionDTOs;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return new List<TransactionDTO>();

        }

        public async Task<bool> InsertOrReplace(TransactionDTO transactionDTO)
        {
            SQLiteAsyncConnection connection = new SQLiteAsyncConnection(Options);
            int rowsModified = await connection.InsertOrReplaceAsync(transactionDTO);
            await connection.CloseAsync();
            if (rowsModified > 0) return true;
            return false;
        }
    }
}
