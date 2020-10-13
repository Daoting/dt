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
using System.Collections.ObjectModel;

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
            if (tbl.Header != null || tbl.Body != null || tbl.Footer != null)
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
                    BuildCells(tblRow,tbl.ColSpan);
                    break;
                case 2:
                    tbl.Header = new RptTblHeader(tbl);
                    tblRow = new RptTblPartRow(tbl.Header);
                    tbl.Header.Rows.Add(tblRow);
                    BuildCells(tblRow,tbl.ColSpan);
                    tbl.Body = new RptTblRow(tbl);
                    tblRow = new RptTblPartRow(tbl.Body);
                    tbl.Body.Rows.Add(tblRow);
                    BuildCells(tblRow,tbl.ColSpan);
                    break;
                default:
                    tbl.Header = new RptTblHeader(tbl);
                    tblRow = new RptTblPartRow(tbl.Header);
                    tbl.Header.Rows.Add(tblRow);
                    BuildCells(tblRow,tbl.ColSpan);
                    tbl.Body = new RptTblRow(tbl);
                    for (int i = 0; i < tbl.RowSpan - 2; i++)
                    {
                        tblRow = new RptTblPartRow(tbl.Body);
                        tbl.Body.Rows.Add(tblRow);
                        BuildCells(tblRow,tbl.ColSpan);
                    }
                    tbl.Footer = new RptTblFooter(tbl);
                    tblRow = new RptTblPartRow(tbl.Footer);
                    tbl.Footer.Rows.Add(tblRow);
                    BuildCells(tblRow,tbl.ColSpan);
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
        internal static void BuildCells(RptTblPartRow p_tblRow,int p_colSpan)
        {
            RptText txt;
            for (int i = 0; i < p_colSpan; i++)
            {
                txt = new RptText(p_tblRow);
                txt.RowSpan = 1;
                txt.ColSpan = 1;
                p_tblRow.Cells.Add(txt);
            }
        }
    }
}
