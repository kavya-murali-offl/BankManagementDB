using BankManagementCipher.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.db
{
    public class CipherOperations
    {
        private static readonly SQLiteConnectionString Options = new SQLiteConnectionString(@"C:\Users\kavya-pt6688\source\repos\BankManagementDB\BankManagementDB\Database.sqlite3", true, key: "pass");

        public static async void CreateTablesIfNotExists()
        {
            try
            {
                SQLiteAsyncConnection connection = new SQLiteAsyncConnection(Options);
                {
                    await connection.CreateTableAsync<CustomerDTO>();
                    await connection.CreateTableAsync<AccountDTO>();
                    await connection.CreateTableAsync<TransactionDTO>();

                    await connection.CloseAsync();

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
