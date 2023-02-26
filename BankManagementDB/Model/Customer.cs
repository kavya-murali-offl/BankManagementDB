using System;

namespace BankManagementDB.Models
{
    public class Customer : ICloneable
    {
        public Customer() { }
      
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
