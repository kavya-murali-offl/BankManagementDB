using System;
using System.Xml.Linq;
using BankManagement.Controller;
using BankManagement.Models;
using BankManagement.Utility;

namespace BankManagement.Model
{
    public class SavingsAccount : Account
    {

        public void DepositInterest(decimal amount) {
            decimal interest = (Balance * CountDays() * InterestRate) / (100 * 12);
            Deposit(interest);
        }

        public override string ToString()
        {
            return $@"Account Type: Savings\n"+
                base.ToString()+
                "\nInterest Rate: " + InterestRate  + 
                "\n========================================\n";
        }

        public int CountDays()
        {
            TransactionController transactionController = new TransactionController();
            DateTime? lastWithdrawnDate = transactionController.GetLastWithdrawnDate();
            if(lastWithdrawnDate.HasValue) { 
                DateTime TodayDate = DateTime.Now;
                int numberOfDays = (int)(DateTime.Now - lastWithdrawnDate)?.TotalDays;
                return numberOfDays;
            }
            return 0;
        }

    }
}
