#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-06-11 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Cells.Data;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Media;
#endregion

namespace Dt.Base.Report
{
    public class RptRender
    {
        readonly RptInfo _info;
        Worksheet _ws;

        public RptRender(RptInfo p_info)
        {
            _info = p_info;
        }

        /// <summary>
        /// 渲染输出
        /// </summary>
        public async Task Render()
        {
            int start;
            RptRootInst inst = _info.Inst;
            List<double> rows = new List<double>();
            var pageSetting = _info.Root.PageSetting;

            // 填充空行空列，统计所有行高列宽
            foreach (PageDefine page in inst.Rows)
            {
                // 页面之间的缝隙，为避免打印时分页边框不正常！
                rows.Add(RptRootInst.PageGap);

                // 记录页面开始行索引
                start = rows.Count;
                page.Offset = start;

                // 页眉行，两行，一行作为与报表内容的间距
                if (inst.HeaderHeight > 0)
                {
                    rows.Add(_info.Root.Header.Height);
                    rows.Add(_info.Root.Header.BodySpacing);
                }

                // 内容行
                double total = 0;
                foreach (double height in page.Size)
                {
                    rows.Add(height);
                    total += height;
                }

                // 填充空行
                if (total < inst.BodyHeight && !pageSetting.AutoPaperSize)
                    rows.Add(inst.BodyHeight - total);

                // 页脚行，两行，一行作为与报表内容的间距
                if (inst.FooterHeight > 0)
                {
                    rows.Add(_info.Root.Footer.BodySpacing);
                    rows.Add(_info.Root.Footer.Height);
                }
                page.Total = rows.Count - start;
            }

            List<double> cols = new List<double>();
            foreach (PageDefine page in inst.Cols)
            {
                // 页面之间的左侧间隔缝隙
                cols.Add(RptRootInst.PageGap);

                // 记录页面开始列索引
                start = cols.Count;
                page.Offset = start;

                double total = 0.0;
                foreach (double width in page.Size)
                {
                    cols.Add(width);
                    total += width;
                }

                // 填充空列
                if (total < inst.BodyWidth && !pageSetting.AutoPaperSize)
                    cols.Add(inst.BodyWidth - total);
                page.Total = cols.Count - start;
            }

            // 创建Worksheet
            _ws = new Worksheet(rows.Count, cols.Count);
            // 不显示选择区黑框和触摸时的两圈，改用 Excel.ShowSelection 控制
            //_ws.SelectionBorderColor = Colors.Transparent;
            //_ws.TouchSelectionGripperBackgroundColor = Colors.Transparent;
            // 单元格不可编辑，图表可拖动
            _ws.LockCell = true;
            // Wp始终不可编辑
            if (Kit.IsPhoneUI)
                _ws.Protect = true;
            _info.Sheet = _ws;

            // 初始化行高列宽
            double totalHeight = 0;
            double totalWidth = 0;
            for (int i = 0; i < rows.Count; i++)
            {
                _ws.Rows[i].Height = rows[i];
                totalHeight += rows[i];
            }
            for (int i = 0; i < cols.Count; i++)
            {
                _ws.Columns[i].Width = cols[i];
                totalWidth += cols[i];
            }

            // 页面设置，输出Pdf和打印时有效
            var pi = _ws.PrintInfo;
            if (pageSetting.AutoPaperSize)
            {
                pi.PaperSize = new PaperSize(
                    Math.Round(totalWidth / 0.96) + pageSetting.LeftMargin + pageSetting.RightMargin + 4,
                    Math.Round(totalHeight / 0.96) + pageSetting.TopMargin + pageSetting.BottomMargin + 4);
            }
            else
            {
                pi.PaperSize = pageSetting.PaperSize;
            }
            pi.Margin = pageSetting.PageMargins;
            pi.Orientation = pageSetting.Landscape ? PrintPageOrientation.Landscape : PrintPageOrientation.Portrait;
            pi.ShowBorder = false;
            pi.PageOrder = PrintPageOrder.OverThenDown;

            // 输出所有项
            foreach (RptPage page in _info.Inst.Pages)
            {
                PageDefine define = page.Rows;
                int startRow = define.Start;
                int offsetRow = define.Offset;
                int rowTotal = define.Total;
                int offsetBody = offsetRow + (page.HeaderItems.Count > 0 ? 2 : 0);
                define = page.Cols;
                int startCol = define.Start;
                int offsetCol = define.Offset;
                page.UpdatePageNum();

                // 页眉
                if (page.HeaderItems.Count > 0)
                {
                    // 采用默认页眉页脚时，动态调整占的列数，每页的列数可能不同！
                    if (_info.Root.Header.DefaultHeader)
                    {
                        var item = page.HeaderItems[0];
                        int tempCol = offsetCol + item.Item.Col;
                        if (tempCol < cols.Count)
                        {
                            // 占用当前页的所有列
                            await RenderText(item, offsetRow, tempCol, -1, page.Cols.Total);
                        }
                    }
                    else
                    {
                        foreach (RptTextInst item in page.HeaderItems)
                        {
                            // 不渲染超出的列
                            int tempCol = offsetCol + item.Item.Col;
                            if (tempCol < cols.Count)
                                await RenderText(item, offsetRow, tempCol);
                        }
                    }
                }

                // 内容
                foreach (RptOutputInst item in page.Items)
                {
                    int row = item.Region.Row - startRow + offsetBody;
                    int col = item.Region.Col - startCol + offsetCol;
                    RptTextInst txt = item as RptTextInst;
                    if (txt != null)
                    {
                        Cells.Data.Cell tmpCell;
                        CellRange range;
                        RptText text = txt.Item as RptText;
                        var dataRow = (txt.Item as RptText).Data;
                        var renderCell = await RenderText(txt, row, col);

                        if (row > startRow && dataRow.Bool("hidetopdup"))
                        {
                            tmpCell = _ws[row - 1, col];
                            if (tmpCell.Tag != null
                                && txt.Item.Data.Bool("hidetopdup")
                                && tmpCell.Text == renderCell.Text)
                            {
                                range = _ws.GetSpanCell(row - 1, col);
                                if (range != null)
                                    tmpCell = _ws[range.Row, range.Column];
                                if (tmpCell.ColumnSpan == renderCell.ColumnSpan)
                                    tmpCell.RowSpan += renderCell.RowSpan;
                            }
                        }

                        if (col > startCol && dataRow.Bool("hideleftdup"))
                        {
                            tmpCell = _ws[row, col - 1];
                            if (tmpCell.Tag != null
                               && txt.Item.Data.Bool("hideleftdup")
                               && tmpCell.Text == renderCell.Text)
                            {
                                range = _ws.GetSpanCell(row, col - 1);
                                if (range != null)
                                    tmpCell = _ws[range.Row, range.Column];
                                if (tmpCell.RowSpan == renderCell.RowSpan)
                                    tmpCell.ColumnSpan += renderCell.ColumnSpan;
                            }
                        }
                    }
                    else if (item is RptChartInst chart)
                    {
                        ((RptChart)chart.Item).Render(_ws, row, col);
                    }
                    else if (item is RptImageInst img)
                    {
                        ((RptImage)img.Item).Render(_ws, row, col);
                    }
                    else if (item is RptSparklineInst spark)
                    {
                        ((RptSparkline)spark.Item).Render(_ws, row, col);
                    }
                }

                // 页脚
                if (page.FooterItems.Count > 0)
                {
                    // 采用默认页眉页脚时，动态调整占的列数，每页的列数可能不同！
                    if (_info.Root.Footer.DefaultFooter)
                    {
                        var item = page.FooterItems[0];
                        int tempCol = offsetCol + item.Item.Col;
                        if (tempCol < cols.Count)
                        {
                            // 占用当前页的所有列
                            await RenderText(item, offsetRow + rowTotal - 1, tempCol, -1, page.Cols.Total);
                        }
                    }
                    else
                    {
                        foreach (RptTextInst item in page.FooterItems)
                        {
                            // 不渲染超出的列
                            int tempCol = offsetCol + item.Item.Col;
                            if (tempCol < cols.Count)
                                await RenderText(item, offsetRow + rowTotal - 1, tempCol);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 输出文本内容
        /// </summary>
        /// <param name="p_txt"></param>
        /// <param name="p_row"></param>
        /// <param name="p_col"></param>
        /// <param name="p_rowSpan"></param>
        /// <param name="p_colSpan"></param>
        async Task<Cells.Data.Cell> RenderText(RptTextInst p_txt, int p_row, int p_col, int p_rowSpan = -1, int p_colSpan = -1)
        {
            var item = p_txt.Item as RptText;
            var cell = _ws[p_row, p_col];
            cell.ColumnSpan = p_colSpan > 0 ? p_colSpan : item.ColSpan;
            cell.RowSpan = p_rowSpan > 0 ? p_rowSpan : item.RowSpan;
            cell.Tag = p_txt;
            item.ApplyStyle(cell);

            if (item.IsScriptRender && _info.ScriptObj != null)
            {
                // 脚本绘制
                await _info.ScriptObj.RenderCell(cell, new RptCellArgs(p_txt));
            }
            else
            {
                cell.Value = await p_txt.GetValue();
            }
            return cell;
        }
    }
}
