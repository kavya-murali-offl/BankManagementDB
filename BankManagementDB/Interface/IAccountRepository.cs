using BankManagementCipher.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.Interface
{
    public interface IAccountRepository
    {

        Task<bool> InsertOrReplace(AccountDTO accountDTO);

        Task<IList<AccountDTO>> Get(Guid id);
    }
}
