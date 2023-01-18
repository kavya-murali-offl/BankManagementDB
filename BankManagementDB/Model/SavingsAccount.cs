using System;
using System.Xml.Linq;
using BankManagement.Controller;
using BankManagement.Models;
using BankManagement.Utility;

namespace BankManagement.Model
{
    public class SavingsAccount : Account
    {

        



        public override string ToString()
        {
            return $"Account Type: Savings\n {base.ToString()}\n" +
                 $"Interest Rate:  {InterestRate}\n" +
                "========================================\n";
        }

    }
}
