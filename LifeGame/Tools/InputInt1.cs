using Altseed2;
using System;

namespace LifeGame
{
    /// <summary>
    /// <see cref="int"/>型の数字を1つ格納するツールコンポーネントのクラス
    /// </summary>
    [Serializable]
    public sealed class InputInt1 : ToolComponent
    {
        /// <summary>
        /// 最大値を取得または設定する
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">設定しようとした値が<see cref="Min"/>未満</exception>
        public int Max
        {
            get => _max;
            set
            {
                if (_max == value) return;
                if (value < _min) throw new ArgumentOutOfRangeException(nameof(value), $"設定しようとした値が{nameof(Min)}未満です\n許容される範囲：{_min}～int.MaxValue({int.MaxValue})\n実際の値：{value}");
                _max = value;
            }
        }
        private int _max = int.MaxValue;
        /// <summary>
        /// 最小値を取得または設定する
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">設定しようとした値が<see cref="Max"/>を超える</exception>
        public int Min
        {
            get => _min;
            set
            {
                if (_min == value) return;
                if (_max < value) throw new ArgumentOutOfRangeException(nameof(value), $"設定しようとした値が{nameof(Max)}を超えます\n許容される範囲：int.MinValue({int.MinValue})～{_max}\n実際の値：{value}");
                _min = value;
            }
        }
        private int _min = int.MinValue;
        /// <summary>
        /// 表示される文字列を取得または設定する
        /// </summary>
        public string Label { get; set; }
        /// <summary>
        /// 値を取得または設定する
        /// </summary>
        public int Value { get; set; }
        /// <summary>
        /// <see cref="Value"/>が変更された時に実行
        /// </summary>
        public event EventHandler<ToolValueEventArgs<int>> ValueChanged;
        /// <summary>
        /// 既定の値を用いて<see cref="InputInt1"/>の新しいインスタンスを生成する
        /// </summary>
        public InputInt1() : this(string.Empty, default) { }
        /// <summary>
        /// 指定した値を用いて<see cref="InputInt1"/>の新しいインスタンスを生成する
        /// </summary>
        /// <param name="label">表示する文字列</param>
        /// <param name="value">初期値</param>
        public InputInt1(string label, int value)
        {
            Label = label;
            Value = value;
        }
        internal override void Update()
        {
            var value = Value;
            Engine.Tool.InputInt(Label ?? string.Empty, ref value);
            value = MathHelper.Clamp(value, _max, _min);
            if (Value == value) return;
            ValueChanged?.Invoke(this, new ToolValueEventArgs<int>(Value, value));
            Value = value;
        }
    }
}
