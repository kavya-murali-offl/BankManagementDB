using BankManagementCipher.Model;
using BankManagementDB.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.Repository
{
    public static class DBRepository
    {
        private readonly static SQLiteConnectionString Options = new SQLiteConnectionString(Environment.GetEnvironmentVariable("DATABASE_PATH"),
   true, key: Environment.GetEnvironmentVariable("DATABASE_PASSWORD"));

        public static async void CreateTablesIfNotExists()
        {
            try
            {
                SQLiteAsyncConnection connection = new SQLiteAsyncConnection(Options);
                {
                    await connection.CreateTableAsync<CustomerDTO>();
                    await connection.CreateTableAsync<AccountDTO>();
                    await connection.CreateTableAsync<TransactionDTO>();
                    await connection.CreateTableAsync<CardDTO>();

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
