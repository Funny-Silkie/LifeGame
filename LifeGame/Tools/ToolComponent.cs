using System;

namespace LifeGame
{
    /// <summary>
    /// <see cref="Altseed2.Tool"/>に用いられるクラスの基本クラス
    /// </summary>
    [Serializable]
    public abstract class ToolComponent
    {
        /// <summary>
        /// インデックスを取得する
        /// </summary>
        public int Index { get; internal set; } = -1;
        /// <summary>
        /// <see cref="ToolComponent"/>の新しいインスタンスを生成する
        /// </summary>
        protected ToolComponent() { }
        internal abstract void Update();
    }
}
