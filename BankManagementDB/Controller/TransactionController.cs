﻿using System;
using System.Collections.Generic;
using System.Data;
using BankManagement.Enums;
using BankManagement.Model;
using BankManagement.Models;
using BankManagementDB.db;
using BankManagementDB.Interface;

namespace BankManagement.Controller
{
    delegate bool InsertTransactionDelegate(Transaction transaction);
    delegate bool TransferDelegate(decimal amount, Account account);

    public class TransactionController : ITransactionServices
    {
        public Account Account { get; set; }

        public static DataTable TransactionTable { get; set; }

        public Transaction RowToTransaction(DataRow row)
        {
                Transaction transaction = new Transaction();
            try
            {
                transaction.Balance = row.Field<decimal>("Balance");
                transaction.Amount = row.Field<decimal>("Amount");
                transaction.AccountID = row.Field<long>("AccountID");
                transaction.ID = row.Field<long>("ID");
                transaction.Description = row.Field<string>("Description");
                transaction.TransactionType = (TransactionTypes)Enum.Parse(typeof(TransactionTypes), row.Field<string>("TransactionType"));
                transaction.RecordedOn = DateTime.Parse(row.Field<string>("RecordedOn"));
            }  catch(Exception e) {
                Console.WriteLine(e.Message);
            }
            return transaction;
        }

        public void ViewAllTransactions()
        {
            DatabaseOperations.PrintDataTable(TransactionTable);  
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

        public bool Deposit(decimal amount, Account account)
          {
            bool isDeposited = false;
            try
            {
                account.Deposit(amount);
                IDictionary<string, object> updateFields = new Dictionary<string, object>
                {
                    { "Balance", account.Balance },
                    { "ID", account.ID }
                };
                AccountsController accountsController = new AccountsController();
                isDeposited = accountsController.UpdateAccount(updateFields);
                if (isDeposited)
                    CreateTransaction("DEPOSIT", amount, account, TransactionTypes.DEPOSIT);
            }catch(Exception ex) {
                Console.WriteLine(ex.Message);
            }
            return isDeposited;
          }

        public bool Withdraw(decimal amount, Account account)
        {
            bool isWithdrawn = false;
            try
            {
                DepositInterest(account);
                isWithdrawn = account.Withdraw(amount);
                IDictionary<string, object> updateFields = new Dictionary<string, object>
                {
                    { "Balance", account.Balance },
                    { "ID", account.ID }
                };
                AccountsController accountController = new AccountsController();
                accountController.UpdateAccount(updateFields);
                if (isWithdrawn)
                    CreateTransaction("WITHDRAWAL", amount, account, TransactionTypes.WITHDRAW);
            }catch(Exception ex) { Console.WriteLine(ex.Message); }
            return isWithdrawn;
        }

        public void DepositInterest(Account account)
        {
            decimal interest = account.DepositInterest();
            if(interest > 0)
                CreateTransaction("Interest", interest, account, TransactionTypes.DEPOSIT);
        }

        public bool Transfer(decimal amount, Account account, long toAccountID )
        {
            AccountsController accountsController = new AccountsController();
            Account transferAccount = accountsController.GetAccountByID(toAccountID);
            if(transferAccount != null)
            {
                TransferDelegate transfer = Withdraw;
                bool isWithdrawn = transfer(amount, account);
                if (isWithdrawn)
                {
                    transfer = Deposit;
                    bool isDeposited = transfer(amount, transferAccount);
                    return isDeposited;
                }
            }
            return false;
        }

        public void CreateTransaction(string description, decimal amount, Account account, TransactionTypes type)
        {
            Transaction transaction = new Transaction(description, amount, account.Balance, type);
            transaction.AccountID = account.ID;    
            InsertTransaction(transaction);
        }

        public bool InsertTransaction(Transaction transaction)
        {
            bool success = false;
            try
            {
                InsertTransactionDelegate insertTransaction = InsertTransactionInDB;
                success = insertTransaction(transaction);
                if (success)
                {
                    insertTransaction = InsertTransactionToDataTable;
                    bool inserted = insertTransaction(transaction);
                    return inserted;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return success;
        }

        public bool InsertTransactionToDataTable(Transaction transaction)
        {
            bool success = false;
            try
            {
                DataRow newRow = TransactionTable.NewRow();
                newRow["Amount"] = transaction.Amount;
                newRow["Balance"] = transaction.Balance;
                newRow["AccountID"] = transaction.Amount;
                newRow["Description"] = transaction.Balance;
                newRow["TransactionType"] = transaction.TransactionType.ToString();
                newRow["RecordedOn"] = transaction.RecordedOn;
                TransactionTable.Rows.Add(newRow);
                success = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return success;
        }

        public bool InsertTransactionInDB(Transaction transaction)
        {
            IDictionary<string, object> updateFields = new Dictionary<string, object>
                {
                    { "Description", transaction.Description },
                    { "Amount", transaction.Amount },
                    { "Balance", transaction.Balance },
                    { "RecordedOn", transaction.RecordedOn },
                    { "TransactionType", transaction.TransactionType.ToString() },
                    { "AccountID", transaction.AccountID },
                };
            return DatabaseOperations.InsertRowToTable("Transactions", updateFields);
        }

        public void FillTable(long accountID)
        {
            IDictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "AccountID", accountID }
            };
            TransactionTable = DatabaseOperations.FillTable("Transactions", parameters);
        }

        public DateTime? GetLastWithdrawnDate()
        {
            for(int i = TransactionTable.Rows.Count ; i>=0; i--)
            {
                if (TransactionTable.Rows[i].Field<string>("TransactionType") == "DEPOSIT")
                    return TransactionTable.Rows[i].Field<DateTime>("RecordedOn");
            }
            return new Nullable<DateTime>();
        }
    }
}
