using System;
using System.Xml.Linq;
using BankManagement.Controller;
using BankManagement.Enums;
using BankManagement.Models;
using BankManagement.Utility;

namespace BankManagement.Model
{
    public class SavingsAccount : Account
    {
        public SavingsAccount() : base()
        {
            InterestRate = AccountInterestRate.SAVINGS_INTEREST_RATE;
            Type = AccountTypes.SAVINGS;
            CreatedOn = DateTime.Now;
        }

        public decimal DepositInterest()
        {
            Helper helper= new Helper();
            decimal interest = (Balance * helper.CountDays() * InterestRate) / (100 * 12);
            interest = Math.Round(interest, 3);
            Deposit(interest);
            return interest;
        }

        public override string ToString()
        {
            return $"Account Type: Savings\n {base.ToString()}\n" +
                 $"Interest Rate:  {InterestRate}\n" +
                "========================================\n";
        }

    }
}
