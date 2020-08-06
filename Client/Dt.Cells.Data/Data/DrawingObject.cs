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
    /// Represents a base drawing object for rules.
    /// </summary>
    public abstract class DrawingObject
    {
        int anchorColumn;
        int anchorRow;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.DrawingObject" /> class.
        /// </summary>
        /// <param name="anchorRow">The anchor row.</param>
        /// <param name="anchorColumn">The anchor column.</param>
        public DrawingObject(int anchorRow, int anchorColumn)
        {
            this.anchorRow = anchorRow;
            this.anchorColumn = anchorColumn;
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object" /> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="T:System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            DrawingObject obj2 = obj as DrawingObject;
            if (obj2 == null)
            {
                return base.Equals(obj);
            }
            return ((this.anchorColumn == obj2.anchorColumn) && (this.anchorRow == obj2.anchorRow));
        }

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return (this.anchorRow | this.anchorColumn);
        }

        /// <summary>
        /// Gets the anchor column.
        /// </summary>
        /// <value>The anchor column.</value>
        public int AnchorColumn
        {
            get { return  this.anchorColumn; }
        }

        /// <summary>
        /// Gets the anchor row.
        /// </summary>
        /// <value>The anchor row.</value>
        public int AnchorRow
        {
            get { return  this.anchorRow; }
        }
    }
}

