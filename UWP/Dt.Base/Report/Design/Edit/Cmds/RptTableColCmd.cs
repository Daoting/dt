#region 名称空间
using Dt.Cells.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        void BuildCells(RptTblPart p_part, int p_index)
        {
            if (p_part != null)
            {
                foreach (RptTblPartRow r in p_part.Rows)
                {
                    RptText txt = new RptText(r);
                    txt.Row = r.Row;
                    txt.RowSpan = 1;
                    txt.Col = p_index;
                    txt.ColSpan = 1;
                    r.Cells.Insert(p_index, txt);
                }
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

            BuildCells(p_table.Header, p_index);
            BuildCells(p_table.Body, p_index);
            BuildCells(p_table.Footer, p_index);
            if (p_table.Groups != null && p_table.Groups.Count > 0)
            {
                foreach (RptTblGroup grp in p_table.Groups)
                {
                    BuildCells(grp.Header, p_index);
                    BuildCells(grp.Footer, p_index);
                }
            }
        }

        void RmvTblCells(RptTable p_table, int p_index)
        {
            if (p_table == null)
                return;

            RemoveCells(p_table.Header, p_index);
            if (p_table.Groups != null && p_table.Groups.Count > 0)
            {
                foreach (RptTblGroup grp in p_table.Groups)
                {
                    RemoveCells(grp.Header, p_index);
                    RemoveCells(grp.Footer, p_index);
                }
            }
            RemoveCells(p_table.Body, p_index);
            RemoveCells(p_table.Footer, p_index);
        }
    }

    internal class DeleTblColCmd : RptCmdBase
    {
        public override object Execute(object p_args)
        {
            DeleRptTblColCmdArgs args = (DeleRptTblColCmdArgs)p_args;
            RptTable rptTab = args.Table;
            int index = args.Index;
            args.Dict.Clear();
            RmvTblCells(rptTab, index, args.Dict);
            rptTab.Data["colspan"] = rptTab.ColSpan - 1;
            rptTab.Update(true);
            return null;
        }

        public override void Undo(object p_args)
        {
            DeleRptTblColCmdArgs args = (DeleRptTblColCmdArgs)p_args;
            RptTable rptTab = args.Table;
            int index = args.Index;
            InsertTblCells(rptTab, index, args.Dict);
            rptTab.Data["colspan"] = rptTab.ColSpan + 1;
            rptTab.Update(false);
        }

        void RemoveCells(RptTblPart p_part, int p_index, Dictionary<string, RptText> p_dict, string p_pre)
        {
            if (p_part != null)
            {
                for (int i = 0; i < p_part.Rows.Count; i++)
                {
                    RptText text = p_part.Rows[i].Cells[p_index];
                    p_dict.Add(p_pre + i.ToString(), text);
                    p_part.Rows[i].Cells.RemoveAt(p_index);
                }
            }
        }

        void InsertCells(RptTblPart p_part, int p_index, Dictionary<string, RptText> p_dict, string p_pre)
        {
            if (p_part != null)
            {
                for (int i = 0; i < p_part.Rows.Count; i++)
                {
                    RptText text = p_dict[p_pre + i.ToString()];
                    p_part.Rows[i].Cells.Insert(p_index, text);
                }
            }
        }
        
        void RmvTblCells(RptTable p_table, int p_index, Dictionary<string, RptText> p_dict)
        {
            RemoveCells(p_table.Header, p_index, p_dict, "header");
            if (p_table.Groups != null && p_table.Groups.Count > 0)
            {
                for (int i = 0; i < p_table.Groups.Count; i++)
                {
                    RptTblGroup grp = p_table.Groups[i];
                    RemoveCells(grp.Header, p_index, p_dict, "grpHeader" + i.ToString());
                    RemoveCells(grp.Footer, p_index, p_dict, "grpFooter" + i.ToString());
                }
            }
            RemoveCells(p_table.Body, p_index, p_dict, "body");
            RemoveCells(p_table.Footer, p_index, p_dict, "footer");
        }

        void InsertTblCells(RptTable p_table, int p_index, Dictionary<string, RptText> p_dict)
        {
            InsertCells(p_table.Header, p_index, p_dict, "header");
            if (p_table.Groups != null && p_table.Groups.Count > 0)
            {
                for (int i = 0; i < p_table.Groups.Count; i++)
                {
                    RptTblGroup grp = p_table.Groups[i];
                    InsertCells(grp.Header, p_index, p_dict, "grpHeader" + i.ToString());
                    InsertCells(grp.Footer, p_index, p_dict, "grpFooter" + i.ToString());
                }
            }
            InsertCells(p_table.Body, p_index, p_dict, "body");
            InsertCells(p_table.Footer, p_index, p_dict, "footer");
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
        public DeleRptTblColCmdArgs(RptTable p_table, int p_index, Dictionary<string, RptText> p_dict)
        {
            Dict = p_dict;
            Index = p_index;
            Table = p_table;
        }

        public Dictionary<string, RptText> Dict { get; }

        public int Index { get; }

        public RptTable Table { get; }
    }
}
