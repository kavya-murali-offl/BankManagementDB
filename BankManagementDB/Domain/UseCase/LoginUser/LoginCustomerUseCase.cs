using BankManagementDB.Config;
using BankManagementDB.Domain.UseCase.LoginUser;
using BankManagementDB.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace BankManagementDB.Domain.UseCase.LoginCustomer
{
    public class LoginCustomerUseCase : UseCaseBase<LoginCustomerRequest, LoginCustomerResponse>
    {
        private readonly IGetCustomerCredentialsDataManager GetCustomerCredentialsDataManager = DependencyContainer.ServiceProvider.GetRequiredService<IGetCustomerCredentialsDataManager>();        private LoginCustomerRequest Request { get; set; }

        protected override void Action(LoginCustomerRequest request, IPresenterCallback<LoginCustomerResponse> presenterCallback)
        {
            GetCustomerCredentialsDataManager.GetCustomerCredentials(request, new LoginCustomerCallback<LoginCustomerResponse>(presenterCallback));
        }

        private class LoginCustomerCallback<T> : IUseCaseCallback<LoginCustomerResponse>
        {
            private IPresenterCallback<LoginCustomerResponse> PresenterCallback { get; set; }
            
            public LoginCustomerCallback(IPresenterCallback<LoginCustomerResponse> presenterCallback)
            {
                PresenterCallback = presenterCallback;
            }

            public void OnSuccess(LoginCustomerResponse loginCustomerResponse) {
                
                PresenterCallback.OnSuccess(loginCustomerResponse);
            }

            public void OnFailure(ZError error) { 
                   PresenterCallback.OnFailure(error);
            }
        }
    }
}
