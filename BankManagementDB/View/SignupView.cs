using System;
using BankManagementDB.Controller;
using BankManagementDB.Model;
using BankManagementDB.Models;
using BankManagementDB.Utility;
using BankManagementDB.Config;
using BankManagementDB.Controller;
using BankManagementDB.EnumerationType;
using BankManagementDB.Interface;
using BankManagementDB.Model;
using BankManagementDB.View;
using Microsoft.Extensions.DependencyInjection;

namespace BankManagementDB.View
{
    public class SignupView
    {
        public SignupView(
            ICustomerController customerController,
            ICardController cardController,
            IAccountFactory accountFactory,
            ICardFactory cardFactory,
            IAccountController accountController,
            ITransactionProcessController transactionProcessController)
        {
            CardController = cardController;
            AccountFactory = accountFactory;
            AccountController = accountController;
            TransactionProcessController = transactionProcessController;
            CardFactory = cardFactory;
            CustomerController = customerController;
        }

        public ICardController CardController { get; set; }
        public ICustomerController CustomerController { get; set; }

        public IAccountController AccountController { get; set; }

        public IAccountFactory AccountFactory { get; set; }

        public ICardFactory CardFactory { get; set; }

        public ITransactionProcessController TransactionProcessController { get; set; }

        public void Signup()
        {

            Helper helper = new Helper();
            string email, password, phone, name;
            int age;

            phone = GetPhoneNumber();
            if (phone != null)
            {
                name = GetValue("Name");
                if (name != null)
                {
                    email = GetEmail();
                    if (email != null)
                    {
                        age = GetInteger("Age: ");
                        if (age > 0)
                        {
                            Console.WriteLine("Enter password: ");
                            password = helper.GetPassword();
                            if (password != null)
                            {
                                bool isVerified = VerifyPassword(password);

                                if (isVerified)
                                {
                                    CreateCustomer(name, password, email, phone, age);
                                    Customer signedUpCustomer = CustomerController.GetCustomer(phone);
                                    CreateAccountAndDeposit(signedUpCustomer);
                                }
                            }
                        }
                    }
                }
            }
        }


        public string GetPhoneNumber()
        {
            Validation validation = new Validation();
            string phoneNumber;

            while (true)
            {
                Console.Write("Enter Mobile Number: ");
                phoneNumber = Console.ReadLine().Trim();

                if (phoneNumber == "0")
                    break;

                if (!string.IsNullOrEmpty(phoneNumber))
                {

                    if (validation.IsPhoneNumber(phoneNumber))
                        if (CheckUniquePhoneNumber(phoneNumber))
                            return phoneNumber;
                        else
                            Notification.Error("Phone Number Already Registered");

                    else
                        Notification.Error("Enter a valid Phone Number");
                }
                else
                    Notification.Error("Phone Number should not be empty");

            }
            return null;
        }

        public int GetInteger(string message)
        {
            int number = 0;
            while (true)
            {
                try
                {
                    Console.Write(message);
                    string input = Console.ReadLine().Trim();
                    if (int.TryParse(input, out number))
                        break;
                    else
                        Notification.Error("Enter a valid number.");
                }
                catch (Exception error)
                {
                    Notification.Error("Enter a valid number. Try Again!");
                }
            }
            return number;
        }

        public void CreateAccountAndDeposit(Customer signedUpCustomer)
        {
            try
            {
                Account account = AccountFactory.GetAccountByType(AccountType.CURRENT);
                account.UserID = signedUpCustomer.ID;
                account.Balance = 0;


                if (AccountController.InsertAccount(account))
                {
                    decimal amount;
                    TransactionView transactionsView = new TransactionView();
                    while (true)
                    {
                        amount = transactionsView.GetAmount();
                        if (amount < account.MinimumBalance)
                            Notification.Error($"Initial Amount should be greater than Minimum Balance Rs. {account.MinimumBalance}");
                        else
                            break;
                    }

                    if (TransactionProcessController.Deposit(amount, account, ModeOfPayment.CASH))
                    {
                        Notification.Success("Account created successfully\n");

                        Card card = CardController.CreateCard(CardType.DEBIT, account.ID, signedUpCustomer.ID);
                        CardController.GetAllCards(account.ID);
                        if (CardController.InsertCard(card))
                        {
                            Notification.Success("Debit card created successfully. Save Card Number and Pin for later use.\n");
                            Console.WriteLine(card);
                            Console.WriteLine($" PIN: {card.Pin}");
                            Console.WriteLine();
                        }
                    }
                    else
                    {
                        Notification.Error("Deposit unsuccessful");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }

        public bool CheckUniquePhoneNumber(string phoneNumber)
        {
            Customer customer = CustomerController.GetCustomer(phoneNumber);
            return customer == null;
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

            bool customerAdded = CustomerController.InsertCustomer(customer, password);
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
                if (value == "0") return null;
                else if (!string.IsNullOrEmpty(value)) return value;
                else return null;
            }
        }

        private string GetEmail()
        {
            string email;
            Validation validation = new Validation();
            while (true)
            {
                Console.Write("Enter email: ");
                email = Console.ReadLine().Trim();
                if (email == "0")
                    email = null;
                else if (string.IsNullOrEmpty(email))
                    Notification.Error("Field should not be empty");
                else
                {
                    if (validation.IsValidEmail(email)) break;
                    else Notification.Error("Please enter a valid email.");
                }
            }
            return email;
        }


        private bool VerifyPassword(string password)
        {
            Validation validation = new Validation();
            Helper helper = new Helper();
            while (true)
            {
                Console.WriteLine("Re-enter password: ");
                string rePassword = helper.GetPassword();

                if (rePassword == null)
                    break;
                if (validation.ValidatePassword(password, rePassword) && rePassword != null)
                    return true;
                else
                    Notification.Error("Password not matching, Enter again");
            }

            return false;
        }
    }
}
