using BankManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.Interface
{
    public interface ITransactionServices
    {
        bool Deposit(decimal amount, Account account);
        bool Withdraw(decimal amount, Account account);
        bool Transfer(decimal amount, Account account, long transferAccountID);

    }
}
