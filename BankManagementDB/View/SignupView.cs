using System;
using BankManagementDB.Model;
using BankManagementDB.Models;
using BankManagementDB.Utility;
using BankManagementDB.EnumerationType;
using BankManagementDB.Interface;
using BankManagementDB.Config;
using Microsoft.Extensions.DependencyInjection;
using BankManagementDB.DataManager;

namespace BankManagementDB.View
{
    public class SignupView
    {
        public SignupView()
        {
            AccountFactory = DependencyContainer.ServiceProvider.GetRequiredService<IAccountFactory>();
            InsertAccountDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IInsertAccountDataManager>();
            InsertCustomerDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IInsertCustomerDataManager>();
            InsertCredentialsDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IInsertCredentialsDataManager>();
            InsertCardDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IInsertCardDataManager>();
            GetCustomerDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IGetCustomerDataManager>();
            GetCardDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IGetCardDataManager>();
        }

        public IGetCardDataManager GetCardDataManager { get; set; }

        public IInsertCardDataManager InsertCardDataManager { get; set; }

        public IInsertCustomerDataManager InsertCustomerDataManager { get; set; }

        public IInsertCredentialsDataManager InsertCredentialsDataManager { get; set; }

        public IGetCustomerDataManager GetCustomerDataManager { get; set; }

        public IInsertAccountDataManager InsertAccountDataManager { get; set; }

        public IAccountFactory AccountFactory { get; set; }

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
                        age = GetAge();
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
                                        Customer signedUpCustomer = GetCustomerDataManager.GetCustomer(phone);
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

        public int GetAge()
        {
            try
            {
                while (true)
                {
                    Console.Write("Age: ");
                    string input = Console.ReadLine().Trim();
                    if (int.TryParse(input, out int number))
                        if (number < 19)
                            Notification.Error("Age should be greater than 18");
                        else
                            return number;
                    else
                        Notification.Error("Enter a valid number.");
                }
            }
            catch (Exception error)
            {
                Notification.Error("Enter a valid number. Try Again!");
            }
            return 0;
        }

        public void CreateAccountAndDeposit(Customer signedUpCustomer)
        {
            try
            {
                Account account = AccountFactory.GetAccountByType(AccountType.CURRENT);
                account.UserID = signedUpCustomer.ID;
                account.Balance = 0;

                if (InsertAccountDataManager.InsertAccount(account))
                {
                    decimal amount = GetAmount(account.MinimumBalance);
                    if (amount > 0)
                    {
                        TransactionView transactionView = new TransactionView();
                        if (transactionView.Deposit(account, amount, ModeOfPayment.CASH, null))
                        {
                            Notification.Success("Account created successfully\n");

                            Console.WriteLine("Do you need Debit Card for this account? Press y to accept or n to decline");
                            while (true)
                            {
                                string input = Console.ReadLine().Trim();
                                if(CreateCard(input, account, signedUpCustomer))
                                    break;
                            }
                        }
                        else
                            Notification.Error("Deposit unsuccessful");
                    }
                }
            }
            catch (Exception e)
            {
                Notification.Error(e.ToString());
            }
        }

        public decimal GetAmount(decimal minimumBalance) {
            TransactionView transactionsView = new TransactionView();
            decimal amount;
            while (true)
            {
                amount = transactionsView.GetAmount();
                if (amount < minimumBalance)
                    Notification.Error($"Initial Amount should be greater than Minimum Balance Rs. {minimumBalance}");
                else
                    break;
            }
            return amount;  
        }

        public bool CreateCard(string input, Account account, Customer signedUpCustomer)
        {
            CardView cardView= new CardView();
                switch (input.ToLower())
                {
                    case "y":
                        Card card = cardView.CreateCard(CardType.DEBIT, account.ID, signedUpCustomer.ID);
                        GetCardDataManager.GetAllCards(signedUpCustomer.ID);
                        if (InsertCardDataManager.InsertCard(card))
                        {
                            Notification.Success("Debit card created successfully. Save Card Number and Pin for later use.\n");
                            Console.WriteLine(card);
                            Console.WriteLine($" PIN: {card.Pin}");
                            Console.WriteLine();
                        }
                        return true;
                    case "n":
                        return true;
                    default:
                        Notification.Error("Please enter a valid input");
                        return false;
                }
        }

        public bool CheckUniquePhoneNumber(string phoneNumber)
        {
            Customer customer = GetCustomerDataManager.GetCustomer(phoneNumber);
            return customer == null;
        }

        private void CreateCustomer(string name, string password, string email, string phone, int age)
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

            if (InsertCredentials(customer, password))
            {
                bool customerAdded = InsertCustomerDataManager.InsertCustomer(customer, password);
                if (customerAdded) Notification.Success("\nSignup successful");
            }
            else Notification.Error("Error in signing up");
        }
        public bool InsertCredentials(Customer customer, string password)
        {
            string salt = AuthServices.GenerateSalt(70);
            string hashedPassword = AuthServices.HashPassword(password, salt);

            CustomerCredentials customerCredentials = new CustomerCredentials();
            customerCredentials.Password = hashedPassword;
            customerCredentials.ID = customer.ID;
            customerCredentials.Salt = salt;
            return InsertCredentialsDataManager.InsertCredentials(customerCredentials);
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
