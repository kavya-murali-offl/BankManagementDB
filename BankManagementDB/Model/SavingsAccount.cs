using System;
using BankManagementDB.EnumerationType;
using BankManagementDB.Models;
using BankManagementDB.Utility;

namespace BankManagementDB.Model
{
    public class SavingsAccount : Account
    {
        public SavingsAccount() : base()
        {
            InterestRate = Constants.AccountConstants.SAVINGS_INTEREST_RATE;
            Type = AccountType.SAVINGS;
            CreatedOn = DateTime.Now;
            MinimumBalance = 0;
        }

        public decimal GetInterest()
        {
            Helper helper = new();
            decimal interest = (Balance * helper.CountDays() * InterestRate) / (100 * 12);
            interest = Math.Round(interest, 3);
            return interest;
        }

        public override string ToString() =>
             $"\nAccount Type: Savings\n Account Number: {AccountNumber} \nAccount Status: {Status} \nBalance: Rs. {Balance}\n" +
                 $"Interest Rate:  {InterestRate}\n" +
                "========================================\n";

    }
}
