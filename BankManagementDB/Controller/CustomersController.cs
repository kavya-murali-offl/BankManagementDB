using System;
using System.Data;
using BankManagement.Models;
using BankManagementDB.db;
using BankManagementDB.Interface;
using BankManagementDB.View;

namespace BankManagement.Controller
{
    public class CustomersController : ICustomerServices
    {

        public Customer GetCustomer(string phoneNumber)
        {
            try
            {
                return CustomerOperations.Get(phoneNumber).Result;
            }catch(Exception ex)
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
                return CustomerOperations.UpdateOrInsert(customer, hashedPassword).Result;
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
                bool success = CustomerOperations.UpdateOrInsert(customer, null).Result;
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
                string passwordFromDB = CustomerOperations.GetHashedPassword(phoneNumber);
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
