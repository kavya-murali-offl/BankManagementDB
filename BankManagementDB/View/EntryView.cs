using System;
using BankManagementDB.Controller;
using BankManagementDB.Config;
using BankManagementDB.Controller;
using BankManagementDB.EnumerationType;
using BankManagementDB.Interface;
using BankManagementDB.View;
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
                    Notification.Error("Enter a valid option. Try Again!");
                }
            }
        }
        
        public bool EntryOperations(EntryCases option)
        {
            ICustomerController customersController = DependencyContainer.ServiceProvider.GetRequiredService<ICustomerController>();
            IAccountController accountController = DependencyContainer.ServiceProvider.GetRequiredService<IAccountController>();
            ITransactionProcessController transactionProcessController = DependencyContainer.ServiceProvider.GetRequiredService<ITransactionProcessController>();
            IAccountFactory accountFactory = DependencyContainer.ServiceProvider.GetRequiredService<IAccountFactory>();
            ICardFactory cardFactory = DependencyContainer.ServiceProvider.GetRequiredService<ICardFactory>();
            ICardController cardController = DependencyContainer.ServiceProvider.GetRequiredService<ICardController>();
            switch (option)
            {
                case EntryCases.LOGIN:

                    LoginView loginView = new LoginView(customersController);
                    loginView.UserChanged += onUserChanged;
                    loginView.Login();
                    return false;

                case EntryCases.SIGNUP:
                    
                    SignupView signupView = new SignupView(customersController, cardController, accountFactory, cardFactory, accountController, transactionProcessController);
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
