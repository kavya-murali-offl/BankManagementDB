using System;
using System.Collections.Generic;
using System.Data;
using BankManagement.Enums;
using BankManagement.Models;
using BankManagement.View;
using BankManagementDB.db;
using System.Linq;

namespace BankManagement.Controller
{

    delegate bool UpdateAccountDelegate(IDictionary<string, object> fields);
    delegate bool InsertAccountDelegate(Account account);

    public class AccountsController
    {
        public AccountsController()
        {
            AccountsView = new AccountsView();
        }

        public static DataTable AccountTable { get; set; }

        public AccountsView AccountsView { get; set; }

        public bool CreateAccount(long userId)
        {
            try
            {
                Account account = AccountsView.GenerateAccount();
                if (account != null)
                {
                    account.UserID = userId;
                    return InsertAccount(account);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
        }

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

        public bool InsertAccountToDB(Account account)
        {
            try
            {
                IDictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "Balance", account.Balance },
                { "InterestRate", account.InterestRate },
                { "Status", account.Status.ToString() },
                { "UserID", account.UserID },
                { "Type", account.Type.ToString() }
            };
                return DatabaseOperations.InsertRowToTable("Account", parameters);
            }catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;   
        }

        public bool InsertAccountToDataTable(Account account)
        {
            try
            {
                DataRow newRow = AccountTable.NewRow();
                newRow["Balance"] = account.Balance;
                newRow["InterestRate"] = account.InterestRate;
                newRow["UserID"] = account.UserID;
                newRow["Type"] = account.Type;
                newRow["Status"] = account.Status;
                AccountTable.Rows.Add(newRow);
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
        }

        public bool UpdateAccount(IDictionary<string, object> updateFields)
        {
            try
            {
                UpdateAccountDelegate updateAccount = UpdateAccountInDB;
                bool success = updateAccount(updateFields);
                if (success)
                {
                    updateAccount = UpdateAccountInDataTable;
                    return updateAccount(updateFields);
                }
            }
            catch(Exception ex) {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        public bool UpdateAccountInDB(IDictionary<string, object> fields)
        {
            try
            {
                return DatabaseOperations.UpdateTable("Account", fields);
            }catch(Exception ex)
            {
                return false;   
            }
        }

        public bool UpdateAccountInDataTable(IDictionary<string, object> fields)
        {
            try
            {
                if(AccountTable?.Rows?.Count > 0)
                {
                    DataRow[] rows = AccountTable.Select("ID = " + fields["ID"]);
                    if (rows.Length > 0)
                    {
                        DataRow row = rows[0];
                        foreach (var pairs in fields)
                        {
                            row[pairs.Key] = pairs.Value;
                        }
                        return true;
                    }
                }
                
            }catch(Exception ex) {
                Console.WriteLine(ex);
            }
            return false;   
        }

        public void FillTable(long id)
        {
            try
            {
                IDictionary<string, object> parameters = new Dictionary<string, object>
                    {
                        { "UserID", id }
                    };
                AccountTable = DatabaseOperations.FillTable("Account", parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public Account GetAccountByID(long id)
        {
            Account account = null;
            try
            {
                DataRow[] rows = AccountTable.Select("ID = " + id);
                if (rows.Count() > 0)
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

        public Account RowToAccount(DataRow row)
        {
            Account account = null;
            try
            {
                AccountTypes enumType = (AccountTypes)Enum.Parse(typeof(AccountTypes), row.Field<string>("Type"));
                account = AccountFactory.GetAccountByType(enumType);
                account.Balance = row.Field<decimal>("Balance");
                account.InterestRate = row.Field<decimal>("InterestRate");
                account.Status = (AccountStatus)Enum.Parse(typeof(AccountStatus), row.Field<string>("Status"));
                account.ID = row.Field<Int64>("ID");
                account.Type = enumType;
                account.UserID = row.Field<Int64>("UserID");
                return account;
            }catch(Exception e)
            {
                Console.WriteLine(e);
            }
            return account;
        }
    }

}
