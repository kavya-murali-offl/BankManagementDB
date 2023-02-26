using BankManagementDB.View;
using BankManagementDB.Repository;
using System;

namespace BankManagementDB
{
    public class Program
    {
        static void Main(string[] args)
        {

            Environment.SetEnvironmentVariable("DATABASE_PATH", "Database.sqlite3");
            Environment.SetEnvironmentVariable("DATABASE_PASSWORD", "pass");

            DBRepository.CreateTablesIfNotExists();
            
            EntryView entryView = new EntryView();
            entryView.Entry();

            Console.ReadKey();

        }
    }
}
