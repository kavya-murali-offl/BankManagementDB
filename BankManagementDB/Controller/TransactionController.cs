using System;
using System.Collections.Generic;
using System.Linq;
using BankManagement.Enums;
using BankManagement.Model;
using BankManagement.Models;
using BankManagementCipher.Model;
using BankManagementCipher.Utility;
using BankManagementDB.db;
using BankManagementDB.Interface;

namespace BankManagement.Controller
{
    delegate bool InsertTransactionDelegate(Transaction transaction);
    delegate bool TransferDelegate(decimal amount, Account account);

    public class TransactionController : ITransactionServices
    {
        public TransactionController() { }

        public TransactionController(IQueryServices<TransactionDTO> queryServices) {
            TransactionOperations = queryServices;
        }   

        public event Action<string> BalanceChanged;

        public static IList<Transaction> TransactionsList { get; set; }

        public IQueryServices<TransactionDTO> TransactionOperations { get; private set;}

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
                return TransactionOperations.InsertOrReplace(dto).Result;

            } catch(Exception e)
            {
                return false;   
            }
        }

        public void FillTable(Guid accountID)
        {
            try
            {
                TransactionsList = new List<Transaction>();
                var transactionDTOs = TransactionOperations.Get(accountID).Result;
                foreach (TransactionDTO transactionDTO in transactionDTOs)
                    TransactionsList.Add(Mapping.DtoToTransaction(transactionDTO));

            }
            catch(Exception e) {
                Console.WriteLine(e);
            }
        }

        public DateTime? GetLastWithdrawDate()
        {
            if(TransactionsList.Count > 0)
            {
                Transaction transaction = TransactionsList.LastOrDefault(data => data.TransactionType == TransactionTypes.WITHDRAW);

                if (transaction == null)
                    transaction = TransactionsList.LastOrDefault(data => data.TransactionType == TransactionTypes.DEPOSIT);

                return transaction.RecordedOn;
            }
            return null;
        }

        public IList<Transaction> GetAllTransactions() => TransactionsList;

    }
}
