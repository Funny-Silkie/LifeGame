using Altseed2;
using Altseed2.ToolAuxiliary;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace LifeGame
{
    static class DataBase
    {
        private static MainScene mainScene;
        private static GraphNode graph;
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
        public static Vector2I Size { get; private set; } = new Vector2I(32, 24);
        public static void Initialize(string[] args = null)
        {
            if (args != null && args.Length == 2 && int.TryParse(args[0], out var x) && int.TryParse(args[1], out var y)) Size = new Vector2I(x, y);
            mainScene = new MainScene();
            graph = new GraphNode();
            Engine.AddNode(mainScene);
            Engine.AddNode(graph);
            Engine.DoEvents();
            Engine.Update();
            ToMain();
        }
        public static void ToGraph()
        {
            Engine.Pause(graph);
            graph.SetIsDrawn(true);
            mainScene.SetIsDrawn(false);
            ToolHelper.ClearComponents();
            ToolHelper.AddComponent(graph.Group);
        }
        public static void ToMain()
        {
            Engine.Pause(mainScene);
            mainScene.SetIsDrawn(true);
            graph.SetIsDrawn(false);
            ToolHelper.ClearComponents();
            ToolHelper.AddComponent(mainScene.Group);
        }
        public static void Serialize(Dictionary<Vector2I, Block> blocks, string path)
        {
            var data = new LifeGameData(blocks);
            var formatter = new BinaryFormatter();
            using var stream = new FileStream(path, FileMode.Create);
            formatter.Serialize(stream, data);
        }
        public static Dictionary<Vector2I, bool> Deserialize(string path)
        {
            var formatter = new BinaryFormatter();
            using var stream = new FileStream(path, FileMode.Open);
            var data = (LifeGameData)formatter.Deserialize(stream);
            foreach (var pair in data.Table) LiveDeadTable[pair.Key] = pair.Value;
            return data.AliveData;
        }
    }
}
