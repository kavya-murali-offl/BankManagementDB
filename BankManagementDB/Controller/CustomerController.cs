using System;
using System.Collections.Generic;
using BankManagementDB.Models;
using BankManagementCipher.Model;
using BankManagementCipher.Utility;
using BankManagementDB.Interface;

namespace BankManagementDB.Controller
{
    public class CustomerController : ICustomerController
    {
        public CustomerController(ICustomerRepository customerRepository)
        {
            CustomerRepository = customerRepository;
        }

        public static Customer CurrentUser { get; set; }

        public ICustomerRepository CustomerRepository { get; set; }

        public void SetCurrentUser(Customer customer)
        {
            CurrentUser = customer; 
        }

        public Customer GetCurrentUser()
        {
            return CurrentUser;
        }

        public Customer GetCustomer(string phoneNumber)
        {
            try
            {
                IList<CustomerDTO> customerDTOs = CustomerRepository.Get(phoneNumber).Result;
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

                string hashedPassword = AuthServices.Encrypt(password);
                CustomerDTO customerDTO = Mapping.CustomerToDto(customer, hashedPassword);
                return CustomerRepository.InsertOrReplace(customerDTO).Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
        }

        public bool UpdateCustomer(Customer customer)
        {
            try
            {
                string password = CustomerRepository.GetHashedPassword(customer.Phone);
                CustomerDTO customerDTO = Mapping.CustomerToDto(customer, password);  
                bool success = CustomerRepository.InsertOrReplace(customerDTO).Result;
                if (success)
                    CurrentUser = customer;
                return success;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
        }
    }
}
