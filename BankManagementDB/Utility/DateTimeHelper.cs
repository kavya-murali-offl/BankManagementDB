using System;

namespace BankManagementDB.Utility
{
    public static class DateTimeHelper
    {
        private static DateTime UNIX_TIME = new DateTime(1970, 1, 1);
        public static long? GetEpoch(DateTime? dateTime)
        {
            long? epoch = (long?)(dateTime - UNIX_TIME)?.TotalSeconds;
            
            if (!epoch.HasValue)
                epoch = (int?)(DateTime.Now - new DateTime(1970, 1, 1)).TotalSeconds; ;
            
            return epoch;
        }

        public static DateTime ConvertEpochToDateTime(long epoch)
        {
            return UNIX_TIME.AddSeconds(epoch);    
        }
    }
}
