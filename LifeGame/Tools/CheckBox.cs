using Altseed2;
using System;

namespace LifeGame
{
    /// <summary>
    /// チェックボックスのクラス
    /// </summary>
    [Serializable]
    public sealed class CheckBox : ToolComponent
    {
        /// <summary>
        /// チェックされているかどうかを取得または設定する
        /// </summary>
        public bool Checked { get; set; }
        /// <summary>
        /// 表示される文字列を取得または設定する
        /// </summary>
        public string Label { get; set; }
        public event EventHandler<ToolValueEventArgs<bool>> ChangeChecked;
        /// <summary>
        /// 既定の情報を持つ<see cref="CheckBox"/>の新しいインスタンスを生成する
        /// </summary>
        public CheckBox() : this(string.Empty, false) { }
        /// <summary>
        /// 指定した情報を持つ<see cref="CheckBox"/>の新しいインスタンスを生成する
        /// </summary>
        /// <param name="label">表示される文字列</param>
        /// <param name="check">チェックされているかどうか</param>
        public CheckBox(string label, bool check)
        {
            Checked = check;
            Label = label;
        }
        internal override void Update()
        {
            var c = Checked;
            if (!Engine.Tool.CheckBox(Label ?? string.Empty, ref c)) return;
            if (c == Checked) return;
            ChangeChecked?.Invoke(this, new ToolValueEventArgs<bool>(Checked, c));
            Checked = c;
        }
    }
}
