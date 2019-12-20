#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Cells.Data
{
    internal class MonitoringRangesChangedEventArgs : EventArgs
    {
        public SheetCellRange[] NewRanges { get; set; }

        public SheetCellRange[] OldRanges { get; set; }

        public string RangeName { get; set; }
    }
}

