using BankManagement.Enums;
using BankManagement.Models;
using System;

namespace BankManagement.Model
{
    public class CurrentAccount : Account
    {
        private readonly decimal MIN_BALANCE = 500;
        private readonly decimal CHARGES = 100;

        public CurrentAccount(): base() {
            InterestRate = 5.6m;
        }
        public void Withdraw(decimal amount)
        {
            bool validTransaction = CheckMinimumBalance(amount);
            if (!validTransaction)
                ChargeForMinBalance();
            base.Withdraw(amount);
        }


        public bool CheckMinimumBalance(decimal amount)
        {
            if (Balance - amount < MIN_BALANCE)
                return false;
            return true;

        }

        public void ChargeForMinBalance()
        {
            Balance = Balance - CHARGES;
        }
        public decimal MinimumBalance { get { return MIN_BALANCE; } }

        public override string ToString()
        {
            return "\nAccount Type: Current" +
                base.ToString() +
                "\nMinimum Balance: " + MinimumBalance +
                "\n========================================\n";
        }

    }
}
