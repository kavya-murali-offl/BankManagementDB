using System;
using System.Data;
using System.Runtime.Remoting.Messaging;
using System.Security.Policy;
using System.Xml.Linq;
using BankManagement.Controller;
using BankManagement.Enums;
using BankManagement.Model;
using BankManagement.Models;
using BankManagement.Utility;
using BankManagementDB.Interface;
using BankManagementDB.View;

namespace BankManagement.View
{
    public class SignupView
    {
        public SignupView(ICustomerServices customerController)
        {
            CustomersController = customerController;
        }

        public ICustomerServices CustomersController { get; set; }

        public void Signup()
        {
            Validation validation = new Validation();
            Helper helper = new Helper();

            string email, password, phone, name;
            int age;

            while (true)
            {
                phone = helper.GetPhoneNumber();
                if (validation.CheckNotEmpty(phone))
                {
                    if (CheckUniquePhoneNumber(phone))
                        break;
                    else
                        Notification.Error("Phone Number Already Registered");
                }
            }

            do
            {
                password = helper.GetPassword("Enter password: ");
            } while (!validation.CheckNotEmpty(password));

            VerifyPassword(password);

            name = GetValue("Name");

            while (true)
            {
                email = GetValue("Email");
                if (validation.IsValidEmail(email)) break;
                else Notification.Error("Please enter a valid email.");
            }

            age = helper.GetInteger("Age: ");

            CreateCustomer(name, password, email, phone, age);
            Customer signedUpCustomer = CustomersController.GetCustomer(phone);
            CreateAccountAndDeposit(signedUpCustomer);
        }

        public void CreateAccountAndDeposit(Customer signedUpCustomer)
        {
            Account account = AccountFactory.GetAccountByType(AccountTypes.CURRENT);
            account.UserID = signedUpCustomer.ID;
            account.Balance = 0;
            AccountsController accountsController = new AccountsController();
            
            if (accountsController.InsertAccount(account))
            {

                TransactionController transactionController = new TransactionController();
                Helper helper = new Helper();
                decimal amount = helper.GetAmount(account as CurrentAccount);
                transactionController.Deposit(amount, account);

                Notification.Success("Account created successfully\n");

            }
        }
        public bool CheckUniquePhoneNumber(string phoneNumber)
        {
            Customer customer = CustomersController.GetCustomer(phoneNumber);
            return customer == null ? true : false;
        }

        private bool CreateCustomer(string name, string password, string email, string phone, int age)
        {

            Customer customer = new Customer()
            {
                ID = Guid.NewGuid(),
                Name = name,
                Age = age,
                Phone = phone,
                Email = email,
                LastLoggedOn = DateTime.Now,
                CreatedOn = DateTime.Now
            };

            bool customerAdded = CustomersController.InsertCustomer(customer, password);

            if (customerAdded) Notification.Success("\nSignup successful");

            return customerAdded;
        }

        private string GetValue(string label)
        {
            while (true)
            {
                Console.Write(label + ": ");
                Validation validation = new Validation();
                string value = Console.ReadLine().Trim();
                if (validation.CheckNotEmpty(value)) return value;
            }
        }

        private void VerifyPassword(string password)
        {
            Validation validation = new Validation();
            Helper helper = new Helper();
            while (true)
            {
                string rePassword = helper.GetPassword("Re-enter password: ");
                if (validation.ValidatePassword(password, rePassword) && rePassword != null)
                    break;
                else
                    Notification.Error("Password not matching, Enter again");
            }
        }
    }
}
