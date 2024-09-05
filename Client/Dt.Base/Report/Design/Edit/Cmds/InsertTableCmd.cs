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
using System.Collections.ObjectModel;
using Windows.UI;

#endregion

namespace Dt.Base.Report
{
    /// <summary>
    /// 
    /// </summary>
    internal class InsertTableCmd : InsertCmdBase
    {
        public override object Execute(object p_args)
        {
            InsertCmdArgs args = (InsertCmdArgs)p_args;
            RptTable tbl = args.RptItem as RptTable;
            RptPart con = tbl.Part;
            //通过重做重新加载table，不用处理
            if (tbl.ColHeader != null || tbl.Body != null || tbl.ColFooter != null)
            {
                con.Items.Add(tbl);
                return tbl;
            }

            CellRange range = args.CellRange;
            tbl.Row = range.Row;
            tbl.Col = range.Column;
            tbl.RowSpan = range.RowCount;
            tbl.ColSpan = range.ColumnCount;
            RptTblPartRow tblRow;
            switch (tbl.RowSpan)
            {
                case 1:
                    tbl.Body = new RptTblRow(tbl);
                    tblRow = new RptTblPartRow(tbl.Body);
                    tbl.Body.Rows.Add(tblRow);
                    BuildCells(tblRow, tbl.ColSpan, false);
                    break;
                case 2:
                    tbl.ColHeader = new RptTblColHeader(tbl);
                    tblRow = new RptTblPartRow(tbl.ColHeader);
                    tbl.ColHeader.Rows.Add(tblRow);
                    BuildCells(tblRow, tbl.ColSpan, true);
                    tbl.Body = new RptTblRow(tbl);
                    tblRow = new RptTblPartRow(tbl.Body);
                    tbl.Body.Rows.Add(tblRow);
                    BuildCells(tblRow, tbl.ColSpan, false);
                    break;
                default:
                    tbl.ColHeader = new RptTblColHeader(tbl);
                    tblRow = new RptTblPartRow(tbl.ColHeader);
                    tbl.ColHeader.Rows.Add(tblRow);
                    BuildCells(tblRow, tbl.ColSpan, true);
                    tbl.Body = new RptTblRow(tbl);
                    for (int i = 0; i < tbl.RowSpan - 2; i++)
                    {
                        tblRow = new RptTblPartRow(tbl.Body);
                        tbl.Body.Rows.Add(tblRow);
                        BuildCells(tblRow, tbl.ColSpan, false);
                    }
                    tbl.ColFooter = new RptTblFooter(tbl);
                    tblRow = new RptTblPartRow(tbl.ColFooter);
                    tbl.ColFooter.Rows.Add(tblRow);
                    BuildCells(tblRow, tbl.ColSpan, true);
                    break;
            }
            con.Items.Add(tbl);
            return tbl;
        }

        /// <summary>
        /// 构建table的单元格。
        /// </summary>
        /// <param name="p_tblRow"></param>
        /// <param name="p_colSpan"></param>
        /// <param name="p_isTitle">标题时启用：默认背景色、居中</param>
        internal static void BuildCells(RptTblPartRow p_tblRow, int p_colSpan, bool p_isTitle)
        {
            RptText txt;
            for (int i = 0; i < p_colSpan; i++)
            {
                txt = new RptText(p_tblRow);
                txt.RowSpan = 1;
                txt.ColSpan = 1;
                if (p_isTitle)
                {
                    txt.Horalign = CellHorizontalAlignment.Center;
                    txt.Background = Color.FromArgb(0xff, 0xE0, 0xE0, 0xE0);
                }
                p_tblRow.Cells.Add(txt);
            }
        }
    }
}
