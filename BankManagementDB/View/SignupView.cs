using BankManagement.Controller;
using BankManagement.Enums;
using BankManagement.Models;
using BankManagement.Utility;
using BankManagementDB.db;
using System;
using System.Security.Policy;
using System.Xml.Linq;

namespace BankManagement.View
{
    public class SignupView
    {
        public void Signup()
        {
            Validation validation= new Validation();
            Helper helper = new Helper();
            string email, password, phone, name;
            while (true)
            {
                phone = helper.GetPhoneNumber();
                if (validation.CheckEmpty(phone))
                {
                    if (CheckUniquePhoneNumber(phone))
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Phone Number Already Registered");
                        return;
                    }
                }
            }

            do
            {
                password = helper.GetPassword();
            } while (!validation.CheckEmpty(password));

            VerifyPassword(password);

            name = GetValue("Name");

            email = GetValue("Email");

            if(CreateCustomer(name, email, phone, password))
                CreateAccount();
            
        }

        public bool CheckUniquePhoneNumber(string phoneNumber)
        {
            CustomersController customersController = new CustomersController();
            return customersController.GetUserByPhoneNumber(phoneNumber) == null ? true : false;
        }

        private bool CreateCustomer(string name, string password, string email, string phone)
        {
            CustomersController customersController = new CustomersController();
            bool customerAdded = customersController.CreateCustomer(name, password, email, phone);
            if (customerAdded) Console.WriteLine("SignUp successful");
            else Console.WriteLine("Retry again..");
            return customerAdded;
        }

        private bool CreateAccount()
        {
            AccountsController accountController = new AccountsController();
            Account account = AccountFactory.CreateAccountByType(AccountTypes.CURRENT);
            bool isInserted = accountController.InsertAccountToDB(account);
            if (account != null) Console.WriteLine("Account created successfully");
            else Console.WriteLine("Error while creating account");
            return isInserted;
        }

        public string GetValue(string label)
        {
            Console.WriteLine(label +": ");
            Validation validation = new Validation();

            string value = Console.ReadLine().Trim();
            if (validation.CheckEmpty(value)) return value;
            else
            {
                GetValue(label);
                return null;
            }
        }

        private string GetRePassword()
        {
            Console.WriteLine("Re-enter password: ");
            return Console.ReadLine().Trim();
        }


        private void VerifyPassword(string password)
        {
            Validation validation = new Validation();
            while (true)
            {
                string rePassword = GetRePassword();
                if (validation.ValidatePassword(password, rePassword))
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Password not matching, Enter again");
                }
            }
        }
    }
}
