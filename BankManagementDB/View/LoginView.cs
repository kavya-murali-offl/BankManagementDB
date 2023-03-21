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
                HelperView helper = new HelperView();
                Notification.Info(Formatter.FormatString(DependencyContainer.GetResource("PressBackButtonInfo")));
                string phoneNumber = GetPhoneNumber();
                if(phoneNumber != null)
                {
                    Customer customer = GetCustomerByPhone(phoneNumber);
                    if(customer != null)
                    {
                        Console.Write(DependencyContainer.GetResource("EnterPassword"));
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
                    {
                        Notification.Error(DependencyContainer.GetResource("PhoneNotRegistered"));
                    }
                }
            }
            catch(Exception ex) {
                Notification.Error(ex.ToString());
            }
        }

        public void LoginCustomer(Customer customer)
        {
            customer.LastLoggedOn = DateTime.Now;
            Store.CurrentUser = customer;
            Notification.Success("\n" + Formatter.FormatString(DependencyContainer.GetResource("WelcomeUser"), customer.Name));
        }

        public void LogoutCustomer()
        {
            Store.CurrentUser = null;
            UserChanged?.Invoke(DependencyContainer.GetResource("LogoutSuccess"));
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
                Console.Write(DependencyContainer.GetResource("EnterPhoneNumber"));
                string phoneNumber = Console.ReadLine()?.Trim();
                Validator validation = new Validator();

                if (phoneNumber == DependencyContainer.GetResource("BackButton"))
                { break; }
                else if (validation.IsPhoneNumber(phoneNumber))
                { return phoneNumber; }
                else
                { Notification.Error(DependencyContainer.GetResource("InvalidPhoneNumber")); }
            }
            return null;
        }

        public bool ValidateLogin(Customer customer, string password)
        {
            bool isValidated = false;
            IGetCustomerCredentialsDataManager customerCredentialsDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IGetCustomerCredentialsDataManager>();
            CustomerCredentials customerCredentials = customerCredentialsDataManager.GetCustomerCredentials(customer.ID);
            isValidated = ValidatePassword(customerCredentials, password);
            if (!isValidated)
            {
                Notification.Error(DependencyContainer.GetResource("PasswordIncorrect"));
            }
            return isValidated;

        }

        private bool ValidatePassword(CustomerCredentials customerCredentials, string password)
        {
            string hashedInput = AuthServices.HashPassword(password, customerCredentials.Salt);
            if (customerCredentials != null)
            {
                return hashedInput.Equals(customerCredentials.Password) ? true : false;
            }
            return false;
        }

    }
}
