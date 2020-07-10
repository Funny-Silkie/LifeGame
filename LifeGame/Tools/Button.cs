using Altseed2;
using System;

namespace LifeGame
{
    /// <summary>
    /// ボタンのツールコンポーネントのクラス
    /// </summary>
    [Serializable]
    public class Button : ButtonBase
    {
        /// <summary>
        /// 見えるかどうかを取得または設定する
        /// </summary>
        public bool IsVisible { get; set; } = true;
        /// <summary>
        /// 表示される文字列を取得または設定する
        /// </summary>
        public string Label { get; set; }
        /// <summary>
        /// サイズを取得または設定する
        /// </summary>
        public Vector2I Size { get; set; }
        /// <summary>
        /// 既定の大きさと文字列を持つ<see cref="Button"/>の新しいインスタンスを生成する
        /// </summary>
        public Button() : this(string.Empty, default) { }
        /// <summary>
        /// 既定の大きさと指定した文字列を持つ<see cref="Button"/>の新しいインスタンスを生成する
        /// </summary>
        /// <param name="label">表示される文字列</param>
        public Button(string label) : this(label, default) { }
        /// <summary>
        /// 指定した大きさと文字列を持つ<see cref="Button"/>の新しいインスタンスを生成する
        /// </summary>
        /// <param name="label">表示される文字列</param>
        /// <param name="size">サイズ</param>
        public Button(string label, Vector2I size)
        {
            Label = label;
            Size = size;
        }
        internal override void Update()
        {
            var result = IsVisible ? Engine.Tool.Button(Label ?? string.Empty, Size) : Engine.Tool.InvisibleButton(Label ?? string.Empty, Size.X == 0 || Size.Y == 0 ? new Vector2I(1, 1) :Size);
            if (result) OnClicked();
        }
    }
}
