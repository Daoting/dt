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
    /// Represents the winloss sparkline view.
    /// </summary>
    internal class PdfWinLossSparklineView : PdfBaseSparklineView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.PdfWinLossSparklineView" /> class.
        /// </summary>
        /// <param name="viewInfo">The view info.</param>
        public PdfWinLossSparklineView(WinLossSparklineViewInfo viewInfo) : base(viewInfo)
        {
        }
    }
}

