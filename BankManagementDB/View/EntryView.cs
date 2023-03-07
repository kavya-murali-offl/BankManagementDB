using System;
using BankManagementDB.Config;
using BankManagementDB.EnumerationType;
using BankManagementDB.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace BankManagementDB.View
{
    public class EntryView
    {
        public void Entry()
        {
            while (true)
            {
                Console.WriteLine();
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
                    Notification.Error(err.ToString());
                }
            }
        }
        
        public bool EntryOperations(EntryCases option)
        {
            IAccountFactory accountFactory = DependencyContainer.ServiceProvider.GetRequiredService<IAccountFactory>();

            switch (option)
            {
                case EntryCases.LOGIN:

                    LoginView loginView = new LoginView();
                    loginView.UserChanged += onUserChanged;
                    loginView.Login();
                    loginView.UserChanged -= onUserChanged;
                    return false;

                case EntryCases.SIGNUP:
                    
                    SignupView signupView = new SignupView(accountFactory);
                    signupView.Signup();
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
