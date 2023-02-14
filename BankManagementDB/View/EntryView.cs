using System;
using BankManagement.Controller;
using BankManagementDB.db;
using BankManagementDB.Interface;
using BankManagementDB.View;

namespace BankManagement.View
{
    public enum EntryCases
    {
        LOGIN, SIGNUP, EXIT
    }

    public class EntryView
    {
        public void Entry()
        {
            while (true)
            {
                for (int i = 0; i < Enum.GetNames(typeof(EntryCases)).Length; i++)
                {
                    EntryCases cases = (EntryCases)i;
                    Console.WriteLine($"{i + 1}. {cases.ToString().Replace("_", " ")}");
                }

                Console.Write("\nEnter your choice: ");

                try
                {
                    string option = Console.ReadLine().Trim();
                    int entryOption = int.Parse(option);
                    if (entryOption != 0 && entryOption <= Enum.GetValues(typeof(EntryCases)).Length)
                    {
                        EntryCases entry = (EntryCases)entryOption - 1;
                        if (EntryOperations(entry))
                            break;
                    }
                }
                catch (Exception err)
                {
                    Notification.Error("Enter a valid option. Try Again!");
                }
            }
        }
        
        public bool EntryOperations(EntryCases option)
        {
            CustomerOperations customerOperations = new CustomerOperations();
            CustomersController customersController = new CustomersController(customerOperations, customerOperations);
            switch (option)
            {
                case EntryCases.LOGIN:

                    LoginView loginView = new LoginView();
                    loginView.UserChanged += onUserChanged;
                    loginView.Login(customersController);
                    return false;

                case EntryCases.SIGNUP:
                    
                    SignupView signupView = new SignupView();
                    signupView.Signup(customersController);
                    return false;

                case EntryCases.EXIT:

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
