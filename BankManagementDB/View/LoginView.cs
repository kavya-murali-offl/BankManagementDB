using System;
using BankManagementDB.Utility;
using BankManagementDB.Models;
using BankManagementDB.Interface;
using BankManagementDB.Config;
using Microsoft.Extensions.DependencyInjection;
using BankManagementDB.Model;
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
                        Console.WriteLine(Properties.Resources.EnterPassword);
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
                        Notification.Error(Properties.Resources.PhoneNotRegistered);
                }
               
            }
            catch(Exception ex) {
                Notification.Error(ex.ToString());
            }
        }

        public void LoginCustomer(Customer customer)
        {
            customer.LastLoggedOn = DateTime.Now;
            CacheData.CurrentUser = customer;
            Notification.Success("\n" + string.Format(Properties.Resources.WelcomeUser, customer.Name) + "\n");
        }

        public void LogoutCustomer()
        {
            CacheData.CurrentUser = null;
            UserChanged.Invoke(Properties.Resources.LogoutSuccess);
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
                Console.Write(Properties.Resources.EnterPhoneNumber);
                string phoneNumber = Console.ReadLine().Trim();
                Validation validation = new Validation();

                if (phoneNumber == Properties.Resources.BackButton)
                    break;
                else if (validation.IsPhoneNumber(phoneNumber))
                    return phoneNumber;
                else
                    Notification.Error(Properties.Resources.InvalidPhoneNumber);
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
                 if (!isValidated) Notification.Error(Properties.Resources.PasswordIncorrect);
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
