using BankManagementDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.Interface
{
    public interface IGetAccountDataManager
    {
        IList<Account> GetAllAccounts(Guid id);

        Account GetAccount(string accountNumber);

        IList<Account> GetAccountsList();

    }
}
