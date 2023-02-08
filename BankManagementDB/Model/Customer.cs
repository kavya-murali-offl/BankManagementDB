using System;

namespace BankManagement.Models
{
    public class Customer : ICloneable
    {
        public Customer(string name, int age, string phone, string email)
        {
            ID = Guid.NewGuid();
            Name = name;
            Age = age;
            Phone = phone;
            Email = email;
            LastLoggedOn = DateTime.Now;
            CreatedOn = DateTime.Now;
        }
        public Guid ID { get; set; }
        
        public int Age { get; set; }
        
        public string Phone { get; set; }
        
        public string Email { get; set; }
        
        public string Name { get; set; }
        
        public DateTime LastLoggedOn { get; set; }
        
        public DateTime CreatedOn { get; set; }

        public override string ToString()
        {
            return $"{Name}";
        }
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
