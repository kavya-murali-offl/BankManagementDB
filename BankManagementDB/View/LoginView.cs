using System;
using BankManagement.Controller;
using System.Collections.Generic;
using System.Data;
using BankManagement.Utility;

namespace BankManagement.View
{
    public class LoginView
    {
       
        public void Login()
        {
            try
            {
                bool isValidated;
                CustomersController customersController = new CustomersController();
                Helper helper = new Helper();
                string phoneNumber = helper.GetPhoneNumber();
                isValidated = ValidateLogin(phoneNumber, customersController);
              
                if (isValidated)
                    {
                        ProfileController profile = new ProfileController();
                        profile.Customer = customersController.GetCustomerByPhoneNumber(phoneNumber);
                        profile.Customer.lastLoginOn = DateTime.Now;
                        
                        AccountsController accountsController = new AccountsController();
                        accountsController.FillTable(profile.ID);

                        DashboardView dashboard = new DashboardView();
                        dashboard.ViewDashboard(profile);
                    }
            }
            catch(Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }

        public bool ValidateLogin(string phoneNumber, CustomersController customersController)
        {
            bool isValidated = false;
            try
            {
                DataRow dr = customersController.GetUserByPhoneNumber(phoneNumber);
                if (dr != null)
                {
                    Helper helper = new Helper();
                    string password = helper.GetPassword();
                    isValidated = customersController.ValidatePassword(phoneNumber, password);
                    if (!isValidated) Console.WriteLine("Incorrect Password");
                }
                else
                {
                    Console.WriteLine("This Phone Number is not registered with us. Please try again!");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);  
            }
            return isValidated;

        }
    }
}
