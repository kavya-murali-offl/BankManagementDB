using BankManagementDB.Controller;
using BankManagementDB.EnumerationType;
using BankManagementDB.Interface;
using BankManagementDB.Model;
using BankManagementDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.View
{
    public class CreditCardView
    {
        public CreditCardView(IGetAccountDataManager getAcountDataManager, ITransactionDataManager transactionDataManager, IUpdateCardDataManager updateCardDataManager, IGetCardDataManager getCardDataManager) {
            UpdateCardDataManager = updateCardDataManager;
            GetCardDataManager = getCardDataManager;
            TransactionDataManager = transactionDataManager;
            GetAccountDataManager = getAcountDataManager;
        }
        public IGetCardDataManager GetCardDataManager { get; private set; }

        public IUpdateCardDataManager UpdateCardDataManager { get; private set; }

        public ITransactionDataManager TransactionDataManager { get; private set; }

        public IGetAccountDataManager GetAccountDataManager { get; private set; }

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

                case CreditCardCases.VIEW_STATEMENT:
                    ViewStatement();
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
                                if (UpdateCardDataManager.UpdateDueAmount(CreditCardCases.PURCHASE, card, amount))
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
                                    if (UpdateCardDataManager.UpdateDueAmount(CreditCardCases.PAYMENT, card, amount))
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

        public void ViewStatement()
        {
            CardView cardView = new CardView();
            string cardNumber = cardView.GetCardNumber();
            bool isAuthenticated = AuthenticateCreditCard(CreditCardCases.VIEW_STATEMENT, cardNumber);
            if (isAuthenticated)
            {
                IEnumerable<Transaction> statements = TransactionDataManager.GetAllCreditCardTransactions(cardNumber);
                foreach(var statement in statements)
                    Console.WriteLine(statement);
            }
        }

    }
}
