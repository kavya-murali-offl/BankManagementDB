using System;
using System.Threading;
using System.Threading.Tasks;

namespace BankManagementDB.Domain.UseCase
{
    public abstract class UseCaseBase<TRequest, TResponse>
    {

        protected virtual bool GetIfAvailableInCache(TRequest request, IPresenterCallback<TResponse> presenterCallback)
        {
            return false;
        }


        public void Execute(TRequest request, IPresenterCallback<TResponse> presenterCallback)
        {

            if (GetIfAvailableInCache(request, presenterCallback)) return;

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(5000);

            Task.Run(() =>
            {
                try
                {
                    Action(request, presenterCallback);
                }
                catch (TaskCanceledException taskCancelledException)
                {

                }
                catch (Exception ex)
                {
                    ZError errObj = new ZError();
                    presenterCallback?.OnFailure(errObj);
                }
            }, cancellationTokenSource.Token);
        }

        protected abstract void Action(TRequest request, IPresenterCallback<TResponse> presenterCallback);
    }
}
