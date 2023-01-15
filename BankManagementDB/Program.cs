using BankManagement.View;
using BankManagementDB.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB
{
    public class Program
    {
        static void Main(string[] args)
        {
            EntryView entryView = new EntryView();
            entryView.Entry();

            Console.ReadKey();  

        }
    }
}
