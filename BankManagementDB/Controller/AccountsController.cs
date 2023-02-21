using System;
using System.Collections.Generic;
using System.Data;
using BankManagement.Enums;
using BankManagement.Models;
using BankManagementDB.db;
using System.Linq;
using BankManagementDB.View;
using BankManagementDB.Interface;
using BankManagementCipher.Model;
using BankManagementCipher.Utility;

namespace BankManagement.Controller
{

    delegate bool UpdateAccountDelegate(Account account);
    delegate bool InsertAccountDelegate(Account account);

    public class AccountsController : IAccountServices
    {
        public AccountsController() { }    

        public AccountsController(IQueryServices<AccountDTO> accountServices)
        {
            AccountServices = accountServices;
        }

        public static IList<Account> AccountsList { get; set; }

        public IQueryServices<AccountDTO> AccountServices { get; private set; }    

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

        public bool InsertAccountToDB(Account account)
        {
            try
            {
                 return AccountServices.InsertOrReplace(Mapping.AccountToDto(account)).Result;
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
                return AccountServices.InsertOrReplace(Mapping.AccountToDto(account)).Result;
            }catch(Exception ex)
            {
                return false;   
            }
        }

        private bool UpdateAccountInList(Account updatedAccount)
        {
            try
            {
                AccountsList ??= new List<Account>();
                if (AccountsList.Count > 0) { 

                    Account account = AccountsList.FirstOrDefault(acc => acc.ID.Equals(updatedAccount.ID));
                    account.Balance = updatedAccount.Balance;
                    account.Status = updatedAccount.Status;
                    account.InterestRate = updatedAccount.InterestRate;
                }
                return true;

            }
            catch(Exception ex) {
                Console.WriteLine(ex);
            }
            return false;   
        }

        public void GetAllAccounts(Guid id)
        {
            try
            {
                AccountsList ??= new List<Account>();   
                var accountDTOs = AccountServices.Get(id).Result;

                foreach(var accountDTO in accountDTOs)
                    AccountsList.Add(Mapping.DtoToAccount(accountDTO));
                
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

        public IList<Account> GetAccountsList() => AccountsList ?? new List<Account>();
    }
}
