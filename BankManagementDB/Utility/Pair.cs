using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankManagementDB.Utility
{
    public sealed class GenericPair<T1, T2> 
    {

        private readonly T1 first;
        private readonly T2 second;

        public GenericPair(T1 first, T2 second)
        {
            this.first = first;
            this.second = second;
        }

        public T1 Key { get { return first; } }

        public T2 Value { get { return second; } }

    }
}
