#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Pdf.Drawing;
using Dt.Pdf.Object;
using Microsoft.UI;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Internal only.
    /// GcPaintHelper
    /// </summary>
    internal partial class GcPaintHelper
    {
        Windows.UI.Color cacheStartColor = Colors.Transparent;
        const int marginOffset = 2;
        const bool PrintRectangle = false;
        Dictionary<ulong, StyleInfo> styleCellDictionary = new Dictionary<ulong, StyleInfo>();
        Dictionary<ulong, StyleInfo> styleColumnHeaderDictionary = new Dictionary<ulong, StyleInfo>();
        Dictionary<ulong, StyleInfo> styleRowHeaderDicitonary = new Dictionary<ulong, StyleInfo>();

        static DashStyle BorderLineStyle2DashStyle(BorderLineStyle style)
        {
            switch (style)
            {
                case BorderLineStyle.None:
                    return DashStyle.Custom;

                case BorderLineStyle.Thin:
                case BorderLineStyle.Medium:
                case BorderLineStyle.Thick:
                    return DashStyle.Solid;

                case BorderLineStyle.Dashed:
                case BorderLineStyle.MediumDashed:
                    return DashStyle.Dash;

                case BorderLineStyle.Dotted:
                    return DashStyle.Dot;

                case BorderLineStyle.Double:
                case BorderLineStyle.SlantedDashDot:
                    return DashStyle.Custom;

                case BorderLineStyle.Hair:
                    return DashStyle.Dot;

                case BorderLineStyle.DashDot:
                case BorderLineStyle.MediumDashDot:
                    return DashStyle.DashDot;

                case BorderLineStyle.DashDotDot:
                case BorderLineStyle.MediumDashDotDot:
                    return DashStyle.DashDotDot;
            }
            return DashStyle.Custom;
        }

        /// <summary>
        /// Draw the border.
        /// </summary>
        /// <param name="border">The border</param>
        /// <param name="bounds">The bounds</param>
        /// <param name="g">The graphics</param>
        /// <param name="state">The state.</param>
        static void DrawBorder(Graphics g, ExporterState state, LineItem border, Windows.Foundation.Rect bounds)
        {
            double num = 0.3;
            BorderLine line = (border == null) ? null : border.Line;
            if (line != null)
            {
                DashStyle style = BorderLineStyle2DashStyle(line.Style);
                if (line.Style != BorderLineStyle.None)
                {
                    if (line.Style == BorderLineStyle.Double)
                    {
                        double num3;
                        double num4;
                        double num5;
                        double num6;
                        double num7;
                        double num8;
                        double num9;
                        double num10;
                        double num11;
                        double num12;
                        double num13;
                        double num14;
                        if (border.Direction == 0)
                        {
                            BorderLineLayoutEngine.CalcDoubleLayout(border, 0.0, 0.0, 0, out num3, out num4, out num5, out num6, out num7, out num8, out num9, out num10, out num11, out num12, out num13, out num14);
                            num7 += num;
                            num8 += num;
                            DrawLine(g, state, new Windows.Foundation.Point(num7, num5 - 1.0), new Windows.Foundation.Point(num9, num6 - 1.0), line.Color, style, 1.0);
                            DrawLine(g, state, new Windows.Foundation.Point(num8, num5 + 1.0), new Windows.Foundation.Point(num10, num6 + 1.0), line.Color, style, 1.0);
                        }
                        else
                        {
                            BorderLineLayoutEngine.CalcDoubleLayout(border, 0.0, 0.0, 1, out num3, out num4, out num5, out num6, out num7, out num8, out num9, out num10, out num11, out num12, out num13, out num14);
                            num13 -= num;
                            num14 -= num;
                            DrawLine(g, state, new Windows.Foundation.Point(num3 - 1.0, num11), new Windows.Foundation.Point(num4 - 1.0, num13), line.Color, style, 1.0);
                            DrawLine(g, state, new Windows.Foundation.Point(num3 + 1.0, num12), new Windows.Foundation.Point(num4 + 1.0, num14), line.Color, style, 1.0);
                        }
                    }
                    else if (line.Style != BorderLineStyle.SlantedDashDot)
                    {
                        double num15;
                        double num16;
                        double num17;
                        double num18;
                        BorderLineLayoutEngine.CalcNormalLayout(border, 0.0, 0.0, border.Direction, out num15, out num16, out num17, out num18);
                        int num2 = (line.StyleData == null) ? 0 : line.StyleData.Thickness;
                        if (border.Direction != 0)
                        {
                            int direction = border.Direction;
                        }
                        if (num2 > 0)
                        {
                            DrawLine(g, state, new Windows.Foundation.Point(num15, num17), new Windows.Foundation.Point(num16, num18), line.Color, style, (double) num2);
                        }
                    }
                }
            }
        }

        static void DrawLine(Graphics g, ExporterState state, Windows.Foundation.Point from, Windows.Foundation.Point to, Windows.UI.Color color, DashStyle style, double thickness)
        {
            SolidColorBrush borderBrush = new SolidColorBrush(color);
            if (thickness > 0.0)
            {
                if (state.BlackAndWhite)
                {
                    Windows.UI.Color grayColor = Utilities.GetGrayColor(color);
                    borderBrush = new SolidColorBrush(grayColor);
                }
                g.SaveState();
                g.MoveTo(from);
                g.LineTo(to);
                g.SetLineDash(style, thickness, null);
                g.ApplyFillEffect(borderBrush, new Windows.Foundation.Rect(from.X, from.Y, to.X - from.X, to.Y - from.Y), true, false);
                g.SetLineWidth(thickness);
                g.SetLineCap(PdfGraphics.LineCapType.Butt);
                g.Stroke();
                g.RestoreState();
            }
        }

        StyleInfo FindStyle(Worksheet worksheet, int r, int c, SheetArea area)
        {
            ulong num = (ulong) r;
            num = num << 0x20;
            num |= (uint)c;
            if (area == SheetArea.ColumnHeader)
            {
                if (this.styleColumnHeaderDictionary.ContainsKey(num))
                {
                    return this.styleColumnHeaderDictionary[num];
                }
                StyleInfo info = worksheet.GetActualStyleInfo(r, c, area);
                if (info != null)
                {
                    this.styleColumnHeaderDictionary.Add(num, info);
                }
                return info;
            }
            if (area == (SheetArea.CornerHeader | SheetArea.RowHeader))
            {
                if (this.styleRowHeaderDicitonary.ContainsKey(num))
                {
                    return this.styleRowHeaderDicitonary[num];
                }
                StyleInfo info2 = worksheet.GetActualStyleInfo(r, c, area);
                if (info2 != null)
                {
                    this.styleRowHeaderDicitonary.Add(num, info2);
                }
                return info2;
            }
            if (area != SheetArea.Cells)
            {
                return null;
            }
            if (this.styleCellDictionary.ContainsKey(num))
            {
                return this.styleCellDictionary[num];
            }
            StyleInfo info3 = worksheet.GetActualStyleInfo(r, c, area);
            if (info3 != null)
            {
                this.styleCellDictionary.Add(num, info3);
            }
            return info3;
        }

        BorderLine GetBorderLine(Worksheet worksheet, int r, int c, SheetArea area, int border)
        {
            StyleInfo info;
            if (((r > -1) && (r < worksheet.GetRowCount(area))) && ((c > -1) && (c < worksheet.GetColumnCount(area))))
            {
                info = this.FindStyle(worksheet, r, c, area);
            }
            else
            {
                info = null;
            }
            if (info != null)
            {
                switch (border)
                {
                    case 0:
                        return info.BorderLeft;

                    case 1:
                        return info.BorderTop;

                    case 2:
                        return info.BorderRight;

                    case 3:
                        return info.BorderBottom;
                }
            }
            return null;
        }

        static List<Windows.Foundation.Point> GetBorderPoints(Windows.Foundation.Rect rect, Inset inset, BorderPart part)
        {
            List<Windows.Foundation.Point> list = new List<Windows.Foundation.Point>();
            switch (part)
            {
                case BorderPart.Top:
                    list.Add(new Windows.Foundation.Point(rect.X, rect.Y));
                    list.Add(new Windows.Foundation.Point(rect.X + rect.Width, rect.Y));
                    list.Add(new Windows.Foundation.Point((rect.X + rect.Width) - inset.Right, rect.Y + inset.Top));
                    list.Add(new Windows.Foundation.Point(rect.X + inset.Left, rect.Y + inset.Top));
                    return list;

                case BorderPart.Left:
                    list.Add(new Windows.Foundation.Point(rect.X, rect.Y));
                    list.Add(new Windows.Foundation.Point(rect.X, rect.Y + rect.Height));
                    list.Add(new Windows.Foundation.Point(rect.X + inset.Left, (rect.Y + rect.Height) - inset.Bottom));
                    list.Add(new Windows.Foundation.Point(rect.X + inset.Left, rect.Y + inset.Top));
                    return list;

                case BorderPart.Bottom:
                    list.Add(new Windows.Foundation.Point(rect.X, rect.Y + rect.Height));
                    list.Add(new Windows.Foundation.Point(rect.X + rect.Width, rect.Y + rect.Height));
                    list.Add(new Windows.Foundation.Point((rect.X + rect.Width) - inset.Right, (rect.Y + rect.Height) - inset.Bottom));
                    list.Add(new Windows.Foundation.Point(rect.X + inset.Left, (rect.Y + rect.Height) - inset.Bottom));
                    return list;

                case BorderPart.Right:
                    list.Add(new Windows.Foundation.Point(rect.X + rect.Width, rect.Y));
                    list.Add(new Windows.Foundation.Point(rect.X + rect.Width, rect.Y + rect.Height));
                    list.Add(new Windows.Foundation.Point((rect.X + rect.Width) - inset.Right, (rect.Y + rect.Height) - inset.Bottom));
                    list.Add(new Windows.Foundation.Point((rect.X + rect.Width) - inset.Right, rect.Y + inset.Top));
                    return list;
            }
            throw new ArgumentOutOfRangeException("part");
        }

        /// <summary>
        /// Gets the grid line widths.
        /// </summary>
        /// <param name="gridline">The grid line</param>
        /// <param name="w1">The w1 width</param>
        /// <param name="w2">The w2 width</param>
        static void GetGridLineWidths(BorderLine gridline, out double w1, out double w2)
        {
            if (gridline == null)
            {
                w1 = 0.0;
                w2 = 0.0;
            }
            else
            {
                w1 = 1.0;
                w2 = 1.0;
            }
        }

        static int GetNextVisableColumn(int columnIndex, Worksheet worksheet, SheetArea area)
        {
            int num = columnIndex - 1;
            for (int i = num; i < worksheet.GetColumnCount(area); i++)
            {
                if (worksheet.GetActualColumnVisible(i, area))
                {
                    return i;
                }
            }
            return -1;
        }

        static int GetNextVisableRow(int rowIndex, Worksheet worksheet, SheetArea area)
        {
            int num = rowIndex + 1;
            for (int i = num; i < worksheet.GetRowCount(area); i++)
            {
                if (worksheet.GetActualRowVisible(i, area))
                {
                    return i;
                }
            }
            return -1;
        }

        SpanCellInfo GetOverFlowTextCellInfo(double x, double y, double width, double height, int offsetCindex, int offsetRindex, int rowIndex, int columnIndex, int overflowTextCellIndex, PartLayoutData rowHeights, PartLayoutData columnWidths, CellRange spanRange, SheetArea area, Worksheet worksheet)
        {
            SpanCellInfo info = new SpanCellInfo {
                StyleRect = new Windows.Foundation.Rect(x, y, width, height)
            };
            for (int i = spanRange.Column; i < offsetCindex; i++)
            {
                x -= Utilities.GetColumnWidth(worksheet, columnWidths, area, i);
            }
            for (int j = spanRange.Row; j < offsetRindex; j++)
            {
                y -= Utilities.GetRowHeight(worksheet, rowHeights, area, j);
            }
            width = 0.0;
            height = 0.0;
            for (int k = spanRange.Column; k < (spanRange.Column + spanRange.ColumnCount); k++)
            {
                width += Utilities.GetColumnWidth(worksheet, columnWidths, area, k);
            }
            for (int m = spanRange.Row; m < (spanRange.Row + spanRange.RowCount); m++)
            {
                height += Utilities.GetRowHeight(worksheet, rowHeights, area, m);
            }
            info.RowIndex = rowIndex;
            info.ColumnIndex = columnIndex;
            info.Text = worksheet.GetText(rowIndex, overflowTextCellIndex);
            info.TextStyle = worksheet.GetActualStyleInfo(rowIndex, overflowTextCellIndex, area);
            info.TextRect = new Windows.Foundation.Rect(x, y, width, height);
            return info;
        }

        static int GetPreviousVisableColumn(int columnIndex, Worksheet worksheet, SheetArea area)
        {
            int num = columnIndex - 1;
            for (int i = num; i >= 0; i--)
            {
                if (worksheet.GetActualColumnVisible(i, area))
                {
                    return i;
                }
            }
            return -1;
        }

        static int GetPreviousVisableRow(int rowIndex, Worksheet worksheet, SheetArea area)
        {
            int num = rowIndex - 1;
            for (int i = num; i >= 0; i--)
            {
                if (worksheet.GetActualRowVisible(i, area))
                {
                    return i;
                }
            }
            return -1;
        }

        public static double GetWidth(int borderwidth, UnitType unitType, int dpi)
        {
            return UnitManager.ConvertTo((double) borderwidth, UnitType.Pixel, unitType, (float) dpi);
        }

        /// <summary>
        /// Determines whether the specified rect1 is intersect.
        /// </summary>
        /// <param name="rect1">The rect1.</param>
        /// <param name="rect2">The rect2.</param>
        /// <returns>
        /// <c>true</c> if the specified rect1 is intersect; otherwise, <c>false</c>.
        /// </returns>
        static bool IsIntersect(Windows.Foundation.Rect rect1, Windows.Foundation.Rect rect2)
        {
            return ((((rect2.X < (rect1.X + rect1.Width)) && (rect1.X < (rect2.X + rect2.Width))) && (rect2.Y < (rect1.Y + rect1.Height))) && (rect1.Y < (rect2.Y + rect2.Height)));
        }

        /// <summary>
        /// Determines whether [is need to draw sticky note] [the specified sticky note].
        /// </summary>
        /// <param name="sNote">The sticky note</param>
        /// <param name="columnStartIndex">Start index of the column</param>
        /// <param name="columnEndIndex">End index of the column</param>
        /// <param name="rowStartIndex">Start index of the row</param>
        /// <param name="rowEndIndex">End index of the row</param>
        /// <param name="cellRect">The cell rectangle</param>
        /// <param name="noteRect">The note rectangle</param>
        /// <param name="isLineOnly">The isLineOnly</param>
        /// <returns>
        /// <c>true</c> if [is need to draw sticky note] [the specified sticky note]; otherwise, <c>false</c>
        /// </returns>
        static bool IsNeedToDrawStickyNode(StickyNote sNote, int columnStartIndex, int columnEndIndex, int rowStartIndex, int rowEndIndex, Windows.Foundation.Rect cellRect, Windows.Foundation.Rect noteRect, ref bool isLineOnly)
        {
            return false;
        }

        static BorderLine MaxBorderLine(BorderLine l1, BorderLine l2, BorderLine gridLine)
        {
            if ((l1 != null) && (l1.Style == BorderLineStyle.Double))
            {
                return l1;
            }
            if ((l2 != null) && (l2.Style == BorderLineStyle.Double))
            {
                return l2;
            }
            BorderLine line = (l1 > l2) ? l1 : l2;
            if (line != null)
            {
                return line;
            }
            return gridLine;
        }

        public void PaintDataBar(Windows.Foundation.Rect rect, Graphics g, ExporterState state, DataBarDrawingObject dataBarObject)
        {
            Brush fillBrush = new SolidColorBrush(dataBarObject.Color);
            if (dataBarObject.Gradient)
            {
                float factor = 0.9f;
                Windows.UI.Color gradientColor = Windows.UI.Color.FromArgb(dataBarObject.Color.A, (byte) ((255f * factor) + (dataBarObject.Color.R * (1f - factor))), (byte) ((255f * factor) + (dataBarObject.Color.G * (1f - factor))), (byte) ((255f * factor) + (dataBarObject.Color.B * (1f - factor))));
                GradientStopCollection gradientStopCollection = new GradientStopCollection();
                GradientStop stop = new GradientStop();
                stop.Color = gradientColor;
                stop.Offset = (dataBarObject.Scale < 0.0) ? ((double)(1f - factor)) : ((double)factor);
                GradientStop stop2 = new GradientStop();
                stop2.Color = dataBarObject.Color;
                stop2.Offset = (dataBarObject.Scale < 0.0) ? ((double)1) : ((double)0);
                gradientStopCollection.Add(stop);
                gradientStopCollection.Add(stop2);
                fillBrush = new LinearGradientBrush(gradientStopCollection, 0.0);
            }
            Windows.Foundation.Rect rect2 = new Windows.Foundation.Rect(rect.X, rect.Y, rect.Width - 5.0, rect.Height - 4.0);
            Brush borderBrush = new SolidColorBrush(dataBarObject.BorderColor);
            Brush axisBrush = new SolidColorBrush(dataBarObject.AxisColor);
            double x = rect2.X;
            double y = rect2.Y;
            double width = Math.Abs((double) (rect2.Width * dataBarObject.Scale));
            double height = rect2.Height;
            g.SaveState();
            if (dataBarObject.DataBarDirection == BarDirection.RightToLeft)
            {
                g.Rotate(-180.0);
                g.Translate(-(rect.Width - 1.0), -(rect.Height - 1.0));
            }
            g.Translate(2.0, 1.5);
            if (dataBarObject.DataBarAxisPosition == 0.0)
            {
                if (dataBarObject.Scale <= 0.0)
                {
                    width = 0.0;
                }
            }
            else if (dataBarObject.DataBarAxisPosition == 1.0)
            {
                if (dataBarObject.Scale < 0.0)
                {
                    x = rect2.Width - width;
                }
                else
                {
                    width = 0.0;
                }
            }
            else
            {
                double num5 = (rect2.Width * dataBarObject.DataBarAxisPosition) + x;
                double num6 = rect2.Y;
                double thickness = 1.0;
                double num8 = rect.Height - 1.0;
                DrawLine(g, state, new Windows.Foundation.Point(num5, num6), new Windows.Foundation.Point(num5, num6 + num8), dataBarObject.AxisColor, DashStyle.Dot, thickness);
                if (dataBarObject.Scale > 0.0)
                {
                    x = num5 + thickness;
                }
                else if (dataBarObject.Scale < 0.0)
                {
                    x = (num5 - thickness) - width;
                }
                else
                {
                    width = 0.0;
                }
            }
            if ((width >= 0.0) && (height >= 0.0))
            {
                g.FillRectangle(new Windows.Foundation.Rect(x, y, width, height), fillBrush);
                if (dataBarObject.ShowBorder)
                {
                    g.DrawRectangle(new Windows.Foundation.Rect(x, y, width, height), borderBrush);
                }
            }
            g.RestoreState();
        }

        static void PaintDoubleLineBorderPart(Graphics g, Windows.Foundation.Rect rect, Inset inset, BorderPart part)
        {
            List<Windows.Foundation.Point> list = GetBorderPoints(rect, inset, part);
            g.MoveTo(list[0]);
            g.LineTo(list[1]);
            g.MoveTo(list[2]);
            g.LineTo(list[3]);
        }

        /// <summary>
        /// Paints the geometry.
        /// </summary>
        /// <param name="geo">The geometry</param>
        /// <param name="g">The graphics</param>
        /// <param name="shape">The shape.</param>
        /// <param name="shapeRect">The shape rect.</param>
        /// <param name="isShadow">if set to <c>true</c> [is shadow].</param>
        /// <param name="state">The state.</param>
        static void PaintGeometry(ShapeGeometry geo, Graphics g, ShapeBase shape, Windows.Foundation.Rect shapeRect, bool isShadow, ExporterState state)
        {
        }

        /// <summary>
        /// Paints the grid line.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="area">The area.</param>
        /// <param name="hGridLine">The horizontal grid line</param>
        /// <param name="vGridLine">The vertical grid line</param>
        /// <param name="rowStartIndex">Start index of the row</param>
        /// <param name="rowEndIndex">End index of the row</param>
        /// <param name="columnStartIndex">Start index of the column</param>
        /// <param name="columnEndIndex">End index of the column</param>
        /// <param name="heights">The heights</param>
        /// <param name="widths">The widths</param>
        /// <param name="spans">The spans</param>
        /// <param name="rect">The rectangle</param>
        /// <param name="borders">The borders</param>
        /// <param name="g">The graphics</param>
        /// <param name="state">The state</param>
        static void PaintGridLine(Worksheet worksheet, SheetArea area, BorderLine hGridLine, BorderLine vGridLine, int rowStartIndex, int rowEndIndex, int columnStartIndex, int columnEndIndex, PartLayoutData heights, PartLayoutData widths, SpanLayoutData spans, Windows.Foundation.Rect rect, List<LineItem> borders, Graphics g, ExporterState state)
        {
            GridLinePainter painter = new GridLinePainter();
            double y = 0.0;
            for (int i = rowStartIndex; i <= rowEndIndex; i++)
            {
                double height = Utilities.GetRowHeight(worksheet, heights, area, i);
                Windows.Foundation.Point sp = new Windows.Foundation.Point(rect.X, y);
                Windows.Foundation.Point ep = new Windows.Foundation.Point(rect.X + rect.Width, y);
                painter.AddTopGridLine(sp, ep, rect.Width, height);
                sp = new Windows.Foundation.Point(rect.X, y + height);
                ep = new Windows.Foundation.Point(rect.X + rect.Width, y + height);
                painter.AddBottomGridLine(sp, ep, rect.Width, height);
                y += height;
            }
            if (area == SheetArea.CornerHeader)
            {
                painter.BottomGridLines = new List<GridLinePainter.Line> { painter.BottomGridLines[painter.BottomGridLines.Count - 1] };
            }
            double x = 0.0;
            for (int j = columnStartIndex; j <= columnEndIndex; j++)
            {
                double width = Utilities.GetColumnWidth(worksheet, widths, area, j);
                Windows.Foundation.Point point3 = new Windows.Foundation.Point(x, rect.Y);
                Windows.Foundation.Point point4 = new Windows.Foundation.Point(x, rect.Y + rect.Height);
                painter.AddLeftGridLine(point3, point4, width, rect.Height);
                point3 = new Windows.Foundation.Point(x + width, rect.Y);
                point4 = new Windows.Foundation.Point(x + width, rect.Y + rect.Height);
                painter.AddRightGridLine(point3, point4, width, rect.Height);
                x += width;
            }
            if (area == SheetArea.CornerHeader)
            {
                painter.RightGridLines = new List<GridLinePainter.Line> { painter.RightGridLines[painter.RightGridLines.Count - 1] };
            }
            x = 0.0;
            y = 0.0;
            if (spans != null)
            {
                IEnumerator enumerator = spans.GetEnumerator(rowStartIndex, columnStartIndex, (rowEndIndex - rowStartIndex) + 1, (columnEndIndex - columnStartIndex) + 1);
                while (enumerator.MoveNext())
                {
                    CellRange current = (CellRange) enumerator.Current;
                    if (current.Row < rowStartIndex)
                    {
                        y = -Utilities.GetRowHeight(worksheet, heights, area, current.Row, rowStartIndex - current.Row);
                    }
                    else
                    {
                        y = Utilities.GetRowHeight(worksheet, heights, area, rowStartIndex, current.Row - rowStartIndex);
                    }
                    if (current.Column < columnStartIndex)
                    {
                        x = -Utilities.GetColumnWidth(worksheet, widths, area, current.Column, columnStartIndex - current.Column);
                    }
                    else
                    {
                        x = Utilities.GetColumnWidth(worksheet, widths, area, columnStartIndex, current.Column - columnStartIndex);
                    }
                    double num7 = Utilities.GetRowHeight(worksheet, heights, area, current.Row, current.RowCount);
                    double num8 = Utilities.GetColumnWidth(worksheet, widths, area, current.Column, current.ColumnCount);
                    painter.SplitGridLineByRectangle(new Windows.Foundation.Rect(x, y, num8, num7));
                }
            }
            if (area == SheetArea.Cells)
            {
                for (int k = rowStartIndex; k <= rowEndIndex; k++)
                {
                    for (int m = columnStartIndex; m <= columnEndIndex; m++)
                    {
                        Brush actualBackground = worksheet.Cells[k, m].ActualBackground;
                        if (Utilities.HasFillEffect(actualBackground) && !actualBackground.Equals(FillEffects.White))
                        {
                            double num11 = Utilities.GetColumnWidth(worksheet, widths, area, columnStartIndex, m - columnStartIndex);
                            double num12 = Utilities.GetRowHeight(worksheet, heights, area, rowStartIndex, k - rowStartIndex);
                            double num13 = Utilities.GetRowHeight(worksheet, heights, area, k, 1);
                            double num14 = Utilities.GetColumnWidth(worksheet, widths, area, m, 1);
                            painter.SplitGridLineByRectangle(new Windows.Foundation.Rect(num11, num12 - 0.5, num14, 1.0002));
                            painter.SplitGridLineByRectangle(new Windows.Foundation.Rect(num11, Utilities.GetRowHeight(worksheet, heights, area, rowStartIndex, (k - rowStartIndex) + 1) - 0.5, num14, 1.0002));
                            painter.SplitGridLineByRectangle(new Windows.Foundation.Rect(Utilities.GetColumnWidth(worksheet, widths, area, columnStartIndex, (m - columnStartIndex) + 1) - 0.0002, num12, 1.0002, num13));
                        }
                    }
                }
            }
            if (borders != null)
            {
                foreach (LineItem item in borders)
                {
                    if (((item != null) && (item.Line != null)) && !item.Line.IsGridLine)
                    {
                        double num15;
                        double num16;
                        double num17;
                        double num18;
                        BorderLineLayoutEngine.CalcNormalLayout(item, 0.0, 0.0, item.Direction, out num15, out num16, out num17, out num18);
                        if (item.Line.StyleData.Thickness == 1)
                        {
                            if (item.Direction == 1)
                            {
                                painter.SplitGridLineByRectangle(new Windows.Foundation.Rect(num16 - 0.5, num17, item.Line.StyleData.Thickness + 0.0002, num18 - num17));
                            }
                            if (item.Direction == 0)
                            {
                                painter.SplitGridLineByRectangle(new Windows.Foundation.Rect(num15, num18 - 0.5, num16 - num15, item.Line.StyleData.Thickness + 0.0002));
                            }
                        }
                        else if (item.Line.StyleData.Thickness == 2)
                        {
                            if (item.Direction == 1)
                            {
                                painter.SplitGridLineByRectangle(new Windows.Foundation.Rect(num16 - 1.0, num17, item.Line.StyleData.Thickness + 0.0002, num18 - num17));
                            }
                            if (item.Direction == 0)
                            {
                                painter.SplitGridLineByRectangle(new Windows.Foundation.Rect(num15, num18 - 1.0, num16 - num15, item.Line.StyleData.Thickness + 0.0002));
                            }
                        }
                        else if (item.Line.StyleData.Thickness == 3)
                        {
                            if (item.Direction == 1)
                            {
                                painter.SplitGridLineByRectangle(new Windows.Foundation.Rect(num16 - 1.5, num17, item.Line.StyleData.Thickness + 0.0002, num18 - num17));
                            }
                            if (item.Direction == 0)
                            {
                                painter.SplitGridLineByRectangle(new Windows.Foundation.Rect(num15, num18 - 1.5, num16 - num15, (double) item.Line.StyleData.Thickness));
                            }
                        }
                    }
                }
            }
            painter.Paint(g, hGridLine, vGridLine, state);
        }

        public void PaintIcon(Windows.Foundation.Rect rect, Graphics g, IconDrawingObject iconObject, ContentAlignment alignment)
        {
            double x = rect.X + 1.0;
            double y = rect.Y + 2.0;
            double width = 16.0;
            double height = 16.0;
            if (iconObject != null)
            {
                if (iconObject.ShowIconOnly)
                {
                    if (alignment.HorizontalAlignment == TextHorizontalAlignment.Center)
                    {
                        x = (rect.X + (rect.Width / 2.0)) - (width / 2.0);
                    }
                    else if (alignment.HorizontalAlignment == TextHorizontalAlignment.Right)
                    {
                        x = ((rect.X + rect.Width) - width) - 2.0;
                    }
                }
                if ((alignment.VerticalAlignment == TextVerticalAlignment.Center) || (alignment.VerticalAlignment == TextVerticalAlignment.General))
                {
                    y = (rect.Y + (rect.Height / 2.0)) - (height / 2.0);
                }
                else if (alignment.VerticalAlignment == TextVerticalAlignment.Bottom)
                {
                    y = ((rect.Y + rect.Height) - height) - 2.0;
                }
                Stream resource = FormatIconsHelper.GetResource(iconObject.IconSetType, iconObject.IndexOfIcon);
                resource.Seek(0L, (SeekOrigin) SeekOrigin.Begin);
                byte[] buffer = new byte[(int) resource.Length];
                resource.Read(buffer, 0, buffer.Length);
                Image instance = Image.GetInstance(buffer);
                g.AddImage(instance, x, y, width, height);
            }
        }

        /// <summary>
        /// Paints the image.
        /// </summary>
        /// <param name="block">The block.</param>
        /// <param name="g">The graphics object.</param>
        /// <param name="state">The state.</param>
        public static void PaintImage(GcBlock block, Graphics g, ExporterState state)
        {
            GcImage data = block.Data as GcImage;
            if ((data != null) && (data.Image != null))
            {
                Windows.Foundation.Rect rect = new Windows.Foundation.Rect(0.0, 0.0, block.Width, block.Height);
                g.SaveState();
                g.Translate(block.X, block.Y);
                Windows.Foundation.Size size = new Windows.Foundation.Size((double) data.Width, (double) data.Height);
                if (data.RotationAngle != 0.0)
                {
                    g.Translate(rect.Width / 2.0, rect.Height / 2.0);
                    g.Rotate(data.RotationAngle);
                    g.Translate(-size.Width / 2.0, -size.Height / 2.0);
                }
                Windows.Foundation.Rect rect2 = new Windows.Foundation.Rect(0.0, 0.0, size.Width, size.Height);
                IBorder border = data.Border;
                if (Utilities.HasFillEffect(data.Background))
                {
                    g.FillRectangle(rect2, data.Background);
                }
                if (!data.Padding.IsEmpty)
                {
                    rect2.X += data.Padding.Left;
                    rect2.Width -= data.Padding.Horizontal;
                    rect2.Y += data.Padding.Top;
                    rect2.Height -= data.Padding.Vertical;
                    g.Rectangle(rect2);
                    g.Clip();
                }
                g.AddImage(data.Image, rect2.X, rect2.Y, rect2.Width, rect2.Height);
                g.RestoreState();
            }
        }

        /// <summary>
        /// Paints the label.
        /// </summary>
        /// <param name="block">The block.</param>
        /// <param name="g">The graphic object.</param>
        /// <param name="state">The state.</param>
        public static void PaintLabel(GcBlock block, Graphics g, ExporterState state)
        {
            GcLabel data = block.Data as GcLabel;
            if (data != null)
            {
                Windows.Foundation.Rect rect = new Windows.Foundation.Rect(0.0, 0.0, block.Width, block.Height);
                g.SaveState();
                g.Translate(block.X, block.Y);
                Windows.Foundation.Size actualSize = data.GetActualSize(state.Context);
                if (data.RotationAngle != 0.0)
                {
                    g.Translate(rect.Width / 2.0, rect.Height / 2.0);
                    g.Rotate(data.RotationAngle);
                    g.Translate(-actualSize.Width / 2.0, -actualSize.Height / 2.0);
                }
                Windows.Foundation.Rect rect2 = new Windows.Foundation.Rect(0.0, 0.0, actualSize.Width, actualSize.Height);
                IBorder border = data.Border;
                if (Utilities.HasFillEffect(data.Background))
                {
                    g.FillRectangle(rect2, state.BlackAndWhite ? FillEffects.White : data.Background);
                }
                if (!data.Padding.IsEmpty)
                {
                    rect2.X += data.Padding.Left;
                    rect2.Width = Math.Max((double) 0.0, (double) (rect2.Width - data.Padding.Horizontal));
                    rect2.Y += data.Padding.Top;
                    rect2.Height = Math.Max((double) 0.0, (double) (rect2.Height - data.Padding.Vertical));
                    g.Rectangle(rect2);
                    g.Clip();
                }
                Font font = data.Font;
                if ((block.Cache != null) && (block.Cache is Watermark))
                {
                    Watermark cache = block.Cache as Watermark;
                    if ((cache.ShowOutline && (cache.OutlineWidth > 0)) && Utilities.HasFillEffect(cache.OutlineFillEffect))
                    {
                        if ((font != null) && font.Bold)
                        {
                            font = (Font) font.Clone();
                            font.Bold = false;
                        }
                        g.SetLineWidth((double) cache.OutlineWidth);
                        g.ApplyFillEffect(state.BlackAndWhite ? FillEffects.Black : cache.OutlineFillEffect, rect2, true, false);
                        if (state.BlackAndWhite)
                        {
                            g.SetTextRenderingMode(PdfGraphics.TextRenderingMode.Stroke);
                        }
                        else
                        {
                            g.SetTextRenderingMode(PdfGraphics.TextRenderingMode.FillAndStroke);
                        }
                    }
                    g.DrawText(data.Text, font, data.Alignment, data.Foreground, rect2, false);
                }
                else
                {
                    g.DrawText(data.Text, font, data.Alignment, state.BlackAndWhite ? FillEffects.Black : data.Foreground, rect2, false);
                }
                g.RestoreState();
            }
        }

        /// <summary>
        /// Paints the link.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="rect">The rectangle.</param>
        /// <param name="g">The graphics object.</param>
        /// <param name="state">The state.</param>
        public static void PaintLink(Uri uri, Windows.Foundation.Rect rect, Graphics g, ExporterState state)
        {
            if ((uri != null) && !string.IsNullOrEmpty(uri.AbsoluteUri))
            {
                PdfLinkAnnotation annotation = new PdfLinkAnnotation(uri.AbsoluteUri);
                Windows.Foundation.Point point = g.GetPoint(rect.Left, rect.Top);
                Windows.Foundation.Point point2 = g.GetPoint(rect.Right, rect.Bottom);
                annotation.Rectangle.LowerLeftX = (float) point.X;
                annotation.Rectangle.LowerLeftY = (float) point.Y;
                annotation.Rectangle.UpperRightX = (float) point2.X;
                annotation.Rectangle.UpperRightY = (float) point2.Y;
                annotation.BorderSize = 0.0;
                state.CurrentPage.Annotations.Add(annotation);
            }
        }

        /// <summary>
        /// Paints the rich label.
        /// </summary>
        /// <param name="block">The block.</param>
        /// <param name="g">The graphic object.</param>
        /// <param name="state">The state.</param>
        public void PaintRichLabel(GcBlock block, Graphics g, ExporterState state)
        {
            GcRichLabel data = block.Data as GcRichLabel;
            if (data != null)
            {
                Windows.Foundation.Rect rect = new Windows.Foundation.Rect(0.0, 0.0, block.Width, block.Height);
                g.SaveState();
                g.Translate(block.X, block.Y);
                Windows.Foundation.Size actualSize = data.GetActualSize(state.Context);
                if (data.RotationAngle != 0.0)
                {
                    g.Translate(rect.Width / 2.0, rect.Height / 2.0);
                    g.Rotate(data.RotationAngle);
                    g.Translate(-actualSize.Width / 2.0, -actualSize.Height / 2.0);
                }
                Windows.Foundation.Rect rect2 = new Windows.Foundation.Rect(0.0, 0.0, actualSize.Width, actualSize.Height);
                IBorder border = data.Border;
                if (Utilities.HasFillEffect(data.Background))
                {
                    g.FillRectangle(rect2, state.BlackAndWhite ? FillEffects.White : data.Background);
                }
                if (!data.Padding.IsEmpty)
                {
                    rect2.X += data.Padding.Left;
                    rect2.Width = Math.Max((double) 0.0, (double) (rect2.Width - data.Padding.Horizontal));
                    rect2.Y += data.Padding.Top;
                    rect2.Height = Math.Max((double) 0.0, (double) (rect2.Height - data.Padding.Vertical));
                    g.Rectangle(rect2);
                    g.Clip();
                }
                if ((block.Cache != null) && (block.Cache is List<List<List<GcPrintableControl>>>))
                {
                    g.Translate(rect2.X, rect2.Y);
                    Windows.Foundation.Rect rect3 = new Windows.Foundation.Rect(0.0, 0.0, rect2.Width, rect2.Height);
                    List<List<List<GcPrintableControl>>> cache = block.Cache as List<List<List<GcPrintableControl>>>;
                    List<List<GcPrintableControl>> lists = cache[0];
                    List<List<GcPrintableControl>> list3 = cache[1];
                    List<List<GcPrintableControl>> list4 = cache[2];
                    if ((lists != null) && (lists.Count > 0))
                    {
                        SmartAdjustRichLabelInnerControls(lists, state, rect3, -1);
                        this.PaintRichLabelInnerControls(lists, g, state, rect3);
                    }
                    if ((list3 != null) && (list3.Count > 0))
                    {
                        SmartAdjustRichLabelInnerControls(list3, state, rect3, 0);
                        this.PaintRichLabelInnerControls(list3, g, state, rect3);
                    }
                    if ((list4 != null) && (list4.Count > 0))
                    {
                        SmartAdjustRichLabelInnerControls(list4, state, rect3, 1);
                        this.PaintRichLabelInnerControls(list4, g, state, rect3);
                    }
                }
                g.RestoreState();
            }
        }

        /// <summary>
        /// Paints the rich label inner controls.
        /// </summary>
        /// <param name="lists">The lists.</param>
        /// <param name="g">The g.</param>
        /// <param name="state">The state.</param>
        /// <param name="rect">The rect.</param>
        void PaintRichLabelInnerControls(List<List<GcPrintableControl>> lists, Graphics g, ExporterState state, Windows.Foundation.Rect rect)
        {
            if ((lists != null) && (lists.Count > 0))
            {
                using (List<List<GcPrintableControl>>.Enumerator enumerator = lists.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        using (List<GcPrintableControl>.Enumerator enumerator2 = enumerator.Current.GetEnumerator())
                        {
                            while (enumerator2.MoveNext())
                            {
                                GcBlock block = enumerator2.Current.GetBlock(state.Context);
                                if (block.IntersectWith(rect.X, rect.Y, rect.Width, rect.Height))
                                {
                                    this.Paint(block, g, state);
                                }
                            }
                            continue;
                        }
                    }
                }
            }
        }

        public void PaintSparkLine(Windows.Foundation.Rect rect, Graphics g, Sparkline sparkline, Worksheet worksheet)
        {
            g.SaveState();
            g.Translate(rect.X, rect.Y);
            PdfBaseSparklineView view = null;
            if (sparkline.SparklineType == SparklineType.Column)
            {
                view = new PdfColumnSparklineView(new ColumnSparklineViewInfo(sparkline));
            }
            else if (sparkline.SparklineType == SparklineType.Line)
            {
                view = new PdfLineSparklineView(new LineSparklineViewInfo(sparkline));
            }
            else
            {
                view = new PdfWinLossSparklineView(new WinLossSparklineViewInfo(sparkline));
            }
            if (view != null)
            {
                Windows.Foundation.Rect? clipBounds = view.GetClipBounds(new Windows.Foundation.Size(rect.Width, rect.Height));
                if (clipBounds.HasValue)
                {
                    g.Rectangle(clipBounds.Value);
                }
                else
                {
                    g.Rectangle(new Windows.Foundation.Rect(new Windows.Foundation.Point(), new Windows.Foundation.Size(rect.Width, rect.Height)));
                }
                g.Clip();
                ((IThemeContextSupport) view).SetContext(worksheet);
                view.Paint(g, new Windows.Foundation.Size(rect.Width, rect.Height));
            }
            g.RestoreState();
        }

        /// <summary>
        /// Smarts the adjust rich label inner controls.
        /// </summary>
        /// <param name="lists">The lists.</param>
        /// <param name="state">The state.</param>
        /// <param name="rect">The rect.</param>
        /// <param name="part">The part.</param>
        static void SmartAdjustRichLabelInnerControls(List<List<GcPrintableControl>> lists, ExporterState state, Windows.Foundation.Rect rect, int part)
        {
            if ((lists != null) && (lists.Count > 0))
            {
                foreach (List<GcPrintableControl> list in lists)
                {
                    bool flag = false;
                    foreach (GcPrintableControl control in list)
                    {
                        if (control is GcPageInfo)
                        {
                            GcPageInfo info = control as GcPageInfo;
                            info.CurrentPage = state.CurrentPageNumber;
                            info.CurrentHPage = state.CurrentHPageNumber;
                            info.CurrentVPage = state.CurrentVPageNumber;
                            info.PageCount = state.PageCount;
                            info.DateTime = state.DateTime;
                            if ((info.Type == PageInfoType.PageNumber) || (info.Type == PageInfoType.PageCount))
                            {
                                flag = true;
                            }
                        }
                    }
                    if (flag)
                    {
                        GcRichLabel.AdjustLineByPart(list, state.Context, part, false, 0, rect);
                    }
                }
            }
        }

        /// <summary>
        /// Specifies the border area.
        /// </summary>
        public enum BorderPart
        {
            Top,
            Left,
            Bottom,
            Right
        }

        public class CellState
        {
            double borderOverlapLeft;
            double borderOverlapRight;
            double borderOverlapTop;
            readonly Dictionary<Windows.Foundation.Rect, List<BorderLine>> borders = new Dictionary<Windows.Foundation.Rect, List<BorderLine>>();
            readonly double gridLineBottom;
            readonly double gridLineLeft;
            readonly double gridLineRight;
            readonly double gridLineTop;
            readonly List<LineItem> lineItems = new List<LineItem>();

            public CellState(double gridLineLeft, double gridLineRight, double gridLineTop, double gridLineBottom, double borderOverlapLeft, double borderOverlapRight, double borderOverlapTop)
            {
                this.gridLineLeft = gridLineLeft;
                this.gridLineRight = gridLineRight;
                this.gridLineTop = gridLineTop;
                this.gridLineBottom = gridLineBottom;
                this.borderOverlapLeft = borderOverlapLeft;
                this.borderOverlapRight = borderOverlapRight;
                this.borderOverlapTop = borderOverlapTop;
            }

            public double BorderOverlapLeft
            {
                get { return  this.borderOverlapLeft; }
                internal set { this.borderOverlapLeft = value; }
            }

            public double BorderOverlapRight
            {
                get { return  this.borderOverlapRight; }
                internal set { this.borderOverlapRight = value; }
            }

            public double BorderOverlapTop
            {
                get { return  this.borderOverlapTop; }
                internal set { this.borderOverlapTop = value; }
            }

            public Dictionary<Windows.Foundation.Rect, List<BorderLine>> Borders
            {
                get { return  this.borders; }
            }

            public double GridLineBottom
            {
                get { return  this.gridLineBottom; }
            }

            public double GridLineLeft
            {
                get { return  this.gridLineLeft; }
            }

            public double GridLineRight
            {
                get { return  this.gridLineRight; }
            }

            public double GridLineTop
            {
                get { return  this.gridLineTop; }
            }

            public List<LineItem> LineItems
            {
                get { return  this.lineItems; }
            }
        }

        /// <summary>
        /// The GridLinePainter Class
        /// </summary>
        internal class GridLinePainter
        {
            public List<Line> BottomGridLines = new List<Line>();
            public List<Line> LeftGridLines = new List<Line>();
            public List<Line> RightGridLines = new List<Line>();
            public List<Line> TopGridLines = new List<Line>();

            /// <summary>
            /// Adds the bottom grid line.
            /// </summary>
            /// <param name="sp">The start point.</param>
            /// <param name="ep">The end point.</param>
            /// <param name="width">The width.</param>
            /// <param name="height">The height.</param>
            public void AddBottomGridLine(Windows.Foundation.Point sp, Windows.Foundation.Point ep, double width, double height)
            {
                this.BottomGridLines.Add(new Line(sp, ep, width, height));
            }

            /// <summary>
            /// Adds the left grid line.
            /// </summary>
            /// <param name="sp">The start point.</param>
            /// <param name="ep">The end point.</param>
            /// <param name="width">The width.</param>
            /// <param name="height">The height.</param>
            public void AddLeftGridLine(Windows.Foundation.Point sp, Windows.Foundation.Point ep, double width, double height)
            {
                this.LeftGridLines.Add(new Line(sp, ep, width, height));
            }

            /// <summary>
            /// Adds the right grid line.
            /// </summary>
            /// <param name="sp">The start point.</param>
            /// <param name="ep">The end point.</param>
            /// <param name="width">The width.</param>
            /// <param name="height">The height.</param>
            public void AddRightGridLine(Windows.Foundation.Point sp, Windows.Foundation.Point ep, double width, double height)
            {
                this.RightGridLines.Add(new Line(sp, ep, width, height));
            }

            /// <summary>
            /// Adds the top grid line.
            /// </summary>
            /// <param name="sp">The start point.</param>
            /// <param name="ep">The end point.</param>
            /// <param name="width">The width.</param>
            /// <param name="height">The height.</param>
            public void AddTopGridLine(Windows.Foundation.Point sp, Windows.Foundation.Point ep, double width, double height)
            {
                this.TopGridLines.Add(new Line(sp, ep, width, height));
            }

            /// <summary>
            /// Gets the first fill effect of the grid line.
            /// </summary>
            /// <param name="gridLine">The grid line</param>
            /// <returns></returns>
            static Brush GetFirstFillEffect(BorderLine gridLine)
            {
                return new SolidColorBrush(gridLine.Color);
            }

            /// <summary>
            /// Gets the second fill effect of the grid line.
            /// </summary>
            /// <param name="gridLine">The grid line</param>
            /// <returns></returns>
            static Brush GetSecondFillEffect(BorderLine gridLine)
            {
                return null;
            }

            /// <summary>
            /// Paints the grid line.
            /// </summary>
            /// <param name="g">The graphics object.</param>
            /// <param name="hGridLine">The horizontal grid line.</param>
            /// <param name="vGridLine">The vertical grid line.</param>
            /// <param name="state">The state.</param>
            public void Paint(Graphics g, BorderLine hGridLine, BorderLine vGridLine, ExporterState state)
            {
                double num;
                double num2;
                double num3;
                double num4;
                GcPaintHelper.GetGridLineWidths(hGridLine, out num3, out num4);
                GcPaintHelper.GetGridLineWidths(vGridLine, out num, out num2);
                foreach (Line line in this.LeftGridLines)
                {
                    double width = Math.Min(line.Width, num);
                    if ((width > 0.0) && !line.StartPoint.Equals(line.EndPoint))
                    {
                        Brush secondFillEffect = GetSecondFillEffect(vGridLine);
                        if (Utilities.HasFillEffect(secondFillEffect))
                        {
                            PaintVerticalLine(g, state.BlackAndWhite ? FillEffects.Black : secondFillEffect, line.StartPoint.X, line.StartPoint.Y, width, line.EndPoint.Y - line.StartPoint.Y, width);
                        }
                    }
                }
                foreach (Line line2 in this.TopGridLines)
                {
                    double height = Math.Min(line2.Height, num3);
                    if ((height > 0.0) && !line2.StartPoint.Equals(line2.EndPoint))
                    {
                        Brush effect = GetSecondFillEffect(hGridLine);
                        if (Utilities.HasFillEffect(effect))
                        {
                            PaintHorizontalLine(g, state.BlackAndWhite ? FillEffects.Black : effect, line2.StartPoint.X, line2.StartPoint.Y, line2.EndPoint.X - line2.StartPoint.X, height, height);
                        }
                    }
                }
                foreach (Line line3 in this.RightGridLines)
                {
                    double num7 = Math.Min(line3.Width, num2);
                    if ((num7 > 0.0) && !line3.StartPoint.Equals(line3.EndPoint))
                    {
                        Brush firstFillEffect = GetFirstFillEffect(vGridLine);
                        if (Utilities.HasFillEffect(firstFillEffect))
                        {
                            PaintVerticalLine(g, state.BlackAndWhite ? FillEffects.Black : firstFillEffect, line3.StartPoint.X - num7, line3.StartPoint.Y, num7, line3.EndPoint.Y - line3.StartPoint.Y, num7);
                        }
                    }
                }
                foreach (Line line4 in this.BottomGridLines)
                {
                    double num8 = Math.Min(line4.Height, num4);
                    if ((num8 > 0.0) && !line4.StartPoint.Equals(line4.EndPoint))
                    {
                        Brush brush4 = GetFirstFillEffect(hGridLine);
                        if (Utilities.HasFillEffect(brush4))
                        {
                            PaintHorizontalLine(g, state.BlackAndWhite ? FillEffects.Black : brush4, line4.StartPoint.X, line4.StartPoint.Y - num8, line4.EndPoint.X - line4.StartPoint.X, num8, num8);
                        }
                    }
                }
            }

            /// <summary>
            /// Paints the horizontal line.
            /// </summary>
            /// <param name="g">The graphics</param>
            /// <param name="fillEffect">The fill effect</param>
            /// <param name="x">The x</param>
            /// <param name="y">The y</param>
            /// <param name="width">The width</param>
            /// <param name="height">The height</param>
            /// <param name="lineWidth">Width of the line</param>
            static void PaintHorizontalLine(Graphics g, Brush fillEffect, double x, double y, double width, double height, double lineWidth)
            {
                g.DrawLine(x, y + (height / 2.0), x + width, y + (height / 2.0), lineWidth, fillEffect);
            }

            /// <summary>
            /// Paints the vertical line.
            /// </summary>
            /// <param name="g">The graphics</param>
            /// <param name="fillEffect">The fill effect</param>
            /// <param name="x">The x</param>
            /// <param name="y">The y</param>
            /// <param name="width">The width</param>
            /// <param name="height">The height</param>
            /// <param name="lineWidth">Width of the line</param>
            static void PaintVerticalLine(Graphics g, Brush fillEffect, double x, double y, double width, double height, double lineWidth)
            {
                g.DrawLine(x + (width / 2.0), y, x + (width / 2.0), y + height, lineWidth, fillEffect);
            }

            /// <summary>
            /// Splits the grid line with a rectangle.
            /// </summary>
            /// <param name="rect">The rectangle.</param>
            public void SplitGridLineByRectangle(Windows.Foundation.Rect rect)
            {
                this.RightGridLines.AddRange((IEnumerable<Line>) SplitVerticalLines((IEnumerable<Line>) this.RightGridLines, rect));
                this.BottomGridLines.AddRange((IEnumerable<Line>) SplitHorizontalLines((IEnumerable<Line>) this.BottomGridLines, rect));
            }

            /// <summary>
            /// Splits the horizontal lines.
            /// </summary>
            /// <param name="lines">The lines</param>
            /// <param name="rect">The rectangle</param>
            /// <returns></returns>
            static List<Line> SplitHorizontalLines(IEnumerable<Line> lines, Windows.Foundation.Rect rect)
            {
                List<Line> list = new List<Line>();
                foreach (Line line in lines)
                {
                    if ((((line.StartPoint.Y > rect.Y) && (line.StartPoint.Y < (rect.Y + rect.Height))) && ((((rect.Y + rect.Height) - line.StartPoint.Y) > 0.0001) && (line.StartPoint.X < (rect.X + rect.Width)))) && (line.EndPoint.X > rect.X))
                    {
                        Windows.Foundation.Point point = new Windows.Foundation.Point(rect.X, line.StartPoint.Y);
                        Windows.Foundation.Point sp = new Windows.Foundation.Point(rect.X + rect.Width, line.StartPoint.Y);
                        Line line2 = new Line(sp, line.EndPoint, line.Width, line.Height);
                        line.EndPoint = point;
                        if (line.StartPoint.X >= line.EndPoint.X)
                        {
                            line.StartPoint = line2.StartPoint;
                            line.EndPoint = line2.EndPoint;
                        }
                        else if (line2.StartPoint.X < line2.EndPoint.X)
                        {
                            list.Add(line2);
                        }
                    }
                }
                return list;
            }

            /// <summary>
            /// Splits the vertical lines.
            /// </summary>
            /// <param name="lines">The lines</param>
            /// <param name="rect">The rectangle</param>
            /// <returns></returns>
            static List<Line> SplitVerticalLines(IEnumerable<Line> lines, Windows.Foundation.Rect rect)
            {
                List<Line> list = new List<Line>();
                foreach (Line line in lines)
                {
                    if ((((line.StartPoint.X > rect.X) && (line.StartPoint.X < (rect.X + rect.Width))) && ((((rect.X + rect.Width) - line.StartPoint.X) > 0.0001) && (line.StartPoint.Y < (rect.Y + rect.Height)))) && (line.EndPoint.Y > rect.Y))
                    {
                        Windows.Foundation.Point point = new Windows.Foundation.Point(line.StartPoint.X, rect.Y);
                        Windows.Foundation.Point sp = new Windows.Foundation.Point(line.StartPoint.X, rect.Y + rect.Height);
                        Line line2 = new Line(sp, line.EndPoint, line.Width, line.Height);
                        line.EndPoint = point;
                        if (line.StartPoint.Y >= line.EndPoint.Y)
                        {
                            line.StartPoint = line2.StartPoint;
                            line.EndPoint = line2.EndPoint;
                        }
                        else if (line2.StartPoint.Y < line2.EndPoint.Y)
                        {
                            list.Add(line2);
                        }
                    }
                }
                return list;
            }

            /// <summary>
            /// The Line Class
            /// </summary>
            internal class Line
            {
                public Windows.Foundation.Point EndPoint;
                public double Height;
                public Windows.Foundation.Point StartPoint;
                public double Width;

                /// <summary>
                /// Initializes a new instance of the Line class.
                /// </summary>
                public Line()
                {
                }

                /// <summary>
                /// Initializes a new instance of the Line class.
                /// </summary>
                /// <param name="sp">The start point.</param>
                /// <param name="ep">The end point.</param>
                /// <param name="width">The width.</param>
                /// <param name="height">The height.</param>
                public Line(Windows.Foundation.Point sp, Windows.Foundation.Point ep, double width, double height)
                {
                    this.StartPoint = sp;
                    this.EndPoint = ep;
                    this.Width = width;
                    this.Height = height;
                }
            }
        }

        internal class SpanCellInfo
        {
            public int ColumnIndex { get; set; }

            public int RowIndex { get; set; }

            public Windows.Foundation.Rect StyleRect { get; set; }

            public string Text { get; set; }

            public Windows.Foundation.Rect TextRect { get; set; }

            public StyleInfo TextStyle { get; set; }
        }
    }
}

