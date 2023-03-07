using System;
using System.Collections.Generic;
using System.Linq;
using BankManagementDB.Model;
using BankManagementDB.Models;
using BankManagementDB.Utility;
using BankManagementDB.Config;
using BankManagementDB.EnumerationType;
using BankManagementDB.Interface;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Principal;

namespace BankManagementDB.View
{
    public class TransactionView
    {
        public TransactionView() {
            TransactionController = DependencyContainer.ServiceProvider.GetRequiredService<ITransactionDataManager>();
            UpdateAccountDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IUpdateAccountDataManager>();
            GetAccountDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IGetAccountDataManager>();
        }

        public ITransactionDataManager TransactionController { get; set; }

        public IUpdateAccountDataManager UpdateAccountDataManager { get; private set; }
        public IGetAccountDataManager GetAccountDataManager { get; private set; }

        public void GoToAccount(Account account)
        {
            TransactionController.FillTable(account.ID);
            while (true)
            {
                Console.WriteLine();
                for (int i = 0; i < Enum.GetNames(typeof(AccountCases)).Length; i++)
                {
                    AccountCases cases = (AccountCases)i;
                    Console.WriteLine($"{i + 1}. {cases.ToString().Replace("_", " ")}");
                }

                Console.Write("\nEnter your choice: ");

                string option = Console.ReadLine().Trim();
                int entryOption = int.Parse(option);
                if (entryOption != 0 && entryOption <= Enum.GetNames(typeof(AccountCases)).Count())
                {
                    AccountCases operation = (AccountCases)entryOption - 1;
                    if (TransactionOperations(operation, account))
                        break;
                }
                else
                    Notification.Error("Enter a valid input.");
            }
        }
       
        public bool TransactionOperations(AccountCases option, Account account)
        {

            switch (option)
            {
                case AccountCases.DEPOSIT:
                    Initiate(account, TransactionType.DEPOSIT);
                    return false;

                case AccountCases.WITHDRAW:
                    Initiate(account, TransactionType.WITHDRAW);
                    return false;

                case AccountCases.TRANSFER:
                    Initiate(account, TransactionType.TRANSFER);
                    return false;

                case AccountCases.CHECK_BALANCE:
                    Notification.Info($"\nBALANCE: Rs. {account.Balance}\n");
                    return false;

                case AccountCases.VIEW_STATEMENT:
                    ViewAllTransactions(account.ID);
                    return false;

                case AccountCases.PRINT_STATEMENT:
                    PrintStatement(account.ID);
                    return false;

                case AccountCases.VIEW_ACCOUNT_DETAILS:
                    Console.WriteLine(account);
                    return false;

                case AccountCases.BACK:
                    return true;

                default:
                    Notification.Error("Invalid option. Try again!");
                    return false;
            }

        }

        public void Initiate(Account account, TransactionType transactionType)
        {
            CardView cardsView = new CardView();
            bool isAuthenticated = false;
            string cardNumber = null;

            ModeOfPayment modeOfPayment = GetModeOfPayment(transactionType);

            if (modeOfPayment != ModeOfPayment.DEFAULT)
            {
                if (cardsView.ValidateModeOfPayment(account.ID, modeOfPayment)) { 
                    if (modeOfPayment == ModeOfPayment.CASH)
                        isAuthenticated = true;
                    else if (modeOfPayment == ModeOfPayment.DEBIT_CARD || modeOfPayment == ModeOfPayment.CREDIT_CARD)
                    {
                        cardNumber = cardsView.GetCardNumber();
                        if (cardNumber != null)
                            isAuthenticated = IsAuthenticated(modeOfPayment, cardNumber);
                    }
                   if(!isAuthenticated) 
                        Notification.Error("Authentication failed. Please try again");
                }
                else
                    Notification.Error("Selected Mode of payment is not enabled");
            }
            if (isAuthenticated)
            {
                decimal amount = GetAmount();
                if (amount > 0)
                {
                    switch (transactionType)
                    {
                        case TransactionType.DEPOSIT:
                            Deposit(account, amount, modeOfPayment, cardNumber);
                            break;

                        case TransactionType.WITHDRAW:
                            Withdraw(account, amount, modeOfPayment, cardNumber);
                            break;

                        case TransactionType.TRANSFER:
                            Transfer(account, amount, modeOfPayment, cardNumber);
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        public bool Deposit(Account account, decimal amount, ModeOfPayment modeOfPayment, string cardNumber)
        {
            //AccountDataManager.BalanceChanged += onBalanceChanged;
            if (UpdateAccountDataManager.UpdateBalance(account, amount, TransactionType.DEPOSIT))
            {
                RecordTransaction("Deposit", amount, account.Balance, TransactionType.DEPOSIT, account.ID, modeOfPayment, cardNumber);
                Notification.Success($"Deposit of Rs. {amount} is successful");
                return true;
            }
            else
                Notification.Error("Deposit unsuccessful");
            //AccountDataManager.BalanceChanged -= onBalanceChanged;
            return false;
        }


        public ModeOfPayment GetModeOfPayment(TransactionType transactionType)
        {
            string input;
            ModeOfPayment modeOfPayment = ModeOfPayment.DEFAULT;
            Console.WriteLine("Choose mode of payment:\n1. CASH\n2. DEBIT CARD\n");
            input = Console.ReadLine().Trim();
            if (input == "0") modeOfPayment = ModeOfPayment.DEFAULT;
            else if (input == "1") modeOfPayment = ModeOfPayment.CASH;
            else if (input == "2") modeOfPayment = ModeOfPayment.DEBIT_CARD;
            return modeOfPayment;
        }

        public bool RecordTransaction(string description, decimal amount, decimal balance, TransactionType transactionType, Guid accountID, ModeOfPayment modeOfPayment, string cardNumber)
        {
            Transaction transaction =
            new Transaction(description, amount, balance, transactionType, accountID, modeOfPayment, cardNumber);
            return TransactionController.InsertTransaction(transaction);
        }

        public bool Withdraw(Account account, decimal amount, ModeOfPayment modeOfPayment, string cardNumber)
        {
            try
            {
                if (amount > account.Balance)
                    Notification.Error("Insufficient Balance");
                else
                {
                    WithdrawHandlers(account);

                    //AccountDataManager.BalanceChanged += onBalanceChanged;
                    if (UpdateAccountDataManager.UpdateBalance(account, amount, TransactionType.DEPOSIT))
                    {

                            Notification.Success("Withdraw successful");
                            bool isTransactionRecorded = RecordTransaction("Withdraw", amount, account.Balance, TransactionType.WITHDRAW, account.ID, modeOfPayment, cardNumber);
                            return true;
                    }
                    else
                        Notification.Error("Withdraw unsuccessful");
                    //AccountDataManager.BalanceChanged -= onBalanceChanged;
                }
            }
            catch(Exception ex) { 
                Notification.Error(ex.Message);
            }
            return false;
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
            try
            {
                if (currentAccount.Balance < currentAccount.MinimumBalance && currentAccount.Balance > currentAccount.CHARGES)
                {
                    UpdateAccountDataManager.UpdateBalance(currentAccount, currentAccount.CHARGES, TransactionType.WITHDRAW);
                    Notification.Info("You have been charged for not maintaining minimum balance");
                    RecordTransaction("Minimum Balance Charge",
                        currentAccount.CHARGES, currentAccount.Balance,
                        TransactionType.WITHDRAW, currentAccount.ID, ModeOfPayment.INTERNAL, null);
                }
            }
            catch (Exception error)
            {
                Notification.Error(error.Message);  
            }
        }

        private decimal DepositInterest(SavingsAccount account)
        {
            try
            {
                decimal interest = account.GetInterest();
                if (interest > 0)
                {
                    Notification.Info($"Interest deposit of Rs. {interest} has been initiated");
                    UpdateAccountDataManager.UpdateBalance(account, interest, TransactionType.DEPOSIT);
                    RecordTransaction("Interest", interest, account.Balance, TransactionType.DEPOSIT, account.ID, ModeOfPayment.INTERNAL, null);
                    return interest;
                }
            }
            catch (Exception error) { 
                Notification.Error(error.ToString());
            }
            return 0;
        }

        public void Transfer(Account account, decimal amount, ModeOfPayment modeOfPayment, string cardNumber)
        {
            
            if (amount > account.Balance) Notification.Error("Insufficient Balance");
            else
            {
                Account transferAccount = GetTransferAccount(account.AccountNumber);
                if (transferAccount != null)
                {
                    if (UpdateAccountDataManager.UpdateBalance(account, amount, TransactionType.WITHDRAW))
                    {
                        if (UpdateAccountDataManager.UpdateBalance(transferAccount, amount, TransactionType.DEPOSIT))
                        {
                            Console.WriteLine("Transfer successful");
                            RecordTransaction("Transferred", amount, account.Balance, TransactionType.TRANSFER, account.ID, modeOfPayment, cardNumber);
                            RecordTransaction("Received", amount, account.Balance, TransactionType.RECEIVED, account.ID, modeOfPayment, cardNumber);
                        }
                        else
                        {
                            Notification.Error("Transfer unsuccessful");
                            UpdateAccountDataManager.UpdateBalance(account, amount, TransactionType.DEPOSIT);
                        }
                    }
                    else
                        Notification.Error("Transfer unsuccessful");
                }
            }
        }

        public void ViewAllTransactions(Guid accountID)
        {
            try
            {
                IEnumerable<Transaction> statements = TransactionController.GetAllTransactions(accountID).Where(transaction=> transaction.AccountID.Equals(accountID));
                foreach (Transaction transaction in statements)
                    Console.WriteLine(transaction);
            }
            catch(Exception error)
            {
                Notification.Error(error.Message);  
            }
        }

        public void PrintStatement(Guid accountID)
        {
            IEnumerable<Transaction> statements = TransactionController.GetAllTransactions(accountID);
            Printer.PrintStatement(statements);
        }

        public Account GetTransferAccount(string accountNumber)
        {
            try
            {
                while (true)
                {
                    Console.Write("Enter Account Number to transfer: ");
                    string transferAccountNumber = Console.ReadLine().Trim();
                    if (transferAccountNumber == "0")
                        break;
                    else

                        if (accountNumber == transferAccountNumber)
                            Notification.Error("Choose a different account number to transfer.");
                        else
                        {
                            Account transferAccount = GetAccountDataManager.GetAccount(accountNumber);
                            if (transferAccount == null)
                            {
                                Notification.Error("Enter a valid Account Number");
                                break;
                            }
                            else return transferAccount;
                        }
                }
            }
            catch (Exception error)
            {
                Notification.Error(error.ToString());
            }
            return null;
        }

        public void onBalanceChanged(string message)
        {
            Notification.Success(message);
        }

        public bool IsAuthenticated(ModeOfPayment modeOfPayment, string cardNumber)
        {
            CardView cardsView = new CardView();

            if (modeOfPayment == ModeOfPayment.CASH)
               return true;
            else
               return cardsView.Authenticate(cardNumber);
        }

        public decimal GetAmount()
        {
            try
            {
                while (true)
                {
                    Console.Write("Enter amount: ");
                    decimal amount = decimal.Parse(Console.ReadLine().Trim());
                    if (amount < 0) Notification.Error("Amount should be greater than zero.");
                    else return amount;
                }
            }
            catch (Exception error)
            {
                Notification.Error(error.ToString());
            }
            return 0;
        }
    }
}
