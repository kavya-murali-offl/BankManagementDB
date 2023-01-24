using BankManagement.Enums;
using BankManagement.Model;
using BankManagement.Models;

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
                        CurrentAccount currentAccount = new CurrentAccount
                        {
                            Type = AccountTypes.CURRENT,
                            InterestRate = 0m
                        };
                        return currentAccount;
                    }
                case AccountTypes.SAVINGS:
                    {
                        SavingsAccount savingsAccount = new SavingsAccount
                        {
                            Type = AccountTypes.SAVINGS,
                            InterestRate = 5.6m
                        };
                        return savingsAccount;
                    }
                default:
                    return null;
            }
        }
    }
}
