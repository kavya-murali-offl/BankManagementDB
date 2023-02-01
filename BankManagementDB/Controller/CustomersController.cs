using BankManagement.Models;
using BankManagementDB.db;
using BankManagementDB.Interface;
using BankManagementDB.View;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace BankManagement.Controller
{
    public class CustomersController : ICustomerServices
    {
        public static DataTable CustomerTable { get; set; }

        public bool InsertCustomer(Customer customer, string password)
        {
            try
            {
                string hashedPassword = AuthServices.ComputeHash(password);
                IDictionary<string, dynamic> parameters = new Dictionary<string, dynamic>
                {
                    { "Name", customer.Name },
                    { "Email", customer.Email },
                    { "Phone", customer.Phone },
                    { "Age", customer.Age },
                    { "CreatedOn", customer.CreatedOn },
                    { "LastLoggedOn", customer.LastLoggedOn },
                    { "HashedPassword", hashedPassword }
                };

                return DatabaseOperations.InsertRowToTable("Customer", parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
        }

        public Customer UpdateCustomer(IDictionary<string, dynamic> parameters)
        {
            try
            {
                bool success = DatabaseOperations.UpdateTable("Customer", parameters);
                FillTable();
                return GetCustomerByQuery("ID = " + (long)parameters["ID"]);
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
                DataRow row = GetUserByQuery("Phone = " + phoneNumber);
                string passwordFromDB = row.Field<string>("HashedPassword");
                return hashedInput == passwordFromDB ? true : false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public DataRow GetUserByQuery(string query)
        {
            try
            {
                if (CustomerTable != null)
                    return CustomerTable.Select(query).LastOrDefault();
            }
            catch (Exception e)
            {
                Notification.Error(e.Message);
            }
            return null;
        }

        public Customer GetCustomerByQuery(string query)
        {
            try
            {
                DataRow customerRow = CustomerTable.Select(query).LastOrDefault();
                return RowToCustomer(customerRow);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return null;
        }

        public Customer RowToCustomer(DataRow row)
        {
            Customer customer = null;
            try
            {

                customer = new Customer(
                    row.Field<string>("Name"),
                    row.Field<long>("Age"),
                    row.Field<string>("Phone"),
                    row.Field<string>("Email")
                 );
                customer.ID = row.Field<long>("ID");
                customer.LastLoggedOn = DateTime.Parse(row.Field<string>("LastLoggedOn")); ;
                customer.CreatedOn = DateTime.Parse(row.Field<string>("CreatedOn"));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return customer;
        }

        public void FillTable()
        {
            CustomerTable = DatabaseOperations.FillTable("Customer", null);
        }
    }
}
