using System;
using System.Data;
using BankManagement.Controller;
using BankManagement.Model;
using BankManagement.Models;
using BankManagement.Utility;
using BankManagementDB.View;

namespace BankManagement.View
{
    public class SignupView
    {
        public void Signup()
        {
            Validation validation = new Validation();
            Helper helper = new Helper();

            string email, password, phone, name;
            int age;

            while (true)
            {
                phone = helper.GetPhoneNumber();
                if (validation.CheckEmpty(phone))
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
            } while (!validation.CheckEmpty(password));

            VerifyPassword(password);

            name = GetValue("Name");

            while (true)
            {
                email = GetValue("Email");
                if (validation.IsValidEmail(email)) break;
                else Notification.Error("PLease enter a valid email.");
            }

            Console.WriteLine("Age: ");
            age = helper.GetInteger();

            bool customerCreated = CreateCustomer(name, password, email, phone, age);
            if (customerCreated)
            {
                CustomersController customersController = new CustomersController();
                customersController.FillTable();
                DataRow user = customersController.GetUserByQuery("Phone = " + phone);
                long userID = (long)user["ID"];
                Notification.Success("Signup Successful");

                AccountsController accountsController = new AccountsController();
                Account account = accountsController.CreateCurrentAccount(userID);
                if(account != null)
                {
                    Notification.Success("Account created successfully");

                    TransactionController transactionController = new TransactionController();
                    decimal amount = helper.GetAmount(account as CurrentAccount);
                    transactionController.Deposit(amount, account);
                }
            }
        }

        public bool CheckUniquePhoneNumber(string phoneNumber)
        {
            CustomersController customersController = new CustomersController();
            return customersController.GetUserByQuery("Phone = " + phoneNumber) == null ? true : false;
        }

        private bool CreateCustomer(string name, string password, string email, string phone, int age)
        {
            CustomersController customersController = new CustomersController();
            bool customerAdded = customersController.CreateCustomer(name, password, email, phone, age);
            return customerAdded;
        }

        public string GetValue(string label)
        {
            while (true)
            {
                Console.WriteLine(label + ": ");
                Validation validation = new Validation();
                string value = Console.ReadLine().Trim();
                if (validation.CheckEmpty(value)) return value;
                else continue;
            }
        }

        private void VerifyPassword(string password)
        {
            Validation validation = new Validation();
            Helper helper = new Helper();   
            while (true)
            {
                string rePassword = helper.GetPassword("Re-enter password");
                if (validation.ValidatePassword(password, rePassword))
                    break;
                else
                    Notification.Error("Password not matching, Enter again");
            }
        }
    }
}
