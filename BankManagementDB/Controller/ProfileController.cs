using System;
using System.Collections.Generic;
using BankManagement.Models;
using BankManagement.Utility;

namespace BankManagement.Controller
{
    public class ProfileController
    {
        public Customer Customer { get; set; }

        public long ID => Customer.ID;

        public string Name => Customer.Name;

        public string Phone => Customer.Phone;

        public string Email => Customer.Email;

        public DateTime lastLoginOn => Customer.lastLoginOn;

    }
}
