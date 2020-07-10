using System;

namespace LifeGame
{
    /// <summary>
    /// ボタンのツールコンポーネントの基底クラス
    /// </summary>
    [Serializable]
    public abstract class ButtonBase : ToolComponent
    {
        /// <summary>
        /// ボタンが押された時に実行
        /// </summary>
        public event EventHandler Clicked;
        /// <summary>
        /// <see cref="ButtonBase"/>の新しいインスタンスを生成する
        /// </summary>
        protected ButtonBase() { }
        /// <summary>
        /// ボタンが押された時に実行
        /// </summary>
        protected virtual void OnClicked()
        {
            Clicked?.Invoke(this, EventArgs.Empty);
        }
    }
}
