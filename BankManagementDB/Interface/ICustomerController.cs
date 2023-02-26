﻿
using BankManagementDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.Interface
{
    public interface ICustomerController
    {
        Customer GetCustomer(string phone);

        bool InsertCustomer(Customer customer, string password);

        bool UpdateCustomer(Customer customer);

        void SetCurrentUser(Customer customer);

        Customer GetCurrentUser();
    }
}
