

using BankManagement.Controller;
using BankManagement.Utility;
using BankManagementDB.View;
using System;

namespace BankManagement.View
{
    public enum Entry
    {
        LOGIN, SIGNUP, EXIT
    }
    public class EntryView
    {
        public void Entry()
        {
            while (true)
            {
                Console.WriteLine("\n1.Login\n2.Signup\n3.Exit\nEnter your choice: ");
                try
                {
                    string option = Console.ReadLine().Trim();
                    int entryOption = int.Parse(option);
                    if (entryOption != 0 && entryOption <= Enum.GetValues(typeof(Entry)).Length)
                        if (EntryOperations(entryOption))
                            break;
                }
                catch (Exception err)
                {
                    Notification.Error("Enter a valid option. Try Again!");
                }
            }
        }
        
        public bool EntryOperations(int option)
        {
            switch (option)
            {
                case 1:
                    LoginView loginView = new LoginView();
                    loginView.UserChanged += onUserChanged;
                    loginView.Login();
                    return false;

                case 2:
                    SignupView signupView = new SignupView();
                    signupView.Signup();
                    return false;

                case 3:
                    Environment.Exit(0);    
                    return true;

                default:
                    Notification.Error("Enter valid option. Try Again!");
                    return false;
            }
        }

        public void onUserChanged(string message)
        {
            Notification.Success(message);
        }
    }
}
