using BankManagement;
using System;
using BankManagementDB.Interface;

namespace BankManagementDB.Controller
{
    public class AuthController : IValidationServices
    {
        public AuthController(IAuthenticationServices authenticationServices) {
             AuthenticationServices = authenticationServices;
        }
        
        public IAuthenticationServices AuthenticationServices { get; set; } 

        public bool ValidatePassword(string phoneNumber, string password)
        {
            try
            {

                string hashedInput = AuthServices.Encrypt(password);
                string passwordFromDB = AuthenticationServices.GetHashedPassword(phoneNumber);
                return hashedInput == passwordFromDB ? true : false;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
        }
    }
}
