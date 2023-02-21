using BankManagement.Models;
using BankManagementDB.Enums;
using System;

namespace BankManagementDB.Interface
{
    public interface IATMTransactionServices
    {

        event Action<string> BalanceChanged;

        bool Deposit(decimal amount, Account account, ModeOfPayment modeOfPayment);

        bool Withdraw(decimal amount, Account account, ModeOfPayment modeOfPayment);

        bool Transfer(decimal amount, Account account, Guid transferAccountID, ModeOfPayment modeOfPayment);

    }
}