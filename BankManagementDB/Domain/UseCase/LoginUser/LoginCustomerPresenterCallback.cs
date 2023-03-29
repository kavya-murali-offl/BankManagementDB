using BankManagementDB.Config;
using BankManagementDB.Events;
using BankManagementDB.ViewModel;
using Microsoft.Extensions.DependencyInjection;

namespace BankManagementDB.Domain.UseCase.LoginCustomer
{
    public class LoginCustomerPresenterCallback : IPresenterCallback<LoginCustomerResponse>
    {

        private LoginViewModel LoginViewModel { get; set; }

        public LoginCustomerPresenterCallback(LoginViewModel loginCustomerViewModel) {
            LoginViewModel = loginCustomerViewModel;
        }

        public void OnSuccess(LoginCustomerResponse response)
        {
            AppEvents appEvents = DependencyContainer.ServiceProvider.GetRequiredService<AppEvents>();
            appEvents.OnSuccessfulLogin(response.IsLoggedIn);
            LoginViewModel.IsLoggedIn =  response.IsLoggedIn;
        }

        public void OnFailure(ZError response)
        {
        }
    }
}
