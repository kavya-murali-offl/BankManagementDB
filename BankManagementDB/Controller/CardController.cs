using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BankManagement.Models;
using BankManagementCipher.Utility;
using BankManagementDB.db;
using BankManagementDB.Enums;
using BankManagementDB.Interface;
using BankManagementDB.Model;
using BankManagementDB.Utility;


namespace BankManagementDB.Controller
{
    public class CardController : ICardServices
    {
        public static IList<Card> CardsList { get; set; }

        public Card CreateCard(Card card, Account account)
        {
            RandomGenerator cardHelper= new RandomGenerator();    
            try
            {
                card.AccountID = account.ID;
                card.CardHolder = "Test";
                card.CardNumber = cardHelper.GenerateCardNumber();  
                card.CVV = cardHelper.GenerateCVV();
                card.Pin = cardHelper.GeneratePin();
            }
            catch(Exception ex) {
                Console.WriteLine(ex);
            }

            return card;
        }

        public bool IsDebitCardEnabled()
        {
           return CardsList.Where(c => c is DebitCard).Count() > 0 ? true : false;
        }

        public bool IsCreditCardEnabled()
        {
            return CardsList.Where(c => c is CreditCard).Count() > 0 ? true : false;
        }

        public bool InsertCard(Card card)
        {
            
            try
            {
                
                CardOperations cardOperations  = new CardOperations();
                bool success = cardOperations.InsertOrReplace(Mapping.CardToDto(card)).Result;
                if (success) CardsList.Add(card);
                return success;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return false;
        }

        public bool UpdateCard(Card card)
        {

            try
            {
                CardOperations cardOperations = new CardOperations();
                bool success = cardOperations.InsertOrReplace(Mapping.CardToDto(card)).Result;
                if(success) CardsList.Add(card);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
        }

        public bool MakePurchase(Card card, decimal amount)
        {

            try
            {
                CardOperations cardOperations = new CardOperations();
                card.Purchase(amount);
                bool success = cardOperations.InsertOrReplace(Mapping.CardToDto(card)).Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return false;
        }

        public bool MakePayment(Card card, decimal amount)
        {
            bool success = false;
            try
            {
                CardOperations cardOperations = new CardOperations();
                card.Payment(amount);
                success = cardOperations.InsertOrReplace(Mapping.CardToDto(card)).Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return success;
        }

        public IList<Card> GetAllCards(Guid accountID)
        {
            IList<Card> cards = new List<Card>();
            try
            {
                CardOperations cardOperations = new CardOperations();
                var cardsList = cardOperations.Get(accountID).Result;

                if (cardsList.Count() > 0)
                    foreach (var card in cardsList)
                        cards.Add(Mapping.DtoToCard(card));

                CardsList = cards;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return cards;
        }

        public Card GetCard(long cardNumber)
        {
            try
            {
                CardOperations cardOperations = new CardOperations();
                var cardsList = cardOperations.Get(cardNumber).Result;

                if (cardsList.Count() > 0)
                {
                    CardDTO cardDTO = cardsList.FirstOrDefault();
                    return Mapping.DtoToCard(cardDTO);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return null;
        }

    }
}
