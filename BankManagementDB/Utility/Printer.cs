using BankManagement.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BankManagement.Utility
{
    public class Printer
    {
        public static void PrintStatement(IEnumerable<Transaction> transactions)
        {
            try
            {
                StreamWriter sw = new StreamWriter("C:\\Users\\kavya-pt6688\\source\\repos\\BankManagementDB\\Statement.txt");
                sw.WriteLine("AccountID: " + transactions.ElementAt(0).AccountID);
                sw.WriteLine("No. of Transactions: " + transactions.Count());
                sw.WriteLine("S.No.\t\tTransaction ID\tTransaction Time\tTransaction Type\tAmount\t\tBalance");
                for (int i = 0; i < transactions.Count(); i++)
                {
                    sw.WriteLine(i + 1 + "\t\t" +
                    transactions.ElementAt(i).ID + "\t\t" +
                    transactions.ElementAt(i).RecordedOn + "\t\t" +
                    transactions.ElementAt(i).TransactionType + "\t\t" +
                    transactions.ElementAt(i).Amount + "\t\t" +
                    transactions.ElementAt(i).Balance + "\t\t"); 
                }
                Console.WriteLine("File written successfully");
                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
        }
    }
}
