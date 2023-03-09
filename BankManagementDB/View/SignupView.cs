using System;
using BankManagementDB.Model;
using BankManagementDB.Models;
using BankManagementDB.Utility;
using BankManagementDB.EnumerationType;
using BankManagementDB.Interface;
using BankManagementDB.Config;
using Microsoft.Extensions.DependencyInjection;
using BankManagementDB.Properties;

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
                name = GetValue(Resources.EnterName);
                if (name != null)
                {
                    email = GetEmail();
                    if (email != null)
                    {
                        age = GetAge();
                        if (age > 0)
                        {
                                Console.WriteLine(Resources.EnterPassword);
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
                Console.Write(Resources.EnterPhoneNumber);
                phoneNumber = Console.ReadLine().Trim();

                if (phoneNumber == Resources.BackButton)
                    break;

                if (!string.IsNullOrEmpty(phoneNumber))

                    if (validation.IsPhoneNumber(phoneNumber))
                        if (CheckUniquePhoneNumber(phoneNumber))
                            return phoneNumber;
                        else
                            Notification.Error(Resources.PhoneAlreadyRegistered);

                    else
                        Notification.Error(Resources.InvalidPhone);
                else
                    Notification.Error(Resources.EmptyFieldError);

            }
            return null;
        }

        public int GetAge()
        {
            try
            {
                while (true)
                {
                    Console.Write(Resources.EnterAge);
                    string input = Console.ReadLine().Trim();
                    if (int.TryParse(input, out int number))
                        if (number < 19)
                            Notification.Error(Resources.AgeGreaterThan18);
                        else
                            return number;
                    else
                        Notification.Error(Resources.InvalidInteger);
                }
            }
            catch (Exception error)
            {
                Notification.Error(error.ToString());
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

                TransactionView transactionView = new TransactionView();

                if (InsertAccountDataManager.InsertAccount(account))
                {
                    decimal amount = GetAmount(account.MinimumBalance);
                    if (amount > 0)
                    {
                        if (transactionView.Deposit(account, amount, ModeOfPayment.CASH, null))
                        {
                            Notification.Success(Resources.AccountInsertSuccess);

                            Console.WriteLine(Resources.IsDebitCardRequired);
                            while (true)
                            {
                                string input = Console.ReadLine().Trim();
                                if(CreateCard(input, account, signedUpCustomer))
                                    break;
                            }
                        }
                        else
                            Notification.Error(Resources.DepositFailure);
                    }
                }
                else
                     Notification.Error(Resources.AccountInsertFailure);
                    
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
                    Notification.Error(string.Format(Resources.InitialDepositAmountWarning, minimumBalance));
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
                            Notification.Success(Resources.CardInsertSuccess);
                            Console.WriteLine(card);
                            Console.WriteLine(string.Format(Resources.PinDisplay, card.Pin));
                            Console.WriteLine();
                        }
                        return true;
                    case "n":
                        return true;
                    default:
                        Notification.Error(Resources.InvalidInput);
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
                if (customerAdded) Notification.Success(Resources.SignupSuccess);
            }
            else Notification.Error(Resources.SignupFailure);
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
                Console.Write(label);
                string value = Console.ReadLine().Trim();
                if (value == Resources.BackButton) return null;
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
                Console.Write(Resources.EnterEmail);
                email = Console.ReadLine().Trim();
                if (email == Resources.BackButton)
                    email = null;
                else if (string.IsNullOrEmpty(email))
                    Notification.Error(Resources.EmptyFieldError);
                else
                {
                    if (validation.IsValidEmail(email)) break;
                    else Notification.Error(Resources.InvalidEmail);
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
                Console.WriteLine(Resources.EnterRePassword);
                string rePassword = helper.GetPassword();

                if (rePassword == null)
                    break;
                if (validation.ValidatePassword(password, rePassword) && rePassword != null)
                    return true;
                else
                    Notification.Error(Resources.PasswordMismatch);
            }
            return false;
        }
    }
}
