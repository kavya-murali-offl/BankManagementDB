using BankManagement.Enums;
using BankManagement.Model;
using BankManagement.Models;
using BankManagement.Utility;
using System;

namespace BankManagement.Controller
{
    public static class AccountFactory
    {

        public static Account CreateAccountByType(AccountTypes accountType)
        {
            switch (accountType)
            {
                case AccountTypes.CURRENT:
                    {
                        CurrentAccount currentAccount = new CurrentAccount();
                        Helper helper = new Helper();
                        decimal amount = helper.GetAmount();
                        currentAccount.Balance = amount;
                        currentAccount.Type = AccountTypes.CURRENT;
                        return currentAccount;
                    }
                case AccountTypes.SAVINGS:
                    {
                        SavingsAccount savingsAccount = new SavingsAccount();
                        savingsAccount.Type = AccountTypes.SAVINGS;
                        return savingsAccount;
                    }
                default:
                    return null;
            }
             
        }

        public static Account GetAccountByType(AccountTypes accountType)
        {
            switch (accountType)
            {
                case AccountTypes.CURRENT:
                    {
                        CurrentAccount currentAccount = new CurrentAccount();
                        return currentAccount;
                    }
                case AccountTypes.SAVINGS:
                    {
                        SavingsAccount savingsAccount = new SavingsAccount();
                        return savingsAccount;
                    }
                default:
                    return null;
            }

        }
    }
}
