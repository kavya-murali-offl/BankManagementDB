using BankManagementDB.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.Utility
{
    internal class Formatter
    {
        public static string FormatString(string unformattedString, params object[] args)
        {
            return string.Format(unformattedString, args).Replace("\\n", "\n");
        }
    }
}
