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
    /// 插入表格分组
    /// </summary>
    internal class InsertTblGrpCmd : RptCmdBase
    {
        public override object Execute(object p_args)
        {
            InsertTblGrpCmdArgs args = (InsertTblGrpCmdArgs)p_args;
            RptTable table = args.Table;
            if (table.Groups == null)
            {
                table.Groups = new List<RptTblGroup>();
            }
            RptTblPartRow row = new RptTblPartRow(args.Grp.Header);
            InsertTableCmd.BuildCells(row,table.ColSpan);
            args.Grp.Header.Rows.Add(row);
            RptTblPartRow r = new RptTblPartRow(args.Grp.Footer);
            InsertTableCmd.BuildCells(r,table.ColSpan);
            args.Grp.Footer.Rows.Add(r);
            args.Table.Groups.Add(args.Grp);
            args.Table.CalcRowSpan();
            args.Table.Update(false);
            return null;
        }

        public override void Undo(object p_args)
        {
            InsertTblGrpCmdArgs args = (InsertTblGrpCmdArgs)p_args;
            args.Table.Groups.Remove(args.Grp);
            args.Table.CalcRowSpan();
            args.Table.Update(true);
        }
    }

    /// <summary>
    /// 清空表格分组
    /// </summary>
    internal class ClearTblGrpCmd : RptCmdBase 
    {
        public override object Execute(object p_args)
        {
            ClearTblGrpCmdArgs args = (ClearTblGrpCmdArgs)p_args;
            args.Table.Groups = null;
            args.Table.CalcRowSpan();
            args.Table.Update(true);
            return null;
        }

        public override void Undo(object p_args)
        {
            ClearTblGrpCmdArgs args = (ClearTblGrpCmdArgs)p_args;
            args.Table.Groups = args.Grps;
            args.Table.CalcRowSpan();
            args.Table.Update(false);
        }
    }

    internal class InsertTblGrpCmdArgs
    {
        public InsertTblGrpCmdArgs(RptTable p_table, RptTblGroup p_grp)
        {
            Table = p_table;
            Grp = p_grp;
        }

        public RptTblGroup Grp { get; }

        public RptTable Table { get; }
    }

    internal class ClearTblGrpCmdArgs 
    {
        public ClearTblGrpCmdArgs(RptTable p_table, List<RptTblGroup> p_grps) 
        {
            Table = p_table;
            Grps = p_grps;
        }

        public RptTable Table { get; }

        public List<RptTblGroup> Grps { get; }
    }
} 
