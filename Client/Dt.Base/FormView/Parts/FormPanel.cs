#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-10-29 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Base.FormView
{
    /// <summary>
    /// 单元格布局面板
    /// </summary>
    public partial class FormPanel : Panel
    {
        #region 成员变量
        static Rect _rcEmpty = new Rect();

        // 单元格宽度范围
        const double CellMaxWidth = 456;
        const double CellMinWidth = 296;

        /// <summary>
        /// 面板最大尺寸，宽高始终不为无穷大！
        /// </summary>
        Size _maxSize = Size.Empty;

        readonly Fv _owner;
        // 为uno节省可视树级数
        readonly Rectangle _border = new Rectangle { Stroke = Res.浅灰2, IsHitTestVisible = false };
        #endregion

        public FormPanel(Fv p_owner)
        {
            _owner = p_owner;
            Background = Res.浅灰1;
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;

            // 左上边距和容器凑成边框1的效果
            if (!Kit.IsPhoneUI)
                Margin = new Thickness(-1, -1, 0, 0);

            // uno不画出界的线，右下边框1
#if UWP
            _border.Margin = new Thickness(0, 0, -1, -1);
#endif
            Children.Add(_border);
        }

        /// <summary>
        /// 设置面板的最大尺寸，宽高始终不为无穷大！
        /// </summary>
        internal void SetMaxSize(Size p_size)
        {
            _maxSize = p_size;
        }

        /// <summary>
        /// 清空子元素，自动添加边框
        /// </summary>
        internal void Clear()
        {
            Children.Clear();
            Children.Add(_border);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (Children.Count == 0
                || availableSize.Width == 0
                || availableSize.Height == 0)
                return base.MeasureOverride(availableSize);

            int colCount;
            double colWidth, width;
            double maxWidth = _maxSize.Width;

            // 确定列数和列宽
            if (maxWidth < CellMinWidth * 2 || _owner.MaxColCount == 1)
            {
                // 只能放一列
                colWidth = maxWidth > CellMaxWidth ? CellMaxWidth : maxWidth;
                colCount = 1;
            }
            else
            {
                // 可视高度的80%
                double maxHeight = Math.Floor(_maxSize.Height * 0.8);
                if (maxHeight >= GetTotalHeight(1, CellMaxWidth, false))
                {
                    // 一列在可视高度的80%以内，仍一列
                    colWidth = CellMaxWidth;
                    colCount = 1;
                }
                else if (_owner.MaxColCount == 2
                    || maxWidth < CellMinWidth * 3
                    || maxHeight >= GetTotalHeight(2, (maxWidth / 2) > CellMaxWidth ? CellMaxWidth : (maxWidth / 2), false))
                {
                    // 采用两列：
                    // 外部设置最多两列；
                    // 最多只能放两列；
                    // 两列在可视高度的80%以内；
                    width = maxWidth / 2;
                    colWidth = width > CellMaxWidth ? CellMaxWidth : width;
                    colCount = 2;
                }
                else if (_owner.MaxColCount == 3
                    || maxWidth < CellMinWidth * 4
                    || maxHeight >= GetTotalHeight(3, (maxWidth / 3) > CellMaxWidth ? CellMaxWidth : (maxWidth / 3), false))
                {
                    // 采用三列：
                    // 外部设置最多三列；
                    // 最多只能放三列；
                    // 三列在可视高度的80%以内；
                    width = maxWidth / 3;
                    colWidth = width > CellMaxWidth ? CellMaxWidth : width;
                    colCount = 3;
                }
                else
                {
                    // 最多四列
                    width = maxWidth / 4;
                    colWidth = width > CellMaxWidth ? CellMaxWidth : width;
                    colCount = 4;
                }
            }

            double height = GetTotalHeight(colCount, colWidth, true);
            width = colCount * colWidth;
            Size size = new Size(width, height);
            _border.Measure(size);
            return size;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Children.Count == 0)
                return base.ArrangeOverride(finalSize);

            foreach (var cell in Children.OfType<IFvCell>())
            {
                cell.Arrange((cell.Visibility == Visibility.Visible) ? cell.Bounds : _rcEmpty);
            }
            _border.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
            return finalSize;
        }

        /// <summary>
        /// 分成n列时需要的高度
        /// </summary>
        /// <param name="p_colCount">列数</param>
        /// <param name="p_colWidth">列宽</param>
        /// <param name="p_measure">是否测量</param>
        /// <returns></returns>
        double GetTotalHeight(int p_colCount, double p_colWidth, bool p_measure)
        {
            int rowIndex = 0;
            Size szCollapsed = new Size(p_colWidth, Res.RowOuterHeight);

            // 只一列
            if (p_colCount == 1)
            {
                foreach (var cell in Children.OfType<IFvCell>())
                {
                    if (cell.Visibility == Visibility.Collapsed)
                    {
                        cell.Measure(szCollapsed);
                        continue;
                    }

                    // 确定行数
                    int rowSpan;
                    if (cell.RowSpan > 0)
                    {
                        rowSpan = cell.RowSpan;
                    }
                    else
                    {
                        // 自动行高，计算占用行数，uno中无法使用double.MaxValue！
                        cell.Measure(new Size(p_colWidth, Kit.ViewHeight));
                        // uno中高度莫名多出小数点后的
                        rowSpan = (int)Math.Ceiling(Math.Floor(cell.DesiredSize.Height) / Res.RowOuterHeight);
                    }

                    // 测量
                    if (p_measure)
                    {
                        Size size = new Size(p_colWidth, Res.RowOuterHeight * rowSpan);
                        cell.Bounds = new Rect(
                            new Point(0, rowIndex * Res.RowOuterHeight),
                            size);
                        // 自动行高不再测量
                        if (cell.RowSpan > 0)
                            cell.Measure(size);
                    }
                    rowIndex += rowSpan;
                }
                return rowIndex * Res.RowOuterHeight;
            }

            // 多列情况
            List<bool[]> map = new List<bool[]>();
            int colIndex = 0;

            // 地图记录单元格占用情况
            foreach (var cell in Children.OfType<IFvCell>())
            {
                if (cell.Visibility == Visibility.Collapsed)
                {
                    cell.Measure(szCollapsed);
                    continue;
                }

                if (cell.IsHorStretch)
                {
                    // 从列头开始
                    if (colIndex != 0)
                    {
                        // 新行
                        colIndex = 0;
                        if (++rowIndex >= map.Count)
                            map.Add(new bool[p_colCount]);
                    }

                    // 查找放置位置
                    while (true)
                    {
                        if (rowIndex >= map.Count)
                            map.Add(new bool[p_colCount]);

                        bool find = true;
                        for (int i = 0; i < p_colCount; i++)
                        {
                            if (map[rowIndex][i])
                            {
                                find = false;
                                break;
                            }
                        }
                        if (find)
                            break;
                        rowIndex++;
                    }

                    // 确定行数
                    int rowSpan;
                    if (cell.RowSpan > 0)
                    {
                        rowSpan = cell.RowSpan;
                    }
                    else
                    {
                        // 自动行高，计算占用行数
                        cell.Measure(new Size(p_colWidth * p_colCount, Kit.ViewHeight));
                        rowSpan = (int)Math.Ceiling(cell.DesiredSize.Height / Res.RowOuterHeight);
                    }

                    // 标志占用
                    for (int i = 0; i < rowSpan; i++)
                    {
                        if (rowIndex + i >= map.Count)
                            map.Add(new bool[p_colCount]);

                        for (int j = 0; j < p_colCount; j++)
                        {
                            map[rowIndex + i][j] = true;
                        }
                    }

                    // 测量
                    if (p_measure)
                    {
                        Size size = new Size(p_colWidth * p_colCount, Res.RowOuterHeight * rowSpan);
                        cell.Bounds = new Rect(
                            new Point(0, rowIndex * Res.RowOuterHeight),
                            size);
                        // 自动行高不再测量
                        if (cell.RowSpan > 0)
                            cell.Measure(size);
                    }

                    // 下一位置
                    rowIndex += cell.RowSpan;
                }
                else
                {
                    // 查找放置位置
                    while (true)
                    {
                        if (rowIndex >= map.Count)
                            map.Add(new bool[p_colCount]);

                        if (!map[rowIndex][colIndex])
                            break;

                        // 下一列
                        colIndex++;
                        if (colIndex >= p_colCount)
                        {
                            // 从下一行开始
                            colIndex = 0;
                            rowIndex++;
                        }
                    }

                    // 确定行数
                    int rowSpan;
                    if (cell.RowSpan > 0)
                    {
                        rowSpan = cell.RowSpan;
                    }
                    else
                    {
                        // 自动行高，计算占用行数
                        cell.Measure(new Size(p_colWidth, Kit.ViewHeight));
                        rowSpan = (int)Math.Ceiling(cell.DesiredSize.Height / Res.RowOuterHeight);
                    }

                    // 标志占用
                    for (int i = 0; i < rowSpan; i++)
                    {
                        if (rowIndex + i >= map.Count)
                            map.Add(new bool[p_colCount]);
                        map[rowIndex + i][colIndex] = true;
                    }

                    // 测量
                    if (p_measure)
                    {
                        Size size = new Size(p_colWidth, Res.RowOuterHeight * rowSpan);
                        cell.Bounds = new Rect(
                            new Point(colIndex * p_colWidth, rowIndex * Res.RowOuterHeight),
                            size);
                        // 自动行高不再测量
                        if (cell.RowSpan > 0)
                            cell.Measure(size);
                    }

                    // 下一位置
                    colIndex++;
                    if (colIndex >= p_colCount)
                    {
                        // 从下一行开始
                        colIndex = 0;
                        rowIndex++;
                    }
                }
            }
            return map.Count * Res.RowOuterHeight;
        }
    }
}