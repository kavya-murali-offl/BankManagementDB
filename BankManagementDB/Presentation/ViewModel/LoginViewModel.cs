using BankManagementDB.Config;
using BankManagementDB.Domain.UseCase;
using BankManagementDB.Domain.UseCase.LoginCustomer;
using BankManagementDB.Domain.UseCase.LoginUser;
using BankManagementDB.Events;
using BankManagementDB.Interface;
using BankManagementDB.Model;
using BankManagementDB.Models;
using BankManagementDB.Presentation.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace BankManagementDB.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        public bool IsLoggedIn;
       
        public Customer GetCustomerByPhone(string phoneNumber)
        {
            IGetCustomerDataManager getCustomerDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IGetCustomerDataManager>();
            return getCustomerDataManager.GetCustomer(phoneNumber);
        }

        public async Task LoginUser(string customerID, string password) 
        {
            LoginCustomerPresenterCallback presenterCallback = new LoginCustomerPresenterCallback(this);
            LoginCustomerRequest loginCustomerRequest = new LoginCustomerRequest()
            {
                CustomerID = customerID,
                InputPassword = password
            };
            UseCaseBase<LoginCustomerRequest, LoginCustomerResponse> loginCustomerUseCase = new LoginCustomerUseCase();
            
            loginCustomerUseCase.Execute(loginCustomerRequest, presenterCallback);
           
        }

    }
}
