using System;
using BankManagementDB.EnumerationType;
using BankManagementDB.Models;
using BankManagementDB.Config;
using BankManagementDB.Interface;
using Microsoft.Extensions.DependencyInjection;
using BankManagementDB.Properties;
using BankManagementDB.Utility;
using BankManagementDB.Model;
using BankManagementDB.Data;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using BankManagementDB.DataManager;
using System.Security.Principal;
using BankManagementDB.Controller;

namespace BankManagementDB.View
{
    public class AccountView
    {
        private Account GeneratedAccount { get; set; }

        public static Account SelectedAccount { get;  set; }
       
        public AccountView()
        {
            UpdateAccountDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IUpdateAccountDataManager>();
            TransactionView = new TransactionView();
        }

        public TransactionView TransactionView { get; private set; }

        public IUpdateAccountDataManager UpdateAccountDataManager { get; private set; }

        public Account CreateAccount(AccountType accountType)
        {

            IAccountFactory AccountFactory = DependencyContainer.ServiceProvider.GetRequiredService<IAccountFactory>();

            Account account = AccountFactory.GetAccountByType(accountType);
            account.ID = Guid.NewGuid().ToString();
            account.AccountNumber = RandomGenerator.GenerateAccountNumber();
            account.Balance = 0;
            account.Status = AccountStatus.ACTIVE;
            account.InterestRate = Constants.AccountConstants.CURRENT_INTEREST_RATE;
            account.Type = AccountType.CURRENT;
            account.CreatedOn = DateTime.Now;
            account.MinimumBalance = 500;

            return account;
        }

        public bool InitiateTransaction(TransactionType transactionType)
        {
            CardView cardsView = new CardView();
            bool isAuthenticated = false;
            string cardNumber = null;

            ModeOfPayment modeOfPayment = GetModeOfPayment(transactionType);

            if (modeOfPayment != ModeOfPayment.DEFAULT)
            {
                if (cardsView.ValidateModeOfPayment(SelectedAccount.ID, modeOfPayment))
                {
                    if (modeOfPayment == ModeOfPayment.CASH)
                    {
                        isAuthenticated = true;
                    }
                    else if (modeOfPayment == ModeOfPayment.DEBIT_CARD || modeOfPayment == ModeOfPayment.CREDIT_CARD)
                    {
                        cardNumber = cardsView.GetCardNumber();
                        if (cardNumber != null)
                        {
                            isAuthenticated = IsAuthenticated(modeOfPayment, cardNumber);
                        }
                    }
                    if (!isAuthenticated)
                    {
                        Notification.Error(Resources.CardVerificationFailed);
                    }
                }
                else
                {
                    Notification.Error(Resources.PaymentModeNotEnabled);
                }
            }

            if (isAuthenticated)
            {
                HelperView helperView = new HelperView();
                decimal amount = helperView.GetAmount();
                if (amount > 0)
                {
                    switch (transactionType)
                    {
                        case TransactionType.DEPOSIT:
                            Deposit(amount, modeOfPayment, cardNumber);
                            break;

                        case TransactionType.WITHDRAW:
                            Withdraw(amount, modeOfPayment, cardNumber);
                            break;

                        case TransactionType.TRANSFER:
                            Transfer(amount, modeOfPayment, cardNumber);
                            break;

                        default:
                            break;
                    }
                }
            }
            return false;
        }

        public ModeOfPayment GetModeOfPayment(TransactionType transactionType)
        {
            string input;
            ModeOfPayment modeOfPayment = ModeOfPayment.DEFAULT;
            Console.WriteLine(Resources.ChoosePaymentMode);
            Console.WriteLine(Resources.ModeOfPayments + "\n");
            input = Console.ReadLine()?.Trim();
            Console.WriteLine();
            if (input == Resources.BackButton)
            {
                modeOfPayment = ModeOfPayment.DEFAULT;
            }
            else if (input == "1")
            {
                modeOfPayment = ModeOfPayment.CASH;
            }
            else if (input == "2")
            {
                modeOfPayment = ModeOfPayment.DEBIT_CARD;
            }

            return modeOfPayment;
        }

        public bool Withdraw(decimal amount, ModeOfPayment modeOfPayment, string cardNumber)
        {
            if (amount > SelectedAccount.Balance)
            {
                Notification.Error(Resources.InsufficientBalance);
            }
            else
            {
                WithdrawHandlers();

                if (UpdateBalance(SelectedAccount, amount, TransactionType.WITHDRAW))
                {
                    Notification.Success(Formatter.FormatString(Resources.WithdrawSuccess, amount));
                    bool isTransactionRecorded = TransactionView.RecordTransaction("Withdraw", amount, SelectedAccount.Balance, TransactionType.WITHDRAW, SelectedAccount.AccountNumber, modeOfPayment, cardNumber, null);
                    return true;
                }
                else
                    Notification.Error(Resources.WithdrawFailure);
            }

            return false;
        }

        private void WithdrawHandlers()
        {
            if (SelectedAccount is SavingsAccount)
            {
                DepositInterest(SelectedAccount as SavingsAccount);
            }

            else if (SelectedAccount is CurrentAccount)
            {
                ChargeForMinBalance(SelectedAccount as CurrentAccount);
            }
        }

        private void ChargeForMinBalance(CurrentAccount currentAccount)
        {
            if (currentAccount.Balance < currentAccount.MinimumBalance && currentAccount.Balance > currentAccount.CHARGES)
            {
                UpdateBalance(currentAccount, currentAccount.CHARGES, TransactionType.WITHDRAW);
                Notification.Info(Resources.MinimumBalanceCharged);
                TransactionView.RecordTransaction("Minimum Balance Charge",
                    currentAccount.CHARGES, currentAccount.Balance,
                    TransactionType.WITHDRAW, currentAccount.AccountNumber, ModeOfPayment.INTERNAL, null, null);
            }
        }

        private decimal DepositInterest(SavingsAccount account)
        {
            decimal interest = account.GetInterest();
            if (interest > 0)
            {
                Notification.Info(Formatter.FormatString(Resources.InterestDepositInitiated));
                UpdateBalance(account, interest, TransactionType.DEPOSIT);
                TransactionView.RecordTransaction("Interest", interest, account.Balance, TransactionType.DEPOSIT, null, ModeOfPayment.INTERNAL,null, account.AccountNumber);
                return interest;
            }
            return 0;
        }

        public void Transfer(decimal amount, ModeOfPayment modeOfPayment, string cardNumber)
        {
            if (amount > SelectedAccount.Balance)
            {
                Notification.Error(Resources.InsufficientBalance);
            }
            else
            {
                Account transferAccount = GetTransferAccount(SelectedAccount.AccountNumber);
                if (transferAccount != null)
                {
                    if (UpdateBalance(SelectedAccount, amount, TransactionType.WITHDRAW))
                    {
                        if (UpdateBalance(transferAccount, amount, TransactionType.DEPOSIT))
                        {
                            Notification.Success(Formatter.FormatString(Resources.TransferSuccess, amount));
                            TransactionView.RecordTransaction("Transferred", amount, SelectedAccount.Balance, TransactionType.TRANSFER, SelectedAccount.AccountNumber, modeOfPayment, cardNumber, null);
                            TransactionView.RecordTransaction("Received", amount, SelectedAccount.Balance, TransactionType.RECEIVED, null, modeOfPayment, cardNumber, SelectedAccount.AccountNumber);
                        }
                        else
                        {
                            Notification.Error(Resources.TransferFailure);
                            UpdateBalance(SelectedAccount, amount, TransactionType.DEPOSIT);
                        }
                    }
                    else
                    {
                        Notification.Error(Resources.TransferFailure);
                    }
                }
            }
        }

        public bool ViewAccountDetails()
        {
            Console.WriteLine(SelectedAccount);
            return false;
        }

        public bool IsAuthenticated(ModeOfPayment modeOfPayment, string cardNumber)
        {
            CardView cardsView = new CardView();

            if (modeOfPayment == ModeOfPayment.CASH)
            {
                return true;
            }
            else
            {
                return cardsView.Authenticate(cardNumber);
            }
        }

        public bool Deposit(decimal amount, ModeOfPayment modeOfPayment, string cardNumber)
        {
            if (UpdateBalance(SelectedAccount, amount, TransactionType.DEPOSIT))
            {
                TransactionView.RecordTransaction("Deposit", amount, SelectedAccount.Balance, TransactionType.DEPOSIT, null, modeOfPayment, cardNumber, SelectedAccount.AccountNumber);
                Notification.Success(Formatter.FormatString(Resources.DepositSuccess, amount));
                return true;
            }
            else
            {
                Notification.Error(Resources.DepositFailure);
            }
            return false;
        }

        public Account GetTransferAccount(string accountNumber)
        {
            while (true)
            {
                Console.Write(Resources.TransferAccountNumber);
                string transferAccountNumber = Console.ReadLine()?.Trim();
                if (transferAccountNumber == Resources.BackButton)
                { break; }
                else
                {
                    if (accountNumber == transferAccountNumber)
                    {
                        Notification.Error(Resources.ChooseDifferentTransferAccount);
                    }
                    else
                    {
                        Account transferAccount = Store.GetAccountByAccountNumber(transferAccountNumber);
                        if (transferAccount == null)
                        {
                            Notification.Error(Resources.InvalidAccountNumber);
                            break;
                        }
                        else
                        {
                            return transferAccount;
                        }
                    }
                }
            }
            return null;
        }

        public bool UpdateBalance(Account account, decimal amount, TransactionType transactionType) =>
        transactionType switch
        {
            TransactionType.DEPOSIT => Deposit(account, amount),
            TransactionType.WITHDRAW => Withdraw(account, amount),
            _ => false
        };

        private bool Deposit(Account account, decimal amount)
        {
            account.Deposit(amount);
            return UpdateAccountDataManager.UpdateAccount(account);
        }

        private bool Withdraw(Account account, decimal amount)
        {
            account.Withdraw(amount);
            return UpdateAccountDataManager.UpdateAccount(account);
        }

        public void GoToAccount(Account account)
        {
            SelectedAccount = account;
            TransactionView.LoadAllTransactions(account.AccountNumber);

            OptionsDelegate<AccountCases> options = AccountOperations;
            HelperView helperView = new HelperView();
            helperView.PerformOperation(options);
        }

        public bool AccountOperations(AccountCases command) =>
            command switch
            {

                AccountCases.DEPOSIT => InitiateTransaction(TransactionType.DEPOSIT),
                AccountCases.WITHDRAW => InitiateTransaction(TransactionType.WITHDRAW),
                AccountCases.TRANSFER => InitiateTransaction(TransactionType.TRANSFER),
                AccountCases.CHECK_BALANCE => CheckBalance(),
                AccountCases.VIEW_STATEMENT => TransactionView.ViewAllTransactions(),
                AccountCases.PRINT_STATEMENT => TransactionView.PrintStatement(),
                AccountCases.VIEW_ACCOUNT_DETAILS => ViewAccountDetails(),
                AccountCases.GO_BACK => true,
                AccountCases.EXIT => Exit(),
                _ => Default(),

            };

        private bool Exit()
        {
            Environment.Exit(0);
            return true;
        }

        private bool CheckBalance()
        {
            Notification.Info(Formatter.FormatString(Resources.BalanceDisplay, SelectedAccount.Balance));
            return false;
        }

        private bool Default()
        {
            Notification.Error(Resources.InvalidInput);
            return false;
        }

        public Account GenerateAccount()
        {
            Notification.Info(Resources.PressBackButtonInfo);

            OptionsDelegate<AccountType> options = GetAccountByType;
            HelperView helperView = new HelperView();
            helperView.PerformOperation(options);

            return GeneratedAccount;
        }

        public bool GetAccountByType(AccountType accountType)
        {
            IAccountFactory AccountFactory = DependencyContainer.ServiceProvider.GetRequiredService<IAccountFactory>();
            GeneratedAccount = AccountFactory.GetAccountByType(accountType);
            GeneratedAccount.ID = Guid.NewGuid().ToString();
            GeneratedAccount.AccountNumber = RandomGenerator.GenerateAccountNumber();
            GeneratedAccount.Balance = 0;
            GeneratedAccount.Status = AccountStatus.ACTIVE;
            GeneratedAccount.CreatedOn = DateTime.Now;
            GeneratedAccount.MinimumBalance = 0;
            return true;
        }

        public Account GetAccount()
        {
            while (true)
            {
                Console.Write(Resources.AccountNumber + ": ");
                string accountNumber = Console.ReadLine()?.Trim();
                if (accountNumber == Resources.BackButton)
                { break; }
                else
                {
                    Account account = Store.GetAccountByAccountNumber(accountNumber);
                    if (account == null)
                    { Notification.Error(Resources.InvalidAccountNumber); }
                    else
                    {
                        return account;
                    }
                }
            }
            return null;
        }
    }
}
