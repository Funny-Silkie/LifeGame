using Altseed2;
using System;

namespace LifeGame
{
    /// <summary>
    /// ツールコンポーネントのテキストのクラス
    /// </summary>
    [Serializable]
    public sealed class Text : ToolComponent
    {
        /// <summary>
        /// 色を取得または設定する
        /// </summary>
        public Color Color { get; set; }
        /// <summary>
        /// 表示される文字列を取得または設定する
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 既定の文字列と色を持つ<see cref="Text"/>の新しいインスタンスを生成する
        /// </summary>
        public Text() : this(string.Empty, new Color(255, 255, 255)) { }
        /// <summary>
        /// 指定した文字列と既定の色を持つ<see cref="Text"/>の新しいインスタンスを生成する
        /// </summary>
        /// <param name="message">表示する文字列</param>
        public Text(string message) : this(message, new Color(255, 255, 255)) { }
        /// <summary>
        /// 指定した文字列と色を持つ<see cref="Text"/>の新しいインスタンスを生成する
        /// </summary>
        /// <param name="message">表示する文字列</param>
        /// <param name="color">色</param>
        public Text(string message, Color color)
        {
            Message = message;
            Color = color;
        }
        internal override void Update()
        {
            Engine.Tool.TextColored(Color, Message);
        }
    }
}
