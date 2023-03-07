﻿using BankManagementDB.EnumerationType;
using SQLite;
using System;

namespace BankManagementDB.Model
{
    [Table("Card")]
    public class Card
    {
        public Card()
        {
            ID = Guid.NewGuid();
            ExpiryMonth = DateTime.Now.Month.ToString();
            ExpiryYear = (DateTime.Now.Year + 7).ToString();
            CreatedOn = DateTime.Now;
        }

        [PrimaryKey]
        public virtual Guid ID { get; set; }

        public DateTime CreatedOn { get; set; }

        public string Pin { get; set; }

        public string CardNumber { get; set; }

        public Guid AccountID { get; set; }

        public virtual Guid CustomerID { get; set; }

        public string CVV { get; set; }

        public string ExpiryMonth { get; set; }

        public string ExpiryYear { get; set; }

        public CardType Type { get; set; }

    }
}
