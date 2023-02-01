using System;
using System.Security.Cryptography;
using System.Text;

namespace BankManagement
{
    public class AuthServices
    {
        public static string ComputeHash(string plainText)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(plainText));

                StringBuilder stringbuilder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                    stringbuilder.Append(bytes[i].ToString("x2"));
                
                return stringbuilder.ToString();
            }
        }
    }
}
