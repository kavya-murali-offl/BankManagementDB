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

        public DataTable AccountTable { get; set; }

        public AccountsView AccountsView { get; set; }

        public IList<Account> GetAccountsAsList()
        {
            IList<Account> accountsList= new List<Account>();
            foreach (DataRow row in AccountTable.Rows)
            {
                AccountTypes enumType = (AccountTypes)Enum.Parse(typeof(AccountTypes), row.Field<string>("Type"));
                Account account = AccountFactory.GetAccountByType(enumType);
                account.Balance = (decimal)row.Field<double>("Balance");
                account.InterestRate =(decimal) row.Field<double>("InterestRate");
                account.Status = (AccountStatus)Enum.Parse(typeof(AccountStatus), row.Field<string>("Status"));
                account.ID = row.Field<Int64>("ID");
                account.UserID = row.Field<Int64>("UserID");
                accountsList.Add(account);
            }
            return accountsList;
        }
        public bool CreateAccount(Int64 userId)
        {
            try
            {
                Account account = AccountsView.GenerateAccount();
                if (account != null)
                {
                    account.UserID =userId;
                    InsertAccountToDB(account);
                    AddAccountToDataTable(account);
                    return true;
                }
                else
                {
                    throw new ArgumentNullException("Something went wrong in creating the account.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
            return false;
        }

        public void UpdateAccount(Int64 accountID, IDictionary<string, object> parameters) {
            Database.UpdateTable("Account", parameters);
        }
        public void AddAccountToDataTable(Account account)
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

        public void InsertAccountToDB(Account account)
        {
            IDictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("ID", account.ID);
            parameters.Add("Balance", account.Balance);
            parameters.Add("InterestRate", account.InterestRate);
            parameters.Add("Status", account.Status.ToString());
            parameters.Add("UserID", account.UserID);
            parameters.Add("HashedPassword", account.Type.ToString());

            parameters.Add("ID", account.ID);
            Database.InsertRowToTable("Account", parameters);
        }

        public void FillTable(Int64 id)
        {
            IDictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("ID", id);
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


    }

}
