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

        public long Age => Customer.Age;

        public string Phone => Customer.Phone;

        public string Email => Customer.Email;

        public DateTime LastLoggedOn => Customer.LastLoggedOn;

        public DateTime CreatedOn => Customer.CreatedOn;

    }
}
