using Altseed2;
using Altseed2.ToolAuxiliary;
using System;
using System.Collections.Generic;

namespace LifeGame
{
    class GraphNode : Node
    {
        private Node rawData;
        private Node substruct;
        protected override void OnAdded()
        {
            InitTool();
            AddChildNode(new LineNode()
            {
                Position = new Vector2F(100, 120),
                Point1 = new Vector2F(100, 120),
                Point2 = new Vector2F(100, 620),
                Thickness = 5f,
                ZOrder = -1
            });
            AddChildNode(new LineNode()
            {
                Position = new Vector2F(100, 620),
                Point1 = new Vector2F(100, 620),
                Point2 = new Vector2F(850, 620),
                Thickness = 5f,
                ZOrder = -1
            });
            rawData = new Node();
            substruct = new Node();
            PlotData(rawData, DataBase.Data, new Color(100, 255, 100));
            PlotData(substruct, CreateSubstracts(DataBase.Data), new Color(255, 100, 100));
            AddChildNode(rawData);
            AddChildNode(substruct);
        }
        protected override void OnRemoved()
        {
            ToolHelper.ClearComponents();
            RemoveChildNode(rawData);
            RemoveChildNode(substruct);
            rawData = null;
            substruct = null;
        }
        private static int[] CreateSubstracts(ICollection<int> data)
        {
            if (data.Count <= 1) return Array.Empty<int>();
            var result = new int[data.Count];
            var pre = 0;
            var i = 0;
            foreach (var current in data)
            {
                result[i++] = current - pre;
                pre = current;
            }
            return result;
        }
        private static int[] CreateSum(ICollection<int> data)
        {
            var result = new int[data.Count];
            var i = 0;
            var sum = 0;
            foreach (var current in data)
            {
                sum += current;
                result[i++] = sum;
            }
            return result;
        }
        private static void PlotData(Node registeredNode, ICollection<int> data, Color color)
        {
            var cellCount = DataBase.Size.X * DataBase.Size.Y;
            if (data.Count > 0)
            {
                var positions = new Vector2F[data.Count];
                var interval = 750f / data.Count;
                var i = 0;
                foreach (var current in data) positions[i++] = new Vector2F(100 + interval * i, 620f - current * 500f / cellCount);
                for (i = 1; i < data.Count; i++)
                {
                    var node = new LineNode()
                    {
                        Color = color,
                        Position = GetSmallPos(positions[i - 1], positions[i]),
                        Point1 = positions[i - 1],
                        Point2 = positions[i],
                        Thickness = 2f
                    };
                    registeredNode.AddChildNode(node);
                }
            }
        }
        static Vector2F GetSmallPos(Vector2F left, Vector2F right) => new Vector2F(MathF.Min(left.X, right.X), MathF.Min(left.Y, right.Y));
        #region Tool
        private void InitTool()
        {
            ToolHelper.Position = new Vector2F(960, 0);
            ToolHelper.Size = new Vector2I(300, 720);
            ToolHelper.Name = "Settings";
            ToolHelper.WindowFlags = ToolWindowFlags.NoCollapse | ToolWindowFlags.NoMove | ToolWindowFlags.NoResize;
            var tool_RawData = new CheckBox("Raw Data", true);
            tool_RawData.ChangeChecked += new EventHandler<ToolValueEventArgs<bool>>(Tool_RawData);
            ToolHelper.AddComponent(tool_RawData);
            var tool_Substract = new CheckBox("Substract", true);
            tool_Substract.ChangeChecked += new EventHandler<ToolValueEventArgs<bool>>(Tool_Substract);
            ToolHelper.AddComponent(tool_Substract);
            var tool_ToMain = new Button("Back");
            tool_ToMain.Clicked += (x, y) => DataBase.ToMain();
            ToolHelper.AddComponent(tool_ToMain);
        }
        private void Tool_RawData(object sender, ToolValueEventArgs<bool> e)
        {
            if (e.NewValue) AddChildNode(rawData);
            else RemoveChildNode(rawData);
        }
        private void Tool_Substract(object sender, ToolValueEventArgs<bool> e)
        {
            if (e.NewValue) AddChildNode(substruct);
            else RemoveChildNode(substruct);
        }
        #endregion
    }
}
