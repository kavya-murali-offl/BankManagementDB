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
                        CurrentAccount currentAccount  = new CurrentAccount();
                        currentAccount.InterestRate = Constants.AccountConstants.CURRENT_INTEREST_RATE;

                        return currentAccount;
                    }
                case AccountType.SAVINGS:
                    {
                        SavingsAccount savingsAccount = new SavingsAccount();
                        savingsAccount.InterestRate = Constants.AccountConstants.SAVINGS_INTEREST_RATE;

                        return savingsAccount;
                    }
                default:
                    return null;
            }
        }
    }
}
