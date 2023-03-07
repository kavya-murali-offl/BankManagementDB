using SQLite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.Interface
{
    public interface IDatabaseAdapter
    {
        Task CreateTable<T>() where T : new();

        AsyncTableQuery<T> GetAll<T>() where T : new();

        Task<int> Insert<T>(T obj);

        Task<int> Update<T>(T obj);

        Task<IEnumerable<T>> ExecuteQuery<T>(string query) where T : new();

    }
}
