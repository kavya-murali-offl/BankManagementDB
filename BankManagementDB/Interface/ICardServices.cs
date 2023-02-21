using BankManagement.Models;
using BankManagementDB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.Interface
{
    public interface ICardServices
    {
        bool InsertCard(Card card);

        bool UpdateCard(Card card);

        IList<Card> GetAllCards(Guid accountID);
    }
}
