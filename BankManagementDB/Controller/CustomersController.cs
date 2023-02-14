using System;
using System.Collections.Generic;
using BankManagement.Models;
using BankManagementCipher.Model;
using BankManagementCipher.Utility;
using BankManagementDB.db;
using BankManagementDB.Interface;
using BankManagementDB.View;

namespace BankManagement.Controller
{
    public class CustomersController : ICustomerServices
    {

        public CustomersController(IQueryServices<CustomerDTO> queryOperations, IAuthenticationServices authenticationServices)
        {
            CustomerOperations = queryOperations;
            PasswordServices = authenticationServices;
        }

        public IQueryServices<CustomerDTO> CustomerOperations { get; set; }
        public IAuthenticationServices PasswordServices { get; set; }

        public Customer GetCustomer(string phoneNumber)
        {
            try
            {
                CustomerOperations customerOperations = new CustomerOperations();
                IList<CustomerDTO> customerDTOs = customerOperations.Get(phoneNumber).Result;
                if (customerDTOs.Count > 0)
                    return Mapping.DtoToCustomer(customerDTOs[0]);

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
            return null;
        }

        public bool InsertCustomer(Customer customer, string password)
        {
            try
            {

                string hashedPassword = AuthServices.ComputeHash(password);
                CustomerDTO customerDTO = Mapping.CustomerToDto(customer, hashedPassword);
                return CustomerOperations.InsertOrReplace(customerDTO).Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
        }

        public Customer UpdateCustomer(Customer customer)
        {
            try
            {
                string password = PasswordServices.GetHashedPassword(customer.Phone);
                CustomerDTO customerDTO = Mapping.CustomerToDto(customer, password);  
                bool success = CustomerOperations.InsertOrReplace(customerDTO).Result;
                return GetCustomer(customer.Phone);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return null;
        }

        public bool ValidatePassword(string phoneNumber, string password)
        {
            try
            {
                string hashedInput = AuthServices.ComputeHash(password);
                string passwordFromDB = PasswordServices.GetHashedPassword(phoneNumber);
                return hashedInput == passwordFromDB ? true : false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
        }
    }
}
