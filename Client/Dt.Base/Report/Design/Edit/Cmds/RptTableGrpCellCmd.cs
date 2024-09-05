#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-06-21 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Base;
using Dt.Core;
using Dt.Cells.Data;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;

#endregion

namespace Dt.Base.Report
{
    /// <summary>
    /// 合并分组行的单元格
    /// </summary>
    internal class MergeTblGrpCellCmd : RptCmdBase
    {
        public override object Execute(object p_args)
        {
            var args = (SplitTblGrpCellCmdArgs)p_args;
            var curRow = args.RptText.Parent as RptTblPartRow;
            var grp = curRow.Parent as RptTblGroup;
            var index = curRow.Cells.IndexOf(args.RptText);
            int span = Math.Min(curRow.Cells.Count - index - args.RptText.ColSpan, args.Range.ColumnCount - args.RptText.ColSpan);
            args.RptText.ColSpan = span + args.RptText.ColSpan;
            while (span > 0)
            {
                var cell = curRow.Cells[index + 1];
                span -= cell.ColSpan;
                if (span >= 0)
                {
                    curRow.Cells.RemoveAt(index + 1);
                }
            }
            grp.Table.Update(true);
            return null;
        }

        public override void Undo(object p_args)
        {
            var args = (SplitTblGrpCellCmdArgs)p_args;
            var curRow = args.RptText.Parent as RptTblPartRow;
            var grp = curRow.Parent as RptTblGroup;
            var index = curRow.Cells.IndexOf(args.RptText);
            for (int i = 0; i < args.RptText.ColSpan - args.ColSpan; i++)
            {
                InsertTableCmd.BuildCells(curRow, 1, true);
            }
            args.RptText.ColSpan = args.ColSpan;
            grp.Table.Update(true);
        }
    }

    /// <summary>
    /// 拆分分组行的单元格
    /// </summary>
    internal class SplitTblGrpCellCmd : RptCmdBase
    {
        public override object Execute(object p_args)
        {
            var args = (SplitTblGrpCellCmdArgs)p_args;
            var curRow = args.RptText.Parent as RptTblPartRow;
            var grp = curRow.Parent as RptTblGroup;
            for (int i = 0; i < args.RptText.ColSpan - 1; i++)
            {
                InsertTableCmd.BuildCells(curRow, 1, true);
            }
            args.RptText.ColSpan = 1;
            grp.Table.Update(true);
            return null;
        }

        public override void Undo(object p_args)
        {
            var args = (SplitTblGrpCellCmdArgs)p_args;
            var curRow = args.RptText.Parent as RptTblPartRow;
            var grp = curRow.Parent as RptTblGroup;
            var index = curRow.Cells.IndexOf(args.RptText);
            curRow.Cells.RemoveRange(index + 1, args.ColSpan - 1);
            args.RptText.ColSpan = args.ColSpan;
            grp.Table.Update(true);
        }
    }

    internal class SplitTblGrpCellCmdArgs
    {
        public SplitTblGrpCellCmdArgs(RptText p_txt, CellRange p_range)
        {
            RptText = p_txt;
            Range = p_range;
            ColSpan = p_txt.ColSpan;
        }

        public RptText RptText { get; }

        public CellRange Range { get; }

        public int ColSpan { get; }
    }
}
