#region 名称空间
using Dt.Cells.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
#endregion

namespace Dt.Base.Report
{
    internal class InsertTblColCmd : RptCmdBase
    {
        public override object Execute(object p_args)
        {
            InsertRptTblColCmdArgs args = (InsertRptTblColCmdArgs)p_args;
            RptTable rptTab = args.Table;
            int index = args.Index;
            BldTblCells(rptTab, index);
            // 此处直接为data赋值，触发valueChanged事件。
            rptTab.Data["colspan"] = rptTab.ColSpan + 1;
            rptTab.Update(false);
            return null;
        }

        public override void Undo(object p_args)
        {
            InsertRptTblColCmdArgs args = (InsertRptTblColCmdArgs)p_args;
            RptTable rptTab = args.Table;
            int index = args.Index;
            RmvTblCells(rptTab, index);
            rptTab.Data["colspan"] = rptTab.ColSpan - 1;
            rptTab.Update(true);
        }

        public static void BuildCells(RptTblPart p_part, int p_index, bool p_isTitle)
        {
            if (p_part == null)
                return;

            foreach (RptTblPartRow r in p_part.Rows)
            {
                RptText txt = new RptText(r);
                txt.Row = r.Row;
                txt.RowSpan = 1;
                txt.Col = p_index;
                txt.ColSpan = 1;

                if (p_isTitle)
                {
                    txt.Horalign = CellHorizontalAlignment.Center;
                    txt.Background = Color.FromArgb(0xff, 0xE0, 0xE0, 0xE0);
                }

                if (p_index >= r.Cells.Count)
                    r.Cells.Add(txt);
                else
                    r.Cells.Insert(p_index, txt);
            }
        }

        void RemoveCells(RptTblPart p_part, int p_index)
        {
            if (p_part != null)
            {
                foreach (RptTblPartRow r in p_part.Rows)
                {
                    r.Cells.RemoveAt(p_index);
                }
            }
        }

        void BldTblCells(RptTable p_table, int p_index)
        {
            if (p_table == null)
                return;

            BuildCells(p_table.ColHeader, p_index, true);
            BuildCells(p_table.Body, p_index, false);
            BuildCells(p_table.ColFooter, p_index, true);
            if (p_table.Groups != null && p_table.Groups.Count > 0)
            {
                foreach (RptTblGroup grp in p_table.Groups)
                {
                    BuildCells(grp, p_index, true);
                }
            }
        }

        void RmvTblCells(RptTable p_table, int p_index)
        {
            if (p_table == null)
                return;

            RemoveCells(p_table.ColHeader, p_index);
            if (p_table.Groups != null && p_table.Groups.Count > 0)
            {
                foreach (RptTblGroup grp in p_table.Groups)
                {
                    RemoveCells(grp, p_index);
                }
            }
            RemoveCells(p_table.Body, p_index);
            RemoveCells(p_table.ColFooter, p_index);
        }
    }

    internal class DeleTblColCmd : RptCmdBase
    {
        public override object Execute(object p_args)
        {
            DeleRptTblColCmdArgs args = (DeleRptTblColCmdArgs)p_args;
            RptTable rptTab = args.Table;
            RmvTblCells(rptTab, args.Index);
            rptTab.Data["colspan"] = rptTab.ColSpan - 1;
            rptTab.Update(true);
            return null;
        }

        public override void Undo(object p_args)
        {
            DeleRptTblColCmdArgs args = (DeleRptTblColCmdArgs)p_args;
            RptTable rptTab = args.Table;
            InsertTblCells(rptTab, args.Index);
            rptTab.Data["colspan"] = rptTab.ColSpan + 1;
            rptTab.Update(false);
        }

        void RemoveCells(RptTblPart p_part, int p_index)
        {
            if (p_part == null)
                return;

            for (int i = 0; i < p_part.Rows.Count; i++)
            {
                var row = p_part.Rows[i];
                int cur = 0;
                for (int j = 0; j < row.Cells.Count; j++)
                {
                    RptText text = row.Cells[j];
                    cur += text.ColSpan;
                    if (cur > p_index)
                    {
                        if (text.ColSpan == 1)
                            row.Cells.RemoveAt(j);
                        else
                            text.ColSpan--;
                        break;
                    }
                }
            }
        }

        void RmvTblCells(RptTable p_table, int p_index)
        {
            RemoveCells(p_table.ColHeader, p_index);
            if (p_table.Groups != null && p_table.Groups.Count > 0)
            {
                for (int i = 0; i < p_table.Groups.Count; i++)
                {
                    RemoveCells(p_table.Groups[i], p_index);
                }
            }
            RemoveCells(p_table.Body, p_index);
            RemoveCells(p_table.ColFooter, p_index);
        }

        void InsertTblCells(RptTable p_table, int p_index)
        {
            InsertTblColCmd.BuildCells(p_table.ColHeader, p_index, true);
            if (p_table.Groups != null && p_table.Groups.Count > 0)
            {
                for (int i = 0; i < p_table.Groups.Count; i++)
                {
                    InsertTblColCmd.BuildCells(p_table.Groups[i], p_index, true);
                }
            }
            InsertTblColCmd.BuildCells(p_table.Body, p_index, false);
            InsertTblColCmd.BuildCells(p_table.ColFooter, p_index, true);
        }
    }

    internal class InsertRptTblColCmdArgs
    {
        public InsertRptTblColCmdArgs(RptTable p_table, int p_index)
        {
            Index = p_index;
            Table = p_table;
        }

        public int Index { get; }

        public RptTable Table { get; }

    }

    internal class DeleRptTblColCmdArgs
    {
        public DeleRptTblColCmdArgs(RptTable p_table, int p_index)
        {
            Index = p_index;
            Table = p_table;
        }

        public int Index { get; }

        public RptTable Table { get; }
    }
}
