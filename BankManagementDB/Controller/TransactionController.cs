using System;
using System.Collections.Generic;
using System.Linq;
using BankManagement.Enums;
using BankManagement.Model;
using BankManagement.Models;
using BankManagementDB.db;
using BankManagementDB.Interface;
using BankManagementDB.View;

namespace BankManagement.Controller
{
    delegate bool InsertTransactionDelegate(Transaction transaction);
    delegate bool TransferDelegate(decimal amount, Account account);

    public class TransactionController : ITransactionServices
    {
        public event Action<string> BalanceChanged;

        public static IList<Transaction> TransactionsList { get; set; }

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
                    account.Deposit(amount);    

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
                        currentAccount.Withdraw(currentAccount.CHARGES);
                        Notification.Info("You have been charged for not maintaining minimum balance");
                        bool isMinBalanceTransacted = 
                            CreateTransaction("Minimum Balance Charge", 
                            currentAccount.CHARGES, currentAccount,
                            TransactionTypes.WITHDRAW);
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
                Account transferAccount = accountsController.GetAccountByQuery("ID", toAccountID);

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
                else
                    Notification.Error("Enter a valid account ID to transfer");
            }
            catch(Exception ex)
            {
                Notification.Error("Transfer failed");
            }
            return false;
        }

        private bool CreateTransaction(string description, decimal amount, Account account, TransactionTypes type)
        {
            try
            {
                Transaction transaction = new Transaction(description, amount, account.Balance, type);
                transaction.AccountID = account.ID;
                return InsertTransaction(transaction);
            }catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
        }

        private bool InsertTransaction(Transaction transaction)
        {
            bool success = false;
            try
            {
                InsertTransactionDelegate insertTransaction = InsertTransactionInDB;
                success = insertTransaction(transaction);
                if (success)
                {
                    insertTransaction = InsertTransactionToList;
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

        private bool InsertTransactionToList(Transaction transaction)
        {
            bool success = false;
            try
            {
                if (TransactionsList != null)
                {
                    TransactionsList.Add(transaction);
                    success = true;
                }
            }
            catch (Exception ex)
            {
            }
            return success;
        }

        private bool InsertTransactionInDB(Transaction transaction)
        {
            try
            {
                IQueryOperations<Transaction> transactionOperations = new TransactionOperations();
                return transactionOperations.UpdateOrInsert(transaction).Result;
            }catch(Exception e)
            {
                return false;   
            }
        }

        public void FillTable(Guid accountID)
        {
            try
            {
                IQueryOperations<Transaction> transactionOperations = new TransactionOperations();
                TransactionsList = transactionOperations.Get(accountID.ToString()).Result;
            }catch(Exception e) {
                Console.WriteLine(e);
            }
        }

        public DateTime? GetLastWithdrawDate()
        {
            if(TransactionsList.Count > 0)
            {
                Transaction transaction = TransactionsList.LastOrDefault(data => data.TransactionType.ToString() == "WITHDRAW");

                if (transaction == null)
                    transaction = TransactionsList.LastOrDefault(data => data.TransactionType.ToString() == "DEPOSIT");

                return transaction.RecordedOn;
            }
            return null;
        }

        public IList<Transaction> GetAllTransactions() => TransactionsList ?? new List<Transaction>();
    }
}
