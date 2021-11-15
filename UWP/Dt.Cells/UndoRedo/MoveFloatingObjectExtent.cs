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
#endregion

namespace Dt.Cells.UndoRedo
{
    /// <summary>
    /// 
    /// </summary>
    public class MoveFloatingObjectExtent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.UndoRedo.MoveFloatingObjectExtent" /> class.
        /// </summary>
        /// <param name="names">The names.</param>
        /// <param name="offsetX">The X offset.</param>
        /// <param name="offsetY">The Y offset.</param>
        public MoveFloatingObjectExtent(string[] names, double offsetX, double offsetY)
        {
            Names = names;
            OffsetX = offsetX;
            OffsetY = offsetY;
        }

        /// <summary>
        /// Gets the names.
        /// </summary>
        /// <value>
        /// The names.
        /// </value>
        public string[] Names { get; private set; }

        /// <summary>
        /// Gets the X offset.
        /// </summary>
        /// <value>
        /// The X offset.
        /// </value>
        public double OffsetX { get; private set; }

        /// <summary>
        /// Gets the Y offset.
        /// </summary>
        /// <value>
        /// The Y offset.
        /// </value>
        public double OffsetY { get; private set; }
    }
}

