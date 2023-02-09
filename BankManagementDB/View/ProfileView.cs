using System;
using System.Collections.Generic;
using System.Linq;
using BankManagement.Controller;
using BankManagement.Models;
using BankManagementDB.Interface;
using BankManagementDB.Utility;
using BankManagementDB.View;

namespace BankManagement.View
{
    public enum ProfileServiceCases
    {
        VIEW_PROFILE,
        EDIT_PROFILE,
        EXIT
    }

    public class ProfileView
    {
        public void ViewProfileServices(ProfileController profileController)
        {
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

                    if (entryOption != 0 && entryOption <= Enum.GetNames(typeof(ProfileServiceCases)).Count())
                    {
                        ProfileServiceCases cases = (ProfileServiceCases)entryOption - 1;

                        if (ProfileOperations(cases, profileController))
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

        public bool ProfileOperations(ProfileServiceCases cases, ProfileController profileController)
        {
            switch (cases)
            {
                case ProfileServiceCases.VIEW_PROFILE:
                    ViewProfileDetails(profileController);
                    return false;

                case ProfileServiceCases.EDIT_PROFILE:
                    EditProfile(profileController);
                    return false;

                case ProfileServiceCases.EXIT:
                    return true;

                default:
                    Notification.Error("Enter a valid option.\n");
                    return false;
            }
        }

        public void ViewProfileDetails(ProfileController profileController)
        {
            Notification.Info("\n ================== PROFILE ===================\n");
            Notification.Info($"Name: {profileController.Customer.Name}");
            Notification.Info("Age: " + profileController.Customer.Age);
            Notification.Info("Phone: " + profileController.Customer.Phone);
            Notification.Info("Email: " + profileController.Customer.Email);
            Notification.Info("No. of Accounts: " + AccountsController.AccountsList.Count);
            Notification.Info("\n ==============================================\n");
        }

        public void EditProfile(ProfileController profile)
        {
            Customer customer = (Customer)profile.Customer.Clone();

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
            
            if (customer.Name != profile.Customer.Name || customer.Age != profile.Customer.Age)
                UpdateProfile(customer, profile);
        }


        public void UpdateProfile(Customer updatedCustomer, ProfileController profile)
        {
            CustomersController customersController = new CustomersController();
            Customer customer = customersController.UpdateCustomer(updatedCustomer);

            if (customer != null)
            {
                Notification.Success("Profile Updated Successfully");
                profile.Customer = customer;
            }
        }
    }
}
