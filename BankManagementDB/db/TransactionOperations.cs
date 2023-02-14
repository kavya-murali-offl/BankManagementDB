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

namespace BankManagementDB.db
{
    public class TransactionOperations : IQueryServices<TransactionDTO>
    {
        private readonly SQLiteConnectionString Options = new SQLiteConnectionString(@"C:\Users\kavya-pt6688\source\repos\BankManagementDB\BankManagementDB\Database.sqlite3", true, key: "pass");

        public async Task<IList<TransactionDTO>> Get(Guid accountID)
        {

            try
            {
                SQLiteAsyncConnection connection = new SQLiteAsyncConnection(Options);
                {
                    IList<TransactionDTO> transactionDTOs = await connection.QueryAsync<TransactionDTO>("Select * from Transactions where AccountID = ?", accountID);
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
