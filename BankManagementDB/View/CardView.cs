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
using System.ComponentModel.DataAnnotations;

namespace BankManagementDB.View
{ 

    public class CardView
    {
        public CardView()
        {
            CardFactory = DependencyContainer.ServiceProvider.GetRequiredService<ICardFactory>();
            CardController = DependencyContainer.ServiceProvider.GetRequiredService<ICardController>();

        }

        public ICardFactory CardFactory { get; private set; }

       public ICardController CardController { get; private set; }  

        public void ShowCards(Guid customerID)
        {
            CardController.GetAllCards(customerID);
            while (true)
            {
                for (int i = 0; i <Enum.GetNames(typeof(CardCases)).Length; i++)
                {
                    CardCases cases = (CardCases)i;
                    Console.WriteLine($"{i + 1}. {cases.ToString().Replace("_", " ")}");
                }
                Console.Write("\nEnter your choice: ");

                try
                {
                    string option = Console.ReadLine().Trim();
                    int entryOption;

                    if (!int.TryParse(option, out entryOption))
                    {
                    Notification.Error("Invalid input! Please enter a valid number.");
                    continue;
                    }

                    if (entryOption != 0 && entryOption <= Enum.GetNames(typeof(CardCases)).Count())
                    {
                    CardCases cases = (CardCases)entryOption - 1;
                    if (CardOperations(cases, customerID))
                    break;
                    }
                    else if (entryOption == 0)
                    break;
                    else
                    Notification.Error("Invalid input! Please enter a valid number.");
                }
                catch (Exception error)
                {
                    Console.WriteLine(error.Message);
                }
            }


        }

        public bool CardOperations(CardCases operation, Guid customerID)
        {
            switch (operation)
            {
                case CardCases.VIEW_CARDS:
                    ViewCards();
                    return false;

                case CardCases.RESET_PIN:
                    ResetPin();
                    return false;

                case CardCases.EXIT:
                    return true;

                default:
                    Notification.Error("Enter a valid option.\n");
                    return false;
            }
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
                        if (CardController.ResetPin(cardNumber, pin))
                            Console.WriteLine("Pin reset successful");
                        else
                            Console.WriteLine("Reset pin unsuccessful. Please try again.");
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
                    if(CardController.IsCardNumber(cardNumber)) return cardNumber;
                    else Console.WriteLine("Card Number does not exist");
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
                    Console.WriteLine("Please enter a valid 4-digit pin");
            }
        }

        public void ViewCards()
        {
            IList<Card> cards = CardController.GetCardsList();
            foreach(Card card in cards) {
                Console.WriteLine(card);
            }
        }

        public void CreditOperations(Card creditCard, Account account)
        {
            while (true)
            {
                if (creditCard != null)
                    CreditOptions(creditCard);
                else
                {
                    Console.WriteLine("Type yes to add credit card to your account.");
                    string input = Console.ReadLine();
                    if (input.Trim().ToLower() == "yes")
                    {
                    //card = cardController.CreateCard(CardType.CREDIT, account.ID, );

                    //if (cardController.InsertCard(card))
                    //{
                    //    Notification.Success("Credit card created successfully. Save Card Number and Pin for later use.\n");
                    //    Console.WriteLine(card);
                    //    Console.WriteLine($" PIN: {card.Pin}");
                    //    Console.WriteLine();
                    //    break;
                    //}
                    }
                    else
                    break;
                }
            }

        }
        public void DebitOperations(Card debitCard, Account account)
        {

            while (true)
            {
                if (debitCard != null)
                    DebitOptions(debitCard);
                else
                {
                    Console.WriteLine("Type yes to add debit card to your account.");
                    string input = Console.ReadLine();
                    if (input.Trim().ToLower() == "yes")
                    {
                    //card = cardController.CreateCard(card, account);

                    //if (cardController.InsertCard(card))
                    //{
                    //    Notification.Success("Debit card created successfully. Save Card Number and Pin for later use.\n");
                    //    Console.WriteLine(card);
                    //    Console.WriteLine($" PIN: {card.Pin}");
                    //    Console.WriteLine();
                    //    break;
                    //}
                    }
                    else
                    break;
                }
            }
        }
        public void CreditOptions(Card creditCard) { }

        public void DebitOptions(Card debitCard) { }

        public bool ValidateModeOfPayment(Guid id, ModeOfPayment modeOfPayment)
        {

            if (modeOfPayment == ModeOfPayment.CREDIT_CARD)
                return CardController.IsCreditCardEnabled();
            else if (modeOfPayment == ModeOfPayment.DEBIT_CARD)
                return CardController.IsDebitCardEnabled();
            else return true;
        }

        public bool Authenticate()
        {
            CardCredentialsController cardCredentialsController = new CardCredentialsController(CardController);

            string cardNumber, pin;
            string str;
            while (true)
            {
                Console.WriteLine("Enter card number: ");
                cardNumber = Console.ReadLine().Trim();
                if (cardNumber == "0")
                    return false;
                if (CardController.IsCardNumber(cardNumber))
                    break;
                else
                    Notification.Error("Please enter a valid card number"); 
            }

            if (cardCredentialsController.ValidateCardNumber(cardNumber))
            {
                Validation validation= new Validation();    
                Console.WriteLine("Enter pin: ");
                pin = Console.ReadLine().Trim();
                if (!validation.IsValidPin(pin))
                    Notification.Error("Please enter a valid pin");
                if (cardCredentialsController.Authenticate(cardNumber, pin))
                    return true;
            }
            else
                Notification.Error("Card number does not exist");
            return false;
        }

        public void ViewCardDetails(Card card)
        {
            Console.WriteLine(card);
        }
    }
}
