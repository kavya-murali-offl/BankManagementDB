using System;
using BankManagementDB.Utility;
using BankManagementDB.Models;
using BankManagementDB.Interface;
using BankManagementDB.Config;
using Microsoft.Extensions.DependencyInjection;
using BankManagementDB.Controller;
using BankManagementDB.Model;
using BankManagementDB.DataManager;

namespace BankManagementDB.View
{
    public class LoginView
    {
        public LoginView() {
            ValidatePasswordDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IValidatePasswordDataManager>(); 
            GetCustomerDataManager = DependencyContainer.ServiceProvider.GetService<IGetCustomerDataManager>();
        }

        public Action<string> UserChanged;

        public IGetCustomerDataManager GetCustomerDataManager { get; private set; }

        public IValidatePasswordDataManager ValidatePasswordDataManager { get; private set; }

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
                                Customer currentUser = GetCustomerDataManager.GetCustomer(phoneNumber);
                                
                                LoginCustomer(currentUser);

                                DashboardView dashboard = new DashboardView();
                                dashboard.ViewDashboard();

                                LogoutCustomer();

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
            CurrentUserDataManager.CurrentUser = customer;
            UserChanged?.Invoke("\nWelcome " + customer.Name + "!!!\n");
        }

        public void LogoutCustomer()
        {
            CurrentUserDataManager.CurrentUser = null;
            UserChanged.Invoke("User logged out successfully");

        }

        public Customer GetCustomerByPhone(string phoneNumber)
        {
             return GetCustomerDataManager.GetCustomer(phoneNumber);
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
                 isValidated = ValidatePasswordDataManager.ValidatePassword(customer.ID, password);
                 if (!isValidated) Notification.Error("Incorrect Password");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);  
            }
            return isValidated;

        }

    }
}
