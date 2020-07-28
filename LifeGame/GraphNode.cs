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
        private LineGraphDouble.Line rawData;
        private LineGraphDouble.Line substruct;
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
        }
        protected override void OnAdded()
        {
            rawData = graph.AddData(Array.Empty<Vector2F>());
            rawData.Color = new Color(100, 255, 100);
            substruct = graph.AddData(Array.Empty<Vector2F>());
            substruct.Color = new Color(255, 100, 100);
            var array = DataBase.Data.ToArray((x, y) => new Vector2F(x, y));
            rawData.Data = array;
            substruct.Data = CreateSubstracts(array);
            graph.MaxX = DataBase.Data.Count == 0 ? 1 : DataBase.Data.Count;
            graph.MaxY = CalcMax(array);
            InitTool();
        }
        protected override void OnRemoved()
        {
            ToolHelper.ClearComponents();
            RemoveChildNode(rawData);
            RemoveChildNode(substruct);
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
            var tool_Max = new InputInt1("Max", (int)graph.MaxY)
            {
                Min = (int)graph.MinY + 1
            };
            tool_Max.ValueChanged += new EventHandler<ToolValueEventArgs<int>>(Tool_MaxChange);
            ToolHelper.AddComponent(tool_Max);
            var tool_Min = new InputInt1("Min", (int)graph.MinY)
            {
                Max = (int)graph.MaxY - 1
            };
            tool_Min.ValueChanged += new EventHandler<ToolValueEventArgs<int>>(Tool_MinChange);
            ToolHelper.AddComponent(tool_Min);
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
