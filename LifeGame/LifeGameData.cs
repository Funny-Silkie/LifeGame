using Altseed2;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LifeGame
{
    [Serializable]
    public sealed class LifeGameData
    {
        public Dictionary<Vector2I, bool> AliveData { get; }
        public Dictionary<Entry, bool> Table { get; }
        internal LifeGameData(Dictionary<Vector2I, Block> data)
        {
            AliveData = data.ToDictionary(x => x.Key, x => x.Value.IsAlive);
            Table = new Dictionary<Entry, bool>(18);
            foreach (var pair in DataBase.LiveDeadTable) Table.Add(pair.Key, pair.Value);
        }
    }
}
