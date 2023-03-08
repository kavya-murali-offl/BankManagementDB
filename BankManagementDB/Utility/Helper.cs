using System;
using System.Text;
using BankManagementDB.Controller;
using BankManagementDB.Config;
using BankManagementDB.Interface;
using BankManagementDB.View;
using Microsoft.Extensions.DependencyInjection;
using BankManagementDB.Data;
using BankManagementDB.Model;
using System.Linq;
using BankManagementDB.EnumerationType;

namespace BankManagementDB.Utility
{
    public class Helper
    {
        public static T StringToEnum<T>(string data) => (T)Enum.Parse(typeof(T), data);

        public string GetPassword()
        {
            StringBuilder passwordBuilder = new StringBuilder();
            bool continueReading = true;
            char newLineChar = '\r';

            while (continueReading)
            {
                ConsoleKeyInfo consoleKeyInfo = Console.ReadKey(true);
                char passwordChar = consoleKeyInfo.KeyChar;

                if (passwordChar == newLineChar)
                    continueReading = false;
                else
                    passwordBuilder.Append(passwordChar.ToString());
            }
            string password = passwordBuilder.ToString();
           
            if (!string.IsNullOrEmpty(password) && password != "0")
                return password;

            return null;
        }

        public int CountDays()
        {
           
            DateTime? lastWithdrawDate = GetLastWithdrawDate();
            if (lastWithdrawDate.HasValue)
            {
               int numberOfDays = (int)(DateTime.Now - lastWithdrawDate)?.TotalDays;
               if (numberOfDays > 30) return numberOfDays;
            }
            return 0;
        }

        public DateTime? GetLastWithdrawDate()
        {
            if (CacheData.TransactionList.Count > 0)
            {
                Transaction transaction = CacheData.TransactionList.Where(data => data.TransactionType == TransactionType.WITHDRAW).LastOrDefault();
                if (transaction == null)
                    transaction = CacheData.TransactionList.Where(data => data.TransactionType == TransactionType.DEPOSIT).LastOrDefault();
                return transaction.RecordedOn;
            }
            return null;
        }

    }
}
