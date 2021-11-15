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
    /// Represents a horizontal page break.
    /// </summary>
    internal class GcHorizontalPageBreak : GcControl
    {
        /// <summary>
        /// Creates a new horizontal page break.
        /// </summary>
        /// <param name="y">The <i>y</i> value, in hundredths of an inch.</param>
        public GcHorizontalPageBreak(int y)
        {
            base.y = y;
        }

        /// <summary>
        /// Overrides the <see cref="P:Dt.Cells.Data.GcControl.X" /> property.
        /// </summary>
        /// <value>
        /// This property is always <c>0</c>.
        /// </value>
        /// <remarks>This property is read-only.</remarks>
        public override int X
        {
            get { return  0; }
            set
            {
            }
        }
    }
}

