using BankManagement.Enums;
using BankManagement.Model;
using BankManagement.Models;
using BankManagement.Utility;
using BankManagementDB.db;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Security.Principal;

namespace BankManagement.Controller
{
    public class TransactionController
    {
        Helper helper = new Helper();

        public Account Account { get; set; }

        public static DataTable TransactionTable { get; set; }

        public Transaction RowToTransaction(DataRow row)
        {
                Transaction transaction = new Transaction();
            try
            {
                transaction.Balance = row.Field<decimal>("Balance");
                transaction.Amount = row.Field<decimal>("Amount");
                transaction.AccountID = row.Field<Int64>("AccountID");
                transaction.ID = row.Field<Int64>("ID");
                transaction.TransactionType = (TransactionTypes)Enum.Parse(typeof(TransactionTypes), row.Field<string>("TransactionType"));
                transaction.RecordedOn = DateTime.Parse(row.Field<string>("RecordedOn"));
            }catch(Exception e) {
                Console.WriteLine(e.Message);
            }
                return transaction;
        }

        public void ViewAllTransactions()
        {
            Database.PrintDataTable(TransactionTable);  
        }
        public IList<Transaction> GetAllTransactions()
        {
                IList<Transaction> transactionList = new List<Transaction>();
                try
                {
                    foreach (DataRow row in TransactionTable.Rows)
                    {
                        Transaction transaction = RowToTransaction(row);
                        transactionList.Add(transaction);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                return transactionList;
        }

        public bool Deposit(Account account, decimal amount, AccountsController accountsController)
          {
            bool isDeposited = false;
            try
            {
                isDeposited = account.Deposit(amount);
                IDictionary<string, object> updateFields = new Dictionary<string, object>
                {
                    { "Balance", account.Balance },
                    { "ID", account.ID }
                };
                accountsController.UpdateAccountInDB(updateFields);
                accountsController.UpdateDataTable(account.ID, updateFields);
                if (isDeposited)
                    AddTransaction(amount, account, TransactionTypes.DEPOSIT);
            }catch(Exception ex) {
                Console.WriteLine(ex.Message);
            }
            return isDeposited;
          }

        public bool Withdraw(Account account, decimal amount, AccountsController accountsController)
        {
            bool isWithdrawn = false;
            try
            {
                isWithdrawn = account.Withdraw(amount);
                IDictionary<string, object> updateFields = new Dictionary<string, object>
                {
                    { "Balance", account.Balance },
                    { "ID", account.ID }
                };
                accountsController.UpdateAccountInDB(updateFields);
                accountsController.UpdateDataTable(account.ID, updateFields);
                if (isWithdrawn)
                    AddTransaction(amount, account, TransactionTypes.WITHDRAW);
            }catch(Exception ex) { Console.WriteLine(ex.Message); }
            return isWithdrawn;
        }

        public bool Transfer(Account account, Int64 toAccountID, decimal amount, AccountsController accountsController)
        {
            bool isTransferred = false;
            Account transferAccount = accountsController.GetAccountByID(toAccountID);
            if(transferAccount != null)
            {
                Withdraw(account, amount, accountsController);
                Deposit(transferAccount,amount, accountsController);
            }
            return isTransferred;
        }

        public void AddTransaction(decimal amount, Account account, TransactionTypes type)
        {
            Transaction transaction = new Transaction(amount, account.Balance, type);
            transaction.AccountID = account.ID;    
            account.Transactions.Add(transaction);
            IDictionary<string, object> updateFields = new Dictionary<string, object>
                {
                    { "Amount", transaction.Amount },
                    { "Balance", transaction.Balance },
                    { "RecordedOn", transaction.RecordedOn },
                    { "TransactionType", transaction.TransactionType.ToString() },
                    { "AccountID", transaction.AccountID },
                };
            InsertTransactionInDB(updateFields);
            AddTransactionDataTable(transaction, updateFields);
        }

        public void AddTransactionDataTable(Transaction transaction, IDictionary<string, object> fields)
        {
            try
            {
                DataRow newRow = TransactionTable.NewRow();
                newRow["ID"] = transaction.ID;
                newRow["Amount"] = transaction.Amount;
                newRow["Balance"] = transaction.Balance;
                newRow["AccountID"] = transaction.Amount;
                newRow["TransactionType"] = transaction.TransactionType.ToString();
                newRow["RecordedOn"] = transaction.RecordedOn;
                TransactionTable.Rows.Add(newRow);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void InsertTransactionInDB(IDictionary<string, object> updateFields)
        {
               Database.InsertRowToTable("Transactions", updateFields);
        }

        public void FillTable(Int64 accountID)
        {
            IDictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("AccountID", accountID);
            TransactionTable = Database.FillTable("Transactions", parameters);
        }


    }
}
