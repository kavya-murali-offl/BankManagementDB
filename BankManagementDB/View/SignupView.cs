using System;
using BankManagement.Controller;
using BankManagement.Enums;
using BankManagement.Model;
using BankManagement.Models;
using BankManagement.Utility;
using BankManagementCipher.Model;
using BankManagementDB.Controller;
using BankManagementDB.db;
using BankManagementDB.Enums;
using BankManagementDB.Interface;
using BankManagementDB.Model;
using BankManagementDB.View;

namespace BankManagement.View
{
    public class SignupView
    {
        public ICustomerServices CustomersController { get; set; }

        public void Signup(ICustomerServices customerController)
        {

            CustomersController = customerController;
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

          

            name = GetValue("Name");

            while (true)
            {
                email = GetValue("Email");
                if (validation.IsValidEmail(email)) break;
                else Notification.Error("Please enter a valid email.");
            }

            age = helper.GetInteger("Age: ");

            do
            {
                password = helper.GetPassword("Enter password: ");
            } while (!validation.CheckNotEmpty(password));

            VerifyPassword(password);

            CreateCustomer(name, password, email, phone, age);
            Customer signedUpCustomer = CustomersController.GetCustomer(phone);
            CreateAccountAndDeposit(signedUpCustomer);
        }

        public void CreateAccountAndDeposit(Customer signedUpCustomer)
        {

            Account account = AccountFactory.GetAccountByType(AccountTypes.CURRENT);
            account.UserID = signedUpCustomer.ID;
            account.Balance = 0;
            AccountsController accountsController = new AccountsController(new AccountOperations());
            CardController cardController = new CardController();

            if (accountsController.InsertAccountToDB(account))
            {
                ITransactionServices transactionServices = new TransactionController(new TransactionOperations());
                IATMTransactionServices ATMTransactionController = new ATMTransactionsController(transactionServices, accountsController);

                Helper helper = new Helper();
                decimal amount = helper.GetAmount(account as CurrentAccount);
                ATMTransactionController.Deposit(amount, account, ModeOfPayment.CASH);

                Notification.Success("Account created successfully\n");

                Card card = CardFactory.GetCardByType(CardType.DEBIT);
                card = cardController.CreateCard(card, account);
                card.Balance = account.Balance;
                cardController.GetAllCards(account.ID);
                if (cardController.InsertCard(card)) {
                    Notification.Success("Debit card created successfully. Save Card Number and Pin for later use.\n");
                    Console.WriteLine(card);
                    Console.WriteLine($" PIN: {card.Pin}");
                    Console.WriteLine();
                }
            }

        }

        public bool CheckUniquePhoneNumber(string phoneNumber)
        {
            Customer customer = CustomersController.GetCustomer(phoneNumber);
            return customer == null ? true : false;
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

            bool customerAdded = CustomersController.InsertCustomer(customer, password);

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
                if (validation.CheckNotEmpty(value)) return value;
            }
        }

        private void VerifyPassword(string password)
        {
            Validation validation = new Validation();
            Helper helper = new Helper();
            while (true)
            {
                string rePassword = helper.GetPassword("Re-enter password: ");
                if (validation.ValidatePassword(password, rePassword) && rePassword != null)
                    break;
                else
                    Notification.Error("Password not matching, Enter again");
            }
        }
    }
}
