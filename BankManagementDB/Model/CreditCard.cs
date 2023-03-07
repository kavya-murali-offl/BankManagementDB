using BankManagementDB.EnumerationType;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.Model
{
    [Table("CreditCard")]
    public class CreditCard
    {
        public CreditCard()
        {
            CreditLimit = 10000;
            APR = 0.060m;
            CreditPoints = 100;
            TotalDueAmount= 0;
        }

        [PrimaryKey]
        public Guid ID { get; set; }   
        
        public CreditCardType CreditCardType { get; set; }

        public int CreditPoints { get; set; }

        public decimal TotalDueAmount { get; set; }

        public decimal APR { get; set; }

        public decimal CreditLimit { get; set; }
    }
}
