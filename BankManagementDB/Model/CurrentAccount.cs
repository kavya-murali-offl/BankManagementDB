using System;
using BankManagementDB.EnumerationType;
using BankManagementDB.Models;
using BankManagementDB.Constants;

namespace BankManagementDB.Model
{
    public class CurrentAccount : Account
    {
        public readonly decimal CHARGES = 50;
        
        public CurrentAccount(): base() {
            InterestRate = Constants.AccountConstants.CURRENT_INTEREST_RATE;
            Type = AccountType.CURRENT;
            CreatedOn = DateTime.Now;
            MinimumBalance = 500;
        }

        public override string ToString() => $"\nAccount Type: Current\n Account Number: {AccountNumber} \nAccount Status: {Status} \nBalance: Rs. {Balance}\n" +
                $"Minimum Balance: {MinimumBalance}\n" +
                "========================================\n";
    }
}
