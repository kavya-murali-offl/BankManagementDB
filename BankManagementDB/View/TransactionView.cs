using System;
using System.Collections.Generic;
using System.Linq;
using BankManagementDB.Model;
using BankManagementDB.Models;
using BankManagementDB.Utility;
using BankManagementDB.Config;
using BankManagementDB.EnumerationType;
using BankManagementDB.Interface;
using BankManagementDB.Properties;
using Microsoft.Extensions.DependencyInjection;


namespace BankManagementDB.View
{
    public class TransactionView
    {
        public TransactionView() {
            InsertTransactionDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IInsertTransactionDataManager>();
            GetTransactionDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IGetTransactionDataManager>();
            UpdateAccountDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IUpdateAccountDataManager>();
            GetAccountDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IGetAccountDataManager>();
        }

        public IInsertTransactionDataManager InsertTransactionDataManager { get; set; }

        public IGetTransactionDataManager GetTransactionDataManager { get; set; }

        public IUpdateAccountDataManager UpdateAccountDataManager { get; private set; }

        public IGetAccountDataManager GetAccountDataManager { get; private set; }

        public void GoToAccount(Account account)
        {
            GetTransactionDataManager.GetAllTransactions(account.ID);
            while (true)
            {
                Console.WriteLine();
                for (int i = 0; i < Enum.GetNames(typeof(AccountCases)).Length; i++)
                {
                    AccountCases cases = (AccountCases)i;
                    Console.WriteLine($
                        
                        
                        {i + 1}. {cases.ToString().Replace("_", " ")}");
                }

                Console.Write(Resources.EnterChoice);

                string option = Console.ReadLine().Trim();
                if (int.TryParse(option, out int entryOption))
                {
                    if (entryOption != 0 && entryOption <= Enum.GetNames(typeof(AccountCases)).Count())
                    {
                        AccountCases operation = (AccountCases)entryOption - 1;
                        if (TransactionOperations(operation, account))
                            break;
                    }

                    else
                        Notification.Error(Resources.InvalidInteger);
                }
                else
                    Notification.Error(Resources.InvalidInput);
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
                    Notification.Info(string.Format(Resources.BalanceDisplay, account.Balance));
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
                    Notification.Error(Resources.InvalidInput);
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
                        Notification.Error(Resources.CardVerificationFailed);
                }
                else
                    Notification.Error(Resources.PaymentModeNotEnabled);
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
            if (UpdateBalance(account, amount, TransactionType.DEPOSIT))
            {
                RecordTransaction("Deposit", amount, account.Balance, TransactionType.DEPOSIT, account.ID, modeOfPayment, cardNumber);
                Notification.Success(Resources.DepositSuccess);
                return true;
            }
            else
                Notification.Error(Resources.DepositFailure);
            //AccountDataManager.BalanceChanged -= onBalanceChanged;
            return false;
        }


        public ModeOfPayment GetModeOfPayment(TransactionType transactionType)
        {
            string input;
            ModeOfPayment modeOfPayment = ModeOfPayment.DEFAULT;
            Console.WriteLine(Resources.ChoosePaymentMode);
            Console.WriteLine(Resources.ModeOfPayments);
            input = Console.ReadLine().Trim();
            if (input == Resources.BackButton) modeOfPayment = ModeOfPayment.DEFAULT;
            else if (input == "1") modeOfPayment = ModeOfPayment.CASH;
            else if (input == "2") modeOfPayment = ModeOfPayment.DEBIT_CARD;
            return modeOfPayment;
        }

        public bool RecordTransaction(string description, decimal amount, decimal balance, TransactionType transactionType, Guid accountID, ModeOfPayment modeOfPayment, string cardNumber)
        {
            Transaction transaction =
            new Transaction(description, amount, balance, transactionType, accountID, modeOfPayment, cardNumber);
            return InsertTransactionDataManager.InsertTransaction(transaction);
        }

        public bool Withdraw(Account account, decimal amount, ModeOfPayment modeOfPayment, string cardNumber)
        {
            try
            {
                if (amount > account.Balance)
                    Notification.Error(Resources.InsufficientBalance);
                else
                {
                    WithdrawHandlers(account);

                    if (UpdateBalance(account, amount, TransactionType.DEPOSIT))
                    {

                            Notification.Success(Resources.WithdrawSuccess);
                            bool isTransactionRecorded = RecordTransaction("Withdraw", amount, account.Balance, TransactionType.WITHDRAW, account.ID, modeOfPayment, cardNumber);
                            return true;
                    }
                    else
                        Notification.Error(Resources.WithdrawFailure);
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
                    UpdateBalance(currentAccount, currentAccount.CHARGES, TransactionType.WITHDRAW);
                    Notification.Info(Resources.MinimumBalanceCharged);
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
                    Notification.Info(string.Format(Resources.InterestDepositInitiated));
                    UpdateBalance(account, interest, TransactionType.DEPOSIT);
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
            
            if (amount > account.Balance) Notification.Error(Resources.InsufficientBalance);
            else
            {
                Account transferAccount = GetTransferAccount(account.AccountNumber);
                if (transferAccount != null)
                {
                    if (UpdateBalance(account, amount, TransactionType.WITHDRAW))
                    {
                        if (UpdateBalance(transferAccount, amount, TransactionType.DEPOSIT))
                        {
                            Console.WriteLine(Resources.TransferSuccess);
                            RecordTransaction("Transferred", amount, account.Balance, TransactionType.TRANSFER, account.ID, modeOfPayment, cardNumber);
                            RecordTransaction("Received", amount, account.Balance, TransactionType.RECEIVED, account.ID, modeOfPayment, cardNumber);
                        }
                        else
                        {
                            Notification.Error(Resources.TransferFailure);
                            UpdateBalance(account, amount, TransactionType.DEPOSIT);
                        }
                    }
                    else
                        Notification.Error(Resources.TransferFailure);
                }
            }
        }

        public void ViewAllTransactions(Guid accountID)
        {
            try
            {
                IEnumerable<Transaction> statements = GetTransactionDataManager.GetAllTransactions(accountID);
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
            IEnumerable<Transaction> statements = GetTransactionDataManager.GetAllTransactions(accountID);
            Printer.PrintStatement(statements);
        }

        public Account GetTransferAccount(string accountNumber)
        {
            try
            {
                while (true)
                {
                    Console.Write(Resources.EnterTransferAccountNumber);
                    string transferAccountNumber = Console.ReadLine().Trim();
                    if (transferAccountNumber == Resources.BackButton)
                        break;
                    else

                        if (accountNumber == transferAccountNumber)
                            Notification.Error(Resources.ChooseDifferentTransferAccount);
                        else
                        {
                            Account transferAccount = GetAccountDataManager.GetAccount(accountNumber);
                            if (transferAccount == null)
                            {
                                Notification.Error(Resources.InvalidAccountNumber);
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

        public bool UpdateBalance(Account account, decimal amount, TransactionType transactionType)
        {

            switch (transactionType)
            {
                case TransactionType.DEPOSIT:
                    account.Deposit(amount);
                    return UpdateAccountDataManager.UpdateAccount(account);
                case TransactionType.WITHDRAW:
                    account.Withdraw(amount);
                    return UpdateAccountDataManager.UpdateAccount(account);
                default:
                    break;
            }
            return false;
        }

        public decimal GetAmount()
        {
            try
            {
                while (true)
                {
                    Console.Write(Resources.EnterAmount);
                    decimal amount = decimal.Parse(Console.ReadLine().Trim());
                    if (amount < 0) Notification.Error(Resources.PositiveAmountWarning);
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
