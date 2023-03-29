using BankManagementDB.Config;
using BankManagementDB.Interface;
using Microsoft.Extensions.Configuration;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BankManagementDB.DatabaseAdapter
{
    public class SQLiteDatabaseAdapter : IDatabaseAdapter
    {
        public SQLiteDatabaseAdapter()
        {
            SQLiteConnectionString connectionString = GetConnectionString();
            Connection = new SQLiteAsyncConnection(connectionString);
        }

        private SQLiteAsyncConnection Connection { get; set; }

        private SQLiteConnectionString GetConnectionString(){
            string databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "BankDB.db3");
            return new SQLiteConnectionString(databasePath, true, key: Environment.GetEnvironmentVariable("DATABASE_PASSWORD"));
        }

        public async Task CreateTable<T>() where T : new() => await Connection.CreateTableAsync<T>();

        public async Task<bool> Insert<T>(T instance) => await Connection.InsertOrReplaceAsync(instance, typeof(T)) > 0;

        public async Task<bool> Update<T>(T instance) => await Connection.InsertOrReplaceAsync(instance, typeof(T)) > 0;

        public AsyncTableQuery<T> GetAll<T>() where T : new() => Connection.Table<T>();

        public async Task<IEnumerable<T>> Query<T>(string query, params object[] args) where T : new() => await Connection.QueryAsync<T>(query, args);

        public async Task<bool> RunInTransaction(IList<Action> actions)
        {
            await Connection.RunInTransactionAsync(tran =>
            {
                foreach (Action action in actions)
                    action();
            });
            return true;
        }
    }
}
