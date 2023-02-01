using BankManagement.Controller;
using BankManagement.Enums;
using BankManagement.Model;
using BankManagement.Models;
using BankManagementDB.View;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagement.Utility
{
    public class Helper
    {
        public decimal GetAmount()
        {
            while (true)
            {
                Console.Write("Enter amount: ");
                try
                {
                    decimal amount = Decimal.Parse(Console.ReadLine().Trim());
                    if (amount > 0) return amount;
                    else Console.WriteLine("Amount should be greater than zero.");
                }
                catch (Exception error)
                {
                    Notification.Error("Enter a valid amount. Try Again!");
                }
            }
        }

        public int GetInteger(string message)
        {
            int number = 0;
            while (true)
            {
                try
                {
                    Console.Write(message);
                    int input = int.Parse(Console.ReadLine().Trim());
                    if (input > 0)
                    {
                        number = input;
                        break;
                    }
                    else
                        Notification.Error("Enter a valid number.");
                }
                catch (Exception error)
                {
                    Notification.Error("Enter a valid number. Try Again!");
                }
            }
            return number;
        }

        public decimal GetAmount(CurrentAccount currentAccount)
        {
            while (true)
            {
                Console.Write("Enter amount: ");
                try
                {
                    decimal amount = Decimal.Parse(Console.ReadLine().Trim());
                    if (amount > 0)
                    {
                        if(amount < currentAccount.MinimumBalance)
                            Notification.Error($"Initial Amount should be greater than Minimum Balance Rs. {currentAccount.MinimumBalance}");
                        
                        else
                            return amount;
                    }
                    else Notification.Error("Amount should be greater than zero.");
                }
                catch (Exception error)
                {
                    Notification.Error(error.Message);
                }
            }
        }

        public string GetPhoneNumber()
        {
            string phoneNumber;
            while (true)
            {
                Console.Write("Enter Mobile Number: ");
                phoneNumber = Console.ReadLine().Trim();
                Validation validation = new Validation();

                if (!validation.IsPhoneNumber(phoneNumber))
                    Notification.Error("Please enter a valid mobile number. ");

                else break;
            }
            return phoneNumber;
        }

        public string GetPassword(string message)
        {
            Console.Write(message);
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

            Validation validation = new Validation();
            if (validation.CheckNotEmpty(passwordBuilder.ToString()))
                return passwordBuilder.ToString();

            return null;
        }

        public int CountDays()
        {
            try
            {
                TransactionController transactionController = new TransactionController();
                DateTime? lastWithdrawDate = transactionController.GetLastWithdrawDate();
                if (lastWithdrawDate.HasValue)
                {
                    int numberOfDays = (int)(DateTime.Now - lastWithdrawDate)?.TotalDays;
                    if(numberOfDays > 30) return numberOfDays;
                    else return 0;
                }
            }catch(Exception e)
            {
                Notification.Error(e.Message);
            }
            return 0;
        }
    }
}
