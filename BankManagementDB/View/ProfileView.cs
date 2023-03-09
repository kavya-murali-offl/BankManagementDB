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
using BankManagementDB.Data;
using BankManagementDB.Properties;

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
                    Console.WriteLine();
                    for (int i = 0; i < Enum.GetNames(typeof(ProfileServiceCases)).Length; i++)
                    {
                        ProfileServiceCases cases = (ProfileServiceCases)i;
                        Console.WriteLine($"{i + 1}. {cases.ToString().Replace("_", " ")}");
                    }

                    Console.Write(Resources.EnterChoice);

                    string option = Console.ReadLine().Trim();
                    int entryOption = int.Parse(option);
                    if (option == Resources.BackButton)
                        break;
                    else if (entryOption <= Enum.GetNames(typeof(ProfileServiceCases)).Count())
                    {
                        ProfileServiceCases cases = (ProfileServiceCases)entryOption - 1;

                        if (ProfileOperations(cases))
                            break;
                    }
                    else
                        Notification.Error(Resources.InvalidOption);
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
                    Notification.Error(Resources.InvalidOption);
                    return false;
            }
        }

        public void ViewProfileDetails()
        {
            try
            {

                IGetAccountDataManager GetAccountDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IGetAccountDataManager>();
                Customer currentUser = CacheData.CurrentUser;
                Notification.Info("\n ================== PROFILE ===================\n");
                Notification.Info(currentUser.ToString());
                Notification.Info("No. of Accounts: " + (GetAccountDataManager.GetAllAccounts(currentUser.ID).Count()));
                Notification.Info("\n ==============================================\n");

            }
            catch(Exception ex)
            {
                Notification.Error(ex.ToString());
            }
        }

        public void EditProfile()
        {
            try
            {
                Customer currentUser = CacheData.CurrentUser;
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
                                            Notification.Error(Resources.AgeGreaterThan18);
                                    else
                                        Notification.Error(Resources.InvalidInteger);
                                }
                     }
                };  

                while (true)
                {
                    Console.WriteLine(string.Format(Resources.EnterName ,customer.Name));
                    Console.WriteLine(string.Format(Resources.EnterName, customer.Age));
                    Console.WriteLine(Resources.AskFieldToEdit);

                    string field = Console.ReadLine().Trim().ToUpper();

                    if (fields.ContainsKey(field))
                    {
                        Console.Write(Resources.EnterNewValue);
                        string value = Console.ReadLine();
                        fields[field](value);
                    }
                    else if (field == Resources.BackButton)
                        break;
                    else
                        Notification.Error(Resources.InvalidOption);
                }


                if (customer.Name != currentUser.Name || customer.Age != currentUser.Age)
                    UpdateProfile(customer);
            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);
            }
        }

            
        public void UpdateProfile(Customer updatedCustomer)
        {
            try
            {
                IUpdateCustomerDataManager UpdateCustomerDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IUpdateCustomerDataManager>();

                if (UpdateCustomerDataManager.UpdateCustomer(updatedCustomer))
                {
                    Notification.Success(Resources.ProfileUpdateSuccess);
                    CacheData.CurrentUser = updatedCustomer;
                }
                else
                    Notification.Error(Resources.ProfileUpdateFailure);

            }catch(Exception error) { 
                 Notification.Error(error.ToString());
            }
        }

    }
}
