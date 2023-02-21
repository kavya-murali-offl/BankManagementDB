using BankManagementCipher.Model;
using BankManagementDB.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.db
{
    public class CardOperations
    {

        private readonly SQLiteConnectionString Options = new SQLiteConnectionString(Environment.GetEnvironmentVariable("DATABASE_PATH"),
            true, key: Environment.GetEnvironmentVariable("DATABASE_PASSWORD"));

        public async Task<IList<CardDTO>> Get(Guid accountID)
        {
            IList<CardDTO> cards = new List<CardDTO>();
            try
            {
                SQLiteAsyncConnection connection = new SQLiteAsyncConnection(Options);
                {
                    var result = connection.Table<CardDTO>().Where(x => x.AccountID == accountID);
                    cards = result.ToListAsync().Result;
                    await connection.CloseAsync();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return cards;

        }

        public async Task<IList<CardDTO>> Get(long cardNumber)
        {
            IList<CardDTO> cards = new List<CardDTO>();
            try
            {
                SQLiteAsyncConnection connection = new SQLiteAsyncConnection(Options);
                {
                    var result = connection.Table<CardDTO>().Where(x => x.CardNumber == cardNumber);
                    cards = result.ToListAsync().Result;
                    await connection.CloseAsync();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return cards;

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
