﻿using System;
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
             $"\nAccount Type: Savings\n Account ID: {ID} \nAccount Status: {Status} \nBalance: Rs. {Balance}\n" +
                 $"Interest Rate:  {InterestRate}\n" +
                "========================================\n";

    }
}
