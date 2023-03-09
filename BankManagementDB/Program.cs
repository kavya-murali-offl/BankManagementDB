using BankManagementDB.View;
using System;
using System.Globalization;
using System.IO;
using System.Threading;

namespace BankManagementDB
{
    public class Program
    {
        static void Main(string[] args)
        {
           
            Environment.SetEnvironmentVariable("DATABASE_PATH", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "BankDB.db3"));
            Environment.SetEnvironmentVariable("DATABASE_PASSWORD", "pass");
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");

            EntryView entryView = new EntryView();
            entryView.Entry();

            Console.ReadKey();

        }
    }
}
