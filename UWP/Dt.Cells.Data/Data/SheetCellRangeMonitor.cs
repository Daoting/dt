#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
#endregion

namespace Dt.Cells.Data
{
    internal class SheetCellRangeMonitor
    {
        Dictionary<IFloatingObjectSheet, List<ISheetCellRangeMonitorSupport>> _cachedChangedRanges = new Dictionary<IFloatingObjectSheet, List<ISheetCellRangeMonitorSupport>>();

        public event EventHandler<SheetCellRangeSupporterAffectedEventArgs> SheetCellRangeSupporterAffected;

        public void EndMonitor(ISheetCellRangeMonitorSupport sheetRangeSupporter)
        {
            SheetCellRange[] monitoringRanges = sheetRangeSupporter.GetMonitoringRanges();
            if ((monitoringRanges != null) && (monitoringRanges.Length != 0))
            {
                foreach (SheetCellRange range in monitoringRanges)
                {
                    if (range != null)
                    {
                        this.EndMonitorSheet(range.Sheet, sheetRangeSupporter);
                    }
                }
                sheetRangeSupporter.MonitoringRangesChanged -= new EventHandler<MonitoringRangesChangedEventArgs>(this.RangeChanged);
            }
        }

        void EndMonitorSheet(IFloatingObjectSheet worksheet, ISheetCellRangeMonitorSupport sheetCellRangeSupporter)
        {
            if ((worksheet != null) && this._cachedChangedRanges.ContainsKey(worksheet))
            {
                List<ISheetCellRangeMonitorSupport> list = this._cachedChangedRanges[worksheet];
                list.Remove(sheetCellRangeSupporter);
                if (list.Count == 0)
                {
                    this._cachedChangedRanges.Remove(worksheet);
                    worksheet.CellChanged -= new EventHandler<CellChangedEventArgs>(this.Sheet_CellChanged);
                }
            }
        }

        void RaiseSheetCellRangeSupporterAffected(string property, ISheetCellRangeMonitorSupport[] affects)
        {
            if (this.SheetCellRangeSupporterAffected != null)
            {
                this.SheetCellRangeSupporterAffected(this, new SheetCellRangeSupporterAffectedEventArgs(property, affects));
            }
        }

        void RangeChanged(object sender, MonitoringRangesChangedEventArgs e)
        {
            if ((e.OldRanges != null) && (e.OldRanges.Length > 0))
            {
                foreach (SheetCellRange range in e.OldRanges)
                {
                    if ((range.Sheet != null) && this._cachedChangedRanges.ContainsKey(range.Sheet))
                    {
                        this.EndMonitorSheet(range.Sheet, sender as ISheetCellRangeMonitorSupport);
                    }
                }
            }
            if ((e.NewRanges != null) && (e.NewRanges.Length > 0))
            {
                foreach (SheetCellRange range2 in e.NewRanges)
                {
                    if (range2.Sheet != null)
                    {
                        this.StartMonitorSheet(range2.Sheet, sender as ISheetCellRangeMonitorSupport);
                    }
                }
            }
        }

        void Sheet_CellChanged(object sender, CellChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
            {
                IFloatingObjectSheet sheet = sender as IFloatingObjectSheet;
                if (this._cachedChangedRanges.ContainsKey(sheet))
                {
                    List<ISheetCellRangeMonitorSupport> list = new List<ISheetCellRangeMonitorSupport>();
                    foreach (ISheetCellRangeMonitorSupport support in this._cachedChangedRanges[sheet])
                    {
                        if (!list.Contains(support))
                        {
                            list.Add(support);
                        }
                    }
                    this.RaiseSheetCellRangeSupporterAffected("Value", list.ToArray());
                }
            }
        }

        public void StartMonitor(ISheetCellRangeMonitorSupport sheetRangeSupporter)
        {
            SheetCellRange[] monitoringRanges = sheetRangeSupporter.GetMonitoringRanges();
            if ((monitoringRanges != null) && (monitoringRanges.Length != 0))
            {
                foreach (SheetCellRange range in monitoringRanges)
                {
                    if (range != null)
                    {
                        this.StartMonitorSheet(range.Sheet, sheetRangeSupporter);
                    }
                }
                sheetRangeSupporter.MonitoringRangesChanged += new EventHandler<MonitoringRangesChangedEventArgs>(this.RangeChanged);
            }
        }

        void StartMonitorSheet(IFloatingObjectSheet worksheet, ISheetCellRangeMonitorSupport sheetCellRangeSupporter)
        {
            if ((worksheet != null) && !Enumerable.Contains<IFloatingObjectSheet>((IEnumerable<IFloatingObjectSheet>) this._cachedChangedRanges.Keys, worksheet))
            {
                worksheet.CellChanged += new EventHandler<CellChangedEventArgs>(this.Sheet_CellChanged);
                List<ISheetCellRangeMonitorSupport> list = new List<ISheetCellRangeMonitorSupport>();
                this._cachedChangedRanges.Add(worksheet, list);
            }
            if (this._cachedChangedRanges.ContainsKey(worksheet))
            {
                this._cachedChangedRanges[worksheet].Add(sheetCellRangeSupporter);
            }
        }
    }
}

