using System;

namespace LifeGame
{
    [Serializable]
    public readonly struct Entry : IEquatable<Entry>
    {
        public int LifeCount { get; }
        public bool SelfAlive { get; }
        public Entry(int lifeCount, bool selfAlive)
        {
            LifeCount = lifeCount;
            SelfAlive = selfAlive;
        }
        public bool Equals(Entry other) => LifeCount == other.LifeCount && SelfAlive == other.SelfAlive;
        public override bool Equals(object obj) => obj is Entry other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(LifeCount, SelfAlive);
        public override string ToString() => $"{LifeCount} - {(SelfAlive ? "Live" : "Dead")}";
        public static bool operator ==(Entry left, Entry right) => left.Equals(right);
        public static bool operator !=(Entry left, Entry right) => !(left == right);
    }
}
