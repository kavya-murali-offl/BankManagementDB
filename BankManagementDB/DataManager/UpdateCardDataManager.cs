using BankManagementDB.EnumerationType;
using BankManagementDB.Interface;
using BankManagementDB.Model;
using BankManagementDB.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.DataManager
{
    public class UpdateCardDataManager : IUpdateCardDataManager
    {
        public event Action<string> CardDueAmountChanged;

        public UpdateCardDataManager(IDBHandler dBHandler)
        {
            DBHandler = dBHandler;
        }

        public IDBHandler DBHandler { get; private set; }

        public bool UpdateCard(Card updatedCard)
        {
            bool success = DBHandler.UpdateCard(updatedCard).Result;
            return success;
        }

        public bool UpdateCreditCard(CreditCard updatedCard)
        {
            return DBHandler.UpdateCreditCard(updatedCard).Result;
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
                    UpdateCreditCard(creditCard);
                    return true;
                case CreditCardCases.PAYMENT:
                    card.TotalDueAmount -= amount;
                    CardDueAmountChanged?.Invoke($"Payment of Rs.{amount} is sucessful");
                    creditCard = Mapper.Map<CardBObj, CreditCard>(card);
                    UpdateCreditCard(creditCard);
                    return true;
                default:
                    return false;
            }
        }
    }
}
