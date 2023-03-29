using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.Domain.UseCase
{
    public interface IUseCaseCallback<T>
    {
        void OnSuccess(T result);

        void OnFailure(ZError error);
    }
}
