using BankManagementDB.EnumerationType;
using BankManagementDB.Model;
using BankManagementDB.Models;
using BankManagementDB.Interface;

namespace BankManagementDB.Controller
{
    public class AccountFactory : IAccountFactory
    {
        public Account GetAccountByType(AccountType accountType)
        {
            switch (accountType)
            {
                case AccountType.CURRENT:
                    {
                        CurrentAccount currentAccount = new CurrentAccount();
                        return currentAccount;
                    }
                case AccountType.SAVINGS:
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
