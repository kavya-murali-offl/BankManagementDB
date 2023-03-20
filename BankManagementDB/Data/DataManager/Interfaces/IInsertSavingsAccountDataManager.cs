using BankManagementDB.Entities;
using BankManagementDB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.Data.DataManager
{
    public interface IInsertSavingsAccountDataManager
    {

        bool InsertSavingsAccount(SavingsAccountDTO savingsAccount);
    }
}
