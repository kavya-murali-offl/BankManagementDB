using BankManagement.Controller;
using BankManagement.Enums;
using BankManagement.Model;
using BankManagement.Models;
using BankManagement.Utility;
using System;
using System.Data;
using System.Runtime.Remoting.Messaging;
using System.Security.Principal;

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

            bool customerCreated = CreateCustomer(name, password, email, phone);
            if (customerCreated)
            {
            CustomersController customersController = new CustomersController();
                customersController.FillTable();
                DataRow user = customersController.GetUserByPhoneNumber(phone);
                long userID = (long)user["ID"];
                CreateAccount(userID);
            }

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

        private void CreateAccount(long userID)
        {
            AccountsController accountController = new AccountsController();
            Account account = AccountFactory.GetAccountByType(AccountTypes.CURRENT);
            account.Balance = GetAmount();
            account.UserID = userID;
            accountController.InsertAccountToDB(account);
            
        }

        public decimal GetAmount()
        {
            Helper helper = new Helper();
            decimal amount = helper.GetAmount();
            CurrentAccount currentAccount = new CurrentAccount();
            if(amount < currentAccount.MinimumBalance)
            {
                Console.WriteLine("Amount should be greater than minimum balance");
                GetAmount();
            }
            return amount;
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


        private void VerifyPassword(string password)
        {
            Validation validation = new Validation();
            while (true)
            {
                string rePassword = GetValue("Re-enter password");
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
