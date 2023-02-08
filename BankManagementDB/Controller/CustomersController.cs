using System;
using System.Data;
using System.Security.Policy;
using System.Xml.Linq;
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
            DataTable table = CustomerOperations.Get(phoneNumber).Result;
            if (table.Rows.Count > 0)
                return RowToCustomer(table.Rows[0]);
            else
                return null;
        }


        public void CreateCustomer(string name, int age, string phone, string email, string password)
        {
            Customer customer = new Customer(name, age, phone, email);
            InsertCustomer(customer, password);
        }

        public bool InsertCustomer(Customer customer, string password)
        {
            try
            {
                string hashedPassword = AuthServices.ComputeHash(password);
                return CustomerOperations.Upsert(customer, hashedPassword).Result;
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
                bool success = CustomerOperations.Upsert(customer, null).Result;
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
                return false;
            }
        }

        private Customer RowToCustomer(DataRow row)
        {
            Customer customer = null;
            try
            {

                customer = new Customer(
                        row.Field<string>("Name"),
                       (int)row.Field<long>("Age"),
                        row.Field<string>("Phone"),
                        row.Field<string>("Email")
                 );

                customer.ID = Guid.Parse(row.Field<string>("ID"));
                customer.CreatedOn = row.Field<DateTime>("CreatedOn");
                customer.LastLoggedOn = row.Field<DateTime>("LastLoggedOn");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return customer;
        }
    }
}
