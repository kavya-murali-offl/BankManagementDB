using System;
using BankManagementDB.Config;
using BankManagementDB.EnumerationType;

namespace BankManagementDB.View
{
    public class EntryView
    { 

        public void Entry()
        {
            try
            {
                OptionsDelegate<EntryCases> options = EntryOperations;

                HelperView helperView = new HelperView();
                helperView.PerformOperation(options);

            }catch(Exception ex)
            {
                Notification.Error(ex.ToString());
            }
            
        }

        public bool EntryOperations(EntryCases command) =>
        command switch
        {
            EntryCases.LOGIN => Login(),
            EntryCases.SIGNUP => Signup(),
            EntryCases.EXIT => Exit(),
            _ => Default()
        };

        public bool Login()
        {
            LoginView loginView = new LoginView();
            loginView.UserChanged += onUserChanged;
            loginView.Login();
            loginView.UserChanged -= onUserChanged;
            return false;
        }

        public bool Signup()
        {
            SignupView signupView = new SignupView();
            signupView.Signup();
            return false;
        }

        public bool Exit()
        {
            Environment.Exit(0);
            return true;
        }

        public bool Default()
        {
            Notification.Error(DependencyContainer.GetResource("InvalidInput"));
            return false;
        }

        public void onUserChanged(string message)
        {
            Notification.Success(message);
        }
    }
}
