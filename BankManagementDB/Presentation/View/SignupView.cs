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
namespace BankManagementDB.View;

public class SignupView
{
    public void Signup()
    {
        string email, password, phone, name;
        int age;
        Notification.Info(Resources.PressBackButtonInfo);
        phone = GetPhoneNumber();
        if (phone != null)
        {
            name = GetValue(Resources.Name);
            if (name != null)
            {
                email = GetEmail();
                if (email != null)
                {
                    age = GetAge();
                    if (Validator.IsValidAge(age))
                    {
                        password = GetPassword(Resources.Password);
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
            Console.Write(Resources.PhoneNumber + ": ");
            phoneNumber = Console.ReadLine()?.Trim();

            if (phoneNumber == Resources.BackButton)
            {
                break;
            }

            if (!string.IsNullOrEmpty(phoneNumber))

            {
                if (validation.IsPhoneNumber(phoneNumber))
                {
                    if (CheckUniquePhoneNumber(phoneNumber))
                    {
                        return phoneNumber;
                    }
                    else
                    {
                        Notification.Error(Resources.PhoneAlreadyRegistered);
                    }
                }
                else
                {
                    Notification.Error(Resources.InvalidPhone);
                }
            }
            else
            {
                Notification.Error(Resources.EmptyFieldError);
            }

        }
        return null;
    }

    public int GetAge()
    {
        try
        {
            while (true)
            {
                Console.Write(Resources.Age + ": ");
                string input = Console.ReadLine()?.Trim();
                if (int.TryParse(input, out int number))
                {
                    if (number < 19)
                    {
                        Notification.Error(Resources.AgeGreaterThan18);
                    }
                    else
                    {
                        return number;
                    }
                }
                else
                {
                    Notification.Error(Resources.InvalidInteger);
                }
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
                Notification.Success(Resources.AccountInsertSuccess);

                Notification.Info(Resources.InterestDepositInitiated);
                AccountView.SelectedAccount = account;
                decimal amount = GetAmount(account.MinimumBalance);
                if (amount > 0)
                {
                    if (accountView.Deposit(amount, ModeOfPayment.CASH, null))
                    {
                        Notification.Info(Resources.IsDebitCardRequired);
                        while (true)
                        {
                            string input = Console.ReadLine()?.Trim();
                            if (CreateCard(input, account, signedUpCustomer))
                            { break; }
                        }
                    }
                    else
                    {
                        Notification.Error(Resources.DepositFailure);
                    }
                }
            }
            else
            {
                Notification.Error(Resources.AccountInsertFailure);
            }

        }
        catch (Exception e)
        {
            Notification.Error(e.ToString());
        }
    }

    public decimal GetAmount(decimal minimumBalance)
    {

        HelperView helperView = new HelperView();
        decimal amount;
        while (true)
        {
            amount = helperView.GetAmount();
            if (amount < minimumBalance)
            {
                Notification.Error(Formatter.FormatString(Resources.InitialDepositAmountWarning, minimumBalance));
            }
            else
            {
                break;
            }
        }
        return amount;
    }

    public bool CreateCard(string input, Account account, Customer signedUpCustomer)
    {
        CardView cardView = new CardView();
        switch (input.ToLower())
        {
            case "y":
                Card card = cardView.CreateCard(CardType.DEBIT, account.ID, signedUpCustomer.ID);
                IGetCardDataManager GetCardDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IGetCardDataManager>();
                GetCardDataManager.GetAllCards(signedUpCustomer.ID);
                IInsertCardDataManager InsertCardDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IInsertCardDataManager>();

                if (InsertCardDataManager.InsertCard(card))
                {
                    Notification.Success(Resources.CardInsertSuccess);
                    Console.WriteLine(card);
                    Console.WriteLine(Formatter.FormatString(Resources.PinDisplay, card.Pin));
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

        string salt = AuthServices.GenerateSalt(70);
        string hashedPassword = AuthServices.HashPassword(password, salt);

        CustomerCredentials customerCredentials = new CustomerCredentials()
        {
            Password = hashedPassword,
            ID = customer.ID,
            Salt = salt
        };

            IInsertCustomerDataManager InsertCustomerDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IInsertCustomerDataManager>();
            bool customerAdded = InsertCustomerDataManager.InsertCustomer(customer, customerCredentials);
            if (customerAdded)
            {
                Notification.Success(Resources.SignupSuccess);
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
        IInsertCredentialsDataManager InsertCredentialsDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IInsertCredentialsDataManager>();
        return InsertCredentialsDataManager.InsertCredentials(customerCredentials);
    }

    private string GetPassword(string label)
    {
        while (true)
        {
            Console.Write(label + ": ");
            string value = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(value) && Validator.IsValidPassword(value)) { return value; }
            else if (value == Resources.BackButton) { return null; }
            else
            {
                Notification.Error(Resources.InvalidPassword);
            }
        }
    }

    private string GetValue(string label)
    {
        while (true)
        {
            Console.Write(label + ": " );
            string value = Console.ReadLine()?.Trim();
            if (value == Resources.BackButton) { return null; }
            else if (!string.IsNullOrEmpty(value)) { return value; }
            else { return null; }
        }
    }

    private string GetEmail()
    {
        string email;
        while (true)
        {
            Console.Write(Resources.Email + ": ");
            email = Console.ReadLine()?.Trim();
            if (email == Resources.BackButton)
            { email = null; }
            else if (string.IsNullOrEmpty(email))
            { Notification.Error(Resources.EmptyFieldError); }
            else
            {
                if (Validator.IsValidEmail(email))
                {
                    break;
                }
                else
                {
                    Notification.Error(Resources.InvalidEmail);
                }
            }
        }
        return email;
    }


    private bool VerifyPassword(string password)
    {
        while (true)
        {
            string rePassword = GetPassword(Resources.RePassword);

            if (rePassword == null)
            {
                break;
            }
            if (password.Equals(rePassword) && rePassword != null)
            {
                return true;
            }
            else
            {
                Notification.Error(Resources.PasswordMismatch);
            }
        }
        return false;
    }
}
