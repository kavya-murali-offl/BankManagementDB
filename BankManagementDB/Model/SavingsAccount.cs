﻿using System;
using BankManagementDB.Config;
using BankManagementDB.EnumerationType;
using BankManagementDB.Models;
using BankManagementDB.Utility;

namespace BankManagementDB.Model
{
    public class SavingsAccount : Account
    {

        public decimal GetInterest()
        {
            Helper helper = new();
            decimal interest = (Balance * helper.CountDays() * InterestRate) / (100 * 12);
            interest = Math.Round(interest, 3);
            return interest;
        }

        public override string ToString() => base.ToString() + Formatter.FormatString(DependencyContainer.GetResource("DisplaySavingsAccount"), InterestRate);

    }
}
