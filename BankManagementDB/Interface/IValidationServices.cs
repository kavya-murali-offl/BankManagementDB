﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.Interface
{
    public interface IValidationServices
    {
        bool ValidatePassword(string phone, string password);
    }
}
