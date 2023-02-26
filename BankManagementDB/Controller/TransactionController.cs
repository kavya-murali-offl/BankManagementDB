using System;
using System.Collections.Generic;
using System.Linq;
using BankManagementDB.EnumerationType;
using BankManagementDB.Model;
using BankManagementDB.Models;
using BankManagementCipher.Model;
using BankManagementCipher.Utility;
using BankManagementDB.Interface;

namespace BankManagementDB.Controller
{
    delegate bool InsertTransactionDelegate(Transaction transaction);

    public class TransactionController : ITransactionController
    {
        public TransactionController(ITransactionRepository transactionRepository) {
            TransactionRepository = transactionRepository;
        }

        public static IList<Transaction> TransactionsList { get; set; }

        public ITransactionRepository TransactionRepository { get; private set; }

        public bool InsertTransaction(Transaction transaction)
        {
            bool success = false;
            try
            {
                InsertTransactionDelegate insertTransaction = InsertTransactionInDB;
                success = insertTransaction(transaction);
                if (success)
                {
                    insertTransaction = InsertTransactionToList;
                    bool inserted = insertTransaction(transaction);
                    return inserted;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return success;
        }

        private bool InsertTransactionToList(Transaction transaction)
        {
            TransactionsList ??= new List<Transaction>();
            TransactionsList.Add(transaction);
            return true;
        }

        private bool InsertTransactionInDB(Transaction transaction)
        {
            try
            {

                TransactionDTO dto = Mapping.TransactionToDto(transaction);
                return TransactionRepository.InsertOrReplace(dto).Result;

            } catch (Exception e)
            {
                return false;
            }
        }

        public void FillTable(Guid accountID)
        {
            try
            {
                TransactionsList = new List<Transaction>();
                var transactionDTOs = TransactionRepository.Get(accountID).Result;
                foreach (TransactionDTO transactionDTO in transactionDTOs)
                    TransactionsList.Add(Mapping.DtoToTransaction(transactionDTO));

            }
            catch (Exception e) {
                Console.WriteLine(e);
            }
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

        public IEnumerable<Transaction> GetAllTransactions(Guid accountID)
        {
            IEnumerable<Transaction> list = from transaction in TransactionsList where accountID.Equals(transaction.AccountID) select transaction;
            return list;
        }

    }
}
