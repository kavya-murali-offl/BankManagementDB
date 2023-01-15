using BankManagement.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;


namespace BankManagementDB.db
{
    public class CustomerDB
    {
        //private static CustomerDB _customerDB;
        //private static readonly object _lock = new object();

        //private CustomerDB() {
        //      FillTable();
        //}
        //public DataTable CustomersTable { get; private set; }

        //public static CustomerDB GetInstance()
        //{
        //    if (_customerDB == null)
        //    {
        //        lock (_lock)
        //            if (_customerDB == null)
        //                _customerDB = new CustomerDB();
        //    }
        //    return _customerDB;
        //}

        //public void FillTable()
        //{
        //    try
        //    {
        //        string query = "SELECT * FROM Customer";
        //        QueryExecution queryExecution = new QueryExecution();
        //        DataTable customersTable = queryExecution.ExecuteQuery(query, null);
        //        CustomersTable = customersTable;
        //    }
        //    catch (Exception err)
        //    {
        //        Console.WriteLine(err);
        //    }
        //    finally
        //    {
        //        PrintDataTable();
        //    }
        //}

        //public void PrintDataTable()
        //{
        //    Console.WriteLine();
        //    Dictionary<string, int> colWidths = new Dictionary<string, int>();

        //    foreach (DataColumn col in CustomersTable.Columns)
        //    {
        //        Console.Write(col.ColumnName);
        //        var maxLabelSize = CustomersTable.Rows.OfType<DataRow>()
        //                .Select(m => (m.Field<object>(col.ColumnName)?.ToString() ?? "").Length)
        //                .OrderByDescending(m => m).FirstOrDefault();

        //        colWidths.Add(col.ColumnName, maxLabelSize);
        //        for (int i = 0; i < maxLabelSize - col.ColumnName.Length + 10; i++) Console.Write(" ");
        //    }

        //    Console.WriteLine();

        //    foreach (DataRow dataRow in CustomersTable.Rows)
        //    {
        //        for (int j = 0; j < dataRow.ItemArray.Length; j++)
        //        {
        //            Console.Write(dataRow.ItemArray[j]);
        //            for (int i = 0; i < colWidths[CustomersTable.Columns[j].ColumnName] - dataRow.ItemArray[j].ToString().Length + 10; i++) Console.Write(" ");
        //        }
        //        Console.WriteLine();
        //    }
        //}

        //public bool  InsertCustomerIntoDB(Customer customer, string hashed)
        //{ 
        //    bool result = false;
        //    var s = 0;
        //    try
        //    {
        //        string query = "INSERT INTO Customer ('Name', 'UserName', 'HashedPassword', 'Email', 'Phone') VALUES (@Name, @UserName, @HashedPassword, @Email, @Phone)";
        //        QueryExecution queryExecution = new QueryExecution();
        //        IDictionary<string, object> parameters= new Dictionary<string, object>();
        //        parameters.Add("@Name", customer.Name);
        //        parameters.Add("@UserName", customer.UserName);
        //        parameters.Add("@Phone", customer.Phone);
        //        parameters.Add("@Email", customer.Email);
        //        parameters.Add("@HashedPassword", hashed);
        //        queryExecution.ExecuteNonQuery(query, parameters);
        //        result = true;
        //    }
        //    catch (Exception err)
        //    {
        //        Console.WriteLine(err);
        //    }
        //    return result;
        //}

        //public bool CheckIfUserNameExistFromTable(string userName)
        //{
        //    try
        //    {
        //        foreach(DataRow dr in CustomersTable.Rows)
        //        {
        //            if(dr["UserName"].ToString() == userName)
        //                return true;
        //        }
        //    }
        //    catch (Exception err)
        //    {
        //        Console.WriteLine(err);
        //    }
        //    return false;
        //}


        //public DataRow GetRowByUserName(string userName)
        //{
        //    try
        //    {
        //       IEnumerable<DataRow> data =  CustomersTable.AsEnumerable().Where(dr => dr.Field<string>("UserName") == userName);
        //        return data?.First();
        //    }
        //    catch (Exception err)
        //    {
        //        Console.WriteLine(err);
        //        return null;
        //    }

        //}
    }
}
