using System;


namespace BankManagement.Models
{
    public class Customer
    {
        public Int64 ID { get; set; }

        public string UserName { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        public override string ToString()
        {
            return "Name: " + Name + "\n Username: " + UserName;
        }
    }
}