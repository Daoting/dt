#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// Represents one cell anchor
    /// </summary>
    public class OneCellAnchor : IAnchor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.TwoCellAnchor" /> class.
        /// </summary>
        public OneCellAnchor()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.TwoCellAnchor" /> class.
        /// </summary>
        /// <param name="fromRow">From row.</param>
        /// <param name="fromColumn">From column.</param>
        public OneCellAnchor(int fromRow, int fromColumn)
        {
            this.FromRow = fromRow;
            this.FromColumn = fromColumn;
        }

        /// <summary>
        /// Gets or sets from column.
        /// </summary>
        /// <value>
        /// From column.
        /// </value>
        public int FromColumn { get; set; }

        /// <summary>
        /// Gets or sets from column offset.
        /// </summary>
        /// <value>
        /// From column offset.
        /// </value>
        public double FromColumnOffset { get; set; }

        /// <summary>
        /// Gets or sets from row.
        /// </summary>
        /// <value>
        /// From row.
        /// </value>
        public int FromRow { get; set; }

        /// <summary>
        /// Gets or sets from row offset.
        /// </summary>
        /// <value>
        /// From row offset.
        /// </value>
        public double FromRowOffset { get; set; }

        /// <summary>
        /// Specifies the height of the object.
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// Specifies the width of the object
        /// </summary>
        public double Width { get; set; }
    }
}

