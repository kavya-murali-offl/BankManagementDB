using BankManagementDB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.Interface
{
    public interface IInsertDebitCardDataManager
    {
        bool InsertDebitCard(DebitCardDTO card);
    }
}
