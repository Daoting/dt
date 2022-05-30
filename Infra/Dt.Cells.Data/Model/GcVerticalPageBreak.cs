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
    /// Represents a vertical page break.
    /// </summary>
    internal class GcVerticalPageBreak : GcControl
    {
        /// <summary>
        /// Creates a new vertical page break.
        /// </summary>
        /// <param name="x">The <i>x</i> value.</param>
        public GcVerticalPageBreak(int x)
        {
            base.x = x;
        }

        /// <summary>
        /// Overrides the <see cref="P:Dt.Cells.Data.GcControl.Y" /> property.
        /// </summary>
        /// <value>
        /// This property is always <c>0</c>.
        /// </value>
        /// <remarks>This property is read-only.</remarks>
        public override int Y
        {
            get { return  0; }
            set
            {
            }
        }
    }
}

