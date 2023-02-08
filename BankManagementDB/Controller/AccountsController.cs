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

        public ITransactionServices TransactionController { get; private set; }    

        public static DataTable AccountTable { get; set; }

        public bool InsertAccount(Account account)
        {
            bool isAdded = false;
            InsertAccountDelegate insertAccount = InsertAccountToDB;
            if (insertAccount(account))
            {
                insertAccount = InsertAccountToDataTable;
                isAdded = insertAccount(account);
            }
            return isAdded;
        }

        private bool InsertAccountToDB(Account account)
        {
            try
            {
                return AccountOperations.Upsert(account).Result;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;   
        }

        

        private bool InsertAccountToDataTable(Account account)
        {
            try
            {
                AccountTable ??= new DataTable();

                DataRow newRow = AccountTable.NewRow();
                newRow["ID"] = account.ID.ToString();
                newRow["Balance"] = account.Balance;
                newRow["InterestRate"] = account.InterestRate;
                newRow["UserID"] = account.UserID.ToString();
                newRow["Type"] = account.Type.ToString();
                newRow["Status"] = account.Status.ToString();

                AccountTable.Rows.Add(newRow);

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
                updateAccount = UpdateAccountInDataTable;
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
                return AccountOperations.Upsert(account).Result;
            }catch(Exception ex)
            {
                return false;   
            }
        }

        private bool UpdateAccountInDataTable(Account account)
        {
            try
            {
                if(AccountTable?.Rows?.Count > 0)
                {
                DataRow[] rows = AccountTable.Select("ID = '" + account.ID.ToString() + "'");
                if (rows.Length > 0)
                {
                DataRow row = rows.LastOrDefault();
                return true;
                }
                }
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
                AccountTable = AccountOperations.Get(id.ToString()).Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public Account GetAccountByQuery(string query)
        {
            Account account = null;
            try
            {
                if (AccountTable != null)
                {
                DataRow row = AccountTable.Select(query).LastOrDefault();
                account = RowToAccount(row);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return account;
        }

        public IList<Account> GetAllAccounts()
        {
            IList<Account> accountsList = new List<Account>();
            try
            {
                foreach (DataRow row in AccountTable.Rows)
                {
                Account account = RowToAccount(row);
                accountsList.Add(account);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return accountsList;
        }

        private Account RowToAccount(DataRow row)
        {
            Account account = null;
            try
            {
                
                AccountTypes enumType = (AccountTypes)Enum.Parse(typeof(AccountTypes), row.Field<string>("Type"));
                account = AccountFactory.GetAccountByType(enumType);
                account.Balance = row.Field<decimal>("Balance");
                account.InterestRate = row.Field<decimal>("InterestRate");
                account.Status = (AccountStatus)Enum.Parse(typeof(AccountStatus), row.Field<string>("Status"));
                account.ID = Guid.Parse(row.Field<string>("ID"));
                account.Type = enumType;
                account.UserID = Guid.Parse(row.Field<string>("UserID"));
                return account;

            } 
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
            return account;
        }
    }
}
