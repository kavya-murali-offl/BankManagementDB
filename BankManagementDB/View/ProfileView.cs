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
using BankManagementDB.Utility;

namespace BankManagementDB.View
{

    public class ProfileView
    {
        public void ViewProfileServices()
        {
            OptionsDelegate<ProfileServiceCases> options = ProfileOperations;

            HelperView helperView = new HelperView();
            helperView.PerformOperation(options);
        }


        public bool ProfileOperations(ProfileServiceCases command) =>
            command switch
            {

                ProfileServiceCases.VIEW_PROFILE => ViewProfile(),
                ProfileServiceCases.EDIT_PROFILE => EditProfile(),
                ProfileServiceCases.EXIT => true,
                _ => Default(),

            };

        private bool Default()
        {
            Notification.Error(DependencyContainer.GetResource("InvalidOption"));
            return false;
        }

        private bool ViewProfile()
        {
            Customer currentUser = Store.CurrentUser;
            Notification.Info(
                Formatter.FormatString(
                    DependencyContainer.GetResource("DisplayProfile"),
                    currentUser.ToString(),
                    Store.AccountsList.Count())
                );
            return false;
        }

        private bool EditProfile()
        {
            Customer currentUser = Store.CurrentUser;
            Customer customer = (Customer)currentUser.Clone();

            IDictionary<string, Action<string>> fields = new Dictionary<string, Action<string>>(){
                     { "NAME", (value) => customer.Name = value },
                     { "AGE", (value) =>
                                {
                                    if (int.TryParse(value, out int age))
                                    {
                                        if(Validator.IsValidAge(age))
                                        {
                                            customer.Age = age;
                                        }
                                        else
                                        {
                                            Notification.Error(DependencyContainer.GetResource("AgeGreaterThan18"));
                                        }
                                    }
                                    else
                                    {
                                        Notification.Error(DependencyContainer.GetResource("InvalidInteger"));
                                    }
                                }
                     }
                };

            while (true)
            {
                Console.WriteLine(DependencyContainer.GetResource("EnterName" + " ") + customer.Name);
                Console.WriteLine(DependencyContainer.GetResource("EnterAge" + " ") + customer.Age);
                Notification.Info(DependencyContainer.GetResource("AskFieldToEdit"));
                Notification.Info(DependencyContainer.GetResource("PressBackButtonInfo"));

                string field = Console.ReadLine()?.Trim().ToUpper();

                if (fields.ContainsKey(field))
                {
                    Console.Write(DependencyContainer.GetResource("EnterNewValue"));
                    string value = Console.ReadLine();
                    fields[field](value);
                }
                else if (field == DependencyContainer.GetResource("BackButton"))
                {
                    break;
                }
                else
                {
                    Notification.Error(DependencyContainer.GetResource("InvalidOption"));
                }
            }


            if (!customer.Name.Equals(currentUser.Name) || !customer.Age.Equals(currentUser.Age))
            {
                UpdateProfile(customer);
            }
            return false;

        }


        public void UpdateProfile(Customer updatedCustomer)
        {
            IUpdateCustomerDataManager UpdateCustomerDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IUpdateCustomerDataManager>();

            if (UpdateCustomerDataManager.UpdateCustomer(updatedCustomer))
            {
                Notification.Success(DependencyContainer.GetResource("ProfileUpdateSuccess"));
                Store.CurrentUser = updatedCustomer;
            }
            else
            {
                Notification.Error(DependencyContainer.GetResource("ProfileUpdateFailure"));
            }

        }

    }
}
