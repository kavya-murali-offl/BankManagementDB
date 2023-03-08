using BankManagementDB.Interface;
using BankManagementDB.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankManagementDB.DatabaseAdapter
{
    public class SQLiteDBAdapter : IDatabaseAdapter
    {
        public SQLiteDBAdapter() {
            SQLiteConnectionString connectionString = GetConnectionString();
            Connection = new SQLiteAsyncConnection(connectionString);
        }

        private SQLiteAsyncConnection Connection { get; set; }

        private SQLiteConnectionString GetConnectionString() =>  new SQLiteConnectionString(Environment.GetEnvironmentVariable("DATABASE_PATH"), true, key: Environment.GetEnvironmentVariable("DATABASE_PASSWORD"));

        public async Task CreateTable<T>() where T : new() => await Connection.CreateTableAsync<T>();

        public async Task<int> Insert<T>(T instance) => await Connection.InsertOrReplaceAsync(instance, typeof(T));

        public async Task<int> Update<T>(T instance) => await Connection.InsertOrReplaceAsync(instance, typeof(T));

        public AsyncTableQuery<T> GetAll<T>() where T : new() => Connection.Table<T>();

        public async Task<IEnumerable<T>> Query<T>(string query) where T : new() => await Connection.QueryAsync<T>(query);
    }
}
