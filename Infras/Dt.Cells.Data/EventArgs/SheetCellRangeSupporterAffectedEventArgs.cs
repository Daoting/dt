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
    internal class SheetCellRangeSupporterAffectedEventArgs : EventArgs
    {
        public SheetCellRangeSupporterAffectedEventArgs(string trigger, ISheetCellRangeMonitorSupport[] affectedSheetCellRangeSupporters)
        {
            this.Trigger = trigger;
            this.AffectedSheetCellRangeSupporters = affectedSheetCellRangeSupporters;
        }

        public ISheetCellRangeMonitorSupport[] AffectedSheetCellRangeSupporters { get; private set; }

        public string Trigger { get; set; }
    }
}

