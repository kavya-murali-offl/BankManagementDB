using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagement.View
{
    public class MessageView
    {
        public void WarningMessage(string Message)
        {
            Console.WriteLine("Warning: " + Message);
        }
    }
}
