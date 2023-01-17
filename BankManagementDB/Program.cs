using BankManagement.Controller;
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
            DatabaseOperations.CreateTableIfNotExists();

            CustomersController customersController = new CustomersController();
            customersController.FillTable();

            EntryView entryView = new EntryView();
            entryView.Entry();

            Console.ReadKey();  

        }
    }
}
