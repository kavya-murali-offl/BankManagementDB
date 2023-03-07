using BankManagementDB.EnumerationType;
using BankManagementDB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.Interface
{
    public interface IUpdateCardDataManager
    {
        bool UpdateCard(Card card);

        bool UpdateCreditCard(CreditCard updatedCard);

        bool UpdateDueAmount(CreditCardCases cases, CardBObj card, decimal amount);


    }
}
