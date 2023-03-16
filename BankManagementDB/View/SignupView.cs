using System;
using BankManagementDB.Model;
using BankManagementDB.Models;
using BankManagementDB.Utility;
using BankManagementDB.EnumerationType;
using BankManagementDB.Interface;
using BankManagementDB.Config;
using Microsoft.Extensions.DependencyInjection;
using BankManagementDB.Properties;
using BankManagementDB.DataManager;
using BankManagementDB.Controller;

namespace BankManagementDB.View;

public class SignupView
{

    public void Signup()
    {
        HelperView helper = new HelperView();
        string email, password, phone, name;
        int age;
        Notification.Info(DependencyContainer.GetResource("PressBackButtonInfo"));
        phone = GetPhoneNumber();
        if (phone != null)
        {
            name = GetValue(DependencyContainer.GetResource("EnterName"));
            if (name != null)
            {
                email = GetEmail();
                if (email != null)
                {
                    age = GetAge();
                    if (age > 0)
                    {
                         Console.Write(DependencyContainer.GetResource("EnterPassword"));
                         password = helper.GetPassword();
                         if (password != null)
                         {
                             bool isVerified = VerifyPassword(password);
                             if (isVerified)
                             {
                                 CreateCustomer(name, password, email, phone, age);
                                 IGetCustomerDataManager GetCustomerDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IGetCustomerDataManager>();

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
            Console.Write(DependencyContainer.GetResource("EnterPhoneNumber"));
            phoneNumber = Console.ReadLine()?.Trim();

            if (phoneNumber == DependencyContainer.GetResource("BackButton"))
                break;

            if (!string.IsNullOrEmpty(phoneNumber))

                if (validation.IsPhoneNumber(phoneNumber))
                    if (CheckUniquePhoneNumber(phoneNumber))
                        return phoneNumber;
                    else
                        Notification.Error(DependencyContainer.GetResource("PhoneAlreadyRegistered"));

                else
                    Notification.Error(DependencyContainer.GetResource("InvalidPhone"));
            else
                Notification.Error(DependencyContainer.GetResource("EmptyFieldError"));

        }
        return null;
    }

    public int GetAge()
    {
        try
        {
            while (true)
            {
                Console.Write(DependencyContainer.GetResource("EnterAge"));
                string input = Console.ReadLine()?.Trim();
                if (int.TryParse(input, out int number))
                    if (number < 19)
                        Notification.Error(DependencyContainer.GetResource("AgeGreaterThan18"));
                    else
                        return number;
                else
                    Notification.Error(DependencyContainer.GetResource("InvalidInteger"));
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
            IAccountFactory AccountFactory  = DependencyContainer.ServiceProvider.GetRequiredService<IAccountFactory>();
            Account account = AccountFactory.GetAccountByType(AccountType.CURRENT);

            account.UserID = signedUpCustomer.ID;
            account.Balance = 0;

            AccountView accountView = new AccountView();

            IInsertAccountDataManager InsertAccountDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IInsertAccountDataManager>();
            if (InsertAccountDataManager.InsertAccount(account))
            {
                AccountView.SelectedAccount = account;
                decimal amount = GetAmount(account.MinimumBalance);
                if (amount > 0)
                {
                    if (accountView.Deposit(amount, ModeOfPayment.CASH, null))
                    {
                        Notification.Success(DependencyContainer.GetResource("AccountInsertSuccess"));

                        Console.WriteLine(DependencyContainer.GetResource("IsDebitCardRequired"));
                        while (true)
                        {
                            string input = Console.ReadLine()?.Trim();
                            if(CreateCard(input, account, signedUpCustomer))
                                break;
                        }
                    }
                    else
                        Notification.Error(DependencyContainer.GetResource("DepositFailure"));
                }
            }
            else
                 Notification.Error(DependencyContainer.GetResource("AccountInsertFailure"));
                
        }
        catch (Exception e)
        {
            Notification.Error(e.ToString());
        }
    }

    public decimal GetAmount(decimal minimumBalance) {

        HelperView helperView = new HelperView();
        decimal amount;
        while (true)
        {
            amount = helperView.GetAmount();
            if (amount < minimumBalance)
                Notification.Error(string.Format(DependencyContainer.GetResource("InitialDepositAmountWarning"), minimumBalance));
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
                    IGetCardDataManager GetCardDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IGetCardDataManager>();
                    GetCardDataManager.GetAllCards(signedUpCustomer.ID);
                    IInsertCardDataManager InsertCardDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IInsertCardDataManager>();

                if (InsertCardDataManager.InsertCard(card))
                {
                    Notification.Success(DependencyContainer.GetResource("CardInsertSuccess"));
                    Console.WriteLine(card);
                    Console.WriteLine(string.Format(DependencyContainer.GetResource("PinDisplay"), card.Pin));
                    Console.WriteLine();
                }
                return true;
                case "n":
                    return true;
                default:
                    Notification.Error(DependencyContainer.GetResource("InvalidInput"));
                    return false;
            }
    }

    public bool CheckUniquePhoneNumber(string phoneNumber)
    {
        IGetCustomerDataManager GetCustomerDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IGetCustomerDataManager>();
        Customer customer = GetCustomerDataManager.GetCustomer(phoneNumber);
        return customer == null;
    }

    private void CreateCustomer(string name, string password, string email, string phone, int age)
    {

        Customer customer = new Customer()
        {
            ID = Guid.NewGuid().ToString(),
            Name = name,
            Age = age,
            Phone = phone,
            Email = email,
            LastLoggedOn = DateTime.Now,
            CreatedOn = DateTime.Now
        };

        if (InsertCredentials(customer, password))
        {
            IInsertCustomerDataManager InsertCustomerDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IInsertCustomerDataManager>();
            bool customerAdded = InsertCustomerDataManager.InsertCustomer(customer, password);
            if (customerAdded) Notification.Success(DependencyContainer.GetResource("SignupSuccess"));
        }
        else Notification.Error(DependencyContainer.GetResource("SignupFailure"));
    }

    public bool InsertCredentials(Customer customer, string password)
    {
        string salt = AuthServices.GenerateSalt(70);
        string hashedPassword = AuthServices.HashPassword(password, salt);

        CustomerCredentials customerCredentials = new CustomerCredentials();
        customerCredentials.Password = hashedPassword;
        customerCredentials.ID = customer.ID;
        customerCredentials.Salt = salt;
        IInsertCredentialsDataManager InsertCredentialsDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IInsertCredentialsDataManager>();
        return InsertCredentialsDataManager.InsertCredentials(customerCredentials);
    }


    private string GetValue(string label)
    {
        while (true)
        {
            Console.Write(label);
            string value = Console.ReadLine()?.Trim();
            if (value == DependencyContainer.GetResource("BackButton")) return null;
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
            Console.Write(DependencyContainer.GetResource("EnterEmail"));
            email = Console.ReadLine()?.Trim();
            if (email == DependencyContainer.GetResource("BackButton"))
                email = null;
            else if (string.IsNullOrEmpty(email))
                Notification.Error(DependencyContainer.GetResource("EmptyFieldError"));
            else
            {
                if (validation.IsValidEmail(email)) break;
                else Notification.Error(DependencyContainer.GetResource("InvalidEmail"));
            }
        }
        return email;
    }


    private bool VerifyPassword(string password)
    {
        Validation validation = new Validation();
        HelperView helper = new HelperView();
        while (true)
        {
            Console.Write(DependencyContainer.GetResource("EnterRePassword"));
            string rePassword = helper.GetPassword();

            if (rePassword == null)
                break;
            if (validation.ValidatePassword(password, rePassword) && rePassword != null)
                return true;
            else
                Notification.Error(DependencyContainer.GetResource("PasswordMismatch"));
        }
        return false;
    }
}
