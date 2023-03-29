using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.Events
{


    public class AppEvents
    {
        public event Action<bool> IsLoggedIn;

        public void OnSuccessfulLogin(bool IsLoginSuccess){
            IsLoggedIn?.Invoke(IsLoginSuccess);
    }
    }
}
