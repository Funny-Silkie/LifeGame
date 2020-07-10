using Altseed2;
using System;
using System.Collections.ObjectModel;

namespace LifeGame
{
    /// <summary>
    /// <see cref="Tool"/>の補助を行うクラス
    /// </summary>
    public static class ToolHelper
    {
        /// <summary>
        /// 格納されている<see cref="ToolComponent"/>を取得する
        /// </summary>
        public static ReadOnlyCollection<ToolComponent> Components => container.AsReadOnly();
        private readonly static ComponentContainer<ToolComponent> container = new ComponentContainer<ToolComponent>();
        /// <summary>
        /// 使用するメニューバーを取得または設定する
        /// </summary>
        public static MenuBar MenuBar { get; set; }
        /// <summary>
        /// ウィンドウにつけられる名前を取得または設定する
        /// </summary>
        public static string Name { get; set; }
        /// <summary>
        /// ウィンドウの座標を取得または設定する
        /// </summary>
        public static Vector2F Position { get; set; }
        /// <summary>
        /// ウィンドウの大きさを取得または設定する
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">設定しようとした値の成分が0未満</exception>
        public static Vector2I Size
        {
            get => _size;
            set
            {
                if (_size == value) return;
                if (value.X < 0 || value.Y < 0) throw new ArgumentOutOfRangeException(nameof(value), "設定しようとした値の成分が負の値です");
                _size = value;
            }
        }
        private static Vector2I _size = new Vector2I(100, 100);
        /// <summary>
        /// ツールウィンドウにおける設定を取得または設定する
        /// </summary>
        public static ToolWindow WindowFlags { get; set; }
        /// <summary>
        /// コンポーネントを追加する
        /// </summary>
        /// <param name="component">追加するコンポーネント</param>
        /// <exception cref="ArgumentNullException"><paramref name="component"/>がnull</exception>
        /// <returns><paramref name="component"/>を重複なく追加出来たらtrue，それ以外でfalse</returns>
        public static bool AddComponent(ToolComponent component) => container.Add(component);
        /// <summary>
        /// 登録されているコンポーネントを全て削除する
        /// </summary>
        public static void ClearComponents() => container.Clear();
        private static ToolWindow GetFlags()
        {
            var result = WindowFlags;
            if (MenuBar != null) result |= ToolWindow.MenuBar;
            return result;
        }
        /// <summary>
        /// コンポーネントを削除する
        /// </summary>
        /// <param name="component">削除するコンポーネント</param>
        /// <exception cref="ArgumentNullException"><paramref name="component"/>がnull</exception>
        /// <returns><paramref name="component"/>を削除出来たらtrue，それ以外でfalse</returns>
        public static bool RemoveComponent(ToolComponent component) => container.Remove(component);
        /// <summary>
        /// ツールを全て描画する
        /// </summary>
        public static void Update()
        {
            Engine.Tool.SetNextWindowSize(_size, ToolCond.None);
            Engine.Tool.SetNextWindowPos(Position, ToolCond.None);
            if (!Engine.Tool.Begin(Name ?? string.Empty, GetFlags())) return;
            MenuBar?.Update();
            for (int i = 0; i < container.Count; i++) container[i].Update();
            Engine.Tool.End();
        }
    }
}
