using BankManagement.Enums;
using BankManagementDB.View;
using System;

namespace BankManagement.Models
{
    public abstract class Account
    {
        public event Action<string> BalanceChanged;

        public Account() { 
            Balance = 0;
            Status = AccountStatus.ACTIVE;
        }

        public long ID { get; set; }

        public decimal Balance { get; set; }

        public decimal MinimumBalance { get; set; }

        public decimal InterestRate { get; set; }

        public AccountTypes Type { get; set; }

        public long UserID { get; set; }

        public DateTime CreatedOn { get; set; }

        public AccountStatus Status { get; set; }

        public bool Deposit(decimal amount)
        {
            if(amount > 0)
            {
                Balance += amount;
                OnBalanceChanged($"Deposit of Rs. {amount} is successful");
                return true;
            }
            return false;
        }


        public bool Withdraw(decimal amount)
        {
            if (Balance > amount)
            {
                Balance -= amount;
                OnBalanceChanged($"Withdrawal of Rs. {amount} is successful");
                return true;
            }
            else
            {
                Notification.Error($"Insufficient Balance");
                return false;
            }
        }

        public void OnBalanceChanged(string message)
        {
            BalanceChanged?.Invoke(message);
        }

        public override string ToString() => $"Account ID: {ID} \nAccount Status: {Status} \nBalance: Rs. {Balance}"; 
    }
}
