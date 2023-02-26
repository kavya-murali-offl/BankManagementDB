using System;
using System.Collections.Generic;
using BankManagementDB.Models;
using System.Linq;
using BankManagementDB.Interface;
using BankManagementCipher.Utility;

namespace BankManagementDB.Controller
{

    delegate bool UpdateAccountDelegate(Account account);
    delegate bool InsertAccountDelegate(Account account);

    public class AccountController : IAccountController
    {
        public AccountController(IAccountRepository accountRepository)
        {
            AccountRepository = accountRepository;
        }

        public static IList<Account> AccountsList { get; set; }

        public IAccountRepository AccountRepository { get; private set; }    

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
                 return AccountRepository.InsertOrReplace(Mapping.AccountToDto(account)).Result;
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
                AccountsList.Insert(0, account);
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
                return AccountRepository.InsertOrReplace(Mapping.AccountToDto(account)).Result;
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
                    AccountsList.Remove(account);
                    AccountsList.Insert(0, updatedAccount);
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
                AccountsList = new List<Account>();   
                var accountDTOs = AccountRepository.Get(id).Result;

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
            try
            {
                AccountsList ??= new List<Account>();
                return AccountsList.FirstOrDefault(acc => acc.GetType().GetProperty(key).GetValue(acc).Equals(value));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return null;
        }

        public IList<Account> GetAccountsList() => AccountsList ?? new List<Account>();
    }
}
