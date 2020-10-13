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
using Windows.UI.Xaml.Controls;

#endregion

namespace Dt.Base.Report
{
    /// <summary>
    /// 插入行
    /// </summary>
    internal class InsertTblRowCmd : RptCmdBase
    {
        public override object Execute(object p_args)
        {
            InsertTblRowCmdArgs args = (InsertTblRowCmdArgs)p_args;
            RptTblPartRow row = new RptTblPartRow(args.Part);
            InsertTableCmd.BuildCells(row,args.Part.Table.ColSpan);
            args.Part.Rows.Insert(args.Index, row);
            RptTable tbl = args.Part.Table;
            tbl.CalcRowSpan();
            tbl.Update(false);
            return row;
        }

        public override void Undo(object p_args)
        {
            InsertTblRowCmdArgs args = (InsertTblRowCmdArgs)p_args;
            args.Part.Rows.RemoveAt(args.Index);
            RptTable tbl = args.Part.Table;
            tbl.CalcRowSpan();
            tbl.Update(true);
        }
    }

    /// <summary>
    /// 删除行
    /// </summary>
    internal class DeleTblRowCmd : RptCmdBase
    {
        public override object Execute(object p_args)
        {
            DeleTblRowCmdArgs args = (DeleTblRowCmdArgs)p_args;
            args.Row.TblPart.Rows.RemoveAt(args.Index);
            RptTable tbl = args.Row.Table;
            tbl.CalcRowSpan();
            tbl.Update(true);
            return args.Row;
        }

        public override void Undo(object p_args)
        {
            DeleTblRowCmdArgs args = (DeleTblRowCmdArgs)p_args;
            args.Row.TblPart.Rows.Insert(args.Index, args.Row);
            RptTable tbl = args.Row.Table;
            tbl.CalcRowSpan();
            tbl.Update(false);
        }
    }

    /// <summary>
    /// 包含表头或表尾
    /// </summary>
    internal class ContainHeadOrFootCmd : RptCmdBase 
    {
        public override object Execute(object p_args)
        {
            ContainHeadOrFootCmdArgs args = (ContainHeadOrFootCmdArgs)p_args;
            RptTblPart part = null;
            if (args.Flag == "Header")
                part = new RptTblHeader(args.Table);
            else
                part = new RptTblFooter(args.Table);
            RptTblPartRow row = new RptTblPartRow(part);
            InsertTableCmd.BuildCells(row, args.Table.ColSpan);
            part.Rows.Add(row);
            if (args.Flag == "Header")
                args.Table.Header = (RptTblHeader)part;
            else
                args.Table.Footer = (RptTblFooter)part;
            args.Table.CalcRowSpan();
            args.Table.Update(false);
            return null;
        }

        public override void Undo(object p_args)
        {
            ContainHeadOrFootCmdArgs args = (ContainHeadOrFootCmdArgs)p_args;
            if (args.Flag == "Header")
                args.Table.Header = null;
            else
                args.Table.Footer = null;
            args.Table.CalcRowSpan();
            args.Table.Update(true);
        }
    }

    /// <summary>
    /// 移除表头或表尾
    /// </summary>
    internal class RemoveHeadOrFootCmd : RptCmdBase 
    {
        public override object Execute(object p_args)
        {
            RemoveHeadOrFootCmdArgs args = (RemoveHeadOrFootCmdArgs)p_args;
            if (args.Flag == "Header")
                args.Table.Header = null;
            else
                args.Table.Footer = null;
            args.Table.CalcRowSpan();
            args.Table.Update(true);
            return null;
        }

        public override void Undo(object p_args)
        {
            RemoveHeadOrFootCmdArgs args = (RemoveHeadOrFootCmdArgs)p_args;
            RptTblPart part = null;
            if (args.Flag == "Header")
                part = new RptTblHeader(args.Table);
            else
                part = new RptTblFooter(args.Table);
            foreach (RptTblPartRow row in args.Rows) 
            {
                part.Rows.Add(row);
            }
            if (args.Flag == "Header")
                args.Table.Header = (RptTblHeader)part;
            else
                args.Table.Footer = (RptTblFooter)part;
            args.Table.CalcRowSpan();
            args.Table.Update(false);
        }
    }

    internal class InsertTblRowCmdArgs
    {
        public InsertTblRowCmdArgs(RptTblPart p_part, int p_index)
        {
            Part = p_part;
            Index = p_index;
        }

        internal RptTblPart Part { get; }

        public int Index { get; }
    }

    internal class DeleTblRowCmdArgs
    {
        public DeleTblRowCmdArgs(int p_index, RptTblPartRow p_row)
        {
            Index = p_index;
            Row = p_row;
        }

        public int Index { get; }

        public RptTblPartRow Row { get; }
    }

    internal class ContainHeadOrFootCmdArgs 
    {
        public ContainHeadOrFootCmdArgs(string p_flag, RptTable p_table) 
        {
            Flag = p_flag;
            Table = p_table;
        }

        public string Flag { get; }

        public RptTable Table { get; }
    }

    internal class RemoveHeadOrFootCmdArgs 
    {
        public RemoveHeadOrFootCmdArgs(string p_flag, RptTable p_table, RptTblPartRow[] p_rows) 
        {
            Flag = p_flag;
            Table = p_table;
            Rows = p_rows;
        }

        public string Flag { get; }

        public RptTable Table { get; }

        public RptTblPartRow[] Rows { get; }
    }
} 
