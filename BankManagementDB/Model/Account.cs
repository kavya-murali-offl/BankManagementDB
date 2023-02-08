using BankManagement.Enums;
using BankManagementDB.View;
using System;

namespace BankManagement.Models
{
    public class Account
    {
        public Account() {
            ID = Guid.NewGuid();
            Balance = 0;
            Status = AccountStatus.ACTIVE;
        }
        
        public Guid ID { get; set; }
        
        public decimal Balance { get; set; }
        
        public decimal MinimumBalance { get; set; }
        
        public decimal InterestRate { get; set; }
        
        public AccountTypes Type { get; set; }
        
        public Guid UserID { get; set; }
        
        public DateTime CreatedOn { get; set; }
        
        public AccountStatus Status { get; set; }

        public bool Deposit(decimal amount)
        {
            if(amount > 0)
            {
                Balance += amount;
                return true;
            }
            return false;
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
                Notification.Error($"Insufficient Balance");
                return false;
            }
        }

        public override string ToString() => $"Account ID: {ID} \nAccount Status: {Status} \nBalance: Rs. {Balance}"; 
    }
}
