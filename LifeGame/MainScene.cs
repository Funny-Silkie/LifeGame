using Altseed2;
using System;
using System.Collections.Generic;

namespace LifeGame
{
    class MainScene : Node
    {
        private int count;
        private bool stopped;
        private int updateSpan = 10;
        public static Dictionary<Entry, bool> LiveDeadTable { get; } = new Dictionary<Entry, bool>(18)
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
        private static readonly Vector2I Size = new Vector2I(32, 24);
        public Dictionary<Vector2I, Block> Blocks { get; } = new Dictionary<Vector2I, Block>(Size.X * Size.Y);
        protected override void OnAdded()
        {
            InitTool();
            for (int x = 0; x < Size.X; x++)
                for (int y = 0; y < Size.Y; y++)
                {
                    var location = new Vector2I(x, y);
                    var block = new Block(location, this);
                    Blocks.Add(location, block);
                    AddChildNode(block);
                }
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
        }
        private void Tool_Clear(object sender, EventArgs e)
        {
            count = 1;
            foreach (var pair in Blocks) pair.Value.IsAlive = false;
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
        #endregion
    }
}
