using Altseed2;
using System.Collections.Generic;

namespace LifeGame
{
    static class DataBase
    {
        private static readonly MainScene mainScene;
        private static readonly GraphNode graph;
        public static LinkedList<int> Data { get; } = new LinkedList<int>();
        public static Dictionary<Entry, bool> LiveDeadTable { get; } = new Dictionary<Entry, bool>(18, new EntryComparer())
        {
            { new Entry(0, false), false }, { new Entry(0, true), false },
            { new Entry(1, false), false }, { new Entry(1, true), false },
            { new Entry(2, false), false }, { new Entry(2, true), true },
            { new Entry(3, false), true }, { new Entry(3, true), true },
            { new Entry(4, false), false }, { new Entry(4, true), false },
            { new Entry(5, false), false }, { new Entry(5, true), false },
            { new Entry(6, false), false }, { new Entry(6, true), false },
            { new Entry(7, false), false }, { new Entry(7, true), false },
            { new Entry(8, false), false }, { new Entry(8, true), false },
        };
        public static Vector2I Size { get; } = new Vector2I(32, 24);
        static DataBase()
        {
            mainScene = new MainScene();
            graph = new GraphNode();
        }
        public static void Initialize()
        {
            Engine.AddNode(mainScene);
        }
        public static void ToGraph()
        {
            Engine.RemoveNode(mainScene);
            Engine.AddNode(graph);
        }
        public static void ToMain()
        {
            Engine.RemoveNode(graph);
            Engine.AddNode(mainScene);
        }
    }
}
