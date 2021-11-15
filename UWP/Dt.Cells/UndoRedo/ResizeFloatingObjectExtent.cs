#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.CompilerServices;
using Windows.Foundation;
#endregion

namespace Dt.Cells.UndoRedo
{
    /// <summary>
    /// 
    /// </summary>
    internal class ResizeFloatingObjectExtent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.UndoRedo.MoveFloatingObjectExtent" /> class.
        /// </summary>
        /// <param name="names">The names.</param>
        /// <param name="resizedRects">The resized rectangles.</param>
        public ResizeFloatingObjectExtent(string[] names, Rect[] resizedRects)
        {
            Names = names;
            ResizedRects = resizedRects;
        }

        /// <summary>
        /// Gets the names.
        /// </summary>
        /// <value>
        /// The names.
        /// </value>
        public string[] Names { get; private set; }

        /// <summary>
        /// Gets the resized rectangles.
        /// </summary>
        /// <value>
        /// The resized rectangles.
        /// </value>
        public Rect[] ResizedRects { get; private set; }
    }
}

