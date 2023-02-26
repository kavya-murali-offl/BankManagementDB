using BankManagementDB.Models;
using BankManagementDB.EnumerationType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.Interface
{
    public interface ITransactionProcessController
    {
        event Action<string> BalanceChanged;

        bool Deposit(decimal amount, Account account, ModeOfPayment modeOfPayment);

        bool Withdraw(decimal amount, Account account, ModeOfPayment modeOfPayment);

        bool Transfer(decimal amount, Account account, Guid transferAccountID, ModeOfPayment modeOfPayment);

    }
}
