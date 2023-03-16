using BankManagementDB.Data;
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

        public async Task<bool> RunInTransaction(IList<Action> actions) => await DatabaseAdapter.RunInTransaction(actions);


        public IDatabaseAdapter DatabaseAdapter { get; set; }

        // Customer

        public async Task<bool> InsertCustomer(Customer customer) => await DatabaseAdapter.Insert(customer);

        public async Task<bool> UpdateCustomer(Customer customer) => await DatabaseAdapter.Update(customer);

        public Task<List<Customer>> GetCustomer(string phoneNumber) => DatabaseAdapter.GetAll<Customer>().Where(customer => customer.Phone.Equals(phoneNumber)).ToListAsync();

        // Customer Credentials

        public async Task<List<CustomerCredentials>> GetCredentials(string customerID) => await DatabaseAdapter.GetAll<CustomerCredentials>().Where(x => x.ID == customerID).ToListAsync();

        public async Task<bool> InsertCredentials(CustomerCredentials customerCredentials) => await DatabaseAdapter.Insert<CustomerCredentials>(customerCredentials);

        public async Task<bool> UpdateCredentials(CustomerCredentials customerCredentials) => await DatabaseAdapter.Update<CustomerCredentials>(customerCredentials);

        // Account

        public async Task<List<Account>> GetAccounts(string userID) => await DatabaseAdapter.GetAll<Account>().Where(x => x.UserID == userID).OrderByDescending(x => x.CreatedOn).ToListAsync();

        public async Task<bool> InsertAccount(Account account) => await DatabaseAdapter.Insert<Account>(account);

        public async Task<bool> UpdateAccount(Account account) => await DatabaseAdapter.Update(account);

        //Card

        public async Task<IEnumerable<CreditCard>> GetCreditCardByCustomerID(string customerID) => await
                DatabaseAdapter.Query<CreditCard>($"Select * from Card Inner Join CreditCard on Card.ID = CreditCard.ID where CustomerID = '{customerID}'");
        
        public async Task<IEnumerable<DebitCard>> GetDebitCardByCustomerID(string customerID) => await DatabaseAdapter.Query<DebitCard>($"Select * from Card  Inner Join DebitCard on Card.ID = DebitCard.ID where CustomerID = '{customerID}'");

        // Card

        public async Task<bool> InsertCard(Card card) => await DatabaseAdapter.Insert<Card>(card);

        public async Task<bool> UpdateCard(Card card)  => await DatabaseAdapter.Update<Card>(card); // Credit Card

        public async Task<bool> InsertCreditCard(CreditCardDTO creditCard) => await DatabaseAdapter.Insert(creditCard);

        public async Task<bool> UpdateCreditCard(CreditCardDTO creditCard) => await DatabaseAdapter.Update(creditCard);

        public async Task<bool> InsertDebitCard(DebitCardDTO creditCard) => await DatabaseAdapter.Insert(creditCard);

        public async Task<bool> UpdateDebitCard(DebitCardDTO creditCard) => await DatabaseAdapter.Update(creditCard);

        // Transaction

        public async Task<IList<Transaction>> GetTransactionByAccountNumber(string accountNumber) => await DatabaseAdapter.GetAll<Transaction>().Where(x => x.FromAccountNumber.Equals(accountNumber) || x.ToAccountNumber.Equals(accountNumber)).OrderByDescending(x => x.RecordedOn).ToListAsync();

        public async Task<IList<Transaction>> GetTransactionByCardNumber(string cardNumber) => await DatabaseAdapter.GetAll<Transaction>().Where(x => x.CardNumber == cardNumber).OrderByDescending(x => x.RecordedOn).ToListAsync();

        public async Task<bool> InsertTransaction(Transaction transaction) => await DatabaseAdapter.Insert(transaction);

        public async Task<bool> UpdateTransaction(Transaction transaction) => await DatabaseAdapter.Update<Transaction>(transaction);

        // Create tables

        public async void CreateTables()
        {
            await DatabaseAdapter.CreateTable<Customer>();
            await DatabaseAdapter.CreateTable<CustomerCredentials>();
            await DatabaseAdapter.CreateTable<Card>();
            await DatabaseAdapter.CreateTable<Account>();
            await DatabaseAdapter.CreateTable<Transaction>();
            await DatabaseAdapter.CreateTable<CreditCardDTO>();
            await DatabaseAdapter.CreateTable<DebitCardDTO>();
        }

    }
}
