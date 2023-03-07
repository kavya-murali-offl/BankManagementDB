using BankManagementDB.EnumerationType;
using BankManagementDB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.Interface
{
    public interface IGetCardDataManager
    {
        void GetAllCards(Guid customerID);

        bool IsDebitCardEnabled(Guid accountID);

        bool IsCreditCardEnabled();

        CardBObj GetCardByType(CardType cardType);
        bool IsCardNumber(string cardNumber);
        CardBObj GetCard(string cardNumber);
        
        IList<CardBObj> GetCardsList();

        public bool IsDebitCardLinked(Guid accountID);
        bool Authenticate(string cardNumber, string pin);

        bool IsCreditCard(string cardNumber);

    }
}
