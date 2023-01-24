using BankManagement;
using BankManagement.Controller;
using BankManagement.View;
using BankManagementDB.db;
using System;

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

        }
    }
}
