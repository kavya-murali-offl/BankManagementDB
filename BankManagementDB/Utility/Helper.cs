using System;
using System.Text;
using BankManagementDB.Controller;
using BankManagementDB.Config;
using BankManagementDB.Interface;
using BankManagementDB.View;
using Microsoft.Extensions.DependencyInjection;

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
            try
            {
                ITransactionController transactionController = DependencyContainer.ServiceProvider.GetRequiredService<TransactionController>();   
                DateTime? lastWithdrawDate = transactionController.GetLastWithdrawDate();
                if (lastWithdrawDate.HasValue)
                {
                    int numberOfDays = (int)(DateTime.Now - lastWithdrawDate)?.TotalDays;
                    if (numberOfDays > 30) return numberOfDays;
                }
            }catch(Exception e)
            {
                Notification.Error(e.Message);
            }
            return 0;
        }

    }
}
