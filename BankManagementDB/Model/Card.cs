
using BankManagementDB.Enums;
using System;

namespace BankManagementDB.Model
{
    public class Card
    {
        public Guid ID { get; set; }

        public string CardHolder { get; set; }

        public string Pin { get; set; }

        public string CardNumber { get; set; }

        public Guid AccountID { get; set; }

        public string CVV { get; set; }

        public string Expiry { get; set; }

        public CardType CardType { get; set; }    

    }
}
