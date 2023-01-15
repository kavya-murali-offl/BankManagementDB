using BankManagement.Models;
using BankManagement.Utility;
using BankManagementDB.db;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Principal;
using System.Xml.Linq;

namespace BankManagement.Controller
{
    public class CustomersController
    {
        public CustomersController() {
        }

        public DataTable CustomerDB { get; set; }

        public DataRow GetUserByUserName(string userName)
        {
            
            IEnumerable<DataRow> rows = CustomerDB.AsEnumerable()
               .Where(r => r.Field<string>("UserName") == userName);
            if(rows != null && rows.Count() > 0)
            {
              DataRow row = rows.FirstOrDefault();
            return row;
            }
            return null;    

        }

        public Customer GetCustomerByUserName(string username) {
            Customer customer = new Customer();
            try
            {
                DataRow dr = GetUserByUserName(username);
                customer.ID = dr.Field<Int64>("ID");
                customer.UserName = dr.Field<string>("UserName");
                customer.Name = dr.Field<string>("Name");
                customer.Phone = dr.Field<string>("Phone");
                customer.Email = dr.Field<string>("Email");
            }catch(Exception ex) {
                Console.WriteLine(ex);
            }
            return customer;
        }

        public bool CreateCustomer(string userName, string password, string name, string email, string phone) 
        {
            try
            {
                string hashedPassword = AuthServices.ComputeHash(password);
                IDictionary<string, object> parameters = new Dictionary<string, object>();
                parameters.Add("Name", name);
                parameters.Add("Email", email);
                parameters.Add("Phone", phone);
                parameters.Add("UserName", userName);
                parameters.Add("HashedPassword", hashedPassword);
                return Database.InsertRowToTable("Customer", parameters);

            }catch(Exception ex) {
                Console.WriteLine(ex);
            }
            return false;
        }

        public void FillTable() {
            CustomerDB = Database.FillTable("Customer", null);
        }
      

        public bool ValidatePassword(string userName, string password)
        {
            try
            {
                string hashedInput = AuthServices.ComputeHash(password);
                DataRow dr = GetUserByUserName(userName);
                string passwordFromDB = dr.Field<string>("HashedPassword");
                return hashedInput == passwordFromDB ? true : false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }

}
