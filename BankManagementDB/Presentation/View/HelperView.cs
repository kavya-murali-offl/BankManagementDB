using BankManagementDB.Config;
using BankManagementDB.Properties;
using System;
using System.Linq;
using System.Text;

namespace BankManagementDB.View
{
    public delegate bool OptionsDelegate<T>(T option);

    public class HelperView
    {
        public decimal GetAmount()
        {
            try
            {
                while (true)
                {
                    Console.Write(Resources.Amount + ": ");
                    decimal amount = decimal.Parse(Console.ReadLine()?.Trim());
                    if (amount < 0)
                    {
                        Notification.Error(Resources.PositiveAmountWarning);
                    }
                    else
                    {
                        return amount;
                    }
                }
            }
            catch (Exception error)
            {
                Notification.Error(error.ToString());
            }
            return 0;
        }

        public string GetPassword()
        {
            StringBuilder passwordBuilder = new StringBuilder();
            ConsoleKey key;

            do
            {
                var keyInfo = Console.ReadKey(intercept: true);
                key = keyInfo.Key;

                if (key == ConsoleKey.Backspace && passwordBuilder.Length > 0)
                {
                    Console.Write("\b \b");
                    passwordBuilder.Remove(passwordBuilder.Length - 1, 1);
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    passwordBuilder.Append(keyInfo.KeyChar);
                    Console.Write("*");
                }

            } while (key != ConsoleKey.Enter);

            Notification.Info("");

            string password = passwordBuilder.ToString();

            if (!string.IsNullOrEmpty(password) && password != Resources.BackButton)
            {
                return password;
            }

            return null;
        }

        public void PerformOperation<T>(OptionsDelegate<T> function)
        {
            if (function == null)
            {
                throw new ArgumentNullException(Resources.NullDelegateException);
            }
            else
            {
                while (true)
                {
                    for (int i = 0; i < Enum.GetNames(typeof(T)).Length; i++)
                    {
                        T cases = (T)(object)i;
                        Console.WriteLine($"{i + 1}. {cases.ToString().Replace("_", " ")}");
                    }

                    Console.Write("\n" + Resources.EnterChoice + ": ");

                    string option = Console.ReadLine()?.Trim();
                    Console.WriteLine();
                    if (!int.TryParse(option, out int entryOption))
                    {
                        Notification.Error(Resources.InvalidInteger);
                    }
                    else
                    {
                        if (entryOption == 0)
                        {
                            break;
                        }
                        else if (entryOption <= Enum.GetNames(typeof(T)).Count())
                        {
                            T cases = (T)(object)(entryOption - 1);
                            if (function(cases))
                            {
                                break;
                            }
                        }

                        else
                        {
                            Notification.Error(Resources.InvalidOption);
                        }
                    }
                }
            }
        }
    }
}
