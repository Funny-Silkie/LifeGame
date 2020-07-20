using Altseed2;
using Altseed2.ToolAuxiliary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LifeGame
{
    class MainScene : Node
    {
        private int count;
        private bool stopped = true;
        private int updateSpan = 5;
        public Dictionary<Vector2I, Block> Blocks { get; } = new Dictionary<Vector2I, Block>(DataBase.Size.X * DataBase.Size.Y);
        public MainScene()
        {
            for (int x = 0; x < DataBase.Size.X; x++)
                for (int y = 0; y < DataBase.Size.Y; y++)
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
            if (!stopped && count++ != 0 && count % updateSpan == 0)
            {
                foreach (var pair in Blocks)
                {
                    var block = pair.Value;
                    block.CheckCount();
                    if (block.IsAlive) liveCount++;
                }
                foreach (var pair in Blocks) pair.Value.ChangeAlive();
                tool_LifeCount.Message = $"Lives : {liveCount}";
                tool_TimeCount.Message = $"Time : {(uint)(count / updateSpan)}";
                DataBase.Data.AddLast(liveCount);
            }
        }
        #region Tool
        private Text tool_LifeCount;
        private Text tool_TimeCount;
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
            var tool_ChangeState = new Button(stopped ? (count == 0 ? "Run" :"Resume") : "Stop");
            tool_ChangeState.Clicked += new EventHandler(Tool_ChangeState);
            ToolHelper.AddComponent(tool_ChangeState);
            tool_LifeCount = new Text($"Lives : {Blocks.Sum(x => x.Value.IsAlive ? 1 : 0)}");
            ToolHelper.AddComponent(tool_LifeCount);
            tool_TimeCount = new Text($"Time : {(uint)(count / updateSpan)}");
            ToolHelper.AddComponent(tool_TimeCount);
            Register();
            IOBinary();
            var tool_Export = new Button("Export as csv");
            tool_Export.Clicked += new EventHandler(Tool_Export);
            ToolHelper.AddComponent(tool_Export);
            var tool_ToGraph = new Button("Show Graph");
            tool_ToGraph.Clicked += (x, y) => DataBase.ToGraph();
            ToolHelper.AddComponent(tool_ToGraph);
        }
        private void IOBinary()
        {
            var tree = new TreeNode("I/O");
            ToolHelper.AddComponent(tree);
            var tool_LoadBinary = new Button("Load Binary");
            tool_LoadBinary.Clicked += new EventHandler(Tool_LoadBinary);
        }
        private void Register()
        {
            var tree = new TreeNode("Dead-Alive setting");
            ToolHelper.AddComponent(tree);
            for (int i = 0; i <= 8; i++)
            {
                var value = i;
                var check_Alive = new CheckBox($"{value}-Alive", DataBase.LiveDeadTable[new Entry(value, true)]);
                var check_Dead = new CheckBox($"{value}-Dead", DataBase.LiveDeadTable[new Entry(value, false)]);
                check_Alive.ChangeChecked += (x, y) => DataBase.LiveDeadTable[new Entry(value, true)] = y.NewValue;
                check_Dead.ChangeChecked += (x, y) => DataBase.LiveDeadTable[new Entry(value, false)] = y.NewValue;
                var line = new Line();
                line.AddComponent(check_Alive);
                line.AddComponent(check_Dead);
                tree.AddComponent(line);
            }
        }
        private void Tool_Clear(object sender, EventArgs e)
        {
            count = 0;
            foreach (var pair in Blocks) pair.Value.IsAlive = false;
            DataBase.Data.Clear();
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
        private void Tool_LoadBinary(object sender, EventArgs e)
        {

        }
        private void Tool_Export(object sender, EventArgs e)
        {
            var builder = new StringBuilder();
            var head = true;
            foreach (var current in DataBase.Data)
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

}
