using BankManagementCipher.Model;
using BankManagementDB.Model;
using BankManagementDB.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankManagementDB.Interface
{
    public interface IDBHandler
    {
        // Customer
        Task<List<Customer>> GetCustomer(Guid id);

        Task<bool> InsertCustomer(Customer customer);

        Task<bool> UpdateCustomer(Customer customer);

        public Task<List<Customer>> GetCustomer(string phoneNumber);

        
        // Customer Credentials

        Task<List<CustomerCredentials>> GetCredentials(Guid customerID);

        Task<bool> InsertCredentials(CustomerCredentials customerCredentials);

        Task<bool> UpdateCredentials(CustomerCredentials customerCredentials);


        // Account
        Task<List<AccountDTO>> GetAccounts(Guid userID);

        Task<bool> InsertAccount(AccountDTO accountDTO);

        Task<bool> UpdateAccount(AccountDTO accountDTO);


        // Card
        Task<IEnumerable<CardBObj>> GetCard(Guid customerID);

        Task<List<CardBObj>> GetCard(string cardNumber);

        Task<bool> InsertCard(Card card);

        Task<bool> UpdateCard(Card card);

        // Credit Card

        Task<bool> InsertCreditCard(CreditCard creditCard);

        Task<bool> UpdateCreditCard(CreditCard creditCard);


        // Transaction

        Task<IList<Transaction>> GetTransaction(Guid accountID);

        Task<IList<Transaction>> GetTransaction(string cardNumber);

        Task<bool> InsertTransaction(Transaction transaction);

        Task<bool> UpdateTransaction(Transaction transaction);

        // Create Table

        void CreateTables();
    }
}
