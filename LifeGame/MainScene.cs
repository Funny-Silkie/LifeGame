using Altseed2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static LifeGame.DataBase;

namespace LifeGame
{
    class MainScene : Node
    {
        private int count;
        private bool stopped;
        private int updateSpan = 10;
        private static readonly Vector2I Size = new Vector2I(32, 24);
        public Dictionary<Vector2I, Block> Blocks { get; } = new Dictionary<Vector2I, Block>(Size.X * Size.Y);
        public MainScene()
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
        protected override void OnAdded()
        {
            InitTool();
        }
        protected override void OnRemoved()
        {
            ToolHelper.ClearComponents();
        }
        protected override void OnUpdate()
        {
            var liveCount = 0;
            if (!stopped && count++ % updateSpan == 0)
            {
                foreach (var pair in Blocks)
                {
                    var block = pair.Value;
                    block.CheckCount();
                    if (block.IsAlive) liveCount++;
                }
                foreach (var pair in Blocks) pair.Value.ChangeAlive();
                tool_Count.Message = $"Lives : {liveCount}";
                Data.AddLast(liveCount);
            }
        }
        #region Tool
        private Text tool_Count;
        private void InitTool()
        {
            ToolHelper.Position = new Vector2F(960, 0);
            ToolHelper.Size = new Vector2I(300, 720);
            ToolHelper.Name = "Settings";
            ToolHelper.WindowFlags = ToolWindow.NoCollapse | ToolWindow.NoMove | ToolWindow.NoResize;
            var tool_UpdateSpan = new InputInt1("Update Span", updateSpan)
            {
                Max = 60,
                Min = 1
            };
            tool_UpdateSpan.ValueChanged += new EventHandler<ToolValueEventArgs<int>>(Tool_ChangeUpdateSpan);
            ToolHelper.AddComponent(tool_UpdateSpan);
            var tool_Clear = new Button("Clear");
            tool_Clear.Clicked += new EventHandler(Tool_Clear);
            ToolHelper.AddComponent(tool_Clear);
            var tool_ChangeState = new Button("Stop");
            tool_ChangeState.Clicked += new EventHandler(Tool_ChangeState);
            ToolHelper.AddComponent(tool_ChangeState);
            tool_Count = new Text("Lives : 0");
            ToolHelper.AddComponent(tool_Count);
            Register(false);
            Register(true);
            var tool_Export = new Button("Export");
            tool_Export.Clicked += new EventHandler(Tool_Export);
            ToolHelper.AddComponent(tool_Export);
            var tool_ToGraph = new Button("Show Graph");
            tool_ToGraph.Clicked += (x, y) => ToGraph();
            ToolHelper.AddComponent(tool_ToGraph);
        }
        private void Register(bool selfAlive)
        {
            for (int i = 0; i <= 8; i++)
            {
                var value = i;
                var check = new CheckBox($"{value}-{(selfAlive ? "Alive" : "Dead")}", LiveDeadTable[new Entry(value, selfAlive)]);
                check.ChangeChecked += (x, y) => LiveDeadTable[new Entry(value, selfAlive)] = y.NewValue;
                ToolHelper.AddComponent(check);
            }
        }
        private void Tool_Clear(object sender, EventArgs e)
        {
            count = 1;
            foreach (var pair in Blocks) pair.Value.IsAlive = false;
            Data.Clear();
        }
        private void Tool_ChangeUpdateSpan(object sender, ToolValueEventArgs<int> e)
        {
            updateSpan = e.NewValue;
        }
        private void Tool_ChangeState(object sender, EventArgs e)
        {
            stopped = !stopped;
            ((Button)sender).Label = stopped ? "Resume" : "Stop";
        }
        private void Tool_Export(object sender, EventArgs e)
        {
            var builder = new StringBuilder();
            var head = true;
            foreach (var current in Data)
            {
                if (!head) builder.Append(',');
                builder.Append(current);
                if (head) head = false;
            }
            using var writer = new StreamWriter("Data.txt", false);
            writer.Write(builder.ToString());
        }
        #endregion
    }
    class GraphNode : Node
    {
        private Node sceneNode;
        protected override void OnAdded()
        {
            InitTool();
            AddChildNode(new LineNode()
            {
                Point1 = new Vector2F(100, 120),
                Point2 = new Vector2F(100, 620),
                Thickness = 5f
            });
            AddChildNode(new LineNode()
            {
                Point1 = new Vector2F(100, 620),
                Point2 = new Vector2F(850, 620),
                Thickness = 5f
            });
            sceneNode = new Node();

            AddChildNode(sceneNode);
        }
        protected override void OnRemoved()
        {
            ToolHelper.ClearComponents();
            RemoveChildNode(sceneNode);
            sceneNode = null;
        }
        #region Tool
        private void InitTool()
        {
            ToolHelper.Position = new Vector2F(960, 0);
            ToolHelper.Size = new Vector2I(300, 720);
            ToolHelper.Name = "Settings";
            ToolHelper.WindowFlags = ToolWindow.NoCollapse | ToolWindow.NoMove | ToolWindow.NoResize;
            var tool_ToMain = new Button("Back");
            tool_ToMain.Clicked += (x, y) => ToMain();
            ToolHelper.AddComponent(tool_ToMain);
        }
        #endregion
    }
}
