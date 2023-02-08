using BankManagement.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.db
{
    public class CustomerOperations
    {

        private static string connectionString = "Data source=database.sqlite3";

        public async static Task<bool> Upsert(Customer customer, string hashedPassword)
        {
            bool result = false;
            try
            {
                using (SQLiteConnection con = new SQLiteConnection(connectionString))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(con))
                    {
                        await con.OpenAsync();
                        cmd.CommandText = @"INSERT INTO Customer(
                                    ID, Name, Age, Phone, Email, HashedPassword, LastLoggedOn, CreatedOn) 
                                    VALUES(@ID, @Name, @Age, @Phone, @Email, @HashedPassword, @LastLoggedOn, @CreatedOn)
                                    ON CONFLICT(ID) 
                                    DO UPDATE SET 
                                    Name = @Name, Age = @Age, LastLoggedOn = @LastLoggedOn";
                        ;

                        cmd.Parameters.AddWithValue("@ID", customer.ID.ToString());
                        cmd.Parameters.AddWithValue("@Name", customer.Name);
                        cmd.Parameters.AddWithValue("@Age", customer.Age);
                        cmd.Parameters.AddWithValue("@Phone", customer.Phone);
                        cmd.Parameters.AddWithValue("@Email", customer.Email);
                        cmd.Parameters.AddWithValue("@HashedPassword", hashedPassword);
                        cmd.Parameters.AddWithValue("@CreatedOn", customer.CreatedOn);
                        cmd.Parameters.AddWithValue("@LastLoggedOn", customer.LastLoggedOn);

                        await cmd.ExecuteNonQueryAsync();
                        cmd.Dispose();
                    }
                    con.Close();
                }
                return true;
            }
            catch (Exception err)
            { Console.WriteLine(err.Message); }
            return result;
        }

        public async static Task<DataTable> Get(string phoneNumber)
        {
            DataTable dataTable = new DataTable();
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(connectionString))
                {
                    await conn.OpenAsync();
                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                    {
                        cmd.CommandText = @"Select ID, Name, Age, Phone, Email, LastLoggedOn, CreatedOn FROM Customer WHERE Phone = @Phone";
                        
                        cmd.Parameters.AddWithValue("Phone", phoneNumber);

                        await cmd.ExecuteNonQueryAsync();
                        using (var reader = await cmd.ExecuteReaderAsync())
                            dataTable.Load(reader);
                        
                        cmd.Dispose();
                    }
                    conn.Close();
                }
            }
            catch (Exception err)
            { Console.WriteLine(err.Message); }
            return dataTable;
        }

        public static string GetHashedPassword(string phoneNumber)
        {
            string result = null;
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                    {
                        cmd.CommandText = "Select HashedPassword FROM Customer WHERE Phone = @Phone";
                        cmd.Parameters.AddWithValue("@Phone", phoneNumber);
                        
                        result = (string)cmd.ExecuteScalar();

                        cmd.Dispose();
                    }
                    conn.Close();
                }
            }
            catch (Exception err)
            { Console.WriteLine(err.Message); }

            return result;
        }
    }
}
