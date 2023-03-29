using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.Domain.EventsArgs
{
    public class LoginCustomerEventArgs : PropertyChangedEventArgs
    {
        public LoginCustomerEventArgs(string propertyName) : base(propertyName)
        {

        }
    }
}
