using System;
using BankManagementDB.Models;
using BankManagementDB.Utility;
using BankManagementDB.Properties;

namespace BankManagementDB.Model
{
    public class CurrentAccount : Account
    {
        public decimal CHARGES = 100;

        public override string ToString() => base.ToString() + Formatter.FormatString(Resources.DisplayCurrentAccount, MinimumBalance);
    }
}
