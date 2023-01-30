using BankManagement.Models;
using BankManagementDB;
using BankManagementDB.db;
using BankManagementDB.View;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace BankManagement.Controller
{
    public class CustomersController
    {
        public static DataTable CustomerTable { get; set; }

        public bool CreateCustomer(string name, string password, string email, string phone, int age)
        {
            try
            {
                string hashedPassword = AuthServices.ComputeHash(password);
                IDictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "Name", name },
                    { "Email", email },
                    { "Phone", phone },
                    { "Age", age },
                    { "CreatedOn", DateTime.Now },
                    { "LastLoggedOn", DateTime.Now },
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

        public Customer UpdateCustomer(IDictionary<string, object> parameters)
        {
            try
            {
                bool success = DatabaseOperations.UpdateTable("Customer", parameters);
                FillTable();
                return GetCustomerByQuery("ID = "+(long)parameters["ID"]);
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
                DataRow row = GetUserByQuery("Phone = "+ phoneNumber);
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
            }catch(Exception e)
            {
                Notification.Error(e.Message);
            }
            return null;
        }

        public Customer GetCustomerByQuery(string query)
        {
            try
            {
                DataRow dr = CustomerTable.Select(query).LastOrDefault();
                return RowToCustomer(dr);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return null;
        }

        public Customer RowToCustomer(DataRow row)
        {
            Customer customer = new Customer();
            try
            {
                customer.ID = row.Field<long>("ID");
                customer.Name = row.Field<string>("Name");
                customer.Age = row.Field<long>("Age");
                customer.Phone = row.Field<string>("Phone");
                customer.CreatedOn = DateTime.Parse(row.Field<string>("CreatedOn"));
                customer.LastLoggedOn = DateTime.Parse(row.Field<string>("LastLoggedOn"));
                customer.Email = row.Field<string>("Email");
            }catch(Exception ex)
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
