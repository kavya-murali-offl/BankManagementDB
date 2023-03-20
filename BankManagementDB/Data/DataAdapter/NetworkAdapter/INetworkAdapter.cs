using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.Data.Adapter.NetworkAdapter
{
    public interface INetworkAdapter
    {
        Task Get(string url);
        Task<string> Post(string url, string data);
    }
}
