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
    /// 插入表格分组行
    /// </summary>
    internal class InsertTblGrpRowCmd : RptCmdBase
    {
        public override object Execute(object p_args)
        {
            var args = (InsertTblGrpRowCmdArgs)p_args;
            RptTblPartRow row = new RptTblPartRow(args.Grp);
            args.Row = row;
            InsertTableCmd.BuildCells(row, args.Table.ColSpan, true);
            args.Grp.Rows.Add(row);
            args.Table.CalcRowSpan();
            args.Table.Update(false);
            return null;
        }

        public override void Undo(object p_args)
        {
            var args = (InsertTblGrpRowCmdArgs)p_args;
            args.Grp.Rows.Remove(args.Row);
            args.Table.CalcRowSpan();
            args.Table.Update(true);
        }
    }

    /// <summary>
    /// 删除分组行
    /// </summary>
    internal class DelTblGrpRowCmd : RptCmdBase
    {
        public override object Execute(object p_args)
        {
            var args = (InsertTblGrpRowCmdArgs)p_args;
            args.Grp.Rows.Remove(args.Row);
            args.Table.CalcRowSpan();
            args.Table.Update(true);
            return null;
        }

        public override void Undo(object p_args)
        {
            var args = (InsertTblGrpRowCmdArgs)p_args;
            args.Grp.Rows.Add(args.Row);
            args.Table.CalcRowSpan();
            args.Table.Update(false);
        }
    }

    internal class InsertTblGrpRowCmdArgs
    {
        public InsertTblGrpRowCmdArgs(RptTable p_table, RptTblGroup p_grp)
        {
            Table = p_table;
            Grp = p_grp;
        }

        public RptTblGroup Grp { get; }

        public RptTable Table { get; }

        public RptTblPartRow Row { get; set; }
    }
}
