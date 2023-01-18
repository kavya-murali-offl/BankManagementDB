using BankManagement.Models;
using BankManagementDB.db;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace BankManagement.Controller
{
    public class CustomersController
    {
        public static DataTable CustomerTable { get; set; }

        public bool CreateCustomer(string name, string password, string email, string phone)
        {
            try
            {
                string hashedPassword = AuthServices.ComputeHash(password);
                IDictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "Name", name },
                    { "Email", email },
                    { "Phone", phone },
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

        public bool UpdateCustomer(string name, string password, string email, string phone)
        {
            try
            {
                string hashedPassword = AuthServices.ComputeHash(password);
                IDictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "Name", name },
                    { "Email", email },
                    { "Phone", phone },
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

        public bool ValidatePassword(string phoneNumber, string password)
        {
            try
            {
                string hashedInput = AuthServices.ComputeHash(password);
                DataRow dr = GetUserByPhoneNumber(phoneNumber);
                string passwordFromDB = dr.Field<string>("HashedPassword");
                return hashedInput == passwordFromDB ? true : false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public DataRow GetUserByPhoneNumber(string phoneNumber)
        {
            try
            {
                IEnumerable<DataRow> rows = CustomerTable.AsEnumerable()
                   .Where(r => r.Field<string>("Phone") == phoneNumber);
                if (rows != null && rows.Count() > 0)
                {
                    return rows.FirstOrDefault();
                }
            }catch(Exception ex) {
                Console.WriteLine(ex.Message);
            }
            return null;    
        }

        public Customer GetCustomerByPhoneNumber(string phoneNumber) {
            Customer customer = new Customer();
            try
            {
                DataRow dr = GetUserByPhoneNumber(phoneNumber);
                customer.ID = dr.Field<long>("ID");
                customer.Name = dr.Field<string>("Name");
                customer.Phone = dr.Field<string>("Phone");
                customer.Email = dr.Field<string>("Email");
            }catch(Exception ex) {
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
