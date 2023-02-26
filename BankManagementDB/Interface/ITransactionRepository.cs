using BankManagementCipher.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.Interface
{
    public interface ITransactionRepository
    {
        Task<bool> InsertOrReplace(TransactionDTO transactionDTO);

        Task<IList<TransactionDTO>> Get(Guid id);
    }
}
