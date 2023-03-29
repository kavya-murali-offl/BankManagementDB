using BankManagementDB.Models;
using BankManagementDB.Config;
using BankManagementDB.EnumerationType;
using BankManagementDB.Interface;
using BankManagementDB.Model;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Collections.Generic;
using BankManagementDB.Utility;
using BankManagementDB.Data;
using BankManagementDB.DataManager;
using BankManagementDB.Properties;

namespace BankManagementDB.View
{

    public class CardView
    {
        public void ShowCards()
        {
            try
            {
                IGetAccountDataManager GetAccountDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IGetAccountDataManager>();

                IGetCardDataManager GetCardDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IGetCardDataManager>();
                GetCardDataManager.GetAllCards(Store.CurrentUser.ID);
                GetAccountDataManager.GetAllAccounts(Store.CurrentUser.ID);

                OptionsDelegate<CardCases> options = CardOperations;

                HelperView helperView = new HelperView();
                helperView.PerformOperation(options);

            }
            catch (Exception err)
            {
                Notification.Error(err.ToString());
            }
        }

        public bool CardOperations(CardCases command) =>
            command switch
            {
                CardCases.VIEW_CARDS => ViewCards(),
                CardCases.ADD_CARD => AddCard(),
                CardCases.RESET_PIN => ResetPin(),
                CardCases.VIEW_TRANSACTIONS => ViewAllTransactions(),
                CardCases.CREDIT_CARD_SERVICES => GoToCreditCardServices(),
                CardCases.GO_BACK => true,
                CardCases.EXIT => Exit(),
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

        private bool GoToCreditCardServices()
        {
            CreditCardView creditCardView = new CreditCardView();
            creditCardView.CreditCardServices();
            return false;
        }

        private bool ViewAllTransactions()
        {
            string cardNumber = GetCardNumber();
            if (cardNumber != null)
            {
                string pin = GetPin();
                if (pin != null)
                {
                    IGetTransactionDataManager GetTransactionDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IGetTransactionDataManager>();

                    IList<Transaction> transactions = GetTransactionDataManager.GetTransactionsByCardNumber(cardNumber);
                    foreach (Transaction transaction in transactions)
                    { 
                        Console.WriteLine(transaction);
                    }
                }
            }
            return false;
        }

        private bool AddCard()
        {
            Notification.Info(Resources.PressBackButtonInfo);

            OptionsDelegate <CardType> optionsDelegate = CreateCardBasedOnType;

            HelperView helperView = new HelperView();
            helperView.PerformOperation(optionsDelegate);

            return false;
        }

        private bool CreateCardBasedOnType(CardType cardType)
        {
            Card card = null;

            if (cardType == CardType.DEBIT)
            {
                Console.Write(Resources.AccountNumber + ": ");
                string accountNumber = Console.ReadLine()?.Trim();
                if (accountNumber != Resources.BackButton)
                {
                    Account account = Store.GetAccountByAccountNumber(accountNumber);
                    if (account == null)
                    {
                        Notification.Error(Resources.InvalidAccountNumber);
                    }
                    else
                    {
                        if (Store.IsDebitCardLinked(account.ID))
                        {
                            Notification.Error(Resources.DebitCardLinkedError);
                        }
                        else
                        {
                            card = CreateCard(CardType.DEBIT, account.ID, Store.CurrentUser.ID);
                        }
                    }
                }
            }
            else if (cardType == CardType.CREDIT)
            {
                card = CreateCard(CardType.CREDIT, null, Store.CurrentUser.ID);
            }
            if(card != null)
            {
                InsertCard(card);
            }

            return true;
        }

        public void InsertCard(Card card)
        {
            IInsertCardDataManager InsertCardDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IInsertCardDataManager>();
            if (InsertCardDataManager.InsertCard(card))
            {
                if (card.Type == CardType.CREDIT)
                {
                    CreditCardView creditCardView = new CreditCardView();
                    creditCardView.CreateCreditCard(card);
                }
                else if(card.Type == CardType.DEBIT)
                {
                    IInsertDebitCardDataManager insertDebitCardDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IInsertDebitCardDataManager>();
                    insertDebitCardDataManager.InsertDebitCard(
                        new DebitCardDTO()
                        {
                            ID = card.ID,
                        });
                }

                Notification.Success(Resources.CardInsertSuccess);
                Console.WriteLine(card);
                Console.WriteLine(Formatter.FormatString(Resources.PinDisplay, card.Pin));
                Console.WriteLine();
                IGetCardDataManager GetCardDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IGetCardDataManager>();

                GetCardDataManager.GetAllCards(Store.CurrentUser.ID);
            }
            else
            {
                Notification.Error(Resources.CardInsertFailure);
            }
        }

        public Card CreateCard(CardType cardType, string accountID, string customerID) =>
            new Card
            {
                ID = Guid.NewGuid().ToString(),
                ExpiryMonth = DateTime.Now.Month.ToString(),
                ExpiryYear = (DateTime.Now.Year + 7).ToString(),
                CreatedOn = DateTime.Now,
                Type = cardType,
                AccountID = accountID,
                CustomerID = customerID,
                CardNumber = RandomGenerator.GenerateCardNumber(),
                CVV = RandomGenerator.GenerateCVV(),
                Pin = RandomGenerator.GeneratePin()
            };


        private bool ResetPin()
        {
            while (true)
            {
                string cardNumber = GetCardNumber();
                if (cardNumber != null)
                {
                    string pin = GetPin();
                    if (pin != null)
                    {
                        Card card = Store.GetCard(cardNumber);
                        if (card != null)
                        {
                            card.Pin = pin;
                            IUpdateCardDataManager UpdateCardDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IUpdateCardDataManager>();

                            if (UpdateCardDataManager.UpdateCard(card))
                            {
                                Notification.Success(Resources.ResetPinSuccess);
                            }
                            else
                            {
                                Notification.Error(Resources.ResetPinFailure);
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
            return false;
        }

        public string GetCardNumber()
        {
            while (true)
            {
                Console.Write(Resources.CardNumber + ": ");
                string cardNumber = Console.ReadLine()?.Trim();
                if (cardNumber == Resources.BackButton)
                { return null; }
                else
                {
                    if (Store.IsCardNumber(cardNumber)) { 
                        return cardNumber;
                    }
                    else {
                        Notification.Error(Resources.CardNumberNotExist); 
                    }
                }
            }
        }

        private string GetPin()
        {
            Validator validation = new Validator();
            while (true)
            {
                Console.Write(Resources.NewPin + ": ");
                string pin = Console.ReadLine()?.Trim();
                if (pin == Resources.BackButton)
                {
                    return null;
                }
                else if (validation.IsValidPin(pin))
                {
                    return pin;
                }
                else
                {
                    Notification.Error(Resources.InvalidPin);
                }
            }
        }

        private bool ViewCards()
        {
            IEnumerable<Card> cards = Store.GetCardsList();
            if (cards.Count() == 0)
            {
                Notification.Info(Resources.NoCardsLinked);
            }
            foreach (Card card in cards)
            {
                Console.WriteLine(card);
            }
            return false;
        }

        public bool ValidateModeOfPayment(string accountID, ModeOfPayment modeOfPayment)
        {
            if (modeOfPayment == ModeOfPayment.CREDIT_CARD)
            { 
                return Store.IsCreditCardEnabled();
            }
            else if (modeOfPayment == ModeOfPayment.DEBIT_CARD)
            { 
                return Store.IsDebitCardEnabled(accountID);
            }
            else { 
                return true;
            }
        }

        public bool Authenticate(string cardNumber)
        {
            if (Store.IsCardNumber(cardNumber))
            {
                Validator validation = new Validator();
                Console.Write(Resources.Pin + ": ");
                string pin = Console.ReadLine()?.Trim();
                if (validation.IsValidPin(pin))
                {
                    return VerifyPin(cardNumber, pin);
                }
            }
            else
            {
                Notification.Error(Resources.CardNumberNotExist);
            }

            return false;
        }

        private bool VerifyPin(string cardNumber, string pin)
        {
            Card card = Store.GetCard(cardNumber);
            if (card != null)
            {
                return card.Pin == pin;
            }
            return false;
        }
    }
}
