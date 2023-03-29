using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.Domain.UseCase.LoginUser
{
    public class LoginCustomerRequest
    {
        public LoginCustomerRequest() { }

        public string CustomerID { get; set; }  

        public string InputPassword { get; set; }  
    }
}
