using System;


namespace BankManagement.Models
{
    public class Customer
    {
        public long ID { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }
        
        public DateTime lastLoginOn { get; set; }   

        public override string ToString()
        {
            return $@"Customer ID: ${ID}\nName: {Name} \nPhone: {Phone}\nEmail: {Email}\n";
        }
    }
}