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

        Task<bool> Insert<T>(T obj);

        Task<bool> Update<T>(T obj);

        Task<IEnumerable<T>> Query<T>(string query, params object[] args) where T : new();

        Task<bool> RunInTransaction(IList<Action> actions);

    }
}
