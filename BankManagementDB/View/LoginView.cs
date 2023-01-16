using System;
using BankManagement;
using BankManagement.Models;
using BankManagement.Controller;
using BankManagement.Utility;
using BankManagementDB.db;
using System.Collections;
using System.Collections.Generic;
using System.Data;

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
                customersController.FillTable();
                string userName = GetUserName();
                isValidated = ValidateLogin(userName, customersController);
                if (isValidated)
                {
                    ProfileController profile = new ProfileController();
                    profile.Customer = customersController.GetCustomerByUserName(userName);
                    Console.WriteLine("Customer", profile.Customer);
                    DashboardView dashboard = new DashboardView();
                    IDictionary<string, object> parameters = new Dictionary<string, object>();
                    parameters.Add("UserID", profile.ID);
                    AccountsController accountsController = new AccountsController();   
                    accountsController.FillTable(profile.ID);
                    dashboard.ViewDashboard(profile, accountsController);
                }
            }
            catch(Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }

        public String GetUserName()
        {
            Console.WriteLine("Enter UserName: ");
            return Console.ReadLine();
        }

        public String GetPassword()
        {
            Console.WriteLine("Enter password: ");
            return Console.ReadLine();
        }

        public bool ValidateLogin(string username, CustomersController customersController)
        {
            bool isValidated = false;
            try
            {
                DataRow dr = customersController.GetUserByUserName(username);
                if (dr != null)
                {
                    string password = GetPassword();
                    isValidated = customersController.ValidatePassword(username, password);
                    if (!isValidated) Console.WriteLine("Incorrect Password");
                }
                else
                {
                    Console.WriteLine("UserName does not exist.");
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
