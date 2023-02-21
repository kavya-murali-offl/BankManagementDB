using BankManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.Interface
{
    public interface IAccountServices
    {
        bool InsertAccount(Account account);

        bool UpdateAccount(Account account);

        public void GetAllAccounts(Guid id);

    }
}
