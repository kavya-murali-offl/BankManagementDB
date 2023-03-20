using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.Model
{
    [Table("CustomerCredentials")]
    public class CustomerCredentials
    {
        [PrimaryKey]
        public string ID { get; set; }

        public string Password { get; set; }

        public string Salt { get; set; }
    }
}
