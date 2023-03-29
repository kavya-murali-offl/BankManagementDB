using System;
using BankManagementDB.Utility;
using BankManagementDB.Models;
using BankManagementDB.Interface;
using BankManagementDB.Config;
using Microsoft.Extensions.DependencyInjection;
using BankManagementDB.Model;
using BankManagementDB.Data;
using BankManagementDB.Properties;
using BankManagementDB.ViewModel;
using BankManagementDB.Events;

namespace BankManagementDB.View
{
    public class LoginView
    {
        public Action<string> UserChanged;

        public LoginView()
        {
            LoginViewModel = new LoginViewModel();
        }

        private LoginViewModel LoginViewModel { get; set; }

        public Customer Customer;
        public void Login()
        {
            try
            {
                HelperView helper = new HelperView();
                Notification.Info(Formatter.FormatString(Resources.PressBackButtonInfo));
                string phoneNumber = GetPhoneNumber();
                if (phoneNumber != null)
                {
                    Customer = LoginViewModel.GetCustomerByPhone(phoneNumber);
                    if (Customer != null)
                    {
                        Console.Write(Resources.Password + ": ");
                        string password = helper.GetPassword();
                        if (password != null)
                        {
                            AppEvents commonEvents = DependencyContainer.ServiceProvider.GetRequiredService<AppEvents>();
                            commonEvents.IsLoggedIn += LoginCustomer;
                            LoginViewModel.LoginUser(Customer.ID, password);
                            commonEvents.IsLoggedIn -= LoginCustomer;
                        }
                    }
                    else
                        Notification.Error(Resources.PhoneNotRegistered);
                }
            }
            catch (Exception ex)
            {
                Notification.Error(ex.ToString());
            }
        }

        public void LoginCustomer(bool isLoggedIn)
        {
            if (isLoggedIn)
            {
                Customer.LastLoggedOn = DateTime.Now;
                Store.CurrentUser = Customer;
                Notification.Success("\n" + Formatter.FormatString(Resources.WelcomeUser, Customer.Name));
                DashboardView dashboard = new DashboardView();
                dashboard.ViewDashboard();
            }
            else
            {
                Notification.Error(Resources.InvalidPassword);
            }
        }

        public void LogoutCustomer()
        {
            Store.CurrentUser = null;
            UserChanged?.Invoke(Resources.LogoutSuccess);
        }


        public string GetPhoneNumber()
        {
            while (true)
            {
                Console.Write(Resources.PhoneNumber + ": ");
                string phoneNumber = Console.ReadLine()?.Trim();
                Validator validation = new Validator();

                if (phoneNumber == Resources.BackButton)
                    break;
                else if (validation.IsPhoneNumber(phoneNumber))
                    return phoneNumber;
                else
                    Notification.Error(Resources.InvalidPhoneNumber);
            }
            return null;
        }
    }
}
