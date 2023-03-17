using BankManagementDB.DatabaseHandler;
using BankManagementDB.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.DataManager
{
    public class TranferAmountDataManager : ITransferAmountDataManager
    {
        public TranferAmountDataManager(IDBHandler dBHandler)
        {
            DBHandler = dBHandler;
        }

        private IDBHandler DBHandler { get; set; }

        public bool PerformTransaction(IList<Action> actions) =>
             DBHandler.RunInTransaction(actions).Result;
    }
}
