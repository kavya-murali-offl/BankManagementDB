using System;
using System.Collections.Generic;
using BankManagement.Enums;
using BankManagement.Model;
using BankManagement.Models;

namespace BankManagement.Utility
{
    public class AccountData
    {
      

        public IList<Account> GetAccountsByUsername(string username)
        {
            IList<Account> AllUserAccounts = GetAllAccounts();
            if (AllUsersAccounts.ContainsKey(username)) return AllUsersAccounts[username];
            else return null;
        }

        public void AddAccount(string username, Account account)
        {
            //AccountDB.AddAccountToDB();
            if (AllUsersAccounts.ContainsKey(username)) {
                AllUsersAccounts[username].Add(account);
            }
            else
            {
                IList<Account> userAccounts = new List<Account>();
                userAccounts.Add(account);
                AllUsersAccounts.Add(username, userAccounts);
            }
        }

        public IList<Account> GetAllAccounts()
        {
            return new List<Account>();
        }

        public IDictionary<string, IList<Account>> AllUsersAccounts { get; set; }


        //public AccountDB AccountDB { get; set; }
    }
}
