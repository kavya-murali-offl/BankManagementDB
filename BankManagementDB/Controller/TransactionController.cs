using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using BankManagement.Enums;
using BankManagement.Model;
using BankManagement.Models;
using BankManagement.Utility;
using BankManagementDB.db;
using BankManagementDB.Interface;
using BankManagementDB.View;

namespace BankManagement.Controller
{
    delegate bool InsertTransactionDelegate(Transaction transaction);

    public class TransactionController : ITransactionServices
    {
        public event Action<string> BalanceChanged;

        public static DataTable TransactionTable { get; set; }

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

                AccountsController accountsController = new AccountsController();
                isDeposited = accountsController.UpdateAccount(account);

                if (isDeposited)
                {
                    BalanceChanged?.Invoke($"Deposit of Rs. {amount} is successful");
                    bool isTransacted = CreateTransaction("Deposit", amount, account, TransactionTypes.DEPOSIT);
                }
                else
                {
                    Notification.Error("Deposit unsuccessful");
                    account.Withdraw(amount);
                }
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
                WithdrawHandlers(account);

                account.Withdraw(amount);

                AccountsController accountController = new AccountsController();
                isWithdrawn = accountController.UpdateAccount(account);

                if (isWithdrawn)
                {
                    BalanceChanged?.Invoke($"Withdrawal of Rs. {amount} is successful");
                    CreateTransaction("Withdraw", amount, account, TransactionTypes.WITHDRAW);
                }
                else
                {
                    Notification.Error("Withdraw failed");
                    account.Deposit(amount);    
                }

            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            return isWithdrawn;
        }

        public void WithdrawHandlers(Account account)
        {
            if (account is SavingsAccount)
                DepositInterest(account as SavingsAccount);

            else if (account is CurrentAccount)
                ChargeForMinBalance(account as CurrentAccount);
        }

        private void ChargeForMinBalance(CurrentAccount currentAccount)
        {
                if (currentAccount.Balance < currentAccount.MinimumBalance && currentAccount.Balance > currentAccount.CHARGES)
                {
                    if (currentAccount.Withdraw(currentAccount.CHARGES))
                    {
                        Notification.Info("You have been charged for not maintaining minimum balance");
                        bool isMinBalanceTransacted = CreateTransaction("Minimum Balance Charge", currentAccount.CHARGES, currentAccount, TransactionTypes.WITHDRAW);
                    }
                }
        }

        private decimal DepositInterest(SavingsAccount account)
        {
            decimal interest = account.GetInterest();
            if (interest > 0)
            {
                Notification.Info($"Interest deposit of Rs. {interest} has been initiated");
                if(Deposit(interest, account)) { 
                    if(CreateTransaction("Interest", interest, account, TransactionTypes.DEPOSIT))
                        return interest;
                }
                else
                    Notification.Error("Interest deposit unsuccessful");
            }
            return 0;
        }

        public bool Transfer(decimal amount, Account account, Guid toAccountID)
        {
            try
            {
                AccountsController accountsController = new AccountsController();
                Account transferAccount = accountsController.GetAccountByQuery($"ID = '{toAccountID.ToString()}'");

                if (transferAccount != null)
                {
                    using (System.Transactions.TransactionScope scope = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required, new System.Transactions.TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Serializable }))
                    {
                        try
                        {
                            bool isWithdrawn = Withdraw(amount, account);
                            if (!isWithdrawn)
                            {
                                throw new Exception("Withdraw operation failed.");
                            }

                            bool isDeposited = Deposit(amount, account);
                            if (!isDeposited)
                            {
                                throw new Exception("Deposit operation failed.");
                            }
                            scope.Complete();
                            BalanceChanged?.Invoke("Transfer successful");
                        }
                        catch (Exception ex)
                        {
                            Notification.Error("Transfer failed");
                        }
                    }
                }
                else
                    Notification.Error("Enter a valid account ID to transfer");

            }
            catch(Exception ex)
            {
                Notification.Error("Transfer failed");
            }
            return false;
        }

        public bool CreateTransaction(string description, decimal amount, Account account, TransactionTypes type)
        {
            Transaction transaction = new Transaction(description, amount, account.Balance, type);
            transaction.AccountID = account.ID;
            return InsertTransaction(transaction);
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
                    newRow["ID"] = transaction.ID.ToString();
                    newRow["AccountID"] = transaction.AccountID.ToString();
                    newRow["RecordedOn"] = transaction.RecordedOn;
                    newRow["TransactionType"] = transaction.TransactionType.ToString();
                    newRow["Description"] = transaction.Description;
                    newRow["Amount"] = transaction.Amount;
                    newRow["Balance"] = transaction.Balance;
                    newRow["ID"] = transaction.ID;
                    TransactionTable.Rows.Add(newRow);
                    success = true;
                }
            }
            catch (Exception ex)
            {
            }
            return success;
        }

        public bool InsertTransactionInDB(Transaction transaction)
        {
            try
            {
                
                return TransactionOperations.Upsert(transaction).Result;
            }catch(Exception e)
            {
                return false;   
            }
        }

        public void FillTable(Guid accountID)
        {
            TransactionTable = TransactionOperations.Get(accountID.ToString()).Result;
        }

        public Transaction RowToTransaction(DataRow row)
        {
            Transaction transaction = new Transaction();
            try
            {
                transaction.Balance = row.Field<decimal>("Balance");
                transaction.Amount = row.Field<decimal>("Amount");
                transaction.AccountID = Guid.Parse(row.Field<string>("AccountID"));
                transaction.ID = Guid.Parse(row.Field<string>("ID"));
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

        public DateTime? GetLastWithdrawDate()
        {
            if(TransactionTable?.Rows?.Count > 0)
            {
                DataRow row = TransactionTable.AsEnumerable().Where((r) => r.Field<string>("TransactionType") == "WITHDRAW").LastOrDefault();

                if (row == null)
                    row = TransactionTable.AsEnumerable().Where((r) => r.Field<string>("TransactionType") == "DEPOSIT").LastOrDefault();

                return DateTime.Parse(row.Field<string>("RecordedOn"));
            }
            return new DateTime?();
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
    }
}
