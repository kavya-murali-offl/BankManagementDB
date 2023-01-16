using BankManagement.Controller;
using BankManagement.Enums;
using BankManagement.Models;
using BankManagement.Utility;
using BankManagementDB.db;
using System;
using System.Xml;

namespace BankManagement.View
{
    public class SignupView
    {
        public void Signup()
        {
            Validation validation= new Validation();
            Helper helper = new Helper();
            string email, password, userName, phone, name;
            while (true)
            {
                userName = GetUsername();
                if (validation.CheckEmpty(userName))
                {
                    if (helper.CheckUniqueUserName(userName))
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Username Already Exists");
                        return;
                    }
                }
            }

            do
            {
                password = GetPassword();
            } while (!validation.CheckEmpty(password));

            VerifyPassword(password);
            name = GetValue("Name");
            email = GetValue("Email");
            phone = GetValue("Phone");
            CustomersController customersController = new CustomersController();
            bool customerAdded = customersController.CreateCustomer(userName, password, name, email, phone);
            if (customerAdded) Console.WriteLine("Sign up successful");
            else Console.WriteLine("Retry again..");
            AccountsController accountController = new AccountsController();
            //bool accountAdded = accountsController.CreateAccount(userName);
            Account account = AccountFactory.CreateAccountByType(AccountTypes.CURRENT);
            accountController.InsertAccountToDB(account);   
            ////bool accountCreated = accountsController.CreateAccount(userName, account);
            //customersController.AddCustomer(userName, password, name);
            //accountsController.AddAccountToUserName(userName, account);
            //Console.WriteLine("Account created Successfully.\n Please Login to continue");
        }

        private string GetUsername()
        {
            Console.WriteLine("Create a unique User Name: ");
            return Console.ReadLine();
        }

        private string GetValue(string label)
        {
            Console.WriteLine(label+": ");
            return Console.ReadLine();
        }

        private string GetRePassword()
        {
            Console.WriteLine("Re-enter password: ");
            return Console.ReadLine();
        }

        public string GetPassword()
        {
            Console.WriteLine("Enter password: ");
            return Console.ReadLine();
        }

        public void VerifyPassword(string password)
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
