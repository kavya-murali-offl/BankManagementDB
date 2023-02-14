using BankManagement.Controller;
using BankManagement.Enums;
using BankManagement.Model;
using BankManagement.Models;
using BankManagement.Utility;
using BankManagementCipher.Model;


namespace BankManagementCipher.Utility
{
    public class Mapping
    {
        public static Customer DtoToCustomer(CustomerDTO customerDTO)
        {
           return new Customer()
           {
                ID = customerDTO.ID,
                Name = customerDTO.Name,
                Age = customerDTO.Age,
                LastLoggedOn = customerDTO.LastLoggedOn,
                CreatedOn = customerDTO.CreatedOn,
                Phone = customerDTO.Phone,
                Email = customerDTO.Email
           };
        }

        public static CustomerDTO CustomerToDto(Customer customer, string password)
        {
            return new CustomerDTO()
            {
                ID = customer.ID,
                Name = customer.Name,
                Age = customer.Age,
                HashedPassword = password,
                LastLoggedOn = customer.LastLoggedOn,
                CreatedOn = customer.CreatedOn,
                Phone = customer.Phone,
                Email = customer.Email
            };
        }

        public static Account DtoToAccount(AccountDTO accountDTO)
        {
            var accountType = Helper.StringToEnum<AccountTypes>(accountDTO.Type);
            var account = AccountFactory.GetAccountByType(accountType);

            account.ID = accountDTO.ID;
            account.Balance = accountDTO.Balance;
            account.InterestRate = accountDTO.InterestRate;
            account.Status = Helper.StringToEnum<AccountStatus>(accountDTO.Status);
            account.Type = accountType;
            account.MinimumBalance = accountDTO.MinimumBalance;
            account.CreatedOn = accountDTO.CreatedOn;
            account.UserID = accountDTO.UserID;

            return account;
        }

        public static AccountDTO AccountToDto(Account account)
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
            };
        }

        public static Transaction DtoToTransaction(TransactionDTO transactionDTO)
        {
            return new Transaction()
            {
                ID = transactionDTO.ID,
                Balance = transactionDTO.Balance,
                Description = transactionDTO.Description,
                AccountID = transactionDTO.AccountID,
                TransactionType = Helper.StringToEnum<TransactionTypes>(transactionDTO.TransactionType),
                Amount = transactionDTO.Amount,
                RecordedOn = transactionDTO.RecordedOn,
            };
        }

        public static TransactionDTO TransactionToDto(Transaction transaction)
        {
            return new TransactionDTO()
            {
                ID = transaction.ID,
                Balance = transaction.Balance,
                Description = transaction.Description,
                AccountID = transaction.AccountID,
                TransactionType = transaction.TransactionType.ToString(),
                Amount = transaction.Amount,
                RecordedOn = transaction.RecordedOn,
            };
        }
    }
}
