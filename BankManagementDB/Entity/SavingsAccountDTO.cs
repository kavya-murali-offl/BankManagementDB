using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.Entities
{
    [Table("SavingsAccount")]
    public class SavingsAccountDTO
    {

        [PrimaryKey]
        public string ID { get; set; }

    }
}
