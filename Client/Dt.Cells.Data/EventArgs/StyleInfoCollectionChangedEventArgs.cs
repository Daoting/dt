#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents the event data for the NamedStyleCollection object's Changed event.
    /// </summary>
    public class StyleInfoCollectionChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The new style.
        /// </summary>
        StyleInfo newstyle;
        /// <summary>
        /// The old style.
        /// </summary>
        StyleInfo oldstyle;
        /// <summary>
        /// Event Type.
        /// </summary>
        StyleInfoCollectionChangedAction type;

        /// <summary>
        /// Initializes a new NamedStyleCollectionEventArgs object with the specified type and styles.
        /// </summary>
        /// <param name="type">
        /// Type of event to occur
        /// </param>
        /// <param name="oldstyle">
        /// Old style, or null if not applicable
        /// </param>
        /// <param name="newstyle">
        /// New style, or null if not applicable
        /// </param>
        internal StyleInfoCollectionChangedEventArgs(StyleInfoCollectionChangedAction type, StyleInfo oldstyle, StyleInfo newstyle)
        {
            this.type = type;
            this.oldstyle = oldstyle;
            this.newstyle = newstyle;
        }

        /// <summary>
        /// Gets the new style that was added, or null if no style was added.
        /// </summary>
        /// <value>The new style information.</value>
        public StyleInfo NewStyle
        {
            get { return  this.newstyle; }
        }

        /// <summary>
        /// Gets the old style that was removed or changed,
        /// or null if no style was removed or changed.
        /// </summary>
        /// <value>The old style information.</value>
        public StyleInfo OldStyle
        {
            get { return  this.oldstyle; }
        }

        /// <summary>
        /// Gets the type of event that occurred.
        /// </summary>
        /// <value>The <see cref="T:Dt.Cells.Data.StyleInfoCollectionChangedAction" /> enumeration that specifies the type of the event.</value>
        public StyleInfoCollectionChangedAction Type
        {
            get { return  this.type; }
        }
    }
}

