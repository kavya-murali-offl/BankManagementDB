using BankManagement.Model;
using BankManagementDB.Utility;
using BankManagement.Enums;
using BankManagementDB.Interface;
using BankManagement.Models;
using BankManagementCipher.Model;
using BankManagementCipher.Utility;
using SQLite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace BankManagementDB.db
{
    public class TransactionOperations : IQueryServices<TransactionDTO>
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
