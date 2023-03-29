using BankManagementDB.Domain.UseCase.LoginCustomer;
using BankManagementDB.Domain.UseCase.LoginUser;
using BankManagementDB.Domain.UseCase;
using BankManagementDB.Interface;
using BankManagementDB.Model;
using BankManagementDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.DataManager
{
    public class GetCustomerCredentialsDataManager : IGetCustomerCredentialsDataManager
    {
        public GetCustomerCredentialsDataManager(IDBHandler dBHandler)
        {
            DBHandler = dBHandler;
        }
        private IDBHandler DBHandler { get; set; }

        public async void GetCustomerCredentials(LoginCustomerRequest request, IUseCaseCallback<LoginCustomerResponse> callback)
        {
            List<CustomerCredentials> customerCredentialsList = await DBHandler.GetCredentials(request.CustomerID);
            var customerCredentials = customerCredentialsList.FirstOrDefault();

            if (customerCredentials != null)
            {
                string hashedInput = AuthServices.HashPassword(request.InputPassword, customerCredentials.Salt);
                bool isLoggedIn = hashedInput.Equals(customerCredentials.Password);

                if (customerCredentials != null)
                {
                    var response = new LoginCustomerResponse()
                    {
                        IsLoggedIn = isLoggedIn
                    };
                    callback.OnSuccess(response);
                }
                else
                {
                    ZError error = new ZError()
                    {
                        Type = "InvalidCredentials",
                        Message = "Invalid Credentials"
                    };
                    callback.OnFailure(error);
                }
            }
            else
            {
                ZError error = new ZError()
                {
                    Type = "UserNotFound",
                    Message = "Invalid Credentials"
                };
                callback.OnFailure(error);
            }
        }
    }
}
