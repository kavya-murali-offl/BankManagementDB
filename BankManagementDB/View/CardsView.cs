using BankManagement.Models;
using BankManagement.Utility;
using BankManagementDB.Controller;
using BankManagementDB.Enums;
using BankManagementDB.Interface;
using BankManagementDB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace BankManagementDB.View
{
    public enum CardCases
    {
        DEBIT_CARD, CREDIT_CARD, EXIT
    }


    public class CardsView
    {


        public void ShowCards(Account account)
        {
            ICardServices cardController = new CardController();
            IList<Card> cardsList = cardController.GetAllCards(account.ID);
            while (true)
            {
                for (int i = 0; i < Enum.GetNames(typeof(CardCases)).Length; i++)
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
                        if (CardOperations(cases, account, cardsList))
                            break;
                    }
                    else
                        Notification.Error("Invalid input! Please enter a valid number.");
                }
                catch (Exception error)
                {
                    Console.WriteLine(error.Message);
                }
            }


        }

        public bool CardOperations(CardCases operation, Account account, IList<Card> cards)
        {
            Card card;
            Card creditCard = cards.Where<Card>(crd => crd.Type == CardType.CREDIT).FirstOrDefault();
            Card debitCard = cards.Where<Card>(crd => crd.Type == CardType.DEBIT).FirstOrDefault();

            switch (operation)
            {
                case CardCases.CREDIT_CARD:
                    Console.WriteLine(creditCard);
                    CreditOperations(creditCard, account);
                    return false;

                case CardCases.DEBIT_CARD:
                    var debitCards = from c in cards where c.Type == CardType.DEBIT select c;
                    Console.WriteLine(debitCard);
                    debitCard.Balance = account.Balance;
                    DebitOperations(debitCard, account);
                    return false;

                case CardCases.EXIT:
                    return true;

                default:
                    Notification.Error("Enter a valid option.\n");
                    return false;
            }
        }

        public void CreditOperations(Card creditCard, Account account)
        {
            CardController cardController = new CardController();
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
                        Card card = CardFactory.GetCardByType(CardType.CREDIT);
                        card = cardController.CreateCard(card, account);

                        if (cardController.InsertCard(card))
                        {
                            Notification.Success("Credit card created successfully. Save Card Number and Pin for later use.\n");
                            Console.WriteLine(card);
                            Console.WriteLine($" PIN: {card.Pin}");
                            Console.WriteLine();
                            break;
                        }
                    }
                    else
                        break;
                }
            }

        }
        public void DebitOperations(Card debitCard, Account account)
        {

            CardController cardController = new CardController();
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
                        Card card = CardFactory.GetCardByType(CardType.DEBIT);
                        card = cardController.CreateCard(card, account);

                        if (cardController.InsertCard(card))
                        {
                            Notification.Success("Debit card created successfully. Save Card Number and Pin for later use.\n");
                            Console.WriteLine(card);
                            Console.WriteLine($" PIN: {card.Pin}");
                            Console.WriteLine();
                            break;
                        }
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
            CardController cardController = new();
            cardController.GetAllCards(id);

            if (modeOfPayment == ModeOfPayment.CREDIT_CARD)
                return cardController.IsCreditCardEnabled();
            else if (modeOfPayment == ModeOfPayment.DEBIT_CARD)
                return cardController.IsDebitCardEnabled();
            else return true;
        }

        public bool Authenticate()
        {
            CardCredentialsController cardCredentialsController = new CardCredentialsController();

            long cardNumber, pin;
            string str;
            while (true)
            {
                Console.WriteLine("Enter card number: ");
                str = Console.ReadLine().Trim();
                if (long.TryParse(str, out cardNumber))
                    break;
                else
                    Notification.Error("Please enter a valid card number");
            }

            if (cardCredentialsController.ValidateCardNumber(cardNumber))
            {
                Console.WriteLine("Enter pin: ");
                str = Console.ReadLine().Trim();
                if (!long.TryParse(str, out pin))
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
