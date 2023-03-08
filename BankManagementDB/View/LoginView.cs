using System;
using BankManagementDB.Utility;
using BankManagementDB.Models;
using BankManagementDB.Interface;
using BankManagementDB.Config;
using Microsoft.Extensions.DependencyInjection;
using BankManagementDB.Controller;
using BankManagementDB.Model;
using BankManagementDB.DataManager;
using BankManagementDB.Data;

namespace BankManagementDB.View
{
    public class LoginView
    {
        public Action<string> UserChanged;

        public void Login()
        {
            try
            {
                Helper helper = new Helper();
                string phoneNumber = GetPhoneNumber();
                if(phoneNumber != null)
                {
                    Customer customer = GetCustomerByPhone(phoneNumber);
                    if(customer != null)
                    {
                        Console.Write("Enter password: ");
                        string password = helper.GetPassword();

                        if (password != null)
                        {
                            bool isValidated = ValidateLogin(customer, password);

                            if (isValidated)
                            {
                                Customer currentUser = customer;
                                
                                LoginCustomer(currentUser);

                                DashboardView dashboard = new DashboardView();
                                dashboard.ViewDashboard();
                            }
                        }
                    }
                    else
                        Notification.Error("This Phone Number is not registered with us. Please try again!");
                }
               
            }
            catch(Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }

        public void LoginCustomer(Customer customer)
        {
            customer.LastLoggedOn = DateTime.Now;
            CacheData.CurrentUser = customer;
            UserChanged?.Invoke("\nWelcome " + customer.Name + "!!!\n");
        }

        public void LogoutCustomer()
        {
            CacheData.CurrentUser = null;
            UserChanged.Invoke("User logged out successfully");
        }

        public Customer GetCustomerByPhone(string phoneNumber)
        {
            IGetCustomerDataManager getCustomerDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IGetCustomerDataManager>();
             return getCustomerDataManager.GetCustomer(phoneNumber);
        }

        public string GetPhoneNumber()
        {
            while (true)
            {
                Console.Write("Enter Mobile Number: ");
                string phoneNumber = Console.ReadLine().Trim();
                Validation validation = new Validation();

                if (phoneNumber == "0")
                    break;
                else if (validation.IsPhoneNumber(phoneNumber))
                    return phoneNumber;
                else
                    Notification.Error("Please enter a valid mobile number. ");
            }
            return null;
        }

        public bool ValidateLogin(Customer customer, string password)
        {
            bool isValidated = false;
            try
            {
                IGetCustomerCredentialsDataManager customerCredentialsDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IGetCustomerCredentialsDataManager>();
                CustomerCredentials customerCredentials = customerCredentialsDataManager.GetCustomerCredentials(customer.ID);
                isValidated = ValidatePassword(customerCredentials, password);
                 if (!isValidated) Notification.Error("Incorrect Password");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);  
            }
            return isValidated;

        }

        private bool ValidatePassword(CustomerCredentials customerCredentials, string password)
        {
            string hashedInput = AuthServices.HashPassword(password, customerCredentials.Salt);
            if (customerCredentials != null)
                return hashedInput.Equals(customerCredentials.Password) ? true : false;
            return false;
        }

    }
}
