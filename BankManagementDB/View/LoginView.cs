using System;
using BankManagement.Controller;
using System.Data;
using BankManagement.Utility;
using BankManagementDB.View;
using BankManagement.Models;
using BankManagementDB.db;
using BankManagementDB.Interface;

namespace BankManagement.View
{
    public class LoginView
    {
        public event Action<string> UserChanged;
        
        public void Login(ICustomerServices customersController)
        {
            try
            {
                bool isValidated;
                Helper helper = new Helper();
                string phoneNumber = helper.GetPhoneNumber();
                isValidated = ValidateLogin(phoneNumber, customersController);
              
                if (isValidated)
                    {
                        ProfileController profile = new ProfileController();
                        profile.Customer = customersController.GetCustomer(phoneNumber);
                        profile.Customer.LastLoggedOn = DateTime.Now;

                        UserChanged?.Invoke("\nWelcome " + profile.Customer.Name + "!!!\n" );
                        
                        AccountsController accountsController = new AccountsController();
                        accountsController.FillTable(profile.Customer.ID);

                        DashboardView dashboard = new DashboardView();
                        dashboard.ViewDashboard(profile);

                        UserChanged?.Invoke($"User {profile.Customer.Name} logged out successfully.");
                        profile.Customer = null;
                }
            }
            catch(Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }

        public bool ValidateLogin(string phoneNumber, ICustomerServices customerServices)
        {
            bool isValidated = false;
            try
            {
                Customer customer = customerServices.GetCustomer(phoneNumber);
                if (customer != null)
                {
                    Helper helper = new Helper();
                    string password = helper.GetPassword("Enter password: ");
                    isValidated = customerServices.ValidatePassword(phoneNumber, password);
                    if (!isValidated) Notification.Error("Incorrect Password");
                }
                else
                    Notification.Error("This Phone Number is not registered with us. Please try again!");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);  
            }
            return isValidated;

        }
    }
}
