using BankManagementDB.Config;
using BankManagementDB.Properties;
using BankManagementDB.EnumerationType;
using BankManagementDB.Interface;
using BankManagementDB.Model;
using BankManagementDB.Models;
using BankManagementDB.Utility;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankManagementDB.Data;
using BankManagementDB.DataManager;
using System.Data.Entity.Core.Mapping;

namespace BankManagementDB.View
{
    public class CreditCardView
    {
        public event Action<string> CardDueAmountChanged;

        public CreditCardView() {
            UpdateCreditCardDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IUpdateCreditCardDataManager>();
            GetCardDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IGetCardDataManager>();
        }
        public IGetCardDataManager GetCardDataManager { get; private set; }

        public IUpdateCreditCardDataManager UpdateCreditCardDataManager { get; private set; }

        public void CreditCardServices()
        {
            try
            {
                OptionsDelegate<CreditCardCases> options = CreditCardOperations;

                HelperView helperView = new HelperView();
                helperView.PerformOperation(options);

            }
            catch (Exception err)
            {
                Notification.Error(err.Message);
            }
        }

        public bool CreditCardOperations(CreditCardCases command) =>
            command switch
            {
                CreditCardCases.PURCHASE => MakePurchase(),
                CreditCardCases.PAYMENT => MakePayment(),
                CreditCardCases.GO_BACK => true,
                CreditCardCases.EXIT => Exit(),
                _ => Default()

            };

        private bool Exit()
        {
            Environment.Exit(0);
            return true;
        }

        private bool Default()
        {
            Notification.Error(Resources.InvalidOption);
            return false;
        }

        public bool CreateCreditCard(Card card)
        {
            CreditCardDTO creditCard = new CreditCardDTO()
            {
                CreditLimit = 10000,
                APR = 0.060m,
                CreditPoints = 100,
                TotalDueAmount = 0,
                ID = card.ID
            };

            IInsertCreditCardDataManager insertCreditCardDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IInsertCreditCardDataManager>();
            return insertCreditCardDataManager.InsertCreditCard(creditCard);
        }

        public bool AuthenticateCreditCard(CreditCardCases cases, string cardNumber)
        {
            CardView cardView = new CardView();
            if (cardNumber != null)
            {
                if (Store.IsCreditCard(cardNumber))
                {
                    if (cardView.Authenticate(cardNumber))
                    {
                        return true;
                    }
                    else
                    {
                        Notification.Error(Resources.InvalidPin);
                    }
                }
                else
                {
                    Notification.Error(Resources.InvalidCreditCardNumber);
                }
            }
            return false;
        }

        private bool MakePurchase()
        {
            CardView cardView = new CardView();
            string cardNumber = cardView.GetCardNumber();
            bool isAuthenticated =  AuthenticateCreditCard(CreditCardCases.PURCHASE, cardNumber);
            if(isAuthenticated)
            {
                CreditCard creditCard = Store.GetCard(cardNumber) as CreditCard;
                if (creditCard != null)
                {
                    HelperView helperView = new HelperView();
                    decimal amount = helperView.GetAmount();
                    if (amount > 0)
                    {
                        if ((amount + creditCard.TotalDueAmount) < creditCard.CreditLimit)
                        {
                            Console.Write(Resources.AccountNumber + ": ");
                            string recipient = Console.ReadLine();
                            if (UpdateDueAmount(CreditCardCases.PURCHASE, creditCard, amount))
                            {
                                Notification.Success(Resources.PurchaseSuccess);
                                TransactionView transactionView = new TransactionView();
                                creditCard.TotalDueAmount += amount;
                                bool isTransacted = transactionView.RecordTransaction("Purchase", amount, creditCard.TotalDueAmount, TransactionType.PURCHASE, null, ModeOfPayment.CREDIT_CARD, creditCard.CardNumber, recipient);
                            }
                            else
                            {
                                Notification.Error(Resources.PurchaseFailure);
                            }
                        }
                        else
                        {
                            Notification.Error(Resources.CreditLimitReached);
                        }
                    }
                }
            }
            return false;

        }

        private bool MakePayment()
        {
            CardView cardView = new CardView();
            string cardNumber = cardView.GetCardNumber();

            bool isAuthenticated = AuthenticateCreditCard(CreditCardCases.PAYMENT, cardNumber);
            if(isAuthenticated)
            {
                CreditCard creditCard = Store.GetCard(cardNumber) as CreditCard;
                if (creditCard != null)
                {
                    AccountView accountView = new AccountView();
                    Account account = accountView.GetAccount();
                     
                    if (account != null)
                    {
                        HelperView helperView = new HelperView();
                        TransactionView transactionView = new TransactionView();
                        decimal amount = helperView.GetAmount();
                        AccountView.SelectedAccount = account;
                        if (amount > 0)
                        {
                            if (accountView.Withdraw(amount, ModeOfPayment.DEBIT_CARD, creditCard.CardNumber))
                            {
                                if (UpdateDueAmount(CreditCardCases.PAYMENT, creditCard, amount))
                                {
                                    Notification.Success(Resources.PaymentSuccess);
                                    creditCard.TotalDueAmount -= amount;
                                    bool isTransacted = transactionView.RecordTransaction("Payment", amount, creditCard.TotalDueAmount, TransactionType.PAYMENT, account.AccountNumber, ModeOfPayment.CREDIT_CARD, creditCard.CardNumber, null);
                                }
                                else
                                {
                                    Notification.Error(Resources.PaymentFailure);
                                }
                            }
                        }
                    }
                }
            }
            return false;

        }

        public bool UpdateDueAmount(CreditCardCases cases, CreditCard creditCard, decimal amount)
        {
            CreditCardDTO creditCardDTO = Mapper.Map<CreditCard, CreditCardDTO>(creditCard);
            switch (cases)
            {
                case CreditCardCases.PURCHASE:

                    creditCardDTO.TotalDueAmount += amount;
                    CardDueAmountChanged?.Invoke($"Purchase of Rs.{amount} is successful");
                    UpdateCreditCardDataManager.UpdateCreditCard(creditCardDTO);
                    return true;

                case CreditCardCases.PAYMENT:

                    creditCardDTO.TotalDueAmount -= amount;
                    CardDueAmountChanged?.Invoke($"Payment of Rs.{amount} is sucessful");
                    UpdateCreditCardDataManager.UpdateCreditCard(creditCardDTO);
                    return true;

                default:
                    return false;
            }
        }
    }
}
