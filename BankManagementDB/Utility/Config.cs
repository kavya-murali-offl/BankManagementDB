using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.Utility
{
    public class Config
    {
        public void SetVariables()
        {
            Environment.SetEnvironmentVariable("DATABASE_PATH", "Database.sqlite3");
            Environment.SetEnvironmentVariable("DATABASE_PASSWORD", "pass");
        }
    }
}
