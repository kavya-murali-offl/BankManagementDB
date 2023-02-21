using BankManagement.Enums;
using BankManagement.Model;
using BankManagement.Models;
using BankManagementDB.Enums;
using BankManagementDB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.Controller
{
    public class CardFactory
    {
        public static Card GetCardByType(CardType cardType)
        {
            switch (cardType)
            {
                case CardType.CREDIT:
                    {
                        CreditCard creditCard = new CreditCard();
                        return creditCard;
                    }
                case CardType.DEBIT:
                    {
                        DebitCard debitCard = new DebitCard();
                        return debitCard;
                    }
                default:
                    return null;
            }
        }
    }
}
