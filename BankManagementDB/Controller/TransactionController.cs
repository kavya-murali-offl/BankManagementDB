using System;
using System.Data;
using System.Collections.Generic;
using BankManagement.Enums;
using BankManagement.Model;
using BankManagement.Models;
using BankManagement.Utility;
using BankManagementDB.db;
using BankManagementDB.Interface;
using System.Linq;

namespace BankManagement.Controller
{
    delegate bool InsertTransactionDelegate(Transaction transaction);
    delegate bool TransferDelegate(decimal amount, Account account);

    public class TransactionController : ITransactionServices, IStatementServices
    {

        public static DataTable TransactionTable { get; set; }

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

        public decimal GetInitialAmount(CurrentAccount currentAccount)
        {
            Helper helper = new Helper();
            decimal amount = helper.GetAmount();
            if (amount < currentAccount.MinimumBalance)
                GetInitialAmount(currentAccount);
            return amount;
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
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return isDeposited;
        }

        public bool Withdraw(decimal amount, Account account)
        {
            bool isWithdrawn = false;
            try
            {
                if (account is SavingsAccount)
                {
                    SavingsAccount savingsAccount = account as SavingsAccount;
                    DepositInterest(savingsAccount);
                }
                isWithdrawn = account.Withdraw(amount);

                if (isWithdrawn)
                {
                    IDictionary<string, object> updateFields = new Dictionary<string, object>
                        {
                            { "Balance", account.Balance },
                            { "ID", account.ID }
                        };
                    AccountsController accountController = new AccountsController();
                    accountController.UpdateAccount(updateFields);
                    CreateTransaction("Withdraw", amount, account, TransactionTypes.WITHDRAW);

                    if (account is CurrentAccount)
                    {
                        CurrentAccount currentAccount = account as CurrentAccount;
                        if (currentAccount.Balance < currentAccount.MinimumBalance)
                        {
                            currentAccount.Charge();
                            CreateTransaction("Minimum Balance Charge", currentAccount.CHARGES, account, TransactionTypes.WITHDRAW);
                        }
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            return isWithdrawn;
        }

        public void DepositInterest(SavingsAccount account)
        {
            decimal interest = account.DepositInterest();
            if (interest > 0)
                CreateTransaction("Interest", interest, account, TransactionTypes.DEPOSIT);
        }

        public bool Transfer(decimal amount, Account account, long toAccountID)
        {
            AccountsController accountsController = new AccountsController();
            Account transferAccount = accountsController.GetAccountByQuery($"ID = {toAccountID}");
            if (transferAccount != null)
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
                if (TransactionTable != null)
                {
                    DataRow newRow = TransactionTable.NewRow();
                    newRow["AccountID"] = transaction.AccountID;
                    newRow["RecordedOn"] = transaction.RecordedOn;
                    newRow["TransactionType"] = transaction.TransactionType.ToString();
                    newRow["Description"] = transaction.Description;
                    newRow["Amount"] = transaction.Amount;
                    newRow["Balance"] = transaction.Balance;
                    newRow["ID"] = 0;
                    TransactionTable.Rows.Add(newRow);
                    success = true;
                }
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
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return transaction;
        }

        public DateTime? GetLastDepositDate()
        {
            if(TransactionTable.Rows.Count > 0)
            {
                DataRow row = TransactionTable.Select("TransactionType = WITHDRAW").LastOrDefault();
                if (row != null)
                    row = TransactionTable.Select("TransactionType = DEPOSIT").LastOrDefault();
                return DateTime.Parse(row.Field<string>("RecordedOn"));
            }
            return new Nullable<DateTime>();
        }
    }
}
