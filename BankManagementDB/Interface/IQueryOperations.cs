using BankManagement.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankManagementDB.Interface
{
    public interface IQueryOperations<T>
    {
        Task<bool> UpdateOrInsert(T t);

        Task<IList<T>> Get(string query);
    }
}
