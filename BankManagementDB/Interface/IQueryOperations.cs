using BankManagement.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankManagementDB.Interface
{
    public interface IQueryServices<T>
    {
        Task<bool> InsertOrReplace(T t);

        Task<IList<T>> Get(Guid id);
    }

    public interface IAuthenticationServices
    {
        string GetHashedPassword(string phoneNumber);

    }
}
