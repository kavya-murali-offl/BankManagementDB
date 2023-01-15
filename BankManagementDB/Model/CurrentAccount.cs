using BankManagement.Enums;
using BankManagement.Models;
using System;

namespace BankManagement.Model
{
    public class CurrentAccount : Account
    {
        private readonly decimal MIN_BALANCE = 5000;
        private readonly decimal CHARGES = 200;

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

        public void Transfer(decimal amount, Account toAccount)
        {
            bool validTransaction = CheckMinimumBalance(amount);
            if (!validTransaction)
                ChargeForMinBalance();
            base.Transfer(amount, toAccount);
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
            return "Account Type: Current" +
                base.ToString() +
                "\nMinimum Balance: " + MinimumBalance +
                "\n========================================\n";
        }

    }
}
