using System;
using BankManagementDB.Controller;
using BankManagementDB.Model;
using BankManagementDB.Models;
using BankManagementDB.EnumerationType;
using BankManagementDB.Interface;
using BankManagementDB.View;


namespace BankManagementDB.Controller
{
    delegate bool TransferDelegate(decimal amount, Account account, ModeOfPayment modeOfPayment);

    public class TransactionProcessController : ITransactionProcessController
    {
        public event Action<string> BalanceChanged;

        public TransactionProcessController(ITransactionController transactionController, IAccountController accountsController) {
            TransactionController = transactionController;
            AccountController = accountsController;
        }
        
        public ITransactionController TransactionController { get; set; }

        public IAccountController AccountController { get; set; }

        public bool Deposit(decimal amount, Account account, ModeOfPayment modeOfPayment)
        {
            bool isDeposited = false;
            try
            {
                account.Deposit(amount);

                isDeposited = AccountController.UpdateAccount(account);
                
                if (isDeposited)
                {
                    BalanceChanged?.Invoke($"Deposit of Rs. {amount} is successful");
                    Transaction transaction = 
                      new  Transaction("Deposit", amount, account.Balance, TransactionType.DEPOSIT, account.ID, modeOfPayment);
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

        public bool Withdraw(decimal amount, Account account, ModeOfPayment modeOfPayment)
        {
            bool isWithdrawn = false;
            try
            {
                WithdrawHandlers(account);

                account.Withdraw(amount);

                isWithdrawn = AccountController.UpdateAccount(account);

                if (isWithdrawn)
                {
                    BalanceChanged?.Invoke($"Withdrawal of Rs. {amount} is successful");
                    Transaction transaction = new Transaction("Withdraw", amount, account.Balance, TransactionType.WITHDRAW, account.ID, modeOfPayment);
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
                    TransactionType.WITHDRAW, currentAccount.ID, ModeOfPayment.INTERNAL);
                bool isMinBalanceTransacted = TransactionController.InsertTransaction(transaction);
                    
            }
        }

        private decimal DepositInterest(SavingsAccount account)
        {
            decimal interest = account.GetInterest();
            if (interest > 0)
            {
                Notification.Info($"Interest deposit of Rs. {interest} has been initiated");
                if (Deposit(interest, account, ModeOfPayment.INTERNAL))
                {
                    Transaction transaction = new Transaction("Interest", interest, account.Balance, TransactionType.DEPOSIT, account.ID, ModeOfPayment.INTERNAL);
                    TransactionController.InsertTransaction(transaction);
                    return interest;
                }
                else
                    Notification.Error("Interest deposit unsuccessful");
            }
            return 0;
        }

        public bool Transfer(decimal amount, Account account, Guid toAccountID, ModeOfPayment modeOfPayment)
        {
            try
            {
                Account transferAccount = AccountController.GetAccountByQuery("ID", toAccountID);

                if (transferAccount != null)
                {
                    TransferDelegate transfer = Withdraw;
                    bool isWithdrawn = transfer(amount, account, modeOfPayment);
                    if (isWithdrawn)
                    {
                        transfer = Deposit;
                        bool isDeposited = transfer(amount, transferAccount, modeOfPayment);
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
