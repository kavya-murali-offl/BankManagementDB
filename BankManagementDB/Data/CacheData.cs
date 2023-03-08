using BankManagementDB.Model;
using BankManagementDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.Data
{
    public class CacheData
    {
        public static Customer CurrentUser { get; set; }

        public static IList<CardBObj> CardsList { get; set; }

        public static IList<Account> AccountsList { get; set; }

        public static IList<Transaction> TransactionList { get; set; }
    }
}
