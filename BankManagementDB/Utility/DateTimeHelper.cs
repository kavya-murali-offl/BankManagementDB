using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.Utility
{
    public static class DateTimeHelper
    {
        public static long GetEpoch(DateTime dateTime)
        {
            return dateTime.ToUniversalTime().Ticks / TimeSpan.TicksPerSecond;
        }
    }
}
