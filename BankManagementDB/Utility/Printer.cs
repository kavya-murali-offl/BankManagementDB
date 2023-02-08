using BankManagement.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagement.Utility
{
    public class Printer
    {
        public static void PrintStatement(IList<Transaction> transactions)
        {
            try
            {
                StreamWriter sw = new StreamWriter("C:\\Users\\kavya-pt6688\\source\\repos\\BankManagementDB\\Statement.txt");
                sw.WriteLine("AccountID: " + transactions[0].AccountID);
                sw.WriteLine("No. of Transactions: " + transactions.Count());
                sw.WriteLine("S.No.\t\tTransaction ID\tTransaction Time\tTransaction Type\tAmount\t\tBalance");
                for (int i = 0; i < transactions.Count; i++)
                {
                    sw.WriteLine(i + 1 + "\t\t" +
                        transactions[i].ID + "\t\t" +
                        transactions[i].RecordedOn + "\t\t" +
                        transactions[i].TransactionType + "\t\t" +
                        transactions[i].Amount + "\t\t" +
                        transactions[i].Balance + "\t\t"); 
                }
                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
        }
    }
}
