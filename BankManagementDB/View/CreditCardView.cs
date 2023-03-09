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


namespace BankManagementDB.View
{
    public class CreditCardView
    {
        public event Action<string> CardDueAmountChanged;

        public CreditCardView(IGetCardDataManager getCardDataManager) {
            UpdateCreditCardDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IUpdateCreditCardDataManager>();
            GetCardDataManager = getCardDataManager;
        }
        public IGetCardDataManager GetCardDataManager { get; private set; }

        public IUpdateCreditCardDataManager UpdateCreditCardDataManager { get; private set; }


        public void CreditCardServices()
        {
            try
            {
                while (true)
                {
                    for (int i = 0; i < Enum.GetNames(typeof(CreditCardCases)).Length; i++)
                    {
                        CreditCardCases cases = (CreditCardCases)i;
                        Console.WriteLine($"{i + 1}. {cases.ToString().Replace("_", " ")}");
                    }

                    Console.Write(Resources.EnterChoice);

                    string option = Console.ReadLine().Trim();

                    if (!int.TryParse(option, out int entryOption))
                        Notification.Error(Resources.InvalidInteger);
                    else
                        if (entryOption == 0)
                            break;
                        else if (entryOption != 0 && entryOption <= Enum.GetNames(typeof(CreditCardCases)).Count())
                        {
                            CreditCardCases cases = (CreditCardCases)entryOption - 1;
                            if (CreditCardOperations(cases))
                                break;
                        }
                        else
                            Notification.Error(Resources.InvalidOption);
                }
            }
            catch (Exception err)
            {
                Notification.Error(err.Message);
            }
        }

        public bool CreditCardOperations(CreditCardCases operation)
        {

            switch (operation)
            {
                case CreditCardCases.PURCHASE:
                    MakePurchase();
                    return false;

                case CreditCardCases.PAYMENT:
                    MakePayment();
                    return false;

                case CreditCardCases.EXIT:
                    return true;

                default:
                    Notification.Error(Resources.InvalidOption);
                    return false;
            }
        }

        public bool AuthenticateCreditCard(CreditCardCases cases, string cardNumber)
        {
            CardView cardView = new CardView();
            if (cardNumber != null)
            {
                if (GetCardDataManager.IsCreditCard(cardNumber))
                    if (cardView.Authenticate(cardNumber))
                        return true;
                    else
                        Notification.Error(Resources.InvalidPin);
                else
                    Notification.Error(Resources.InvalidCreditCardNumber);
            }
            return false;
        }

        public void MakePurchase()
        {
            CardView cardView = new CardView();
            string cardNumber = cardView.GetCardNumber();
            bool isAuthenticated =  AuthenticateCreditCard(CreditCardCases.PURCHASE, cardNumber);
            if(isAuthenticated)
            {
                CardBObj card = GetCardDataManager.GetCard(cardNumber);
                if (card != null)
                    if (card.Type == CardType.CREDIT)
                    {
                        TransactionView transactionView = new TransactionView();
                        decimal amount = transactionView.GetAmount();
                        if (amount > 0)
                        {
                            if ((amount + card.TotalDueAmount) < card.CreditLimit)
                                if (UpdateDueAmount(CreditCardCases.PURCHASE, card, amount))
                                {
                                    Notification.Success(Resources.PurchaseSuccess);
                                    bool isTransacted = transactionView.RecordTransaction("Purchase", amount, card.TotalDueAmount, TransactionType.PURCHASE, Guid.Empty, ModeOfPayment.CREDIT_CARD, card.CardNumber);
                                }
                                else
                                    Notification.Error(Resources.PurchaseFailure);
                            else
                                Notification.Error(Resources.CreditLimitReached);
                        }
                    }
            }
        }

        public void MakePayment()
        {
            CardView cardView = new CardView();
            string cardNumber = cardView.GetCardNumber();

            bool isAuthenticated = AuthenticateCreditCard(CreditCardCases.PAYMENT, cardNumber);
            if(isAuthenticated)
            {
                CardBObj card = GetCardDataManager.GetCard(cardNumber);
                if (card != null)
                {
                    AccountView accountView = new AccountView();
                    Account account = accountView.GetAccount();
                     
                    if (account != null)
                    {
                        TransactionView transactionView = new TransactionView();
                        decimal amount = transactionView.GetAmount();

                        if (amount > 0)
                                if(transactionView.Withdraw(account, amount, ModeOfPayment.DEBIT_CARD, card.CardNumber))
                                    if (UpdateDueAmount(CreditCardCases.PAYMENT, card, amount))
                                    {
                                        Notification.Success(Resources.PaymentSuccess);
                                        bool isTransacted = transactionView.RecordTransaction("Payment", amount, card.TotalDueAmount, TransactionType.PAYMENT, Guid.Empty, ModeOfPayment.CREDIT_CARD, card.CardNumber);
                                    }
                                    else
                                        Notification.Error(Resources.PaymentFailure);
                    }
                }
            }
        }

        public bool UpdateDueAmount(CreditCardCases cases, CardBObj card, decimal amount)
        {
            CreditCard creditCard;

            switch (cases)
            {
                case CreditCardCases.PURCHASE:

                    card.TotalDueAmount += amount;
                    CardDueAmountChanged?.Invoke($"Purchase of Rs.{amount} is successful");
                    creditCard = Mapper.Map<CardBObj, CreditCard>(card);
                    UpdateCreditCardDataManager.UpdateCreditCard(creditCard);
                    return true;

                case CreditCardCases.PAYMENT:

                    card.TotalDueAmount -= amount;
                    CardDueAmountChanged?.Invoke($"Payment of Rs.{amount} is sucessful");
                    creditCard = Mapper.Map<CardBObj, CreditCard>(card);
                    UpdateCreditCardDataManager.UpdateCreditCard(creditCard);
                    return true;

                default:

                    return false;
            }
        }
    }
}
