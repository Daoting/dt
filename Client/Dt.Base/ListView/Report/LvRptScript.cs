#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-04-10 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
#endregion

namespace Dt.Base.ListView
{
    /// <summary>
    /// 导出Lv报表脚本
    /// </summary>
    internal class LvRptScript : RptScript
    {
        LvExporter _exporter;
        Dictionary<Core.Row, LvItem> _rowStyles;

        public void SetExporter(LvExporter p_exporter)
        {
            _exporter = p_exporter;
            if (_exporter.CustomRowStyle)
                _rowStyles = new Dictionary<Core.Row, LvItem>();
        }

        public override Task<Table> GetData(string p_name)
        {
            return Task.FromResult(_exporter.Data);
        }

        public override async Task RenderCell(Cells.Data.Cell p_cell, RptCellArgs p_args)
        {
            Col col = (Col)_exporter.Cols[p_args.Col - (_exporter.Info.RowNO ? 1 : 0)];
            Dot dot = new Dot(p_args.Data);
            dot.ID = col.ID;
            dot.Call = col.Call;
            dot.Format = col.Format;

            // 模拟
            var ui = dot.GetCellUI();
            if (dot.SetCallback != null)
            {
                var callArgs = new CallArgs(dot);
                dot.SetCallback(callArgs);
                // 确保内部所有异步完毕
                await callArgs.EnsureAllCompleted();
            }

            // 自定义行样式
            LvItem rowStyle = null;
            if (_rowStyles != null)
            {
                if (!_rowStyles.TryGetValue(p_args.Data, out rowStyle))
                {
                    rowStyle = new LvItem(_exporter.Lv, p_args.Data, 0);
                    // 调用外部自定义行样式
                    ((IViewItemHost)_exporter.Lv).SetItemStyle(rowStyle);
                    _rowStyles[p_args.Data] = rowStyle;
                }
            }

            // 样式优先级： TextBlock > Dot > RowStyle > RptText
            // 之前已调用RptText.ApplyStyle，此处为应用Dot RowStyle样式，最后CopyTextBlock时应用TextBlock样式
            if (dot.ExistLocalValue(Dot.ForegroundProperty))
                p_cell.Foreground = dot.Foreground;
            else if (rowStyle != null && rowStyle.ExistLocalValue(LvItem.ForegroundProperty))
                p_cell.Foreground = rowStyle.Foreground;

            if (dot.ExistLocalValue(Dot.BackgroundProperty))
                p_cell.Background = dot.Background;
            else if (rowStyle != null && rowStyle.ExistLocalValue(LvItem.BackgroundProperty))
                p_cell.Background = rowStyle.Background;
            
            if (dot.ExistLocalValue(Dot.FontWeightProperty))
                p_cell.FontWeight = dot.FontWeight;
            else if (rowStyle != null && rowStyle.ExistLocalValue(LvItem.FontWeightProperty))
                p_cell.FontWeight = rowStyle.FontWeight;
            
            if (dot.ExistLocalValue(Dot.FontStyleProperty))
                p_cell.FontStyle = dot.FontStyle;
            else if (rowStyle != null && rowStyle.ExistLocalValue(LvItem.FontStyleProperty))
                p_cell.FontStyle = rowStyle.FontStyle;
            
            if (dot.ExistLocalValue(Dot.FontSizeProperty))
                p_cell.FontSize = dot.FontSize;
            else if (rowStyle != null && rowStyle.ExistLocalValue(LvItem.FontSizeProperty))
                p_cell.FontSize = rowStyle.FontSize;

            if (ui is TextBlock tb)
            {
                CopyTextBlock(p_cell, tb);
                return;
            }

            bool toImg = false;
            string txt = null;
            foreach (var elem in ui.FindChildren(true))
            {
                // 只有布局控件和TextBlock时，将所有文本合并
                if (elem is Panel || elem is Border)
                    continue;

                if (elem is TextBlock block)
                {
                    if (txt == null)
                        txt = block.Text;
                    else
                        txt += " " + block.Text;
                    continue;
                }

                // 需要转图片
                toImg = true;
                break;
            }

            if (!toImg)
            {
                // 显示合并后的文本，无样式、无回车换行
                if (txt != null)
                    CopyTextBlock(p_cell, new TextBlock { Text = txt.Replace("\r\n", " ").Replace("\n", " ") });
                return;
            }

            // excel默认字体
            dot.FontSize = 15;
            dot.Background = Res.WhiteBrush;
            // 留出边框
            dot.Width = p_cell.Column.Width - 4;
            dot.Height = p_cell.Row.Height - 4;
            dot.Content = ui;

            var stream = await dot.GetSnapStream();
            var pic = _exporter.Info.Sheet.AddPicture(
                    Guid.NewGuid().ToString().Substring(0, 6),
                    stream,
                    p_cell.Row.Index,
                    2,
                    p_cell.Column.Index,
                    2,
                    p_cell.Row.Index + 1,
                    -2,
                    p_cell.Column.Index + 1,
                    -2);
            // 锁定禁止拖动缩放
            pic.Locked = true;
        }

        void CopyTextBlock(Cells.Data.Cell p_cell, TextBlock p_tb)
        {
            // 转数字自动右对齐
            if (double.TryParse(p_tb.Text, out double dval))
                p_cell.Value = dval;
            else
                p_cell.Value = p_tb.Text;

            if (p_tb.ExistLocalValue(TextBlock.FontFamilyProperty))
                p_cell.FontFamily = p_tb.FontFamily;
            if (p_tb.ExistLocalValue(TextBlock.FontSizeProperty))
                p_cell.FontSize = p_tb.FontSize;
            if (p_tb.ExistLocalValue(TextBlock.FontStyleProperty))
                p_cell.FontStyle = p_tb.FontStyle;

            if (p_tb.TextAlignment == TextAlignment.Right || p_tb.HorizontalAlignment == HorizontalAlignment.Right)
                p_cell.HorizontalAlignment = CellHorizontalAlignment.Right;
            else if (p_tb.TextAlignment == TextAlignment.Center || p_tb.HorizontalAlignment == HorizontalAlignment.Center)
                p_cell.HorizontalAlignment = CellHorizontalAlignment.Center;

            if (p_tb.VerticalAlignment == VerticalAlignment.Top)
                p_cell.VerticalAlignment = CellVerticalAlignment.Top;
            else if (p_tb.VerticalAlignment == VerticalAlignment.Bottom)
                p_cell.VerticalAlignment = CellVerticalAlignment.Bottom;

            if (p_tb.ExistLocalValue(TextBlock.ForegroundProperty))
                p_cell.Foreground = p_tb.Foreground;
        }
    }
}
