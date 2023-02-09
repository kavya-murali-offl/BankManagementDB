using System;
using System.Collections.Generic;
using System.Data;
using BankManagement.Enums;
using BankManagement.Models;
using BankManagement.View;
using BankManagementDB.db;
using System.Linq;
using BankManagement.Utility;
using BankManagement.Model;
using BankManagementDB.View;
using BankManagementDB.Interface;

namespace BankManagement.Controller
{

    delegate bool UpdateAccountDelegate(Account account);
    delegate bool InsertAccountDelegate(Account account);

    public class AccountsController 
    {
        public AccountsController() { }    

        public AccountsController(ITransactionServices transactionController)
        {
            TransactionController = transactionController;
        }
        public static IList<Account> AccountsList { get; set; }

        public ITransactionServices TransactionController { get; private set; }    


        public bool InsertAccount(Account account)
        {
            bool isAdded = false;
            InsertAccountDelegate insertAccount = InsertAccountToDB;
            if (insertAccount(account))
            {
                insertAccount = InsertAccountToList;
                isAdded = insertAccount(account);
            }
            return isAdded;
        }

        private bool InsertAccountToDB(Account account)
        {
            try
            {
                IQueryOperations<Account> accountOperations = new AccountOperations();
                 return accountOperations.UpdateOrInsert(account).Result;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;   
        }

        

        private bool InsertAccountToList(Account account)
        {
            try
            {
                AccountsList ??= new List<Account>();
                AccountsList.Add(account);
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
        }

        public bool UpdateAccount(Account account)
        {
            try
            {
                UpdateAccountDelegate updateAccount = UpdateAccountInDB;
                bool success = updateAccount(account);
                if (success)
                {
                updateAccount = UpdateAccountInList;
                return updateAccount(account);
                }
            }
            catch(Exception ex) {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        private bool UpdateAccountInDB(Account account)
        {
            try
            {
                IQueryOperations<Account> accountOperations = new AccountOperations();

                return accountOperations.UpdateOrInsert(account).Result;
            }catch(Exception ex)
            {
                return false;   
            }
        }

        private bool UpdateAccountInList(Account updatedAccount)
        {
            try
            {
                Account account = AccountsList.FirstOrDefault(acc => acc.ID == updatedAccount.ID);
                account.Balance = updatedAccount.Balance;
                account.Status = updatedAccount.Status;
                account.InterestRate = updatedAccount.InterestRate;
                return true;
            }
            catch(Exception ex) {
                Console.WriteLine(ex);
            }
            return false;   
        }

        public void FillTable(Guid id)
        {
            try
            {
                IQueryOperations<Account> accountOperations = new AccountOperations();
                AccountsList = accountOperations.Get(id.ToString()).Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public Account GetAccountByQuery(string key, object value)
        {
            Account account = null;
            try
            {
                account = AccountsList.FirstOrDefault(acc => acc.GetType().GetProperty(key).GetValue(acc).Equals(value));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return account;
        }

        public IList<Account> GetAllAccounts() => AccountsList ?? new List<Account>();
    }
}
