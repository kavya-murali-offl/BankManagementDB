using BankManagement.Controller;
using BankManagement.Models;
using BankManagementDB.Utility;
using BankManagementDB.View;
using System;
using System.Collections.Generic;
using System.Linq;

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
                    Console.WriteLine("\n\n");
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
            Notification.Info($"Name: {profileController.Name}");
            Notification.Info("Age: " + profileController.Age);
            Notification.Info("Phone: " + profileController.Phone);
            Notification.Info("Email: " + profileController.Email);
            Notification.Info("No. of Accounts: " + AccountsController.AccountTable.Rows.Count);
            Notification.Info("\n ==============================================\n");
        }

        public void EditProfile(ProfileController profile)
        {
            GenericPair<string, string> pair1 = new GenericPair<string, string>("Name", profile.Name);
            GenericPair<string, long> pair2 = new GenericPair<string, long>("Age", profile.Age);

            IDictionary<string, Action<string>> fields = new Dictionary<string, Action<string>>(){
                     { "NAME", (value) => pair1.Value = value },
                     { "AGE", (value) =>
                                {
                                    long age;
                                    if (long.TryParse(value, out age))
                                        pair2.Value = age;
                                    else
                                        Notification.Error("Invalid input! Age should be a number.");
                                }
                     }
            };

            while (true)
            {
                try
                {
                    Console.WriteLine(pair1);
                    Console.WriteLine(pair2);
                    Console.WriteLine("Which field you want to edit? Press 0 to go back!\n");
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

            IDictionary<string, object> updatedFields = new Dictionary<string, object>();   
            if (pair1.Value != profile.Name)
                updatedFields.Add("Name", pair1.Value);
            if(pair2.Value != profile.Age)
                updatedFields.Add("Age", pair2.Value);

            if(updatedFields.Count > 0) 
                UpdateProfile(updatedFields, profile);
        }


        public void UpdateProfile(IDictionary<string, object> updatedFields, ProfileController profile)
        {
            updatedFields.Add("ID", profile.ID);

            CustomersController customersController = new CustomersController();
            Customer customer = customersController.UpdateCustomer(updatedFields);

            if (customer != null)
            {
                Notification.Success("Profile Updated Successfully");
                profile.Customer = customer;
            }
        }
    }
}
