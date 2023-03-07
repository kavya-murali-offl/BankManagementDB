using BankManagementDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.Controller
{
    public static class CurrentUserDataManager
    {
        public static Customer CurrentUser { get; set; }
    }
}
