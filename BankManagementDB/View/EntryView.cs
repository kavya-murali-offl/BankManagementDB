using System;
using System.Resources;
using BankManagementDB.EnumerationType;

namespace BankManagementDB.View
{
    public class EntryView
    { 

        public void Entry()
        {
            try
            {
                while (true)
                {
                    Console.WriteLine();
                    for (int i = 0; i < Enum.GetNames(typeof(EntryCases)).Length; i++)
                    {
                        EntryCases cases = (EntryCases)i;
                        Console.WriteLine($"{i + 1}. {cases.ToString().Replace("_", " ")}");
                    }

                    Console.Write(Properties.Resources.EnterChoice);

                    string option = Console.ReadLine().Trim();

                    if (int.TryParse(option, out int entryOption))
                    {
                        if (entryOption != 0 && entryOption <= Enum.GetValues(typeof(EntryCases)).Length)
                        {
                            EntryCases entry = (EntryCases)entryOption - 1;
                            if (EntryOperations(entry))
                                break;
                        }
                    }
                    else
                        Notification.Error(Properties.Resources.InvalidInput);
                }
            }
            catch (Exception err)
            {
                Notification.Error(err.ToString());
            }
        }
        
        public bool EntryOperations(EntryCases option)
        {
            switch (option)
            {
                case EntryCases.LOGIN:

                    LoginView loginView = new LoginView();
                    loginView.UserChanged += onUserChanged;
                    loginView.Login();
                    loginView.UserChanged -= onUserChanged;
                    return false;

                case EntryCases.SIGNUP:
                    
                    SignupView signupView = new SignupView();
                    signupView.Signup();
                    return false;

                case EntryCases.EXIT:

                    Environment.Exit(0);    
                    return true;

                default:

                    Notification.Error(Properties.Resources.InvalidInput);
                    return false;
            }
        }

        public void onUserChanged(string message)
        {
            Notification.Success(message);
        }
    }
}
