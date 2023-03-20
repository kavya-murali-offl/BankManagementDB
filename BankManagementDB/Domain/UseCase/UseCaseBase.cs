using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BankManagementDB.Domain.UseCase
{
    public abstract class UseCaseBase
    {

        public bool GetIfAvailableInCache()
        {
            return false;
        }


        public void Execute()
        {

            if (GetIfAvailableInCache()) return;

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(5000);

            Task.Run(delegate()
            {
                try
                {
                    Action();
                }
                catch(TaskCanceledException taskCancelledException)
                {
                   
                }
                catch (Exception ex)
                {
                    //            ZError errObj = new
                    //                    Error(ErrorType.Unknown, "Error message");
                    //PresenterCallback?.OnError(errObj);
                }

            }, cancellationTokenSource.Token);
        }

        public abstract void Action();
    }
}
