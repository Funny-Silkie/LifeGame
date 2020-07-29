using Altseed2;
using Altseed2.Stastics;
using Altseed2.ToolAuxiliary;
using System;
using System.Collections.Generic;

namespace LifeGame
{
    class GraphNode : Node
    {
        private readonly LineGraphDouble graph;
        private readonly LineGraphDouble.Line rawData;
        private readonly LineGraphDouble.Line substruct;
        public GraphNode()
        {
            graph = new LineGraphDouble()
            {
                GraphArea = new RectF(100, 100, 760, 520),
                LabelX = "Generation",
                LabelY = "Lives",
                MaxX = 1,
                MaxY = 10,
                Size = new Vector2F(960, 720),
            };
            AddChildNode(graph);
            rawData = graph.AddData(Array.Empty<Vector2F>());
            rawData.Color = new Color(100, 255, 100);
            substruct = graph.AddData(Array.Empty<Vector2F>());
            substruct.Color = new Color(255, 100, 100);
            InitTool();
        }
        private static float CalcMax(IEnumerable<Vector2F> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source), "引数がnullです");
            var result = 1f;
            foreach (var current in source)
                if (result < current.Y)
                    result = current.Y;
            return result;
        }
        private static Vector2F[] CreateSubstracts(ICollection<Vector2F> data)
        {
            if (data.Count <= 1) return Array.Empty<Vector2F>();
            var result = new Vector2F[data.Count];
            var pre = 0f;
            var i = 0;
            foreach (var current in data)
            {
                result[i++] = new Vector2F(current.X, current.Y - pre);
                pre = current.Y;
            }
            return result;
        }
        private static Vector2F[] CreateSum(ICollection<Vector2F> data)
        {
            var result = new Vector2F[data.Count];
            var i = 0;
            var sum = 0f;
            foreach (var current in data)
            {
                sum += current.Y;
                result[i++] = new Vector2F(current.X, sum);
            }
            return result;
        }
        #region Tool
        private readonly Dictionary<int, LineGraphDouble.Line> movingAves = new Dictionary<int, LineGraphDouble.Line>();
        private InputInt1 tool_Max;
        private InputInt1 tool_Min;
        private InputInt1 tool_MA_Count;
        private ColorEdit tool_MA_LineColor;
        public Group Group { get; } = new Group();
        private readonly Group graphButtonGroup = new Group();
        private void InitTool()
        {
            ToolHelper.Position = new Vector2F(960, 0);
            ToolHelper.Size = new Vector2I(300, 720);
            ToolHelper.Name = "Settings";
            ToolHelper.WindowFlags = ToolWindowFlags.NoCollapse | ToolWindowFlags.NoMove | ToolWindowFlags.NoResize;
            var tree_GraphButtons = new TreeNode("Lines")
            {
                DefaultOpened = true,
                FrameType = IToolTreeNode.TreeNodeFrameType.Framed
            };
            Group.AddComponent(tree_GraphButtons);
            var tool_RawData = new CheckBox("Raw Data", true);
            tool_RawData.ChangeChecked += new EventHandler<ToolValueEventArgs<bool>>(Tool_RawData);
            tree_GraphButtons.AddComponent(tool_RawData);
            var tool_Substract = new CheckBox("Substract", true);
            tool_Substract.ChangeChecked += new EventHandler<ToolValueEventArgs<bool>>(Tool_Substract);
            tree_GraphButtons.AddComponent(tool_Substract);
            tree_GraphButtons.AddComponent(graphButtonGroup);
            MovingAverages();
            var tool_ToMain = new Button("Back");
            tool_ToMain.Clicked += (x, y) => DataBase.ToMain();
            Group.AddComponent(tool_ToMain);
            tool_Max = new InputInt1("Max", (int)graph.MaxY)
            {
                Min = (int)graph.MinY + 1
            };
            tool_Max.ValueChanged += new EventHandler<ToolValueEventArgs<int>>(Tool_MaxChange);
            Group.AddComponent(tool_Max);
            tool_Min = new InputInt1("Min", (int)graph.MinY)
            {
                Max = (int)graph.MaxY - 1
            };
            tool_Min.ValueChanged += new EventHandler<ToolValueEventArgs<int>>(Tool_MinChange);
            Group.AddComponent(tool_Min);
        }
        private void MovingAverages()
        {
            var tree = new TreeNode("Moving Average")
            {
                DefaultOpened = true,
                FrameType = IToolTreeNode.TreeNodeFrameType.Framed
            };
            Group.AddComponent(tree);
            tool_MA_Count = new InputInt1("Count", 1)
            {
                Max = 2,
                Min = 1
            };
            tree.AddComponent(tool_MA_Count);
            tool_MA_LineColor = new ColorEdit("Color", new Color(100, 100, 255))
            {
                EditAlpha = false
            };
            tree.AddComponent(tool_MA_LineColor);
            var button_Do = new Button("Create Line");
            button_Do.Clicked += new EventHandler(Tool_MA_Button_Do);
            tree.AddComponent(button_Do);
        }
        public void SetIsDrawn(bool value)
        {
            foreach (var node in EnumerateDescendants<DrawnNode>()) node.IsDrawn = value;
            if (value)
            {
                var array = DataBase.Data.ToArray((x, y) => new Vector2F(x, y));
                rawData.Data = array;
                substruct.Data = CreateSubstracts(array);
                if (graph.MaxY <= 0)
                {
                    graph.MinY = 0;
                    graph.MaxX = DataBase.Data.Count <= 1 ? 1 : DataBase.Data.Count - 1;
                    graph.MaxY = CalcMax(array);
                }
                else
                {
                    graph.MaxX = DataBase.Data.Count <= 1 ? 1 : DataBase.Data.Count - 1;
                    graph.MaxY = CalcMax(array);
                    graph.MinY = 0;
                }
                tool_Max.Value = (int)graph.MaxY;
                tool_Max.Min = 0;
                tool_Min.Value = (int)graph.MinY;
                tool_Min.Max = 0;
                tool_MA_Count.Max = DataBase.Data.Count <= 0 ? 1 : DataBase.Data.Count;
            }
            else
            {
                graphButtonGroup.ClearComponents();
                foreach (var pair in movingAves) System.Diagnostics.Debug.WriteLine(graph.RemoveData(pair.Value));
                movingAves.Clear();
            }
        }
        private void Tool_MaxChange(object sender, ToolValueEventArgs<int> e)
        {
            graph.MaxY = e.NewValue;
            ((InputInt1)sender).Min = (int)graph.MinY + 1;
        }
        private void Tool_MinChange(object sender, ToolValueEventArgs<int> e)
        {
            graph.MinY = e.NewValue;
            ((InputInt1)sender).Max = (int)graph.MaxY - 1;
        }
        private void Tool_MA_Button_Do(object sender, EventArgs e)
        {
            var count = tool_MA_Count.Value;
            if (movingAves.ContainsKey(count) || DataBase.Data.Count <= 0) return;
            var array = new Vector2F[DataBase.Data.Count - count];
            var rawData = this.rawData.Data;
            var current = 0f;
            for (int i = 0; i < count - 1; i++) current += rawData[i].Y;
            for (int i = count; i < rawData.Length; i++)
            {
                current += rawData[i].Y;
                array[i - count] = new Vector2F(i - count / 2, current / count);
                current -= rawData[i - count].Y;
            }
            var graphLine = graph.AddData(array);
            var color = tool_MA_LineColor.Color;
            graphLine.Color = color;
            movingAves.Add(count, graphLine);
            var button = new CheckBox(count.ToString(), true);
            button.ChangeChecked += (x, y) =>
            {
                graphLine.Color = y.NewValue ? color : default;
            };
            var lineComponent = new Line();
            lineComponent.AddComponent(button);
            lineComponent.AddComponent(new ColorButton(count.ToString(), color, new Vector2F(15f, 15f)));
            graphButtonGroup.AddComponent(lineComponent);
        }
        private void Tool_RawData(object sender, ToolValueEventArgs<bool> e)
        {
            rawData.Color = e.NewValue ? new Color(100, 255, 100) : default;
        }
        private void Tool_Substract(object sender, ToolValueEventArgs<bool> e)
        {
            substruct.Color = e.NewValue ? new Color(255, 100, 100) : default;
        }
        #endregion
    }
    public static class DataHelper
    {
        public static TResult[] ToArray<TSource, TResult>(this LinkedList<TSource> list, Func<int, TSource, TResult> converter)
        {
            if (list == null) throw new ArgumentNullException(nameof(list), "引数がnullです");
            if (converter == null) throw new ArgumentNullException(nameof(converter), "引数がnullです");
            if (list.Count == 0) return Array.Empty<TResult>();
            var result = new TResult[list.Count];
            var i = 0;
            foreach (var current in list) result[i] = converter.Invoke(i++, current);
            return result;
        }
        public static T[] ToArray<T>(this LinkedList<T> list)
        {
            if (list == null) throw new ArgumentNullException(nameof(list), "引数がnullです");
            if (list.Count == 0) return Array.Empty<T>();
            var result = new T[list.Count];
            list.CopyTo(result, 0);
            return result;
        }
    }
}
