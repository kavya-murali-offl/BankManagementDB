using BankManagementDB.DatabaseAdapter;
using BankManagementDB.Model;
using BankManagementDB.Utility;
using BankManagementDB.View;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
namespace BankManagementDB
{
    public class Program
    {
        static void Main(string[] args)
        {

            Environment.SetEnvironmentVariable("DATABASE_PASSWORD", "pass");
           
            EntryView entryView = new EntryView();
            entryView.Entry();

            Console.ReadKey();
        }
    }
}
