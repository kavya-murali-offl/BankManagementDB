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
