using BankManagement.Enums;
using BankManagement.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;

namespace BankManagementDB.db
{
    public class AccountDB
    {
        //private static AccountDB _accountDB;
        //private static readonly object _lock = new object();

        //private  AccountDB()
        //{
        //}

        //public static AccountDB GetInstance()
        //{
        //    if (_accountDB == null)
        //    {
        //        lock (_lock)
        //            if (_accountDB == null)
        //                _accountDB = new AccountDB();
        //    }
        //    return _accountDB;
        //}

        //    public DataTable FillTable(Int64 id)
        //    {
        //        try
        //        {
        //            string query = "SELECT * FROM Account WHERE UserID = @UserID";
        //            QueryExecution queryExecution = new QueryExecution();
        //            IDictionary<string, object> parameters = new Dictionary<string, object>();
        //            parameters.Add("@UserID",id);
        //            DataTable AccountTable = new DataTable();
        //            AccountTable = queryExecution.ExecuteQuery(query, parameters);
        //            PrintDataTable(AccountTable);
        //            return AccountTable;
        //        }
        //        catch (Exception err)
        //        {
        //            Console.WriteLine(err);
        //        }
        //        return null;

        //    }

        //    public bool UpdateAccountInDB(Account account, IDictionary<string, object> updateFields)
        //    {
        //        bool result = false;
        //        try
        //        {
        //            string query = "UPDATE Account SET ";
        //            foreach (KeyValuePair<string, object> pairs in updateFields)
        //            {
        //                query += pairs.Key + "= @" + pairs.Key;
        //            }

        //            query += " WHERE ID = @AccountID";
        //            updateFields.Add("AccountID", account.ID);
        //            QueryExecution queryExecution = new QueryExecution();
        //            queryExecution.ExecuteNonQuery(query, updateFields);
        //        result = true;
        //        }catch(Exception err) 
        //        { Console.WriteLine(err.Message); }
        //        return result;
        //    }
        //    public bool InsertAccountIntoDB(Account account)
        //    {
        //        bool result = false;
        //        try
        //        {
        //            string query = "INSERT INTO Account ('Balance', 'InterestRate', 'Status', 'Type', 'UserID') VALUES (@Balance, @InterestRate, @Status, @Type, @UserID)";
        //            QueryExecution queryExecution = new QueryExecution();
        //            IDictionary<string, object> parameters = new Dictionary<string, object>();
        //            parameters.Add("@Balance", account.Balance);
        //            parameters.Add("@InterestRate", account.InterestRate);
        //            parameters.Add("@Status", account.Status.ToString());
        //            parameters.Add("@UserID", account.UserID);
        //            parameters.Add("@Type", account.Type.ToString());
        //            queryExecution.ExecuteNonQuery(query, parameters);
        //            result = true;
        //        }
        //        catch (Exception err)
        //        {
        //            Console.WriteLine(err);
        //        }
        //        return result;
        //    }


        //    public Int64 GetIDFromUserName(string userName)
        //    {
        //        Account account = null;
        //        try
        //        {
        //            int counter = 0;
        //            string query = "SELECT ID FROM Account WHERE UserName == @UserName";
        //            QueryExecution queryExecution = new QueryExecution();
        //            IDictionary<string, object> parameters = new Dictionary<string, object>();
        //            parameters.Add("@UserName", userName);
        //            DataTable accountsTable = queryExecution.ExecuteQuery(query, parameters);
        //            Console.WriteLine(accountsTable);
        //        }
        //        catch (Exception err)
        //        {
        //            Console.WriteLine(err);
        //        }
        //        return account.ID;
        //    }



        //    public void PrintDataTable(DataTable AccountsTable)
        //    {
        //        Console.WriteLine();
        //        Dictionary<string, int> colWidths = new Dictionary<string, int>();

        //        foreach (DataColumn col in AccountsTable.Columns)
        //        {
        //            Console.Write(col.ColumnName);
        //            var maxLabelSize = AccountsTable.Rows.OfType<DataRow>()
        //                    .Select(m => (m.Field<object>(col.ColumnName)?.ToString() ?? "").Length)
        //                    .OrderByDescending(m => m).FirstOrDefault();

        //            colWidths.Add(col.ColumnName, maxLabelSize);
        //            for (int i = 0; i < maxLabelSize - col.ColumnName.Length + 10; i++) Console.Write(" ");
        //        }

        //        Console.WriteLine();

        //        foreach (DataRow dataRow in AccountsTable.Rows)
        //        {
        //            for (int j = 0; j < dataRow.ItemArray.Length; j++)
        //            {
        //                Console.Write(dataRow.ItemArray[j]);
        //                for (int i = 0; i < colWidths[AccountsTable.Columns[j].ColumnName] - dataRow.ItemArray[j].ToString().Length + 10; i++) Console.Write(" ");
        //            }
        //            Console.WriteLine();
        //        }
        //    }

    }
}
