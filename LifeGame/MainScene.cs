using Altseed2;
using System.Collections.Generic;

namespace LifeGame
{
    class MainScene : Node
    {
        public static Dictionary<Entry, bool> Result { get; } = new Dictionary<Entry, bool>(18)
        {
            { new Entry(0, false), false },
            { new Entry(0, true), false },
            { new Entry(1, false), false },
            { new Entry(1, true), false },
            { new Entry(2, false), false },
            { new Entry(2, true), false },
            { new Entry(3, false), false },
            { new Entry(3, true), true },
            { new Entry(4, false), true },
            { new Entry(4, true), true },
            { new Entry(5, false), true },
            { new Entry(5, true), true },
            { new Entry(6, false), false },
            { new Entry(6, true), false },
            { new Entry(7, false), true },
            { new Entry(7, true), false },
            { new Entry(8, false), true },
            { new Entry(8, true), false },
        };
        private static readonly Vector2I Size = new Vector2I(32, 24);
        public Dictionary<Vector2I, Block> Blocks { get; } = new Dictionary<Vector2I, Block>(Size.X * Size.Y);
        protected override void OnAdded()
        {
            for (int x = 0; x < Size.X; x++)
                for (int y = 0; y < Size.Y; y++)
                {
                    var location = new Vector2I(x, y);
                    var block = new Block(location, this);
                    Blocks.Add(location, block);
                    AddChildNode(block);
                }
        }
    }
}
