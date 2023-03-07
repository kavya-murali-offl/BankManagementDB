using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using BankManagementDB.Controller;
using BankManagementDB.Models;
using BankManagementDB.Config;
using BankManagementDB.EnumerationType;
using BankManagementDB.Interface;
using BankManagementDB.DataManager;

namespace BankManagementDB.View
{ 

    public class ProfileView
    {
        public void ViewProfileServices()
        {
            
            try
            {
                while (true)
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

                        if (ProfileOperations(cases))
                            break;
                    }
                    else
                        Notification.Error("Enter a valid input.");
                }
            }
            catch (Exception error)
            {
                Notification.Error(error.Message);
            }
        }

        public bool ProfileOperations(ProfileServiceCases cases)
        {
            switch (cases)
            {
                case ProfileServiceCases.VIEW_PROFILE:
                    ViewProfileDetails();
                    return false;

                case ProfileServiceCases.EDIT_PROFILE:
                    EditProfile();
                    return false;

                case ProfileServiceCases.EXIT:
                    return true;

                default:
                    Notification.Error("Enter a valid option.\n");
                    return false;
            }
        }

        public void ViewProfileDetails()
        {
            try
            {
                IGetAccountDataManager GetAccountDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IGetAccountDataManager>();
                Customer currentUser = CurrentUserDataManager.CurrentUser;
                Notification.Info("\n ================== PROFILE ===================\n");
                Notification.Info($"Name: {currentUser.Name}");
                Notification.Info("Age: " + currentUser.Age);
                Notification.Info("Phone: " + currentUser.Phone);
                Notification.Info("Email: " + currentUser.Email);
                Notification.Info("No. of Accounts: " + (GetAccountDataManager.GetAllAccounts(currentUser.ID).Count()));
                Notification.Info("\n ==============================================\n");
            }catch(Exception ex)
            {
                Notification.Error(ex.ToString());
            }
        }

        public void EditProfile()
        {
            Customer currentUser = CurrentUserDataManager.CurrentUser;
            Customer customer = (Customer)currentUser.Clone();

            IDictionary<string, Action<string>> fields = new Dictionary<string, Action<string>>(){
                     { "NAME", (value) => customer.Name = value },
                     { "AGE", (value) =>
                                {
                                    int age;
                                    if (int.TryParse(value, out age))
                                        if(age > 18)
                                            customer.Age = age;
                                        else
                                            Notification.Error("Age should be greater than 18.");
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
            IUpdateCustomerDataManager UpdateCustomerDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IUpdateCustomerDataManager>();

            if (UpdateCustomerDataManager.UpdateCustomer(updatedCustomer))
            {
                Notification.Success("Profile Updated Successfully");
                CurrentUserDataManager.CurrentUser = updatedCustomer;
            }

        }

    }
}
