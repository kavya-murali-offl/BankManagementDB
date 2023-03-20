using BankManagementDB.Data;
using BankManagementDB.Entities;
using BankManagementDB.EnumerationType;
using BankManagementDB.Interface;
using BankManagementDB.Model;
using BankManagementDB.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankManagementDB.DatabaseHandler
{
    public class DBHandler : IDBHandler
    {
        public DBHandler(IDatabaseAdapter databaseAdapter)
        {
            DatabaseAdapter = databaseAdapter;
            CreateTables();
        }

        private IDatabaseAdapter DatabaseAdapter { get; set; }

        public async Task<bool> RunInTransaction(IList<Action> actions) => await DatabaseAdapter.RunInTransaction(actions);


        // Customer

        public async Task<bool> InsertCustomer(Customer customer) => await DatabaseAdapter.Insert(customer);

        public async Task<bool> UpdateCustomer(Customer customer) => await DatabaseAdapter.Update(customer);

        public Task<List<Customer>> GetCustomer(string phoneNumber) => DatabaseAdapter.GetAll<Customer>().Where(customer => customer.Phone.Equals(phoneNumber)).ToListAsync();

        // Customer Credentials

        public async Task<List<CustomerCredentials>> GetCredentials(string customerID) => await DatabaseAdapter.GetAll<CustomerCredentials>().Where(x => x.ID == customerID).ToListAsync();

        public async Task<bool> InsertCredentials(CustomerCredentials customerCredentials) => await DatabaseAdapter.Insert(customerCredentials);

        public async Task<bool> UpdateCredentials(CustomerCredentials customerCredentials) => await DatabaseAdapter.Update(customerCredentials);

        // Account

        public async Task<IList<CurrentAccount>> GetCurrentAccounts(string userID) =>
            await DatabaseAdapter.GetAll<CurrentAccount>().Where(x => x.UserID.Equals(userID)).OrderByDescending(x => x.CreatedOn).ToListAsync();
       
        public async Task<IList<SavingsAccount>> GetSavingsAccounts(string userID) => await DatabaseAdapter.GetAll<SavingsAccount>().Where(x => x.UserID == 
        userID).OrderByDescending(x => x.CreatedOn).ToListAsync();

        public async Task<bool> InsertAccount(Account account) => await DatabaseAdapter.Insert(account);

        public async Task<bool> UpdateAccount(Account account) => await DatabaseAdapter.Update(account);

        public async Task<bool> InsertCurrentAccount(CurrentAccountDTO account) => await DatabaseAdapter.Insert(account);

        public async Task<bool> UpdateSavingsAccount(SavingsAccountDTO account) => await DatabaseAdapter.Update(account);

        public async Task<bool> InsertSavingsAccount(SavingsAccountDTO account) => await DatabaseAdapter.Insert(account);

        public async Task<bool> UpdateCurrentAccount(CurrentAccountDTO account) => await DatabaseAdapter.Update(account);


        //Card

        public async Task<bool> InsertCard(Card card) => await DatabaseAdapter.Insert(card);

        public async Task<bool> UpdateCard(Card card)  => await DatabaseAdapter.Update(card); 

        
        // Credit Card

        public async Task<bool> InsertCreditCard(CreditCardDTO creditCard) => await DatabaseAdapter.Insert(creditCard);

        public async Task<bool> UpdateCreditCard(CreditCardDTO creditCard) => await DatabaseAdapter.Update(creditCard);

        public async Task<IEnumerable<CreditCard>> GetCreditCardByCustomerID(string customerID) =>
             await DatabaseAdapter.Query<CreditCard>($"Select * from Card Inner Join CreditCard on Card.ID = CreditCard.ID where CustomerID = ?", customerID);


        // Debit Card

        public async Task<bool> InsertDebitCard(DebitCardDTO creditCard) => await DatabaseAdapter.Insert(creditCard);

        public async Task<bool> UpdateDebitCard(DebitCardDTO creditCard) => await DatabaseAdapter.Update(creditCard);

        public async Task<IEnumerable<DebitCard>> GetDebitCardByCustomerID(string customerID) =>
           await DatabaseAdapter.Query<DebitCard>($"Select * from Card  Inner Join DebitCard on Card.ID = DebitCard.ID where CustomerID = ?", customerID);


        // Transaction

        public async Task<IList<Transaction>> GetTransactionByAccountNumber(string accountNumber) => await DatabaseAdapter.GetAll<Transaction>().Where(x => x.FromAccountNumber.Equals(accountNumber) || x.ToAccountNumber.Equals(accountNumber)).OrderByDescending(x => x.RecordedOn).ToListAsync();

        public async Task<IList<Transaction>> GetTransactionByCardNumber(string cardNumber) => await DatabaseAdapter.GetAll<Transaction>().Where(x => x.CardNumber == cardNumber).OrderByDescending(x => x.RecordedOn).ToListAsync();

        public async Task<bool> InsertTransaction(Transaction transaction) => await DatabaseAdapter.Insert(transaction);

        public async Task<bool> UpdateTransaction(Transaction transaction) => await DatabaseAdapter.Update(transaction);

        // Create tables

        public async void CreateTables()
        {
            await DatabaseAdapter.CreateTable<Customer>();
            await DatabaseAdapter.CreateTable<CustomerCredentials>();
            await DatabaseAdapter.CreateTable<Card>();
            await DatabaseAdapter.CreateTable<Account>();
            await DatabaseAdapter.CreateTable<CurrentAccountDTO>();
            await DatabaseAdapter.CreateTable<SavingsAccountDTO>();
            await DatabaseAdapter.CreateTable<Transaction>();
            await DatabaseAdapter.CreateTable<CreditCardDTO>();
            await DatabaseAdapter.CreateTable<DebitCardDTO>();
        }

    }
}
