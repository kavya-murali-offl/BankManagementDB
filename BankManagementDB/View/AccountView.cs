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
using BankManagementDB.UseCase;

namespace BankManagementDB.View
{
    public class AccountView
    {
        private Account _generatedAccount = null;

        public static Account SelectedAccount { get;  set; }
       
        public AccountView()
        {
            UpdateAccountDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IUpdateAccountDataManager>();
            TransactionView = new TransactionView();
        }

        public TransactionView TransactionView { get; private set; }

        public IUpdateAccountDataManager UpdateAccountDataManager { get; private set; }

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
                        isAuthenticated = true;
                    else if (modeOfPayment == ModeOfPayment.DEBIT_CARD || modeOfPayment == ModeOfPayment.CREDIT_CARD)
                    {
                        cardNumber = cardsView.GetCardNumber();
                        if (cardNumber != null)
                            isAuthenticated = IsAuthenticated(modeOfPayment, cardNumber);
                    }
                    if (!isAuthenticated)
                        Notification.Error(DependencyContainer.GetResource("CardVerificationFailed"));
                }
                else
                    Notification.Error(DependencyContainer.GetResource("PaymentModeNotEnabled"));
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
            Console.WriteLine(DependencyContainer.GetResource("ChoosePaymentMode"));
            Console.WriteLine(DependencyContainer.GetResource("ModeOfPayments") + "\n");
            input = Console.ReadLine()?.Trim();
            Console.WriteLine();
            if (input == DependencyContainer.GetResource("BackButton")) modeOfPayment = ModeOfPayment.DEFAULT;
            else if (input == "1") modeOfPayment = ModeOfPayment.CASH;
            else if (input == "2") modeOfPayment = ModeOfPayment.DEBIT_CARD;
            return modeOfPayment;
        }

        public bool Withdraw(decimal amount, ModeOfPayment modeOfPayment, string cardNumber)
        {
            if (amount > SelectedAccount.Balance)
                Notification.Error(DependencyContainer.GetResource("InsufficientBalance"));
            else
            {
                WithdrawHandlers();

                if (UpdateBalance(SelectedAccount, amount, TransactionType.WITHDRAW))
                {
                    Notification.Success(string.Format(DependencyContainer.GetResource("WithdrawSuccess"), amount));
                    bool isTransactionRecorded = TransactionView.RecordTransaction("Withdraw", amount, SelectedAccount.Balance, TransactionType.WITHDRAW, SelectedAccount.AccountNumber, modeOfPayment, cardNumber, null);
                    return true;
                }
                else
                    Notification.Error(DependencyContainer.GetResource("WithdrawFailure"));
            }

            return false;
        }

        private void WithdrawHandlers()
        {
            if (SelectedAccount is SavingsAccount)
                DepositInterest(SelectedAccount as SavingsAccount);

            else if (SelectedAccount is CurrentAccount)
                ChargeForMinBalance(SelectedAccount as CurrentAccount);
        }

        private void ChargeForMinBalance(CurrentAccount currentAccount)
        {
            if (currentAccount.Balance < currentAccount.MinimumBalance && currentAccount.Balance > currentAccount.CHARGES)
            {
                UpdateBalance(currentAccount, currentAccount.CHARGES, TransactionType.WITHDRAW);
                Notification.Info(DependencyContainer.GetResource("MinimumBalanceCharged"));
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
                Notification.Info(string.Format(DependencyContainer.GetResource("InterestDepositInitiated")));
                UpdateBalance(account, interest, TransactionType.DEPOSIT);
                TransactionView.RecordTransaction("Interest", interest, account.Balance, TransactionType.DEPOSIT, null, ModeOfPayment.INTERNAL,null, account.AccountNumber);
                return interest;
            }
            return 0;
        }

        public void Transfer(decimal amount, ModeOfPayment modeOfPayment, string cardNumber)
        {
            //Account transferAccount = GetTransferAccount(SelectedAccount.AccountNumber);
            //if (amount > SelectedAccount.Balance) Notification.Error(DependencyContainer.GetResource("InsufficientBalance);
            //else
            //{
            //    if (transferAccount != null)
            //    {
            //        TransferAmountUseCase transferAmountUseCase = new TransferAmountUseCase();
            //        transferAmountUseCase.TransferAmount(SelectedAccount, transferAccount, amount);
            //    }
            //}
            if (amount > SelectedAccount.Balance) Notification.Error(DependencyContainer.GetResource("InsufficientBalance"));
            else
            {
                Account transferAccount = GetTransferAccount(SelectedAccount.AccountNumber);
                if (transferAccount != null)
                {
                    if (UpdateBalance(SelectedAccount, amount, TransactionType.WITHDRAW))
                    {
                        if (UpdateBalance(transferAccount, amount, TransactionType.DEPOSIT))
                        {
                            Notification.Success(string.Format(DependencyContainer.GetResource("TransferSuccess"), amount));
                            TransactionView.RecordTransaction("Transferred", amount, SelectedAccount.Balance, TransactionType.TRANSFER, SelectedAccount.AccountNumber, modeOfPayment, cardNumber, null);
                            TransactionView.RecordTransaction("Received", amount, SelectedAccount.Balance, TransactionType.RECEIVED, null, modeOfPayment, cardNumber, SelectedAccount.AccountNumber);
                        }
                        else
                        {
                            Notification.Error(DependencyContainer.GetResource("TransferFailure"));
                            UpdateBalance(SelectedAccount, amount, TransactionType.DEPOSIT);
                        }
                    }
                    else
                        Notification.Error(DependencyContainer.GetResource("TransferFailure"));
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
                return true;
            else
                return cardsView.Authenticate(cardNumber);
        }


        public bool Deposit(decimal amount, ModeOfPayment modeOfPayment, string cardNumber)
        {
            if (UpdateBalance(SelectedAccount, amount, TransactionType.DEPOSIT))
            {
                TransactionView.RecordTransaction("Deposit", amount, SelectedAccount.Balance, TransactionType.DEPOSIT, null, modeOfPayment, cardNumber, SelectedAccount.AccountNumber);
                Notification.Success(string.Format(DependencyContainer.GetResource("DepositSuccess"), amount));
                return true;
            }
            else
                Notification.Error(DependencyContainer.GetResource("DepositFailure"));
            return false;
        }

        public Account GetTransferAccount(string accountNumber)
        {
            while (true)
            {
                Console.Write(DependencyContainer.GetResource("EnterTransferAccountNumber"));
                string transferAccountNumber = Console.ReadLine()?.Trim();
                if (transferAccountNumber == DependencyContainer.GetResource("BackButton"))
                    break;
                else
                {
                    if (accountNumber == transferAccountNumber)
                        Notification.Error(DependencyContainer.GetResource("ChooseDifferentTransferAccount"));
                    else
                    {
                        Account transferAccount = Store.GetAccountByAccountNumber(transferAccountNumber);
                        if (transferAccount == null)
                        {
                            Notification.Error(DependencyContainer.GetResource("InvalidAccountNumber"));
                            break;
                        }
                        else return transferAccount;
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
                AccountCases.BACK => true,
                _ => Default(),

            };

        private bool CheckBalance()
        {
            Notification.Info(string.Format(DependencyContainer.GetResource("BalanceDisplay"), SelectedAccount.Balance));
            return false;
        }

        private bool Default()
        {
            Notification.Error(DependencyContainer.GetResource("InvalidInput"));
            return false;
        }

        public Account GenerateAccount()
        {
            Notification.Info(DependencyContainer.GetResource("PressBackButtonInfo"));

            OptionsDelegate<AccountType> options = GetAccountByType;
            HelperView helperView = new HelperView();
            helperView.PerformOperation(options);

            return _generatedAccount;
        }

        public bool GetAccountByType(AccountType accountType)
        {
            IAccountFactory AccountFactory = DependencyContainer.ServiceProvider.GetRequiredService<IAccountFactory>();
            _generatedAccount = AccountFactory.GetAccountByType(accountType);
            _generatedAccount.ID = Guid.NewGuid().ToString();
            _generatedAccount.AccountNumber = RandomGenerator.GenerateAccountNumber();
            _generatedAccount.Balance = 0;
            _generatedAccount.Status = AccountStatus.ACTIVE;
            _generatedAccount.CreatedOn = DateTime.Now;
            _generatedAccount.MinimumBalance = 0;
            return true;
        }

        public Account GetAccount()
        {
            while (true)
            {
                Console.Write(DependencyContainer.GetResource("EnterAccountNumber"));
                string accountNumber = Console.ReadLine()?.Trim();
                if (accountNumber == DependencyContainer.GetResource("BackButton"))
                    break;
                else
                {
                    Account account = Store.GetAccountByAccountNumber(accountNumber);
                    if (account == null)
                        Notification.Error(DependencyContainer.GetResource("InvalidAccountNumber"));
                    else
                        return account;
                }
            }
            return null;
        }
    }

    public class TransferEventArgs : EventArgs
    {
        public decimal amount { get; set; }

        public TransactionType transactionType { get; set; }
    }
}
