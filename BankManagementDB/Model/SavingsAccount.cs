using System;
using System.Xml.Linq;
using BankManagement.Models;
using BankManagement.Utility;

namespace BankManagement.Model
{
    public class SavingsAccount : Account
    {
        public SavingsAccount() : base()
        {
            InterestRate = AccountInterestRate.SAVINGS_INTEREST_RATE;
        }

        public void DepositInterest(decimal amount) {
            decimal interest = Balance * AccountInterestRate.SAVINGS_INTEREST_RATE / 100;
            Deposit(interest);
        }

        public override string ToString()
        {
            return "Account Type: Savings\n" +
                base.ToString()+
                "\nInterest Rate: " + AccountInterestRate.SAVINGS_INTEREST_RATE  + 
                "\n========================================\n";
        }

    }
}
