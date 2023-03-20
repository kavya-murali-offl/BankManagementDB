using BankManagementDB.EnumerationType;
using BankManagementDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.Interface
{
    public interface IUpdateAccountDataManager
    {
        bool UpdateAccount(Account updatedAccount);
    }
}
