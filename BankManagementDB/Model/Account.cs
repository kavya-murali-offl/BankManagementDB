using BankManagement.Enums;
using BankManagementDB.View;
using System;

namespace BankManagement.Models
{
    public abstract class Account
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

        public void Deposit(decimal amount)
        {
                Balance += amount;
        }


        public void Withdraw(decimal amount)
        {
             Balance -= amount;
        }

        public abstract override string ToString(); 
    }
}
