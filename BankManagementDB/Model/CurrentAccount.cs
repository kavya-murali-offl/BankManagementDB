using BankManagement.Controller;
using BankManagement.Enums;
using BankManagement.Models;
using BankManagement.Utility;
using System;

namespace BankManagement.Model
{
    public class CurrentAccount : Account
    {
        public readonly decimal MIN_BALANCE = 500;
        public readonly decimal CHARGES = 50;
        
        public CurrentAccount(): base() {
            InterestRate = AccountInterestRate.CURRENT_INTEREST_RATE;
            Type = AccountTypes.CURRENT;
            CreatedOn = DateTime.Now;
        }

        public decimal MinimumBalance { get { return MIN_BALANCE; } }

        public void Charge()
        {
            Withdraw(CHARGES);
            OnBalanceChanged($"Minumum balance must be maintained. You have been charged Rs. {CHARGES}.");
        }

        public override string ToString()
        {
            return $"Account Type: Current\n {base.ToString()}\n" +
                $"Minimum Balance: {MinimumBalance}\n" +
                "========================================\n";
        }
    }
}
