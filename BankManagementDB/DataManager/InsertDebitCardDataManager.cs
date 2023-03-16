﻿using BankManagementDB.Interface;
using BankManagementDB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.DataManager
{
    public class InsertDebitCardDataManager : IInsertDebitCardDataManager
    {
        public InsertDebitCardDataManager(IDBHandler dBHandler)
        {
            DBHandler = dBHandler;
        }

        public IDBHandler DBHandler { get; private set; }

        public bool InsertDebitCard(DebitCardDTO card) => DBHandler.InsertDebitCard(card).Result;

    }
}
