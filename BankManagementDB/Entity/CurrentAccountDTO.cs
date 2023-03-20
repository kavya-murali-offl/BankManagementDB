using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.Entities
{
    [Table("CurrentAccount")]
    public class CurrentAccountDTO
    {
        [PrimaryKey]
        public string ID { get; set; }
    }
}
