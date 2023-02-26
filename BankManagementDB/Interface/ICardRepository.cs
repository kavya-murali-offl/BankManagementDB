using BankManagementCipher.Model;
using BankManagementDB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.Interface
{
    public interface ICardRepository
    {

        Task<bool> InsertOrReplace(CardDTO accountDTO);

        Task<IList<CardDTO>> Get(Guid id);
    }
}
