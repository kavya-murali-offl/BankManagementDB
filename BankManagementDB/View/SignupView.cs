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
using System.Reflection.Emit;

namespace BankManagementDB.View;

public class SignupView
{
    public SignupView() { 
        
    }

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
                         password = GetPassword(DependencyContainer.GetResource("EnterPassword"));
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
        Validator validation = new Validator();
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

            AccountView accountView = new AccountView();
            Account account = accountView.CreateAccount(AccountType.CURRENT);

            IInsertAccountDataManager InsertAccountDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IInsertAccountDataManager>();

            if (InsertAccountDataManager.InsertAccount(account))
            {
                Notification.Success(DependencyContainer.GetResource("AccountInsertSuccess"));

                Notification.Info(DependencyContainer.GetResource("MakeInitialDeposit"));
                AccountView.SelectedAccount = account;
                decimal amount = GetAmount(account.MinimumBalance);
                if (amount > 0)
                {
                    if (accountView.Deposit(amount, ModeOfPayment.CASH, null))
                    {
                        Notification.Info(DependencyContainer.GetResource("IsDebitCardRequired"));
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
                Notification.Error(Formatter.FormatString(DependencyContainer.GetResource("InitialDepositAmountWarning"), minimumBalance));
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
                    Console.WriteLine(Formatter.FormatString(DependencyContainer.GetResource("PinDisplay"), card.Pin));
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

    private string GetPassword(string label)
    {
        while (true)
        {
            Console.Write(label);
            string value = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(value) && Validator.IsValidPassword(value)) return value;
            else if (value == DependencyContainer.GetResource("BackButton")) return null;
            else Notification.Error(DependencyContainer.GetResource("InvalidPassword"));
        }
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
        Validator validation = new Validator();
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
        while (true)
        {
            string rePassword = GetPassword(DependencyContainer.GetResource("EnterRePassword"));

            if (rePassword == null)
                break;
            if (password.Equals(rePassword) && rePassword != null)
                return true;
            else
                Notification.Error(DependencyContainer.GetResource("PasswordMismatch"));
        }
        return false;
    }
}
