using BankManagementDB.Config;
using BankManagementDB.Controller;
using BankManagementDB.DataManager;
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
                    Console.Write("\nEnter your choice: ");
                    string option = Console.ReadLine().Trim();
                    int entryOption;

                    if (!int.TryParse(option, out entryOption))
                        Notification.Error("Invalid input! Please enter a valid number.");
                    else
                    if (entryOption != 0 && entryOption <= Enum.GetNames(typeof(CreditCardCases)).Count())
                    {
                        CreditCardCases cases = (CreditCardCases)entryOption - 1;
                        if (CreditCardOperations(cases))
                            break;
                    }
                    else if (entryOption == 0)
                        break;
                    else
                        Notification.Error("Invalid input! Please enter a valid number.");
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
                    Notification.Error("Enter a valid option.\n");
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
                        Notification.Error("Incorrect pin");
                else
                    Notification.Error("Please enter a valid credit card number");
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
                {
                    if (card.Type == CardType.CREDIT)
                    {
                        TransactionView transactionView = new TransactionView();
                        decimal amount = transactionView.GetAmount();
                        if (amount > 0)
                        {
                            if ((amount + card.TotalDueAmount) < card.CreditLimit)
                            {
                                if (UpdateDueAmount(CreditCardCases.PURCHASE, card, amount))
                                {
                                    Notification.Success("Purchase successful");
                                    bool isTransacted = transactionView.RecordTransaction("Purchase", amount, card.TotalDueAmount, TransactionType.PURCHASE, Guid.Empty, ModeOfPayment.CREDIT_CARD, card.CardNumber);
                                }
                                else
                                    Notification.Error("Purchase failed");
                            }
                            else
                                Notification.Error("Purchase amount is greater than Available credit limit.");
                        }
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
                    TransactionView transactionView = new TransactionView();
                    AccountView accountView = new AccountView();
                    Account account = accountView.GetAccount();
                     
                    if (account != null)
                    {
                        decimal amount = transactionView.GetAmount();
                        if (amount > 0)
                        {
                                if(transactionView.Withdraw(account, amount, ModeOfPayment.DEBIT_CARD, card.CardNumber))
                                    if (UpdateDueAmount(CreditCardCases.PAYMENT, card, amount))
                                    {
                                        Notification.Success("Payment successful");
                                        bool isTransacted = transactionView.RecordTransaction("Payment", amount, card.TotalDueAmount, TransactionType.PAYMENT, Guid.Empty, ModeOfPayment.CREDIT_CARD, card.CardNumber);
                                    }
                                    else
                                        Notification.Error("Payment failed");
                        }
                    }
                    else
                        Notification.Error("Account not exist");
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
