using BankManagementDB.Config;
using BankManagementDB.EnumerationType;
using BankManagementDB.Utility;
using BankManagementDB.View;
using SQLite;
using System;
using BankManagementDB.Properties;

namespace BankManagementDB.Models
{
    public class Account
    {
        [PrimaryKey]
        public string ID { get; set; }

        public string AccountNumber { get; set; }

        public decimal Balance { get; set; }

        public decimal MinimumBalance { get; set; }

        public decimal InterestRate { get; set; }

        public AccountType Type { get; set; }

        public string UserID { get; set; }

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

        public override string ToString() => Formatter.FormatString(Resources.DisplayAccount, AccountNumber, Type, Status);
    }
}
