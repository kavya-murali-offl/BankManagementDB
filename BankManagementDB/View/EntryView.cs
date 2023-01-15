

using BankManagement.Controller;
using BankManagement.Utility;
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
            AccountsController accountsController = new AccountsController();
            while (true)
            {
                Console.WriteLine("1.Login\n2.Signup\n3.Exit\nEnter your choice: ");
                try
                {
                    string option = Console.ReadLine();
                    int entryOption = int.Parse(option);
                    if (entryOption != 0 && entryOption <= Enum.GetValues(typeof(Entry)).Length)
                    {
                        if (EntryOperations(entryOption, accountsController))
                        {
                            break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Enter proper input.");
                    }
                }
                catch (Exception err)
                {
                    Console.WriteLine("Enter a valid option. Try Again!");
                }
            }
        }
        

        public bool EntryOperations(int option, AccountsController accountsController)
        {

            switch (option)
            {
                case 1:
                    LoginView loginView = new LoginView();
                    loginView.Login(accountsController);
                    return false;
                case 2:
                    SignupView signupView = new SignupView();
                    signupView.Signup(accountsController);
                    return false;
                case 3:
                    Console.WriteLine("Closed");
                    return true;
                default:
                    Console.WriteLine("Enter valid option. Try Again!");
                    return false;
            }
        }
        
    }
}
