using BankManagementDB.Domain.UseCase;
using BankManagementDB.Domain.UseCase.LoginCustomer;
using BankManagementDB.Domain.UseCase.LoginUser;
using BankManagementDB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.Interface
{
    public interface IGetCustomerCredentialsDataManager
    {
        void GetCustomerCredentials(LoginCustomerRequest request, IUseCaseCallback<LoginCustomerResponse> callback);
    }
}

