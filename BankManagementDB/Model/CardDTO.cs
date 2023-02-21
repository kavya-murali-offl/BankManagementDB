using BankManagementDB.Enums;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.Model
{
    [Table("Card")]
    public class CardDTO
    {
        [PrimaryKey]
        public Guid ID { get; set; }

        public string CardHolder { get; set; }

        public int Pin { get; set; }

        public long CardNumber { get; set; }

        public Guid AccountID { get; set; }

        public decimal Balance { get; set; }

        public int CVV { get; set; }

        public int ExpiryMonth { get; set; }

        public int ExpiryYear { get; set; }

        public decimal APR { get; set; }

        public decimal CreditLimit { get; set; }

        public int CreditPoints { get; set; }

        public string Type { get; set; }
    }
}
