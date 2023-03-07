using System;
using System.Collections.Generic;
using System.Linq;
using BankManagementDB.EnumerationType;
using BankManagementDB.Model;
using BankManagementCipher.Model;
using BankManagementDB.Interface;

namespace BankManagementDB.Controller
{
    public class TransactionDataManager : ITransactionDataManager
    {
        public TransactionDataManager(IDBHandler dbHandler) {
            DBHandler = dbHandler;
        }

        public static IList<Transaction> TransactionsList { get; set; }

        public IDBHandler DBHandler { get; private set; }

        public bool InsertTransaction(Transaction transaction)
        {
            if(InsertTransactionInDB(transaction))
                return InsertTransactionToList(transaction);
            return false;
        }

        private bool InsertTransactionToList(Transaction transaction)
        {
            TransactionsList ??= new List<Transaction>();
            TransactionsList.Insert(0, transaction);
            return true;
        }

        private bool InsertTransactionInDB(Transaction transaction)
        {
            return DBHandler.InsertTransaction(transaction).Result;
        }

        public void FillTable(Guid accountID)
        {
            TransactionsList = DBHandler.GetTransaction(accountID).Result;
        }


        public DateTime? GetLastWithdrawDate()
        {
            if (TransactionsList.Count > 0)
            {
                Transaction transaction = TransactionsList.LastOrDefault(data => data.TransactionType == TransactionType.WITHDRAW);
                if (transaction == null)
                    transaction = TransactionsList.LastOrDefault(data => data.TransactionType == TransactionType.DEPOSIT);
                return transaction.RecordedOn;
            }
            return null;
        }

        public IEnumerable<Transaction> GetAllTransactions(Guid accountID) => from transaction in TransactionsList where accountID.Equals(transaction.AccountID) select transaction;

        public IList<Transaction> GetAllCreditCardTransactions(string cardNumber)
        {
            TransactionsList = DBHandler.GetTransaction(cardNumber).Result;
            return TransactionsList;
        }
    }
}
