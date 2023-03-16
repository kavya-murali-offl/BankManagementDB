using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.Model
{
    [Table("DebitCard")]
    public class DebitCardDTO
    {
        [PrimaryKey]
        public string ID { get; set; }  
    }
}
