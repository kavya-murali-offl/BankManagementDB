using BankManagementDB.Entities;
using BankManagementDB.Model;
using BankManagementDB.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankManagementDB.Interface
{
    public interface IDBHandler
    {

        Task<bool> RunInTransaction(IList<Action> actions);

        // Customer
        Task<bool> InsertCustomer(Customer customer);

        Task<bool> UpdateCustomer(Customer customer);

        public Task<List<Customer>> GetCustomer(string phoneNumber);

        
        // Customer Credentials

        Task<List<CustomerCredentials>> GetCredentials(string customerID);

        Task<bool> InsertCredentials(CustomerCredentials customerCredentials);

        Task<bool> UpdateCredentials(CustomerCredentials customerCredentials);


        // Account
        Task<IList<CurrentAccount>> GetCurrentAccounts(string userID);

        Task<IList<SavingsAccount>> GetSavingsAccounts(string userID);

        Task<bool> InsertAccount(Account account);

        Task<bool> UpdateAccount(Account account);

        Task<bool> InsertCurrentAccount(CurrentAccountDTO account);

        Task<bool> InsertSavingsAccount(SavingsAccountDTO account);


        // Debit Card
        Task<IEnumerable<DebitCard>> GetDebitCardByCustomerID(string customerID);

        Task<bool> InsertDebitCard(DebitCardDTO card);

        Task<bool> UpdateDebitCard(DebitCardDTO card);



        // Credit Card
        Task<IEnumerable<CreditCard>> GetCreditCardByCustomerID(string customerID);


        Task<bool> InsertCreditCard(CreditCardDTO creditCard);

        Task<bool> UpdateCreditCard(CreditCardDTO creditCard);

        // Card
        Task<bool> InsertCard(Card card);

        Task<bool> UpdateCard(Card card);


        // Transaction

        Task<IList<Transaction>> GetTransactionByAccountNumber(string accountNumber);

        Task<IList<Transaction>> GetTransactionByCardNumber(string cardNumber);

        Task<bool> InsertTransaction(Transaction transaction);

        Task<bool> UpdateTransaction(Transaction transaction);

        // Create Table

        void CreateTables();
    }
}
