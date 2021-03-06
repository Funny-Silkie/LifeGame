﻿using Altseed2;
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
        private int updateSpan = 1;
        public Dictionary<Vector2I, Block> Blocks { get; } = new Dictionary<Vector2I, Block>(DataBase.Size.X * DataBase.Size.Y);
        public MainScene()
        {
            for (int x = 0; x < DataBase.Size.X; x++)
                for (int y = 0; y < DataBase.Size.Y; y++)
                {
                    var scale = new Vector2F(32, 24) / DataBase.Size;
                    var location = new Vector2I(x, y);
                    var block = new Block(location, this)
                    {
                        Scale = scale
                    };
                    block.Position *= scale;
                    Blocks.Add(location, block);
                    AddChildNode(block);
                }
            InitTool();
        }
        protected override void OnUpdate()
        {
            if (!stopped && count++ != 0 && count % updateSpan == 0)
            {
                var liveCount = 0;
                foreach (var pair in Blocks)
                {
                    var block = pair.Value;
                    block.CheckCount();
                    if (block.IsAlive) liveCount++;
                }
                foreach (var pair in Blocks) pair.Value.ChangeAlive();
                tool_LifeCount.Message = $"Lives : {liveCount}";
                tool_TimeCount.Message = $"Generation : {(uint)(count / updateSpan)}";
                DataBase.Data.AddLast(liveCount);
                if (liveCount == 0) StopOrResume();
            }
        }
        public void SetIsDrawn(bool value)
        {
            foreach (var node in EnumerateDescendants<DrawnNode>()) node.IsDrawn = value;
        }
        private void StopOrResume()
        {
            stopped = !stopped;
            tool_ChangeState.Label = stopped ? "Resume" : "Stop";
        }
        #region Tool
        private Button tool_ChangeState;
        private Text tool_LifeCount;
        private Text tool_TimeCount;
        private Text tool_load_Error;
        private Text tool_save_Error;
        private Dictionary<Entry, CheckBox> buttons;
        public Group Group { get; } = new Group();
        private void InitTool()
        {
            ToolHelper.Position = new Vector2F(960, 0);
            ToolHelper.Size = new Vector2I(300, 720);
            ToolHelper.Name = "Settings";
            ToolHelper.WindowFlags = ToolWindowFlags.NoCollapse | ToolWindowFlags.NoMove | ToolWindowFlags.NoResize;
            var tool_UpdateSpan = new InputInt1("Update Span", updateSpan)
            {
                Max = 60,
                Min = 1
            };
            tool_UpdateSpan.ValueChanged += new EventHandler<ToolValueEventArgs<int>>(Tool_ChangeUpdateSpan);
            Group.AddComponent(tool_UpdateSpan);
            var tool_Clear = new Button("Clear");
            tool_Clear.Clicked += new EventHandler(Tool_Clear);
            Group.AddComponent(tool_Clear);
            tool_ChangeState = new Button(stopped ? (count == 0 ? "Run" :"Resume") : "Stop");
            tool_ChangeState.Clicked += new EventHandler(Tool_ChangeState);
            Group.AddComponent(tool_ChangeState);
            tool_LifeCount = new Text($"Lives : {Blocks.Sum(x => x.Value.IsAlive ? 1 : 0)}");
            Group.AddComponent(tool_LifeCount);
            tool_TimeCount = new Text($"Generation : {(uint)(count / updateSpan)}");
            Group.AddComponent(tool_TimeCount);
            Register();
            IOBinary();
            var tool_ToGraph = new Button("Show Graph");
            tool_ToGraph.Clicked += (x, y) => DataBase.ToGraph();
            Group.AddComponent(tool_ToGraph);
        }
        private void IOBinary()
        {
            var tree = new TreeNode("I/O")
            {
                FrameType = IToolTreeNode.TreeNodeFrameType.Framed
            };
            var line_Load = new Line();
            var tool_LoadBinary = new Button("Load Binary");
            tool_LoadBinary.Clicked += new EventHandler(Tool_LoadBinary);
            line_Load.AddComponent(tool_LoadBinary);
            tool_load_Error = new Text() { IsUpdated = false };
            line_Load.AddComponent(tool_load_Error);
            tree.AddComponent(line_Load);
            var line_Save = new Line();
            var tool_SaveBinary = new Button("Save Binary");
            tool_SaveBinary.Clicked += new EventHandler(Tool_SaveBinary);
            line_Save.AddComponent(tool_SaveBinary);
            tool_save_Error = new Text() { IsUpdated = false };
            line_Save.AddComponent(tool_save_Error);
            var tool_Export = new Button("Export as csv");
            tree.AddComponent(line_Save);
            tool_Export.Clicked += new EventHandler(Tool_Export);
            tree.AddComponent(tool_Export);
            Group.AddComponent(tree);
        }
        private void Register()
        {
            buttons = new Dictionary<Entry, CheckBox>(18);
            var tree = new TreeNode("Dead-Alive setting")
            {
                DefaultOpened = true,
                FrameType = IToolTreeNode.TreeNodeFrameType.Framed
            };
            Group.AddComponent(tree);
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
                buttons.Add(new Entry(i, true), check_Alive);
                buttons.Add(new Entry(i, false), check_Dead);
                tree.AddComponent(line);
            }
        }
        private void Tool_Clear(object sender, EventArgs e)
        {
            count = 0;
            foreach (var pair in Blocks) pair.Value.IsAlive = false;
            DataBase.Data.Clear();
            tool_ChangeState.Label = "Run";
            tool_LifeCount.Message = $"Lives : {Blocks.Sum(x => x.Value.IsAlive ? 1 : 0)}";
            tool_TimeCount.Message = $"Generation : 0";
            stopped = true;
        }
        private void Tool_ChangeUpdateSpan(object sender, ToolValueEventArgs<int> e)
        {
            updateSpan = e.NewValue;
        }
        private void Tool_ChangeState(object sender, EventArgs e) => StopOrResume();
        private void Tool_LoadBinary(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog()
            {
                CheckFileExists = true,
                CheckPathExists = true,
                Filter = "lb",
                InitialDirectory = Environment.CurrentDirectory
            };
            var result = dialog.ShowDialog(out var log);
            tool_load_Error.IsUpdated = !result && !string.IsNullOrEmpty(log);
            tool_load_Error.Message = log;
            if (!result) return;
            var blocks = DataBase.Deserialize(dialog.FileName);
            Tool_Clear(this, e);
            foreach (var pair in blocks) Blocks[pair.Key].IsAlive = pair.Value;
            foreach (var pair in DataBase.LiveDeadTable) buttons[pair.Key].Checked = pair.Value;
        }
        private void Tool_SaveBinary(object sender, EventArgs e)
        {
            var dialog = new SaveFileDialog()
            {
                AddExtension = true,
                CheckPathExists = true,
                DefaultExtension = "lb",
                Filter = "lb",
                InitialDirectory = Environment.CurrentDirectory
            };
            var result = dialog.ShowDialog(out var log);
            tool_save_Error.IsUpdated = !result && !string.IsNullOrEmpty(log);
            tool_save_Error.Message = log;
            if (!result) return;
            DataBase.Serialize(Blocks, dialog.FileName);
        }
        private void Tool_Export(object sender, EventArgs e)
        {
            var builder = new StringBuilder();
            var head = true;
            foreach (var current in DataBase.Data)
            {
                if (!head) builder.Append('\n');
                builder.Append(current);
                if (head) head = false;
            }
            using var writer = new StreamWriter("Data.csv", false);
            writer.Write(builder.ToString());
        }
        #endregion
    }
}
