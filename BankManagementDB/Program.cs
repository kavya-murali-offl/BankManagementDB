using BankManagement;
using BankManagement.Controller;
using BankManagement.Models;
using BankManagement.View;
using BankManagementDB.db;
using BankManagementDB.Utility;
using System;
using System.Data.Linq;
using System.Data.SQLite;
using System.Linq;

namespace BankManagementDB
{
    public class Program
    {
        static void Main(string[] args)
        {
            Config config = new Config();
            config.SetVariables();

            CipherOperations.CreateTablesIfNotExists();
            

            EntryView entryView = new EntryView();
            entryView.Entry();

            Console.ReadKey();

        }
    }
}
