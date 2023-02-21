using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.Utility
{
    public class RandomGenerator
    {
        public long GenerateCardNumber()
        {
            Random random = new Random();
            return random.Next(100000000, 999999999);
        }

        public int GeneratePin()
        {
            Random random = new Random();
            return random.Next(1000, 9999);
        }

        public int GenerateCVV()
        {
            Random random = new Random();
            return random.Next(100, 999);
        }

        public static long GenerateAccountNumber()
        {
            Random random = new Random();
            return random.Next(10000000, 999999999);
        }
    }
}
