using BankManagement.Controller;
using BankManagement.Models;
using BankManagementDB.Interface;
using BankManagementDB.Utility;
using BankManagementDB.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography;

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
                    Console.WriteLine("1. View Profile\n2. Edit Profile\n3. Back");
                    string option = Console.ReadLine().Trim();
                    int entryOption = int.Parse(option);
                    if (entryOption != 0 && entryOption <= Enum.GetNames(typeof(DashboardCases)).Count())
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
                    Notification.Error("Enter a valid option");
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
            Console.WriteLine("\n ================== PROFILE ===================\n");
            Console.WriteLine("Name: " + profileController.Name);
            Console.WriteLine("Äge: " + profileController.Age);
            Console.WriteLine("Phone: " + profileController.Phone);
            Console.WriteLine("Email: " + profileController.Email);
            Console.WriteLine("No. of Accounts: " + AccountsController.AccountTable.Rows.Count);
            Console.WriteLine("\n ==============================================\n");
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

            IDictionary<string, object> updatedFields = new Dictionary<string, object>();   
            if (pair1.Value != profile.Name)
                updatedFields.Add("Name", pair1.Value);
            if(pair2.Value != profile.Age)
                updatedFields.Add("Age", pair2.Value);

            if(updatedFields.Count > 0) {
                updatedFields.Add("ID", profile.ID);
                CustomersController customersController = new CustomersController();
                Customer customer = customersController.UpdateCustomer(updatedFields);
                if(customer != null) {
                    Notification.Success("Profile Updated Successfully");
                    profile.Customer = customer;
                }
            }
        }
    }
}
