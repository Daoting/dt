#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-06-21 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Cells.Data;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Windows.UI;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Base.Report
{
    /// <summary>
    /// 将报表项的变化(增删、报表项属性变化等)同步到Excel
    /// </summary>
    internal class ExcelClerk
    {
        RptDesignWin _owner;

        public ExcelClerk(RptDesignWin p_owner)
        {
            _owner = p_owner;
        }

        /// <summary>
        /// 绑定事件
        /// </summary>
        public void AttachEvent(RptRoot p_root)
        {
            p_root.ItemsChanged += OnItemsChanged;
            p_root.TextChanged += OnTextChanged;
            p_root.Updated += OnUpdated;
        }

        /// <summary>
        /// 松绑事件
        /// </summary>
        /// <param name="p_root"></param>
        public void DetachEvent(RptRoot p_root)
        {
            p_root.ItemsChanged -= OnItemsChanged;
            p_root.TextChanged -= OnTextChanged;
            p_root.Updated -= OnUpdated;
        }

        /// <summary>
        /// 加载报表项
        /// </summary>
        /// <param name="p_item"></param>
        public void LoadItem(RptItem p_item)
        {
            if (p_item is RptText txt)
                LoadText(txt);
            else if (p_item is RptTable tbl)
                LoadTable(tbl);
            else if (p_item is RptMatrix mtx)
                LoadMatrix(mtx);
            else if (p_item is RptChart ct)
                LoadChart(ct);
        }

        #region 事件处理
        /// <summary>
        /// 报表项增删事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Invoke(() =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    for (int i = 0; i < e.NewItems.Count; i++)
                    {
                        LoadItem(e.NewItems[i] as RptItem);
                    }
                }
                else if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    RptItem tmp;
                    for (int i = 0; i < e.OldItems.Count; i++)
                    {
                        tmp = e.OldItems[i] as RptItem;
                        _owner.AfterDelItem(tmp);
                        ClearItem(tmp);
                    }
                    _owner.Excel.DecorationRange = null;
                }

                // 页眉页脚报表项变化时刷新分割线
                if (sender != _owner.Info.Root.Body)
                    _owner.RefreshSpliter();
            });
        }

        /// <summary>
        /// 文本项属性值变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnTextChanged(object sender, Core.Cell e)
        {
            Invoke(() => { LoadText((RptText)sender); });
        }

        /// <summary>
        /// 报表项更新事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnUpdated(object sender, bool e)
        {
            Invoke(() =>
            {
                RptItem item = (RptItem)sender;
                Worksheet ws = _owner.Excel.Sheets[(int)item.Part.PartType];
                if (e)
                {
                    // 按原始位置清除
                    var row = item.Data;
                    CellRange range = new CellRange(
                        row.GetOriginalVal<int>("row"),
                        row.GetOriginalVal<int>("col"),
                        row.GetOriginalVal<int>("rowspan"),
                        row.GetOriginalVal<int>("colspan"));
                    ClearRange(ws, range);
                }
                LoadItem(item);
                // 焦点切换到末单元格，重置选择框
                ws.SetActiveCell(ws.RowCount - 1, ws.ColumnCount - 1, true);
                _owner.Excel.DecorationRange = new CellRange(item.Row, item.Col, item.RowSpan, item.ColSpan);
            });
        }

        #endregion

        #region 显示报表项
        /// <summary>
        /// 加载RptText，一般用于加载报表顶级的RptText
        /// </summary>
        /// <param name="p_txt"></param>
        void LoadText(RptText p_txt)
        {
            Dt.Cells.Data.Cell cell = _owner.Excel.Sheets[(int)p_txt.Part.PartType].Cells[p_txt.Row, p_txt.Col];
            cell.RowSpan = p_txt.RowSpan;
            cell.ColumnSpan = p_txt.ColSpan;
            cell.Value = p_txt.Data.Str("val");
            p_txt.ApplyStyle(cell);
        }

        /// <summary>
        /// 加载table
        /// </summary>
        /// <param name="p_tbl"></param>
        void LoadTable(RptTable p_tbl)
        {
            int rowCount;
            int grpCount = p_tbl.Groups == null ? 0 : p_tbl.Groups.Count;
            List<RptTblGroup> listGrp = p_tbl.Groups;
            int rowIndex = p_tbl.Row;
            int colIndex = p_tbl.Col;
            RptTblPartRow tblRow;

            //加载表头。
            if (p_tbl.Header != null)
            {
                rowCount = p_tbl.Header.Rows.Count;
                for (int i = 0; i < rowCount; i++)
                {
                    tblRow = p_tbl.Header.Rows[i];
                    LoadTblRow(tblRow, rowIndex, colIndex);
                    rowIndex += tblRow.RowSpan == 0 ? 1 : tblRow.RowSpan;
                }
            }

            //加载分组头            
            if (grpCount > 0)
            {
                for (int i = 0; i < grpCount; i++)
                {
                    rowCount = listGrp[i].Header == null ? 0 : listGrp[i].Header.Rows.Count;
                    for (int j = 0; j < rowCount; j++)
                    {
                        tblRow = listGrp[i].Header.Rows[j];
                        LoadTblRow(tblRow, rowIndex, colIndex);
                        rowIndex += tblRow.RowSpan == 0 ? 1 : tblRow.RowSpan;
                    }
                }
            }

            //添加数据
            if (p_tbl.Body != null)
            {
                rowCount = p_tbl.Body.Rows.Count;
                for (int i = 0; i < rowCount; i++)
                {
                    tblRow = p_tbl.Body.Rows[i];
                    LoadTblRow(tblRow, rowIndex, colIndex);
                    rowIndex += tblRow.RowSpan == 0 ? 1 : tblRow.RowSpan;
                }
            }

            //添加分组尾
            if (grpCount > 0)
            {
                for (int i = grpCount; i > 0; i--)
                {
                    rowCount = listGrp[i - 1].Footer == null ? 0 : listGrp[i - 1].Footer.Rows.Count;
                    for (int j = 0; j < rowCount; j++)
                    {
                        tblRow = listGrp[i - 1].Footer.Rows[j];
                        LoadTblRow(tblRow, rowIndex, colIndex);
                        rowIndex += tblRow.RowSpan == 0 ? 1 : tblRow.RowSpan;
                    }
                }
            }

            //加载表尾。
            if (p_tbl.Footer != null)
            {
                rowCount = p_tbl.Footer.Rows.Count;
                for (int i = 0; i < rowCount; i++)
                {
                    tblRow = p_tbl.Footer.Rows[i];
                    LoadTblRow(tblRow, rowIndex, colIndex);
                    rowIndex += tblRow.RowSpan == 0 ? 1 : tblRow.RowSpan;
                }
            }
        }

        /// <summary>
        /// 加载图表
        /// </summary>
        /// <param name="p_chart"></param>
        void LoadChart(RptChart p_chart)
        {
            Dt.Cells.Data.Cell chartCell = _owner.Excel.Sheets[0].Cells[p_chart.Row, p_chart.Col];
            chartCell.RowSpan = p_chart.RowSpan;
            chartCell.ColumnSpan = p_chart.ColSpan;
            chartCell.Background = new SolidColorBrush(Color.FromArgb(0XCC,0XFF,0XFD,0XC5));
            chartCell.VerticalAlignment = CellVerticalAlignment.Center;
            chartCell.HorizontalAlignment = CellHorizontalAlignment.Center;
            chartCell.FontFamily = Res.IconFont;
            chartCell.FontSize = 40;
            chartCell.Text = "\uE08D";
        }

        /// <summary>
        /// 加载table的行单元格，不进行样式清除工作
        /// </summary>
        /// <param name="p_tblRow"></param>
        /// <param name="p_rowIdx"></param>
        /// <param name="p_colIdx"></param>
        void LoadTblRow(RptTblPartRow p_tblRow, int p_rowIdx, int p_colIdx)
        {
            int colIdx = p_colIdx;
            Worksheet ws = _owner.Excel.Sheets[0];

            for (int i = 0; i < p_tblRow.Cells.Count; i++)
            {
                RptText txt = p_tblRow.Cells[i];
                txt.Row = p_rowIdx;
                txt.Col = colIdx++;

                Dt.Cells.Data.Cell cell = ws[txt.Row, txt.Col];
                cell.Value = txt.Data.Str("val");
                txt.ApplyStyle(cell);
            }
        }

        /// <summary>
        /// 清除RptItem所占区域的内容及样式
        /// </summary>
        /// <param name="p_item"></param>
        void ClearItem(RptItem p_item)
        {
            CellRange range = GetRptItemRange(p_item);
            ClearRange(_owner.Excel.Sheets[(int)p_item.Part.PartType], range);
        }

        /// <summary>
        /// 清空sheet上面的矩形内容及样式
        /// </summary>
        /// <param name="p_sheet"></param>
        /// <param name="p_range"></param>
        void ClearRange(Worksheet p_sheet, CellRange p_range)
        {
            // 清空内容
            p_sheet.Clear(p_range.Row, p_range.Column, p_range.RowCount, p_range.ColumnCount);
            // 清空单元格合并 及 样式 及 边框。
            Dt.Cells.Data.Cell cell;
            for (int i = 0; i < p_range.RowCount; i++)
            {
                for (int j = 0; j < p_range.ColumnCount; j++)
                {
                    cell = p_sheet.Cells[p_range.Row + i, p_range.Column + j];
                    cell.RowSpan = 1;
                    cell.ColumnSpan = 1;
                    // 设置 styleName 起到了清除边框的作用
                    cell.StyleName = "";
                }
            }
        }

        /// <summary>
        /// 加载矩阵
        /// </summary>
        /// <param name="p_mat"></param>
        void LoadMatrix(RptMatrix p_mat)
        {
            //行头位置信息
            int rowHeaderCol = p_mat.RowHeader.Col;
            int rowHeaderRowSpan = p_mat.RowHeader.RowSpan;
            int rowHeaderColSpan = p_mat.RowHeader.ColSpan;
            //列头位置信息
            int colHeaderRow = p_mat.ColHeader.Row;
            int colHeaderRowSpan = p_mat.ColHeader.RowSpan;
            int colHeaderColSpan = p_mat.ColHeader.ColSpan;

            if (!p_mat.HideColHeader && !p_mat.HideRowHeader)
            {
                //1、矩阵角
                RptText cor = p_mat.Corner.Item;
                cor.Row = p_mat.Corner.Row;
                cor.Col = p_mat.Corner.Col;
                cor.RowSpan = colHeaderRowSpan;
                cor.ColSpan = rowHeaderColSpan;
                if (string.IsNullOrEmpty(cor.Val))
                    cor.Val = "矩阵角";
                LoadText(cor);
            }

            if (!p_mat.HideRowHeader)
            {
                //2、行头
                if (p_mat.RowHeader != null)
                {
                    LoadMtxLevel(p_mat.RowHeader.Levels);
                }
                //更新矩阵所占列数
                p_mat.ColSpan = rowHeaderColSpan + colHeaderColSpan;
            }

            if (!p_mat.HideColHeader)
            {
                //3、列头
                if (p_mat.ColHeader != null)
                {
                    LoadMtxLevel(p_mat.ColHeader.Levels);
                }
                //更新矩阵所占行数
                p_mat.RowSpan = colHeaderRowSpan + rowHeaderRowSpan;
            }

            //4、数据行
            if (p_mat.Rows != null)
            {
                int sRow = colHeaderRow + (p_mat.HideColHeader ? 0 : colHeaderRowSpan);
                int sCol = rowHeaderCol + (p_mat.HideRowHeader ? 0 : rowHeaderColSpan);
                for (int i = 0; i < p_mat.Rows.Count; i++)
                {
                    RptMtxRow row = p_mat.Rows[i];
                    for (int j = 0; j < row.Cells.Count; j++)
                    {
                        RptText cell = row.Cells[j];
                        cell.Row = sRow + i;
                        cell.Col = sCol + j;
                        LoadText(cell);
                    }
                }
            }
        }

        /// <summary>
        /// 加载层
        /// </summary>
        /// <param name="p_levels"></param>
        void LoadMtxLevel(List<RptMtxLevel> p_levels)
        {
            foreach (RptMtxLevel level in p_levels)
            {
                level.Item.Row = level.Row;
                level.Item.Col = level.Col;
                level.Item.RowSpan = level.RowSpan;
                level.Item.ColSpan = level.ColSpan;
                LoadText(level.Item);

                if (level.SubTotals != null && level.SubTotals.Count > 0)
                {
                    LoadMtxSubTotal(level.SubTotals);
                }
                if (level.SubTitles != null && level.SubTitles.Count > 0)
                {
                    LoadMtxSubTitle(level.SubTitles);
                }
            }
        }

        /// <summary>
        /// 加载小计
        /// </summary>
        /// <param name="p_subTotals"></param>
        void LoadMtxSubTotal(List<RptMtxSubtotal> p_subTotals)
        {
            foreach (RptMtxSubtotal total in p_subTotals)
            {
                if (total.SubTotals != null && total.SubTotals.Count > 0)
                    LoadMtxSubTotal(total.SubTotals);

                total.Item.Row = total.Row;
                total.Item.Col = total.Col;
                total.Item.RowSpan = total.RowSpan;
                total.Item.ColSpan = total.ColSpan;
                LoadText(total.Item);
            }
        }

        /// <summary>
        /// 加载标题
        /// </summary>
        /// <param name="p_subTitles"></param>
        void LoadMtxSubTitle(List<RptMtxSubtitle> p_subTitles)
        {
            foreach (RptMtxSubtitle title in p_subTitles)
            {
                if (title.SubTitles != null && title.SubTitles.Count > 0)
                    LoadMtxSubTitle(title.SubTitles);

                title.Item.Row = title.Row;
                title.Item.Col = title.Col;
                title.Item.RowSpan = title.RowSpan;
                title.Item.ColSpan = title.ColSpan;
                LoadText(title.Item);
            }
        }

        #endregion

        #region 内部方法
        /// <summary>
        /// 得到RptItem所占用的矩形区域
        /// </summary>
        /// <param name="p_item"></param>
        /// <returns></returns>
        Dt.Cells.Data.CellRange GetRptItemRange(RptItem p_item)
        {
            return new CellRange(p_item.Row, p_item.Col, p_item.RowSpan, p_item.ColSpan);
        }

        /// <summary>
        /// 批量执行Excel操作
        /// </summary>
        /// <param name="p_act"></param>
        void Invoke(Action p_act)
        {
            Excel excel = _owner.Excel;
            excel.AutoRefresh = false;
            excel.SuspendEvent();
            p_act();
            excel.ResumeEvent();
            excel.AutoRefresh = true;
        }
        #endregion
    }
}
