using BankManagement.Controller;
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

        public decimal DepositInterest()
        {
            decimal interest = (Balance * CountDays() * InterestRate) / (100 * 12);
            Deposit(interest);
            return interest;
        }

        public int CountDays()
        {
            TransactionController transactionController = new TransactionController();
            DateTime? lastWithdrawnDate = transactionController.GetLastWithdrawnDate();
            if (lastWithdrawnDate.HasValue)
            {
                DateTime TodayDate = DateTime.Now;
                int numberOfDays = (int)(DateTime.Now - lastWithdrawnDate)?.TotalDays;
                return numberOfDays;
            }
            return 0;
        }

        public override string ToString()
        {
            return $"Account ID: {ID} \nAccount Status: {Status} \nBalance: {Balance}";
        }
    }

}
