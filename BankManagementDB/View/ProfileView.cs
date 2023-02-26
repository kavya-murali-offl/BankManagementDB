using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using BankManagementDB.Controller;
using BankManagementDB.Models;
using BankManagementDB.Config;
using BankManagementDB.EnumerationType;
using BankManagementDB.Interface;
using BankManagementDB.Utility;
using BankManagementDB.View;

namespace BankManagementDB.View
{ 

    public class ProfileView
    {

        public ProfileView(ICustomerController customerController) {
            CustomerController = customerController;
        }
        public ICustomerController CustomerController { get; set; }

        public void ViewProfileServices()
        {
            Customer currentUser = CustomerController.GetCurrentUser();
            while (true)
            {
                try
                {
                    Console.WriteLine("\n");
                    for (int i = 0; i < Enum.GetNames(typeof(ProfileServiceCases)).Length; i++)
                    {
                        ProfileServiceCases cases = (ProfileServiceCases)i;
                        Console.WriteLine($"{i + 1}. {cases.ToString().Replace("_", " ")}");
                    }

                    Console.Write("\nEnter your choice: ");

                    string option = Console.ReadLine().Trim();
                    int entryOption = int.Parse(option);
                    if (entryOption == 0)
                        break;
                    else if (entryOption <= Enum.GetNames(typeof(ProfileServiceCases)).Count())
                    {
                        ProfileServiceCases cases = (ProfileServiceCases)entryOption - 1;

                        if (ProfileOperations(cases, currentUser))
                            break;
                    }
                    else
                        Notification.Error("Enter a valid input.");
                }
                catch (Exception error)
                {
                    Notification.Error(error.Message);
                }
            }
        }

        public bool ProfileOperations(ProfileServiceCases cases, Customer currentUser)
        {
            switch (cases)
            {
                case ProfileServiceCases.VIEW_PROFILE:
                    ViewProfileDetails(currentUser);
                    return false;

                case ProfileServiceCases.EDIT_PROFILE:
                    EditProfile(currentUser);
                    return false;

                case ProfileServiceCases.EXIT:
                    return true;

                default:
                    Notification.Error("Enter a valid option.\n");
                    return false;
            }
        }

        public void ViewProfileDetails(Customer currentUser)
        {
            Notification.Info("\n ================== PROFILE ===================\n");
            Notification.Info($"Name: {currentUser.Name}");
            Notification.Info("Age: " + currentUser.Age);
            Notification.Info("Phone: " + currentUser.Phone);
            Notification.Info("Email: " + currentUser.Email);
            Notification.Info("No. of Accounts: " + (AccountController.AccountsList != null ? AccountController.AccountsList.Count() : 0));
            Notification.Info("\n ==============================================\n");
        }

        public void EditProfile(Customer currentUser)
        {
            Customer customer = (Customer)currentUser.Clone();

            IDictionary<string, Action<string>> fields = new Dictionary<string, Action<string>>(){
                     { "NAME", (value) => customer.Name = value },
                     { "AGE", (value) =>
                                {
                                    int age;
                                    if (int.TryParse(value, out age))
                                        customer.Age = age;
                                    else
                                        Notification.Error("Invalid input! Age should be a number.");
                                }
                     }
            };

            while (true)
            {
                try
                {
                    Console.WriteLine($"Name: {customer.Name}" );
                    Console.WriteLine($"Age: {customer.Age}");
                    Console.WriteLine("Which field you want to edit? Press 0 to go back!");

                    string field = Console.ReadLine().Trim().ToUpper();

                    if (fields.ContainsKey(field))
                    {
                        Console.Write("Enter new value: ");
                        string value = Console.ReadLine();
                        fields[field](value);
                    }
                    else if (field == "0")
                        break;
                    else
                       Notification.Error("Invalid field!");
                }
                catch (Exception error)
                {
                    Console.WriteLine(error.Message);
                }
            }
            
            if (customer.Name != currentUser.Name || customer.Age != currentUser.Age)
                UpdateProfile(customer);
        }

            
        public void UpdateProfile(Customer updatedCustomer)
        {
            if (CustomerController.UpdateCustomer(updatedCustomer))
            {
                Notification.Success("Profile Updated Successfully");
                CustomerController.SetCurrentUser(updatedCustomer);
            }

        }

    }
}
