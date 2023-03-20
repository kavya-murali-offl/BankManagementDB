using System;
using BankManagementDB.EnumerationType;
using BankManagementDB.Models;
using BankManagementDB.Constants;
using BankManagementDB.Utility;
using BankManagementDB.Config;

namespace BankManagementDB.Model
{
    public class CurrentAccount : Account
    {
        public decimal CHARGES = 100;

        public override string ToString() => base.ToString() + Formatter.FormatString(DependencyContainer.GetResource("DisplayCurrentAccount"), MinimumBalance);
    }
}
