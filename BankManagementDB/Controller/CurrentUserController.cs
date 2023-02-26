using BankManagementDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.Controller
{
    public static class CurrentUserController
    {
        public static Customer CurrentUser { get; set; }
    }
}
