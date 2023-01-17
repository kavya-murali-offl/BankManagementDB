using BankManagement.Enums;
using BankManagement.Model;
using System;
using System.Collections.Generic;

namespace BankManagement.Models
{
    public abstract class Account
    {
        public Account() { 
            IList<Transaction> transactions = new List<Transaction>();
            Transactions = transactions;
            Balance = 0;
            Status = AccountStatus.ACTIVE;
        }
        public long ID { get; set; }

        public decimal Balance { get; set; }

        public decimal InterestRate { get; set; }

        public AccountTypes Type { get; set; }

        public long UserID { get; set; }

        public AccountStatus Status { get; set; } 
        
        public IList<Transaction> Transactions { get; set; }

        public bool Deposit(decimal amount)
        {
            Balance += amount;
            return true;
        }

        public bool Withdraw(decimal amount)
        {
            if (Balance > amount)
            {
                Balance -= amount;
                return true;
            }
            else
            {
                Console.WriteLine("Insufficient Balance...");
                return false;
            }
                 
        }

        public override string ToString()
        {
            return "\nAccount ID: " + ID+
                "\nAccount Status: " + Status +
                "\nBalance: " + Balance;
        }
    }

}
