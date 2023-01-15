using System;
using System.Collections.Generic;
using BankManagement.Models;
using BankManagement.Utility;

namespace BankManagement.Controller
{
    public class ProfileController
    {
        public Customer Customer { get; set; }

        public string UserName
        {
            get { return Customer.UserName; }
        }

        public string Name => Customer.Name;

        public Int64 ID => Customer.ID;


        public IList<Account> Accounts
        {
            get; set;
        }

        public Account GetAccountByID(string id)
        {
            return null;
        }

    }
}
