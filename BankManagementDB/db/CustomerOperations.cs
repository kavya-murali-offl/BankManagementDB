using BankManagement.Models;
using BankManagementDB.Interface;
using BankManagementDB.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BankManagementDB.db
{
    public class CustomerOperations 
    {

        private static string connectionString = "Data source=database.sqlite3";

        public static async Task<bool> UpdateOrInsert(Customer customer, string hashedPassword)
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
                        cmd.Parameters.AddWithValue("@CreatedOn", DateTimeHelper.GetEpoch(customer.CreatedOn));
                        cmd.Parameters.AddWithValue("@LastLoggedOn", DateTimeHelper.GetEpoch(customer.LastLoggedOn));

                        await cmd.ExecuteNonQueryAsync();
                        cmd.Dispose();
                    }
                    con.Close();
                }
                return true;
            }
            catch (Exception err)
            { Console.WriteLine(err); }
            return result;
        }

        public static async Task<Customer> Get(string phoneNumber)
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(connectionString))
                {
                    await conn.OpenAsync();
                    using (SQLiteCommand cmd = new SQLiteCommand(conn))
                    {
                        cmd.CommandText = @"Select ID, Name, Age, Phone, Email, LastLoggedOn, CreatedOn FROM Customer WHERE Phone = @Phone";
                        
                        cmd.Parameters.AddWithValue("@Phone", phoneNumber);

                        await cmd.ExecuteNonQueryAsync();
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                Customer customer = new Customer();
                                customer.ID = new Guid(reader.GetString(0));
                                customer.Name = reader.GetString(1);
                                customer.Age = (int)reader.GetInt64(2);
                                customer.Phone = reader.GetString(3);
                                customer.Email = reader.GetString(4);
                                customer.LastLoggedOn = DateTimeHelper.ConvertEpochToDateTime(reader.GetInt64(5));
                                return customer;
                            }
                        }
                        
                        cmd.Dispose();
                    }
                    conn.Close();
                }
            }
            catch (Exception err)
            { Console.WriteLine(err.Message); }
            return null;
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
