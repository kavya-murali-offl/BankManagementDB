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

        public override string ToString() => $"\nAccount Type: Current\n Account Number: {AccountNumber} \nAccount Status: {Status} \nBalance: Rs. {Balance}\n" +
                $"Minimum Balance: {MinimumBalance}\n" +
                "========================================\n";
    }
}
