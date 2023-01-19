using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.Utility
{
    public class GenericKeyValuePairs<GenericPair>
    {
        private readonly List<GenericPair> pairs;

        public GenericKeyValuePairs() { 
            this.pairs = new List<GenericPair>();
        }

        public void Add(GenericPair item) { 
             this.pairs.Add(item);
        }

    }
}
