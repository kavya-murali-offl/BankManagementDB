using BankManagementDB.Models;
using BankManagementDB.Config;
using BankManagementDB.EnumerationType;
using BankManagementDB.Interface;
using BankManagementDB.Model;
using BankManagementDB.Properties;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Collections.Generic;
using BankManagementDB.Utility;
using BankManagementDB.Data;

namespace BankManagementDB.View
{ 

    public class CardView
    {
        public CardView()
        {
            GetCardDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IGetCardDataManager>();
            UpdateCardDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IUpdateCardDataManager>();
            InsertCardDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IInsertCardDataManager>();
            InsertCreditCardDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IInsertCreditCardDataManager>();
            GetTransactionDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IGetTransactionDataManager>();
            GetAccountDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IGetAccountDataManager>();
        }

       public IGetCardDataManager GetCardDataManager { get; private set; } 

       public IUpdateCardDataManager UpdateCardDataManager { get; private set; } 

       public IInsertCardDataManager InsertCardDataManager { get; private set; } 

       public IInsertCreditCardDataManager InsertCreditCardDataManager { get; private set; } 

       public IGetAccountDataManager GetAccountDataManager { get; private set; }  

       public IGetTransactionDataManager GetTransactionDataManager { get; private set; }  

        public void ShowCards()
        {
            try
            {
                GetCardDataManager.GetAllCards(CacheData.CurrentUser.ID);
                GetAccountDataManager.GetAllAccounts(CacheData.CurrentUser.ID);

                while (true)
                {
                    for (int i = 0; i < Enum.GetNames(typeof(CardCases)).Length; i++)
                    {
                        CardCases cases = (CardCases)i;
                        Console.WriteLine($"{i + 1}. {cases.ToString().Replace("_", " ")}");
                    }
                    Console.Write(Resources.EnterChoice);
                    string option = Console.ReadLine().Trim();
                    int entryOption;

                    if (!int.TryParse(option, out entryOption))
                        Notification.Error(Resources.InvalidInteger);
                    else
                        if (entryOption != 0 && entryOption <= Enum.GetNames(typeof(CardCases)).Count())
                        {
                            CardCases cases = (CardCases)entryOption - 1;
                            if (CardOperations(cases))
                                break;
                        }
                        else if (entryOption == 0)
                            break;
                        else
                            Notification.Error(Resources.InvalidOption);
                    }
            }
            catch(Exception err)
            {
                Notification.Error(err.ToString());
            }
        }

        public bool CardOperations(CardCases operation)
        {
            switch (operation)
            {
                case CardCases.VIEW_CARDS:
                    ViewCards();
                    return false;

                case CardCases.ADD_CARD:
                    AddCard();
                    return false;

                case CardCases.RESET_PIN:
                    ResetPin();
                    return false;

                case CardCases.VIEW_TRANSACTIONS:
                    ViewAllTransactions();
                    return false;

                case CardCases.CREDIT_CARD_SERVICES:
                    CreditCardView creditCardView = new CreditCardView(GetCardDataManager);
                    creditCardView.CreditCardServices();
                    return false;

                case CardCases.EXIT:
                    return true;

                default:
                    Notification.Error(Resources.InvalidOption);
                    return false;
            }
        }

        public void ViewAllTransactions()
        {
                string cardNumber = GetCardNumber();
                if (cardNumber != null)
                {
                    string pin = GetPin();
                    if (pin != null)
                    {
                        IList<Transaction> transactions = GetTransactionDataManager.GetTransactionsByCardNumber(cardNumber);
                        foreach (Transaction transaction in transactions) 
                             Console.WriteLine(transaction);
                    }
                }
        }

        public void AddCard()
        {
            try
            {
                while (true)
                {
                    Console.WriteLine(Resources.CardTypes);
                    string option = Console.ReadLine().Trim();
                    if (option == Resources.BackButton)
                        break;
                    else if (option == "1")
                    {
                        CreateAndInsertCard(CardType.CREDIT);
                        break;
                    }
                    else if (option == "2")
                    {
                        CreateAndInsertCard(CardType.DEBIT);
                        break;
                    }
                    else
                        Notification.Error(Resources.InvalidInput);
                }
            }catch(Exception ex)
            {
                Notification.Error(ex.ToString());
            }
        }

        public void CreateAndInsertCard(CardType cardType)
        {
            try
            {
                Card card = null;

                if (cardType == CardType.DEBIT)
                {
                    Console.Write(Resources.EnterAccountNumber);
                    string accountNumber = Console.ReadLine().Trim();
                    if (accountNumber != Resources.BackButton)
                    {
                        Account account = GetAccountDataManager.GetAccount(accountNumber);
                        if (account == null)
                            Notification.Error(Resources.InvalidAccountNumber);
                        else
                        {
                            if (GetCardDataManager.IsDebitCardLinked(account.ID))
                                Notification.Error(Resources.DebitCardLinkedError);

                            else
                                card = CreateCard(CardType.DEBIT, account.ID, CacheData.CurrentUser.ID);
                        }
                    }
                }
                else if (cardType == CardType.CREDIT)
                    card = CreateCard(CardType.CREDIT, Guid.Empty, CacheData.CurrentUser.ID);

                InsertCard(card);

            } catch (Exception ex)
            {
                Notification.Error(ex.ToString());
            }
        }

        public void InsertCard(Card card)
        {
            if (card != null)
            {

                if (InsertCardDataManager.InsertCard(card))
                {
                    if (card.Type == CardType.CREDIT)
                    {
                        CreditCard creditCard = new CreditCard();
                        creditCard.ID = card.ID;
                        InsertCreditCardDataManager.InsertCreditCard(creditCard);
                    }
                    Notification.Success(Resources.CardInsertSuccess);
                    Console.WriteLine(card);
                    Console.WriteLine(string.Format(Resources.PinDisplay), card.Pin);
                    Console.WriteLine();
                    GetCardDataManager.GetAllCards(CacheData.CurrentUser.ID);
                }
                else
                    Notification.Error(Resources.CardInsertFailure);
            }
        }

        public Card CreateCard(CardType cardType, Guid accountID, Guid customerID)
        {
            Card card = new Card();
            card.Type = cardType;
            card.AccountID = accountID;
            card.CustomerID = customerID;
            card.CardNumber = RandomGenerator.GenerateCardNumber();
            card.CVV = RandomGenerator.GenerateCVV();
            card.Pin = RandomGenerator.GeneratePin();
            return card;
        }

        private void ResetPin()
        {
            while(true)
            {
              
                string cardNumber = GetCardNumber();
                if (cardNumber != null)
                {
                    string pin = GetPin();
                    if (pin != null)
                    {
                        CardBObj cardBObj = GetCardDataManager.GetCard(cardNumber);
                        if (cardBObj != null)
                        {
                            cardBObj.Pin = pin;
                            Card card = Mapper.Map<CardBObj, Card>(cardBObj);
                            if (UpdateCardDataManager.UpdateCard(card))
                                Notification.Success(Resources.ResetPinSuccess);
                            else
                                Notification.Error(Resources.ResetPinFailure);
                        }
                        
                        break;
                    }
                }
            }
        }

        public string GetCardNumber() {
            while (true)
            {
                Console.Write(Resources.EnterCardNumber);
                string cardNumber = Console.ReadLine().Trim();
                if (cardNumber == Resources.BackButton)
                    return null;
                else
                {
                    if(GetCardDataManager.IsCardNumber(cardNumber)) return cardNumber;
                    else Notification.Error(Resources.CardNumberNotExist);
                }
            }
        }

        private string GetPin()
        {
            Validation validation = new Validation();
            while (true)
            {
                Console.Write(Resources.EnterNewPin);
                string pin = Console.ReadLine().Trim();
                if (pin == Resources.BackButton)
                    return null;
                else if (validation.IsValidPin(pin))
                    return pin;
                else
                    Notification.Error(Resources.InvalidPin);
            }
        }

        private void ViewCards()
        {
            IList<CardBObj> cards = GetCardDataManager.GetCardsList();
            if (cards.Count() == 0) Notification.Info(Resources.NoCardsLinked);
            foreach(CardBObj card in cards) {
                Console.WriteLine(card);
            }
        }

        public bool ValidateModeOfPayment(Guid accountID, ModeOfPayment modeOfPayment)
        {

            if (modeOfPayment == ModeOfPayment.CREDIT_CARD)
                return GetCardDataManager.IsCreditCardEnabled();
            else if (modeOfPayment == ModeOfPayment.DEBIT_CARD)
                return GetCardDataManager.IsDebitCardEnabled(accountID);
            else return true;
        }

        public bool Authenticate(string cardNumber)
        {
            string pin;

            if (GetCardDataManager.IsCardNumber(cardNumber))
            {
                Validation validation= new Validation();    
                Console.Write(Resources.EnterPin);
                pin = Console.ReadLine().Trim();
                if (!validation.IsValidPin(pin))
                    Notification.Error(Resources.InvalidPin);
                else
                    return VerifyPin(cardNumber, pin);
            }
            else
                Notification.Error(Resources.CardNumberNotExist);
            return false;
        }

        private bool VerifyPin(string cardNumber, string pin)
        {
            CardBObj card = GetCardDataManager.GetCard(cardNumber);
            if (card != null)
                return card.Pin == pin;
            return false;
        }
    }
}
