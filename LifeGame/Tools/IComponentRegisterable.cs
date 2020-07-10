using System;
using System.Collections.Generic;

namespace LifeGame
{
    /// <summary>
    /// <see cref="ToolComponent"/>を登録できるオブジェクトを表す
    /// </summary>
    public interface IComponentRegisterable<TComponent> where TComponent : ToolComponent
    {
        /// <summary>
        /// 登録されている<typeparamref name="TComponent"/>を取得する
        /// </summary>
        IEnumerable<TComponent> Components { get; }
        /// <summary>
        /// コンポーネントを追加する
        /// </summary>
        /// <param name="component">追加するコンポーネント</param>
        /// <exception cref="ArgumentNullException"><paramref name="component"/>がnull</exception>
        /// <returns><paramref name="component"/>を追加出来たらtrue，それ以外でそれ以外でfalse</returns>
        bool AddComponent(TComponent component);
        /// <summary>
        /// コンポーネントを削除する
        /// </summary>
        /// <param name="component">削除するコンポーネント</param>
        /// <exception cref="ArgumentNullException"><paramref name="component"/>がnull</exception>
        /// <returns><paramref name="component"/>を削除出来たらtrue，それ以外でfalse</returns>
        bool RemoveComponent(TComponent component);
    }
}
