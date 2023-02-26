using System;
using BankManagementDB.Controller;
using BankManagementDB.Utility;
using BankManagementDB.View;
using BankManagementDB.Models;
using BankManagementDB.Interface;
using BankManagementDB.Config;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using BankManagementDB.Controller;

namespace BankManagementDB.View
{
    public class LoginView
    {
        public event Action<string> UserChanged;
        
        public LoginView(ICustomerController customerController) {
            CustomerController = customerController;
        }
        
        public ICustomerController CustomerController { get; set; }
        public void Login()
        {
            try
            {
                Helper helper = new Helper();
                string phoneNumber = GetPhoneNumber();
                if(phoneNumber != null)
                {
                    if (ValidatePhoneNumber(phoneNumber))
                    {
                        Console.Write("Enter password: ");
                        string password = helper.GetPassword();

                        if (password != null)
                        {
                            bool isValidated = ValidateLogin(phoneNumber, password);
                            if (isValidated)
                            {
                                Customer currentUser = CustomerController.GetCustomer(phoneNumber);
                                currentUser.LastLoggedOn = DateTime.Now;
                                CustomerController.SetCurrentUser(currentUser);

                                UserChanged?.Invoke("\nWelcome " + currentUser.Name + "!!!\n");

                                DashboardView dashboard = new DashboardView();
                                dashboard.ViewDashboard(currentUser);

                                UserChanged?.Invoke($"User {currentUser.Name} logged out successfully.");
                                CustomerController.SetCurrentUser(null);
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

        public bool ValidatePhoneNumber(string phoneNumber)
        {
            Customer customer = CustomerController.GetCustomer(phoneNumber);
            return customer == null ? false : true;    
            
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
            return null; ;
        }

        public bool ValidateLogin(string phoneNumber, string password)
        {
            bool isValidated = false;
            IValidationServices validationServices = DependencyContainer.ServiceProvider.GetRequiredService<IValidationServices>();

            try
            {
                Customer customer = CustomerController.GetCustomer(phoneNumber);

                if (customer != null)
                {
                    isValidated = validationServices.ValidatePassword(phoneNumber, password);
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
