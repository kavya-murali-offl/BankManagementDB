﻿using BankManagementCipher.Model;
using BankManagementDB.Controller;
using BankManagementDB.DBHandler;
using BankManagementDB.Interface;
using BankManagementDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.DataManager
{
    public class UpdateCustomerDataManager : IUpdateCustomerDataManager
    {
        public UpdateCustomerDataManager(IDBHandler dBHandler)
        {
            DBHandler = dBHandler;
        }

        public IDBHandler DBHandler { get; set; }

        public bool UpdateCustomer(Customer customer)
        {
            bool success = DBHandler.UpdateCustomer(customer).Result;
            if (success)
                CurrentUserDataManager.CurrentUser = customer;
            return success;
        }
    }
}
