using BankManagementDB.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BankManagementDB.Utility
{
    public class Printer
    {
        public static void PrintStatement(IEnumerable<Transaction> transactions)
        {
            try
            {
                StreamWriter sw = new StreamWriter("C:\\Users\\kavya-pt6688\\source\\repos\\BankManagementDB\\Statement.txt");

                sw.WriteLine("Account: " + transactions.ElementAt(0).FromAccountNumber);
                sw.WriteLine("No. of Transactions: " + transactions.Count());
                sw.WriteLine("S.No.\t\tTransaction ID\t\t\t\t\tTransaction Time\t\t\t\t\tTransaction Type\t\t\tAmount\t\t\t\tBalance");

                for (int i = 0; i < transactions.Count(); i++)
                {
                    Transaction transaction = transactions.ElementAt(i);
                    sw.WriteLine(i + 1 + "\t\t" +
                    transaction.ID + "\t\t" +
                    transaction.RecordedOn + "\t\t" +
                    transaction.TransactionType + "\t\t" +
                    transaction.Amount + "\t\t" +
                    transaction.Balance + "\t\t"); 
                }
                Console.WriteLine("File written successfully");
                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
