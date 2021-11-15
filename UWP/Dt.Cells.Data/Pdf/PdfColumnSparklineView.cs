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
    /// Represents the column sparkline view.
    /// </summary>
    internal class PdfColumnSparklineView : PdfBaseSparklineView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.PdfColumnSparklineView" /> class.
        /// </summary>
        /// <param name="viewInfo">The view info.</param>
        public PdfColumnSparklineView(ColumnSparklineViewInfo viewInfo) : base(viewInfo)
        {
        }
    }
}

