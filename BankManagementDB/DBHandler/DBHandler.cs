using BankManagementDB.Interface;
using BankManagementDB.Model;
using BankManagementDB.Models;
using BankManagementDB.View;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankManagementDB.DBHandler
{
    public class DBHandler : IDBHandler
    {
        public DBHandler(IDatabaseAdapter databaseAdapter)
        {
            DatabaseAdapter = databaseAdapter;
            CreateTables();
        }

        public IDatabaseAdapter DatabaseAdapter { get; set; }


        // Customer

        public async Task<List<Customer>> GetCustomer(Guid id) => await DatabaseAdapter.GetAll<Customer>().Where(customer => customer.ID.Equals(id)).ToListAsync();

        public async Task<bool> InsertCustomer(Customer customer)
        {
            int rowsModified = await DatabaseAdapter.Insert(customer);
            if (rowsModified > 0) return true;
            else return false;
        }

        public async Task<bool> UpdateCustomer(Customer customer)
        {
            int rowsModified = await DatabaseAdapter.Update(customer);
            if (rowsModified > 0) return true;
            else return false;
        }

        public Task<List<Customer>> GetCustomer(string phoneNumber) => DatabaseAdapter.GetAll<Customer>().Where(customer => customer.Phone.Equals(phoneNumber)).ToListAsync();


        // Customer Credentials

        public async Task<List<CustomerCredentials>> GetCredentials(Guid customerID) => await DatabaseAdapter.GetAll<CustomerCredentials>().Where(x => x.ID == customerID).ToListAsync();

        public async Task<bool> InsertCredentials(CustomerCredentials customerCredentials)
        {
            int rowsModified = await DatabaseAdapter.Insert<CustomerCredentials>(customerCredentials);
            if (rowsModified > 0) return true;
            return false;
        }

        public async Task<bool> UpdateCredentials(CustomerCredentials customerCredentials)
        {
            int rowsModified = await DatabaseAdapter.Update<CustomerCredentials>(customerCredentials);
            if (rowsModified > 0) return true;
            return false;
        }


        // Account

        public async Task<List<Account>> GetAccounts(Guid userID) => await DatabaseAdapter.GetAll<Account>().Where(x => x.UserID == userID).OrderByDescending(x => x.CreatedOn).ToListAsync();

        public async Task<bool> InsertAccount(Account account)
        {
            try
            {
                int rowsModified = await DatabaseAdapter.Insert<Account>(account);
                if (rowsModified > 0) return true;
            }
            catch(Exception ex)
            {
                Notification.Error(ex.ToString());
            }
           
            return false;
        }

        public async Task<bool> UpdateAccount(Account account)
        {
            int rowsModified = await DatabaseAdapter.Update(account);
            if (rowsModified > 0) return true;
            return false;
        }


        // Card

        public async Task<IEnumerable<CardBObj>> GetCard(Guid customerID) => await DatabaseAdapter.Query<CardBObj>($"Select * from Card Inner Join CreditCard on Card.ID = CreditCard.ID where CustomerID = '{customerID}'");

        public async Task<List<CardBObj>> GetCard(string cardNumber) => await DatabaseAdapter.GetAll<CardBObj>().Where(x => x.CardNumber.Equals(cardNumber)).ToListAsync();

        public async Task<bool> InsertCard(Card card)
        {
            int rowsModified = await DatabaseAdapter.Insert<Card>(card);
            if (rowsModified > 0) return true;
            return false;
        }

        public async Task<bool> UpdateCard(Card card)
        {
            int rowsModified = await DatabaseAdapter.Update<Card>(card);
            if (rowsModified > 0) return true;
            return false;
        }


        // Credit Card

        public async Task<bool> InsertCreditCard(CreditCard creditCard)
        {
            int rowsModified = await DatabaseAdapter.Insert(creditCard);
            if (rowsModified > 0) return true;
            return false;
        }

        public async Task<bool> UpdateCreditCard(CreditCard creditCard)
        {
            int rowsModified = await DatabaseAdapter.Update(creditCard);
            if (rowsModified > 0) return true;
            return false;
        }


        // Transaction

        public async Task<IList<Transaction>> GetTransaction(Guid accountID) => await DatabaseAdapter.GetAll<Transaction>().Where(x => x.AccountID == accountID).OrderByDescending(x => x.RecordedOn).ToListAsync();

        public async Task<IList<Transaction>> GetTransaction(string cardNumber) => await DatabaseAdapter.GetAll<Transaction>().Where(x => x.CardNumber == cardNumber).OrderByDescending(x => x.RecordedOn).ToListAsync();

        public async Task<bool> InsertTransaction(Transaction transaction)
        {
            int rowsModified = await DatabaseAdapter.Insert(transaction);
            if (rowsModified > 0) return true;
            return false;
        }

        public async Task<bool> UpdateTransaction(Transaction transaction)
        {
            int rowsModified = await DatabaseAdapter.Update<Transaction>(transaction);
            if (rowsModified > 0) return true;
            return false;
        }

        // Create tables

        public async void CreateTables()
        {
            await DatabaseAdapter.CreateTable<Customer>();
            await DatabaseAdapter.CreateTable<CustomerCredentials>();
            await DatabaseAdapter.CreateTable<Card>();
            await DatabaseAdapter.CreateTable<Account>();
            await DatabaseAdapter.CreateTable<Transaction>();
            await DatabaseAdapter.CreateTable<CreditCard>();
        }

    }
}
