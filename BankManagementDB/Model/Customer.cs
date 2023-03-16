using SQLite;
using System;

namespace BankManagementDB.Models
{
    [Table("Customer")]
    public class Customer : ICloneable
    {
        [PrimaryKey]
        public string ID { get; set; }
        
        public int Age { get; set; }
        
        public string Phone { get; set; }
        
        public string Email { get; set; }
        
        public string Name { get; set; }
        
        public DateTime LastLoggedOn { get; set; }
        
        public DateTime CreatedOn { get; set; }

        public override string ToString() =>
             $"Name: {Name}\nAge: {Age}\nEmail: {Email}\nPhone: {Phone}\nLast Logged On: {LastLoggedOn}";

        public object Clone() => this.MemberwiseClone();
    }
}
