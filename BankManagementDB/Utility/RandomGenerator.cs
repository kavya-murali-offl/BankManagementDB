using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.Utility
{
    public class RandomGenerator
    {
        static Random random = new Random();

         public static string GenerateCardNumber()
         {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < 16; i++)
            {
                int digit = random.Next(0, 10);
                builder.Append(digit);

                if (i == 3 || i == 7 || i == 11)
                {
                    builder.Append("-");
                }
            }

            return builder.ToString();
         }

        public static string GeneratePin()
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < 4; i++)
            {
                int digit = random.Next(0, 10);
                builder.Append(digit);
            }

            return builder.ToString();
        }

        public static string GenerateCVV()
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < 3; i++)
            {
                int digit = random.Next(0, 10);
                builder.Append(digit);
            }

            return builder.ToString();
        }

        public static string GenerateAccountNumber()
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < 12; i++)
            {
                int digit = random.Next(0, 10);
                builder.Append(digit);
            }

            return builder.ToString();
        }
    }
}
