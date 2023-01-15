using System;
using System.Collections.Generic;
using BankManagement.Models;

namespace BankManagement.Utility
{
    public class CustomersData
    {
        public CustomersData()
        {
            CustomersList = new List<Customer>();   
        }

        public IList<Customer> CustomersList { get; set; }

        public Customer GetDataByUsername(string username)
        {
            foreach (Customer customer in CustomersList)
            {
                if (customer.UserName == username)
                {
                    return customer;
                }
            }
            return null;
        }

        public void AddCustomer(Customer customer)
        {
            CustomersList.Add(customer);
        }

        public void ViewCustomers()
        {
            foreach(Customer customer in CustomersList) {
                Console.WriteLine(customer);
            }
        }

    }
}
