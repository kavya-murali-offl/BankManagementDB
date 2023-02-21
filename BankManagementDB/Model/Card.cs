﻿
using BankManagementDB.Enums;
using System;

namespace BankManagementDB.Model
{
    public abstract class Card
    {
        public Guid ID { get; set; }

        public string CardHolder { get; set; }

        public int CreditPoints { get; set; }

        public int Pin { get; set; }

        public long CardNumber { get; set; }

        public Guid AccountID { get; set; }

        public decimal Balance { get; set; }

        public int CVV { get; set; }

        public int ExpiryMonth { get; set; }

        public int ExpiryYear { get; set; }

        public decimal APR { get; set; }

        public decimal CreditLimit { get; set; }

        public CardType Type { get; set; }

        public abstract void Purchase(decimal amount);

        public abstract void Payment(decimal amount);

        public abstract override string ToString();

    }
}
