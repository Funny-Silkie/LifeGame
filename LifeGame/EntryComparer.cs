using System.Collections;
using System.Collections.Generic;

namespace LifeGame
{
    class EntryComparer : IEqualityComparer<Entry>, IEqualityComparer
    {
        public bool Equals(Entry x, Entry y) => x == y;
        bool IEqualityComparer.Equals(object x, object y) => x is Entry ex && x is Entry ey && Equals(ex, ey);
        public int GetHashCode(Entry obj)
        {
            var result = obj.LifeCount;
            if (obj.SelfAlive) result += 9;
            return result;
        }
        int IEqualityComparer.GetHashCode(object obj) => obj switch
        {
            null => 0,
            Entry e => GetHashCode(e),
            _ => obj.GetHashCode()
        };
    }
}
