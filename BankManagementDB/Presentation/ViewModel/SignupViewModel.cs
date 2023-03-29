using BankManagementDB.Config;
using BankManagementDB.DataManager;
using BankManagementDB.EnumerationType;
using BankManagementDB.Interface;
using BankManagementDB.Model;
using BankManagementDB.Models;
using BankManagementDB.View;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.ViewModel
{
    public class SignupViewModel
    {

        public Customer SignupUser(string phone, string name, string email, int age, string password)
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
            if (InsertCustomerDataManager.InsertCustomer(customer, customerCredentials)) return customer;
            else return null;

        }


        public bool CheckUniquePhoneNumber(string phoneNumber)
        {
            IGetCustomerDataManager GetCustomerDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IGetCustomerDataManager>();
            Customer customer = GetCustomerDataManager.GetCustomer(phoneNumber);
            return customer == null;
        }

        public bool InsertAccount(Account account)
        {
                IInsertAccountDataManager InsertAccountDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IInsertAccountDataManager>();
                return InsertAccountDataManager.InsertAccount(account);
               
        }

       

}
}
