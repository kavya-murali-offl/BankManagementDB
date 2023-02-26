using BankManagementDB.EnumerationType;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementCipher.Model
{
    [Table("Account")]
    public class AccountDTO
    {
        [PrimaryKey]
        public Guid ID { get; set; }

        public string AccountNumber { get; set; }

        public decimal Balance { get; set; }

        public decimal MinimumBalance { get; set; }

        public decimal InterestRate { get; set; }

        public string Type { get; set; }

        public Guid UserID { get; set; }

        public DateTime CreatedOn { get; set; }

        public string Status { get; set; }
    }
}
