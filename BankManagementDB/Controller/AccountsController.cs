using System;
using System.Collections.Generic;
using System.Data;
using BankManagement.Enums;
using BankManagement.Models;
using BankManagement.Utility;
using BankManagement.View;
using BankManagementDB.db;
using System.Linq;


namespace BankManagement.Controller
{
    public class AccountsController
    {
        public AccountsController()
        {
            AccountsView = new AccountsView();
        }

        public static DataTable AccountTable { get; set; }

        public AccountsView AccountsView { get; set; }

        public bool CreateAccount(Int64 userId)
        {
            try
            {
                Account account = AccountsView.GenerateAccount();
                if (account != null)
                {
                    account.UserID =userId;
                    InsertAccountToDB(account);
                    InsertAccountToDataTable(account);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
        }

        public bool UpdateAccountInDB(IDictionary<string, object> parameters) {
            return Database.UpdateTable("Account", parameters);
        }

        public void InsertAccountToDataTable(Account account)
        {
            try
            {
                DataRow newRow = AccountTable.NewRow();
                newRow["Balance"] = account.Balance;
                newRow["InterestRate"] = account.InterestRate;
                newRow["UserID"] = account.UserID;
                newRow["Type"] = (AccountTypes)account.Type;
                newRow["Status"] = (AccountStatus)account.Status;
                AccountTable.Rows.Add(newRow);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public Account GetAccountByID(Int64 id)
        {
            Account account = null;
            try
            {
                DataRow[] rows = AccountTable.Select("ID = " + id);
                if (rows.Count()>0)
                {
                    DataRow row = rows.First();
                    account = RowToAccount(row);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return account;
        }

        public Account RowToAccount(DataRow row)
        {
            AccountTypes enumType = (AccountTypes)Enum.Parse(typeof(AccountTypes), row.Field<string>("Type"));
            Account account = AccountFactory.GetAccountByType(enumType);
            account.Balance = row.Field<decimal>("Balance");
            account.InterestRate = row.Field<decimal>("InterestRate");
            account.Status = (AccountStatus)Enum.Parse(typeof(AccountStatus), row.Field<string>("Status"));
            account.ID = row.Field<Int64>("ID");
            account.Type = (AccountTypes)Enum.Parse(typeof(AccountTypes), row.Field<string>("Type"));
            account.UserID = row.Field<Int64>("UserID");
            return account;
        }

        public void InsertAccountToDB(Account account)
        {
            IDictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("Balance", account.Balance);
            parameters.Add("InterestRate", account.InterestRate);
            parameters.Add("Status", account.Status.ToString());
            parameters.Add("UserID", account.UserID);
            parameters.Add("Type", account.Type.ToString());
            Database.InsertRowToTable("Account", parameters);
        }

        public void FillTable(Int64 id)
        {
            IDictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("UserID", id);
            AccountTable = Database.FillTable("Account", parameters);
        }

         public void UpdateDataTable(Int64 id, IDictionary<string, object> fields)
         {
            DataRow[] rows = AccountTable.Select("ID = "+ id);
            if(rows.Length > 0)
            {
                DataRow row = rows[0];
                foreach (KeyValuePair<string, object> pairs in fields)
                {
                    row[pairs.Key] = pairs.Value;
                }
            }
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
            }catch(Exception ex) {
                Console.WriteLine(ex.Message);
            }
                return accountsList;
         }
    }

}
