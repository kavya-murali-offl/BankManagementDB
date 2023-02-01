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

            bool customerCreated = CreateCustomer(name, password, email, phone, age);
            if (customerCreated)
            {
                CustomersController customersController = new CustomersController();
                customersController.FillTable();
                DataRow user = customersController.GetUserByQuery("Phone = " + phone);
                long userID = (long)user["ID"];
                Notification.Success("Signup Successful\n");

                AccountsController accountsController = new AccountsController();
                Account account = accountsController.CreateCurrentAccount(userID);

                if(account != null)
                {

                    TransactionController transactionController = new TransactionController();
                    decimal amount = helper.GetAmount(account as CurrentAccount);
                    transactionController.Deposit(amount, account);

                    Notification.Success("Account created successfully\n");
                }
            }
        }

        public bool CheckUniquePhoneNumber(string phoneNumber)
        {
            CustomersController customersController = new CustomersController();
            return customersController.GetUserByQuery("Phone = " + phoneNumber).Equals(null) ? true : false;
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
                string rePassword = helper.GetPassword("Re-enter password");
                if (validation.ValidatePassword(password, rePassword))
                    break;
                else
                    Notification.Error("Password not matching, Enter again");
            }
        }
    }
}
