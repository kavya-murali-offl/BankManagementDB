using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementCipher.Model
{
    [Table("Customer")]
    public class CustomerDTO
    {

        [PrimaryKey]
        public Guid ID { get; set; }

        public string Name { get; set; }

        public int Age { get; set; }

        public string HashedPassword { get; set; }

        [Unique]
        public string Phone { get; set; }

        public string Email { get; set; }

        public DateTime LastLoggedOn { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
