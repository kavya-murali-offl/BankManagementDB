using BankManagement.Controller;
using BankManagement.Enums;
using BankManagement.Model;
using BankManagement.Models;
using BankManagementDB.Interface;
using BankManagementDB.View;
using System;


namespace BankManagementDB.Controller
{
    public class ATMTransactionsController : IATMTransactionServices
    {
        public event Action<string> BalanceChanged;

        public ATMTransactionsController(ITransactionServices transactionController) {
            TransactionController = transactionController;
        }
        
        public ITransactionServices TransactionController { get; set; }

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
                    Transaction transaction = new Transaction("Deposit", amount, account.Balance, TransactionTypes.DEPOSIT, account.ID);
                    bool isTransacted = TransactionController.InsertTransaction(transaction);
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
                    Transaction transaction = new Transaction("Withdraw", amount, account.Balance, TransactionTypes.WITHDRAW, account.ID);
                    bool isTransacted = TransactionController.InsertTransaction(transaction);
                }
                else
                {
                    Notification.Error("Withdraw unsuccessful");
                    account.Deposit(amount);
                }

            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            return isWithdrawn;
        }

        private void WithdrawHandlers(Account account)
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
                Transaction transaction = new Transaction("Minimum Balance Charge",
                    currentAccount.CHARGES, currentAccount.Balance,
                    TransactionTypes.WITHDRAW, currentAccount.ID);
                bool isMinBalanceTransacted = TransactionController.InsertTransaction(transaction);
                    
            }
        }

        private decimal DepositInterest(SavingsAccount account)
        {
            decimal interest = account.GetInterest();
            if (interest > 0)
            {
                Notification.Info($"Interest deposit of Rs. {interest} has been initiated");
                if (Deposit(interest, account))
                {
                    Transaction transaction = new Transaction("Interest", interest, account.Balance, TransactionTypes.DEPOSIT, account.ID);
                    TransactionController.InsertTransaction(transaction);
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
            catch (Exception ex)
            {
                Notification.Error("Transfer failed");
            }
            return false;
        }

    }
}
