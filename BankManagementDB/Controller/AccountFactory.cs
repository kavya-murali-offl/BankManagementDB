﻿using BankManagement.Enums;
using BankManagement.Model;
using BankManagement.Models;
using BankManagement.Utility;
using BankManagement.View;
using System;

namespace BankManagement.Controller
{
    public static class AccountFactory
    {
        
        public static Account GetAccountByType(AccountTypes accountType)
        {
            switch (accountType)
            {
                case AccountTypes.CURRENT:
                    {
                        CurrentAccount currentAccount = new CurrentAccount();
                        currentAccount.Type = AccountTypes.CURRENT;
                        currentAccount.InterestRate = 0m;

                        return currentAccount;
                    }
                case AccountTypes.SAVINGS:
                    {
                        SavingsAccount savingsAccount = new SavingsAccount();
                        savingsAccount.Type = AccountTypes.SAVINGS;
                        savingsAccount.InterestRate = 5.6m;
                        return savingsAccount;
                    }
                default:
                    return null;
            }
        }
    }
}
