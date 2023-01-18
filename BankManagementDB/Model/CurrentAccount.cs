﻿using BankManagement.Enums;
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

        public bool Withdraw(decimal amount)
        {
            bool validTransaction = CheckMinimumBalance(amount);
            if (!validTransaction)
                ChargeForMinBalance();
            base.Withdraw(amount);
            return validTransaction;
        }

        public bool CheckMinimumBalance(decimal amount)
        {
            if (Balance - amount < MIN_BALANCE)
                return false;
            return true;
        }

        public void ChargeForMinBalance()
        {
            if (Balance < CHARGES)
                Balance = 0;
            else
            {
                Balance = Balance - CHARGES;
            }
        }

        public decimal MinimumBalance { get { return MIN_BALANCE; } }

        public override string ToString()
        {
            return $"Account Type: Current\n {base.ToString()}\n" +
                $"Minimum Balance: {MinimumBalance}\n" +
                "========================================\n";
        }

    }
}
