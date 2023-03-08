using BankManagementDB.Models;
using BankManagementDB.Config;
using BankManagementDB.EnumerationType;
using BankManagementDB.Controller;
using BankManagementDB.Interface;
using BankManagementDB.Model;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Collections.Generic;
using BankManagementDB.Utility;
using BankManagementDB.Data;
using BankManagementDB.DataManager;

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
                    Console.Write("\nEnter your choice: ");
                    string option = Console.ReadLine().Trim();
                    int entryOption;

                    if (!int.TryParse(option, out entryOption))
                        Notification.Error("Invalid input! Please enter a valid number.");
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
                            Notification.Error("Invalid input! Please enter a valid number.");
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
                    Notification.Error("Enter a valid option.\n");
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
                    Console.WriteLine("1.CREDIT CARD\n2.DEBIT CARD");
                    string option = Console.ReadLine().Trim();
                    if (option == "0")
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
                        Notification.Error("Please enter a valid input");
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
                    Console.Write("Enter Account Number: ");
                    string accountNumber = Console.ReadLine().Trim();
                    if (accountNumber != "0")
                    {
                        Account account = GetAccountDataManager.GetAccount(accountNumber);
                        if (account == null)
                            Notification.Error("Invalid Account Number");
                        else
                        {
                            if (GetCardDataManager.IsDebitCardLinked(account.ID))
                                Notification.Error("This account already has a debit card linked to it");

                            else
                                card = CreateCard(CardType.DEBIT, account.ID, CacheData.CurrentUser.ID);
                        }
                    }
                }
                else if (cardType == CardType.CREDIT)
                    card = CreateCard(CardType.CREDIT, Guid.Empty, CacheData.CurrentUser.ID);

                if (card != null)
                {
                    
                    if (InsertCardDataManager.InsertCard(card))
                    {
                        if(card.Type == CardType.CREDIT)
                        {
                            CreditCard creditCard = new CreditCard();
                            creditCard.ID = card.ID;
                            InsertCreditCardDataManager.InsertCreditCard(creditCard);
                        }
                        Notification.Success("Card created successfully. Save Card Number and Pin for later use.\n");
                        Console.WriteLine(card);
                        Console.WriteLine($" PIN: {card.Pin}");
                        Console.WriteLine();
                        GetCardDataManager.GetAllCards(CacheData.CurrentUser.ID);
                    }
                    else
                        Notification.Error("Card not created");
                }
            } catch (Exception ex)
            {
                Notification.Error(ex.ToString());
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

        public void ResetPin()
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
                                Notification.Success("Pin reset successful");
                            else
                                Notification.Error("Reset pin unsuccessful. Please try again.");
                        }
                        
                        break;
                    }
                }
            }
        }

        public string GetCardNumber() {
            while (true)
            {
                Console.Write("Enter Card Number: ");
                string cardNumber = Console.ReadLine().Trim();
                if (cardNumber == "0")
                    return null;
                else
                {
                    if(GetCardDataManager.IsCardNumber(cardNumber)) return cardNumber;
                    else Notification.Error("Card Number does not exist");
                }
            }
        }

        public string GetPin()
        {
            Validation validation = new Validation();
            while (true)
            {
                Console.Write("Enter new pin: ");
                string pin = Console.ReadLine().Trim();
                if (pin == "0")
                    return null;
                else if (validation.IsValidPin(pin))
                    return pin;
                else
                    Notification.Error("Please enter a valid 4-digit pin");
            }
        }

        public void ViewCards()
        {
            IList<CardBObj> cards = GetCardDataManager.GetCardsList();
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
                Console.Write("Enter pin: ");
                pin = Console.ReadLine().Trim();
                if (!validation.IsValidPin(pin))
                    Notification.Error("Please enter a valid pin");
                else
                    return VerifyPin(cardNumber, pin);
            }
            else
                Notification.Error("Card number does not exist");
            return false;
        }

        public bool VerifyPin(string cardNumber, string pin)
        {
            CardBObj card = GetCardDataManager.GetCard(cardNumber);
            if (card != null)
                return card.Pin == pin;
            return false;
        }
    }
}
