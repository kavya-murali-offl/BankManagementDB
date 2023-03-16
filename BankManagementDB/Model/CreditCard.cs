﻿using BankManagementDB.EnumerationType;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.Model
{
    public class CreditCard : Card
    {
     
        public CreditCardType CreditCardType { get; set; }

        public int CreditPoints { get; set; }

        public decimal TotalDueAmount { get; set; }

        public decimal APR { get; set; }

        public decimal CreditLimit { get; set; }


    }

}
