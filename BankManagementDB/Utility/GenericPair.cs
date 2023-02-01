using System;

namespace BankManagementDB.Utility
{
    public sealed class GenericPair<T1, T2> 
    {

        private readonly T1 first;
        private T2 second;

        public GenericPair(T1 first, T2 second)
        {
            this.first = first;
            this.second = second;
        }

        public T1 Key { get { return first; } }

        public T2 Value { get { return second; } set { second = value; } }

        public override string ToString() => $"{Key}: {Value}";       

    }
}
