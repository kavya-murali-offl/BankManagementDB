using System;
using System.Globalization;

namespace BankManagement.Models
{
    public class Customer
    {
        public Customer(string name, long age, string phone, string email)
        {
            Name = name;
            Age = age;
            Phone = phone;  
            Email = email;
            LastLoggedOn = DateTime.Now; 
            CreatedOn = DateTime.Now;
        }

        public long ID { get; set; }

        public long Age { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        public DateTime LastLoggedOn { get; set; }

        public DateTime CreatedOn { get; set; }

        public override string ToString()
        {
            return $"Customer ID: {ID}\nName: {Name} \nAge: {Age} \nPhone: {Phone}\nEmail: {Email}\n";
        }
    }
}