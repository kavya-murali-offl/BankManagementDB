using BankManagementCipher.Model;
using BankManagementDB.Interface;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly SQLiteConnectionString Options = new SQLiteConnectionString(Environment.GetEnvironmentVariable("DATABASE_PATH"),
   true, key: Environment.GetEnvironmentVariable("DATABASE_PASSWORD"));


        public async Task<IList<CustomerDTO>> Get(Guid id)
        {
            IList<CustomerDTO> customers = new List<CustomerDTO>();
            try
            {
                SQLiteAsyncConnection connection = new SQLiteAsyncConnection(Options);
                {
                    customers = await connection.QueryAsync<CustomerDTO>("Select * from Customer Where ID = ?", id);

                    await connection.CloseAsync();

                    if (customers != null && customers.Count > 0)
                        foreach (var customer in customers.ToList())
                            customers.Add(customer);

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return customers;
        }

        public async Task<bool> InsertOrReplace(CustomerDTO customerDTO)
        {
            try
            {

                SQLiteAsyncConnection connection = new SQLiteAsyncConnection(Options);
                int rowsModified = await connection.InsertOrReplaceAsync(customerDTO);
                await connection.CloseAsync();
                if (rowsModified > 0) return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
        }

        public string GetHashedPassword(string phoneNumber)
        {
            try
            {

                SQLiteConnection connection = new SQLiteConnection(Options);
                var passwordsList = connection.QueryScalars<string>("Select HashedPassword from Customer where Phone = ?", phoneNumber);
                var password = passwordsList.FirstOrDefault();
                connection.Close();
                return password;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return null;
        }

        public async Task<IList<CustomerDTO>> Get(string phoneNumber)
        {
            IList<CustomerDTO> customers = new List<CustomerDTO>();
            try
            {
                SQLiteAsyncConnection connection = new SQLiteAsyncConnection(Options);
                {
                    customers = await connection.QueryAsync<CustomerDTO>("Select * from Customer Where Phone = ?", phoneNumber);

                    await connection.CloseAsync();
                    if (customers != null && customers.Count > 0)
                        foreach (var customer in customers.ToList())
                            customers.Add(customer);

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return customers;
        }
    }
}
