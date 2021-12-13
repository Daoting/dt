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
    /// Specifies a two cell anchor placeholder for a group, a shape, or a drawing element.
    /// </summary>
    public class TwoCellAnchor : IAnchor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.TwoCellAnchor" /> class.
        /// </summary>
        public TwoCellAnchor()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.TwoCellAnchor" /> class.
        /// </summary>
        /// <param name="fromRow">From row.</param>
        /// <param name="fromColumn">From column.</param>
        /// <param name="toRow">To row.</param>
        /// <param name="toColumn">To column.</param>
        public TwoCellAnchor(int fromRow, int fromColumn, int toRow, int toColumn)
        {
            this.FromRow = fromRow;
            this.FromColumn = fromColumn;
            this.ToRow = toRow;
            this.ToColumn = toColumn;
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
        /// Gets or sets to column.
        /// </summary>
        /// <value>
        /// To column.
        /// </value>
        public int ToColumn { get; set; }

        /// <summary>
        /// Gets or sets to column offset.
        /// </summary>
        /// <value>
        /// To column offset.
        /// </value>
        public double ToColumnOffset { get; set; }

        /// <summary>
        /// Gets or sets to row.
        /// </summary>
        /// <value>
        /// To row.
        /// </value>
        public int ToRow { get; set; }

        /// <summary>
        /// Gets or sets to row offset.
        /// </summary>
        /// <value>
        /// To row offset.
        /// </value>
        public double ToRowOffset { get; set; }
    }
}

