using BankManagement.Controller;
using BankManagement.Enums;
using BankManagement.Models;
using BankManagement.Utility;
using System;

namespace BankManagement.Model
{
    public class CurrentAccount : Account
    {
        public readonly decimal CHARGES = 50;
        
        public CurrentAccount(): base() {
            InterestRate = AccountInterestRate.CURRENT_INTEREST_RATE;
            Type = AccountTypes.CURRENT;
            CreatedOn = DateTime.Now;
            MinimumBalance = 500;
        }


        public void Charge()
        {
            Withdraw(CHARGES);
            OnBalanceChanged($"Minumum balance must be maintained. You have been charged Rs. {CHARGES}.");
        }

        public override string ToString() => $"Account Type: Current\n {base.ToString()}\n" +
                $"Minimum Balance: {MinimumBalance}\n" +
                "========================================\n";
    }
}
