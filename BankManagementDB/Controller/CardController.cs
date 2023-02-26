using System;
using System.Collections.Generic;
using System.Linq;
using BankManagementDB.Models;
using BankManagementCipher.Utility;
using BankManagementDB.Config;
using BankManagementDB.EnumerationType;
using BankManagementDB.Interface;
using BankManagementDB.Model;
using BankManagementDB.Repository;
using BankManagementDB.Utility;
using Microsoft.Extensions.DependencyInjection;

namespace BankManagementDB.Controller
{
    public class CardController : ICardController
    {
        public CardController(ICardFactory cardFactory, ICardRepository cardRepository) {
            CardFactory = cardFactory;
            CardRepository = cardRepository;
        }

        public ICardFactory CardFactory { get; private set; }
        public ICardRepository CardRepository { get; private set; }

        public static IList<Card> CardsList { get; set; }

        public Card CreateCard(CardType cardType, Guid accountID, Guid customerID)
        {
            try
            {
                Card card = CardFactory.GetCardByType(cardType);
                card.AccountID = accountID;
                card.CustomerID = customerID;
                card.CardNumber = RandomGenerator.GenerateCardNumber();  
                card.CVV = RandomGenerator.GenerateCVV();
                card.Pin = RandomGenerator.GeneratePin();
                return card;
            }
            catch (Exception ex) {
                Console.WriteLine(ex);
            }
            return null;
        }

        public bool IsDebitCardEnabled()
        {
           return CardsList.Where(c => c is DebitCard).Count() > 0 ? true : false;
        }

        public bool IsCreditCardEnabled()
        {
            return CardsList.Where(c => c is CreditCard).Count() > 0 ? true : false;
        }

        public Card GetCardByType(CardType cardType)
        {
            return CardsList.Where<Card>(card => card.Type == cardType).FirstOrDefault();
        }

        public bool IsCardNumber(string cardNumber)
        {
           return CardsList.Where<Card>(card => card.CardNumber == cardNumber).Any();
        }

        public bool ResetPin(string cardNumber, string pin)
        {
            Card card = GetCard(cardNumber);
            if(card != null)
                card.Pin = pin;
            return UpdateCard(card);
        }

        public bool InsertCard(Card card)
        {
            
            try
            {
                bool success = CardRepository.InsertOrReplace(Mapping.CardToDto(card)).Result;
                if (success) CardsList.Add(card);
                return success;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return false;
        }

        public bool UpdateCard(Card updatedCard)
        {
            try
            {
                bool success = CardRepository.InsertOrReplace(Mapping.CardToDto(updatedCard)).Result;
                if (success)
                {
                    Card card = CardsList.FirstOrDefault(acc => acc.ID.Equals(updatedCard.ID));
                    CardsList.Remove(card);
                    CardsList.Insert(0, updatedCard);
                }
                return success;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
        }

        public void GetAllCards(Guid customerID)
        {
            try
            {
                var cardsList = CardRepository.Get(customerID).Result;
                IList<Card> cards = new List<Card>();
                if (cardsList.Count() > 0)
                    foreach (var card in cardsList)
                        cards.Add(Mapping.DtoToCard(card));

                CardsList = cards;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }

        public Card GetCard(string cardNumber)
        {
            try
            {
                return CardsList.Where<Card>(card => card.CardNumber == cardNumber).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return null;
        }
        public IList<Card> GetCardsList() => CardsList ??= new List<Card>();
       
    }
}
