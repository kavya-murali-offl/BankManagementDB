using BankManagementDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.Interface
{
    public interface IValidatePasswordDataManager
    {
        bool ValidatePassword(Guid customerID, string password);
    }
}
