#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.CalcEngine;
using Dt.Cells.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Cells.Data
{
    public partial class Worksheet : IXmlSerializable, ICellsSupport, ICalcSource, IEqualityComparer<ICalcSource>, ICustomNameSupport, ISubtotalSupport, IMultiSourceProvider, IBindableSheet, ISparklineSheet, ITableSheet, IUIActionExecuter, IPrintSupportInternal, IFloatingObjectSheet, ICalcEvaluator, IThemeSupport
    {
        bool _lockCell;

        /// <summary>
        /// 获取设置是否锁定单元格只读，报表预览中用，只读优先级高于Protect
        /// 在报表预览中实现单元格不可编辑且图表可拖动
        /// </summary>
        public bool LockCell
        {
            get { return _lockCell; }
            set { _lockCell = value; }
        }

        /// <summary>
        /// 获取视口当前左上角位置（相对于第一行第一列）
        /// </summary>
        /// <param name="p_rowViewportIndex"></param>
        /// <param name="p_colViewportIndex"></param>
        /// <returns></returns>
        public Point GetTopLeftLocation(int p_rowViewportIndex, int p_colViewportIndex)
        {
            ViewportInfo view = GetViewportInfo();
            if (view.RowViewportCount <= 0
                || view.ColumnViewportCount <= 0
                || p_rowViewportIndex < 0
                || p_rowViewportIndex >= view.RowViewportCount
                || p_colViewportIndex < 0
                || p_colViewportIndex >= view.ColumnViewportCount)
                return new Point();

            int topRow = view.TopRows[p_rowViewportIndex];
            int leftCol = view.LeftColumns[p_colViewportIndex];
            double x = 0.0;
            double y = 0.0;

            for (int i = 0; i < leftCol; i++)
            {
                if (_columnRangeGroup != null
                    && !_columnRangeGroup.IsEmpty()
                    && _columnRangeGroup.IsCollapsed(i))
                    continue;

                AxisInfo info = _viewportColumns[i];
                if (info == null)
                {
                    x += DefaultColumnWidth;
                }
                else if (info.Visible)
                {
                    if (info.IsSizeSet() && info.Size >= 0.0)
                        x += info.Size;
                    else
                        x += DefaultColumnWidth;
                }
            }

            for (int j = 0; j < topRow; j++)
            {
                if (_rowRangeGroup != null
                    && !_rowRangeGroup.IsEmpty()
                    && _rowRangeGroup.IsCollapsed(j))
                    continue;

                AxisInfo info = _viewportRows[j];
                if (info == null)
                {
                    y += DefaultRowHeight;
                }
                else if (info.Visible)
                {
                    if (info.IsSizeSet() && info.Size >= 0.0)
                        y += info.Size;
                    else
                        y += DefaultRowHeight;
                }
            }
            return new Point(x, y);
        }

        /// <summary>
        /// 获取活动视口中指定区域的矩形（相对与整个Excel的左上角）
        /// </summary>
        /// <param name="p_range"></param>
        /// <returns></returns>
        public Rect GetRangeBound(CellRange p_range)
        {
            var info = GetViewportInfo();
            int startRow = GetViewportTopRow(info.ActiveRowViewport);
            int startCol = GetViewportLeftColumn(info.ActiveColumnViewport);
            Rect rc = new Rect();
            if (p_range.Row < startRow || p_range.Column < startCol)
                return rc;

            Point pt = new Point();
            double width = 0;
            double height = 0;

            // 多视口情况
            if (info.ActiveRowViewport > 0)
            {
                for (int i = 0; i < info.ActiveRowViewport; i++)
                {
                    pt.Y += GetViewportHeight(i);
                }
            }
            if (info.ActiveColumnViewport > 0)
            {
                for (int i = 0; i < info.ActiveColumnViewport; i++)
                {
                    pt.X += GetViewportWidth(i);
                }
            }

            // 内容行列
            for (int i = startRow; i < p_range.Row; i++)
            {
                pt.Y += GetActualRowHeight(i, SheetArea.Cells);
            }
            for (int i = startCol; i < p_range.Column; i++)
            {
                pt.X += GetActualColumnWidth(i, SheetArea.Cells);
            }
            for (int i = 0; i < p_range.RowCount; i++)
            {
                height += GetActualRowHeight(i + p_range.Row, SheetArea.Cells);
            }
            for (int i = 0; i < p_range.ColumnCount; i++)
            {
                width += GetActualColumnWidth(i + p_range.Column, SheetArea.Cells);
            }

            // 行头列头
            for (int i = 0; i < ColumnHeader.RowCount; i++)
            {
                pt.Y += GetActualRowHeight(i, SheetArea.ColumnHeader);
            }
            for (int i = 0; i < RowHeader.ColumnCount; i++)
            {
                pt.X += GetActualColumnWidth(i, SheetArea.RowHeader);
            }

            rc.X = pt.X;
            rc.Y = pt.Y;
            rc.Width = width;
            rc.Height = height;
            return rc;
        }

        /// <summary>
        /// 获取指定区域的矩形（从0行0列按0,0开始算）
        /// </summary>
        /// <param name="p_range"></param>
        /// <returns></returns>
        public Rect GetRangeLocation(CellRange p_range)
        {
            if (p_range == null || !p_range.IsValidRange(this))
                return new Rect();

            double top = 0;
            double left = 0;
            double width = 0;
            double height = 0;

            for (int i = 0; i < p_range.Column; i++)
            {
                left += GetActualColumnWidth(i, SheetArea.Cells);
            }

            for (int i = 0; i < p_range.ColumnCount; i++)
            {
                width += GetActualColumnWidth(i + p_range.Column, SheetArea.Cells);
            }

            for (int i = 0; i < p_range.Row; i++)
            {
                top += GetActualRowHeight(i, SheetArea.Cells);
            }

            for (int i = 0; i < p_range.RowCount; i++)
            {
                height += GetActualRowHeight(i + p_range.Row, SheetArea.Cells);
            }
            return new Rect(left, top, width, height);
        }

        /// <summary>
        /// 获取有内容的最末行索引
        /// </summary>
        /// <returns>有内容的最末行索引</returns>
        public int GetLastDirtyRow()
        {
            int ldr = GetLastDirtyRow(StorageType.Axis | StorageType.Sparkline | StorageType.Tag | StorageType.Style | StorageType.Data);
            foreach (SpreadChart chart in Charts)
            {
                if (chart.EndRow > ldr)
                    ldr = chart.EndRow;
            }
            // 在Excel上直接插入图片,用GetLastDirtyRow(StorageType type)获得不到图片占用的最后行
            foreach (Picture pic in Pictures)
            {
                if (pic.EndRow > ldr)
                    ldr = pic.EndRow;
            }
            return ldr;
        }

        /// <summary>
        /// 获取有内容的最末列索引
        /// </summary>
        /// <returns>有内容的最末列索引</returns>
        public int GetLastDirtyColumn()
        {
            int ldc = GetLastDirtyColumn(StorageType.Axis | StorageType.Sparkline | StorageType.Tag | StorageType.Style | StorageType.Data);
            foreach (SpreadChart chart in Charts)
            {
                if (chart.EndColumn > ldc)
                    ldc = chart.EndColumn;
            }
            foreach (Picture pic in Pictures)
            {
                if (pic.EndColumn > ldc)
                    ldc = pic.EndColumn;
            }
            return ldc;
        }
    }
}

