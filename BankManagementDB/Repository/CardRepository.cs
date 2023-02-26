using BankManagementDB.Interface;
using BankManagementDB.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.Repository
{
    public class CardRepository : ICardRepository
    {

        private readonly static SQLiteConnectionString Options = new SQLiteConnectionString(Environment.GetEnvironmentVariable("DATABASE_PATH"),
  true, key: Environment.GetEnvironmentVariable("DATABASE_PASSWORD"));

        public async Task<IList<CardDTO>> Get(Guid customerID)
        {

            try
            {
                SQLiteAsyncConnection connection = new SQLiteAsyncConnection(Options);
                {
                    var result = connection.Table<CardDTO>().Where(x => x.CustomerID == customerID);
                    IList<CardDTO> cardDTOs = result.ToListAsync().Result;
                    await connection.CloseAsync();

                    return cardDTOs;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return new List<CardDTO>();

        }

        public async Task<IList<CardDTO>> Get(string cardNumber)
        {

            try
            {
                SQLiteAsyncConnection connection = new SQLiteAsyncConnection(Options);
                {
                    var result = connection.Table<CardDTO>().Where(x => x.CardNumber.Equals(cardNumber));
                    IList<CardDTO> cardDTOs = result.ToListAsync().Result;
                    await connection.CloseAsync();

                    return cardDTOs;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return new List<CardDTO>();

        }

        public async Task<bool> InsertOrReplace(CardDTO cardDTO)
        {
            SQLiteAsyncConnection connection = new SQLiteAsyncConnection(Options);
            int rowsModified = await connection.InsertOrReplaceAsync(cardDTO);
            await connection.CloseAsync();
            if (rowsModified > 0) return true;
            return false;
        }
}
}
