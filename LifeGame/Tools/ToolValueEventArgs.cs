using System;

namespace LifeGame
{
    /// <summary>
    /// 値を扱うコンポーネントにて，値が変化したときに用いられる<see cref="EventArgs"/>のクラス
    /// </summary>
    [Serializable]
    public sealed class ToolValueEventArgs<T> : EventArgs
    {
        /// <summary>
        /// 変更後の値
        /// </summary>
        public T NewValue { get; }
        /// <summary>
        /// 変更前の値
        /// </summary>
        public T OldValue { get; }
        /// <summary>
        /// <see cref="ToolValueEventArgs{T}"/>の新しいインスタンスを生成する
        /// </summary>
        /// <param name="oldValue">変更前の値</param>
        /// <param name="newValue">変更後の値</param>
        public ToolValueEventArgs(T oldValue, T newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
