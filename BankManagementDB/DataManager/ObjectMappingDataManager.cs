using BankManagementDB.Model;
using BankManagementDB.Models;
using BankManagementDB.Utility;
using BankManagementCipher.Model;
using BankManagementDB.Config;
using BankManagementDB.EnumerationType;
using BankManagementDB.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace BankManagementDB.Controller
{
    public class ObjectMappingDataManager : IObjectMappingDataManager
    {

        public ObjectMappingDataManager()
        {
            AccountFactory = DependencyContainer.ServiceProvider.GetRequiredService<IAccountFactory>();
        }

        public IAccountFactory AccountFactory { get; private set; } 



        public Account DtoToAccount(AccountDTO accountDTO)
        {
            var accountType = Helper.StringToEnum<AccountType>(accountDTO.Type);
            var account = AccountFactory.GetAccountByType(accountType);

            account.ID = accountDTO.ID;
            account.Balance = accountDTO.Balance;
            account.InterestRate = accountDTO.InterestRate;
            account.Status = Helper.StringToEnum<AccountStatus>(accountDTO.Status);
            account.Type = accountType;
            account.MinimumBalance = accountDTO.MinimumBalance;
            account.CreatedOn = accountDTO.CreatedOn;
            account.UserID = accountDTO.UserID;
            account.AccountNumber = accountDTO.AccountNumber;

            return account;
        }

        public AccountDTO AccountToDto(Account account)
        {
            return new AccountDTO()
            {
                ID = account.ID,
                Balance = account.Balance,
                InterestRate = account.InterestRate,
                Status = account.Status.ToString(),
                Type = account.Type.ToString(),
                MinimumBalance = account.MinimumBalance,
                CreatedOn = account.CreatedOn,
                UserID = account.UserID,
                AccountNumber = account.AccountNumber
        };
        }


    }
}
