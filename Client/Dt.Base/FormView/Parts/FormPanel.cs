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
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Shapes;
#endregion

namespace Dt.Base.FormView
{
    /// <summary>
    /// 单元格布局面板
    /// </summary>
    public partial class FormPanel : Panel
    {
        #region 成员变量
        // 单元格宽度范围
        internal const double CellMaxWidth = 456;
        internal const double CellMinWidth = 296;

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
#if WIN
            _border.Margin = new Thickness(0, 0, -1, -1);
#endif
            Children.Add(_border);
        }

        /// <summary>
        /// 设置面板的最大尺寸，宽高始终不为无穷大！
        /// </summary>
        internal void SetMaxSize(Size p_size)
        {
            // 尺寸变化大于2有效，否则iOS版易造成死循环，每次 p_size 有微小变化！！！
            if (Math.Abs(_maxSize.Width - p_size.Width) > 2 || Math.Abs(_maxSize.Height - p_size.Height) > 2)
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

        /// <summary>
        /// 获取按指定列数布局时占用的总高度
        /// </summary>
        /// <param name="p_colCount">列数，范围 1~4</param>
        /// <returns></returns>
        internal double GetTotalHeight(int p_colCount)
        {
            if (p_colCount == 1)
                return GetSingleColomnHeight(CellMaxWidth, false);
            if (p_colCount > 1)
                return GetMultiColomnHeight(Math.Min(p_colCount, 4), CellMaxWidth, false);
            return 0;
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
                if (maxHeight >= GetSingleColomnHeight(CellMaxWidth, false))
                {
                    // 一列在可视高度的80%以内，仍一列
                    colWidth = CellMaxWidth;
                    colCount = 1;
                }
                else if (_owner.MaxColCount == 2
                    || maxWidth < CellMinWidth * 3
                    || maxHeight >= GetMultiColomnHeight(2, (maxWidth / 2) > CellMaxWidth ? CellMaxWidth : (maxWidth / 2), false))
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
                    || maxHeight >= GetMultiColomnHeight(3, (maxWidth / 3) > CellMaxWidth ? CellMaxWidth : (maxWidth / 3), false))
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

            double height = colCount == 1 ?
                GetSingleColomnHeight(colWidth, true)
                : GetMultiColomnHeight(colCount, colWidth, true);
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
                cell.Arrange((cell.Visibility == Visibility.Visible) ? cell.Bounds : Res.HideRect);
            }
            _border.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
            return finalSize;
        }

        /// <summary>
        /// 计算只一列时需要的高度
        /// </summary>
        /// <param name="p_colWidth">列宽</param>
        /// <param name="p_measure">是否测量</param>
        /// <returns></returns>
        double GetSingleColomnHeight(double p_colWidth, bool p_measure)
        {
            int rowIndex = 0;
            Size szCollapsed = new Size(p_colWidth, Res.RowOuterHeight);
            // 一列未占满时，已布局单元格占的比例
            double totalColSpan = 0;
            // 一列未占满时，已布局单元格占用的最大行数
            int lastRowSpan = 0;

            foreach (var cell in Children.OfType<IFvCell>())
            {
                if (cell.Visibility == Visibility.Collapsed)
                {
                    cell.Measure(szCollapsed);
                    continue;
                }

                // 当前格占用的行数、列比例
                int rowSpan = 0;
                double colSpan = (cell.ColSpan <= 0 || cell.ColSpan >= 1) ? 1 : cell.ColSpan;

                #region 放在新行，占用整列
                if (colSpan == 1)
                {
                    // 清除以前未占满的状态
                    totalColSpan = 0;
                    rowIndex += lastRowSpan;
                    lastRowSpan = 0;

                    // 确定行数
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
                    continue;
                }
                #endregion

                #region 放在新行，占用部分列
                if (totalColSpan == 0 || totalColSpan + colSpan > 1)
                {
                    // 清除以前未占满的状态
                    totalColSpan = colSpan;
                    rowIndex += lastRowSpan;

                    // 确定行数
                    if (cell.RowSpan > 0)
                    {
                        lastRowSpan = cell.RowSpan;
                    }
                    else
                    {
                        // 自动行高，计算占用行数，uno中无法使用double.MaxValue！
                        cell.Measure(new Size(p_colWidth * colSpan, Kit.ViewHeight));
                        // uno中高度莫名多出小数点后的
                        lastRowSpan = (int)Math.Ceiling(Math.Floor(cell.DesiredSize.Height) / Res.RowOuterHeight);
                    }

                    // 测量
                    if (p_measure)
                    {
                        Size size = new Size(p_colWidth * colSpan, Res.RowOuterHeight * lastRowSpan);
                        cell.Bounds = new Rect(
                            new Point(0, rowIndex * Res.RowOuterHeight),
                            size);
                        // 自动行高不再测量
                        if (cell.RowSpan > 0)
                            cell.Measure(size);
                    }
                    continue;
                }
                #endregion

                #region 放在当前行，占用部分列
                // 确定行数
                if (cell.RowSpan > 0)
                {
                    rowSpan = cell.RowSpan;
                }
                else
                {
                    // 自动行高，计算占用行数，uno中无法使用double.MaxValue！
                    cell.Measure(new Size(p_colWidth * colSpan, Kit.ViewHeight));
                    // uno中高度莫名多出小数点后的
                    rowSpan = (int)Math.Ceiling(Math.Floor(cell.DesiredSize.Height) / Res.RowOuterHeight);
                }

                // 取一行中最大占用行数
                if (rowSpan > lastRowSpan)
                    lastRowSpan = rowSpan;

                // 测量
                if (p_measure)
                {
                    Size size = new Size(p_colWidth * colSpan, Res.RowOuterHeight * rowSpan);
                    cell.Bounds = new Rect(
                        new Point(p_colWidth * totalColSpan, rowIndex * Res.RowOuterHeight),
                        size);
                    // 自动行高不再测量
                    if (cell.RowSpan > 0)
                        cell.Measure(size);
                }

                // 累计已布局单元格占的比例
                totalColSpan += colSpan;
                #endregion
            }
            return (rowIndex + lastRowSpan) * Res.RowOuterHeight;
        }

        /// <summary>
        /// 计算多列时需要的高度
        /// </summary>
        /// <param name="p_colCount">列数</param>
        /// <param name="p_colWidth">列宽</param>
        /// <param name="p_measure">是否测量</param>
        /// <returns></returns>
        double GetMultiColomnHeight(int p_colCount, double p_colWidth, bool p_measure)
        {
            // 当前布局的行序号
            int rowIndex = 0;
            // 当前布局的列序号
            int colIndex = 0;
            // 一列未占满时，已布局单元格占的比例
            double totalColSpan = 0;
            Size szCollapsed = new Size(p_colWidth, Res.RowOuterHeight);
            // 单元格占用情况
            List<bool[]> map = new List<bool[]>();

            // 地图记录单元格占用情况
            foreach (var cell in Children.OfType<IFvCell>())
            {
                if (cell.Visibility == Visibility.Collapsed)
                {
                    cell.Measure(szCollapsed);
                    continue;
                }

                #region 水平填充
                // 水平填充，放在新行，占用整行所有列
                int rowSpan = 0;

                if (cell.ColSpan <= 0)
                {
                    totalColSpan = 0;
                    if (colIndex != 0)
                    {
                        // 新行开头
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
                    rowIndex += rowSpan;
                    continue;
                }
                #endregion

                #region 放在新列
                // 从新列的开头布局，可能是当前行或新行
                double colSpan = (cell.ColSpan >= 1) ? 1 : cell.ColSpan;

                if (totalColSpan == 0
                    || totalColSpan + colSpan > 1
                    || colSpan == 1)
                {
                    totalColSpan = colSpan == 1 ? 0 : colSpan;

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
                    if (cell.RowSpan > 0)
                    {
                        rowSpan = cell.RowSpan;
                    }
                    else
                    {
                        // 自动行高，计算占用行数
                        cell.Measure(new Size(p_colWidth * colSpan, Kit.ViewHeight));
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
                        Size size = new Size(p_colWidth * colSpan, Res.RowOuterHeight * rowSpan);
                        cell.Bounds = new Rect(
                            new Point(colIndex * p_colWidth, rowIndex * Res.RowOuterHeight),
                            size);
                        // 自动行高不再测量
                        if (cell.RowSpan > 0)
                            cell.Measure(size);
                    }

                    // 下一位置
                    if (colSpan == 1)
                    {
                        colIndex++;
                        if (colIndex >= p_colCount)
                        {
                            // 从下一行开始
                            colIndex = 0;
                            rowIndex++;
                        }
                    }
                    continue;
                }
                #endregion

                #region 放在当前列
                // 确定行数
                if (cell.RowSpan > 0)
                {
                    rowSpan = cell.RowSpan;
                }
                else
                {
                    // 自动行高，计算占用行数，uno中无法使用double.MaxValue！
                    cell.Measure(new Size(p_colWidth * colSpan, Kit.ViewHeight));
                    // uno中高度莫名多出小数点后的
                    rowSpan = (int)Math.Ceiling(Math.Floor(cell.DesiredSize.Height) / Res.RowOuterHeight);
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
                    Size size = new Size(p_colWidth * colSpan, Res.RowOuterHeight * rowSpan);
                    cell.Bounds = new Rect(
                        new Point(colIndex * p_colWidth + p_colWidth * totalColSpan, rowIndex * Res.RowOuterHeight),
                        size);
                    // 自动行高不再测量
                    if (cell.RowSpan > 0)
                        cell.Measure(size);
                }

                // 累计已布局单元格占的比例
                totalColSpan += colSpan;
                #endregion
            }
            return map.Count * Res.RowOuterHeight;
        }
    }
}