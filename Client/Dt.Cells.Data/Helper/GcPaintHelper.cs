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
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Internal only.
    /// GcPaintHelper
    /// </summary>
    internal class GcPaintHelper
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

        /// <summary>
        /// Paints the specified block.
        /// </summary>
        /// <param name="block">The block.</param>
        /// <param name="g">The graphics.</param>
        /// <param name="state">The state.</param>
        public void Paint(GcBlock block, Graphics g, ExporterState state)
        {
            g.SaveState();
            if (!(block.Data is GcShape))
            {
                g.Rectangle(block.X, block.Y, block.Width, block.Height);
                g.Clip();
            }
            if (block.Data != null)
            {
                if (block.Data is GcPrintableControl)
                {
                    GcPrintableControl data = block.Data as GcPrintableControl;
                    if ((data != null) && (data.NavigationUri != null))
                    {
                        PaintLink(data.NavigationUri, block.Rect, g, state);
                    }
                    if ((data != null) && !data.Bookmark.IsEmpty)
                    {
                        ExporterState.BookmarkState state2 = new ExporterState.BookmarkState(data.Bookmark, state.CurrentPage, g.GetPoint(block.Rect.X, block.Rect.Y));
                        state.Bookmarks.Add(state2);
                    }
                }
                if (block.Data is GcLabel)
                {
                    if (block.Data is GcPageInfo)
                    {
                        ((GcPageInfo) block.Data).CurrentPage = state.CurrentPageNumber;
                        ((GcPageInfo) block.Data).CurrentHPage = state.CurrentHPageNumber;
                        ((GcPageInfo) block.Data).CurrentVPage = state.CurrentVPageNumber;
                        ((GcPageInfo) block.Data).PageCount = state.PageCount;
                        ((GcPageInfo) block.Data).DateTime = state.DateTime;
                    }
                    PaintLabel(block, g, state);
                }
                else if (block.Data is GcImage)
                {
                    PaintImage(block, g, state);
                }
                else if (block.Data is GcRichLabel)
                {
                    this.PaintRichLabel(block, g, state);
                }
            }
            else if (block is GcSheetBlock)
            {
                GcSheetBlock block2 = block as GcSheetBlock;
                if (!string.IsNullOrEmpty(block2.Bookmark))
                {
                    Bookmark bookmark = new Bookmark {
                        Text = block2.Bookmark
                    };
                    ExporterState.BookmarkState state3 = new ExporterState.BookmarkState(bookmark, state.CurrentPage, g.GetPoint(block.Rect.X, block.Rect.Y));
                    state.Bookmarks.Add(state3);
                }
                this.PaintSheetBlock(block2, g, state);
            }
            g.RestoreState();
        }

        /// <summary>
        /// Paints the specified page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="g">The graphics.</param>
        /// <param name="state">The state.</param>
        public void Paint(GcPageBlock page, Graphics g, ExporterState state)
        {
            g.SaveState();
            if (page.TopMargin != null)
            {
                this.Paint(page.TopMargin, g, state);
            }
            if (page.BottomMargin != null)
            {
                this.Paint(page.BottomMargin, g, state);
            }
            double realZoom = state.Context.Report.RealZoom;
            g.Rectangle(page.Rectangles.CropRectangle.X, page.Rectangles.CropRectangle.Y, page.Rectangles.CropRectangle.Width * realZoom, page.Rectangles.CropRectangle.Height * realZoom);
            g.Clip();
            g.Translate(page.Rectangles.CropRectangle.X, page.Rectangles.CropRectangle.Y);
            g.Scale(realZoom, realZoom);
            Windows.Foundation.Rect empty = Windows.Foundation.Rect.Empty;
            Centering centering = state.Context.Report.Centering;
            if (centering != Centering.None)
            {
                foreach (GcBlock block in page.Blocks)
                {
                    if (empty.IsEmpty)
                    {
                        empty = new Windows.Foundation.Rect(block.Rect.Left, block.Rect.Top, block.Rect.Width, block.Rect.Height);
                    }
                    else
                    {
                        empty.Union(block.Rect);
                    }
                }
            }
            double offsetX = 0.0;
            double offsetY = 0.0;
            double height = page.Rectangles.CropRectangle.Height;
            if (page.PageHeader != null)
            {
                height -= page.PageHeader.Height;
            }
            if (page.PageFooter != null)
            {
                height -= page.PageFooter.Height;
            }
            switch (centering)
            {
                case Centering.None:
                    break;

                case Centering.Horizontal:
                    offsetX = Math.Max((double) 0.0, (double) ((page.Rectangles.CropRectangle.Width - empty.Width) - empty.X)) / 2.0;
                    break;

                case Centering.Vertical:
                    offsetY = Math.Max((double) 0.0, (double) ((height - empty.Height) - empty.Y)) / 2.0;
                    break;

                case Centering.Both:
                    offsetX = Math.Max((double) 0.0, (double) ((page.Rectangles.CropRectangle.Width - empty.Width) - empty.X)) / 2.0;
                    offsetY = Math.Max((double) 0.0, (double) ((height - empty.Height) - empty.Y)) / 2.0;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (page.PageHeader != null)
            {
                if ((offsetY != 0.0) && (page.PageHeader.Y != 0.0))
                {
                    GcRangeBlock pageHeader = page.PageHeader;
                    pageHeader.Y += offsetY;
                }
                this.Paint(page.PageHeader, g, state);
            }
            if (page.PageFooter != null)
            {
                if ((offsetY != 0.0) && (page.PageFooter.Y != (page.Rectangles.CropRectangle.Height - page.PageFooter.Height)))
                {
                    GcRangeBlock pageFooter = page.PageFooter;
                    pageFooter.Y += offsetY;
                }
                this.Paint(page.PageFooter, g, state);
            }
            if ((offsetX != 0.0) || (offsetY != 0.0))
            {
                g.Translate(offsetX, offsetY);
            }
            foreach (GcBlock block2 in page.Blocks)
            {
                if (block2 is GcRangeBlock)
                {
                    this.Paint((GcRangeBlock) block2, g, state);
                }
                else
                {
                    this.Paint(block2, g, state);
                }
            }
            g.RestoreState();
            g.SaveState();
            if (!state.Context.Report.Watermark.IsEmpty)
            {
                GcBlock block3 = state.Context.Report.Watermark.GetBlock(state.Context);
                block3.Cache = state.Context.Report.Watermark;
                this.Paint(block3, g, state);
            }
            g.RestoreState();
        }

        /// <summary>
        /// Paints the specified range block.
        /// </summary>
        /// <param name="rangeBlock">The range block.</param>
        /// <param name="g">The graphics.</param>
        /// <param name="state">The state.</param>
        public void Paint(GcRangeBlock rangeBlock, Graphics g, ExporterState state)
        {
            bool flag = false;
            bool showBorder = false;
            Windows.Foundation.Rect empty = Windows.Foundation.Rect.Empty;
            foreach (GcBlock block in rangeBlock.Blocks)
            {
                if (block is GcSheetBlock)
                {
                    GcSheetBlock block2 = block as GcSheetBlock;
                    flag = true;
                    showBorder = block2.State.ShowBorder;
                    Windows.Foundation.Rect rect = block2.GetRect();
                    empty.Union(new Windows.Foundation.Rect(block2.X, block2.Y, rect.Width, rect.Height));
                }
            }
            g.SaveState();
            g.Rectangle(rangeBlock.X, rangeBlock.Y, rangeBlock.Width, rangeBlock.Height);
            g.Clip();
            g.Translate(rangeBlock.X - rangeBlock.OffsetX, rangeBlock.Y - rangeBlock.OffsetY);
            foreach (GcBlock block3 in rangeBlock.Blocks)
            {
                if (block3 is GcRangeBlock)
                {
                    this.Paint((GcRangeBlock) block3, g, state);
                }
                else
                {
                    this.Paint(block3, g, state);
                }
            }
            if ((flag && showBorder) && !empty.IsEmpty)
            {
                g.SaveState();
                g.SetLineWidth(1.0);
                g.DrawRectangle(empty, FillEffects.Black);
                g.RestoreState();
            }
            g.RestoreState();
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

        /// <summary>
        /// Paints the sheet block.
        /// </summary>
        /// <param name="block">The block.</param>
        /// <param name="g">The graphics.</param>
        /// <param name="state">The state.</param>
        public void PaintSheetBlock(GcSheetBlock block, Graphics g, ExporterState state)
        {
            g.SaveState();
            g.Rectangle(block.Rect);
            g.Clip();
            g.Translate(block.X, block.Y);
            if (block.Header)
            {
                this.PaintSheetPart(GcSheetBlock.PartType.Header, block, g, state);
            }
            if (block.Frozen)
            {
                this.PaintSheetPart(GcSheetBlock.PartType.Frozen, block, g, state);
            }
            if (block.Repeat)
            {
                this.PaintSheetPart(GcSheetBlock.PartType.Repeat, block, g, state);
            }
            this.PaintSheetPart(GcSheetBlock.PartType.Cell, block, g, state);
            if (block.FrozenTrailing)
            {
                this.PaintSheetPart(GcSheetBlock.PartType.FrozenTrailing, block, g, state);
            }
            if (block.Footer)
            {
                this.PaintSheetPart(GcSheetBlock.PartType.Footer, block, g, state);
            }
            g.RestoreState();
        }

        /// <summary>
        /// Paints the sheet cell.
        /// </summary>
        /// <param name="r">The r</param>
        /// <param name="c">The c</param>
        /// <param name="rect">The rectangle</param>
        /// <param name="styleRect">The style rect.</param>
        /// <param name="overFlowText">The over flow text.</param>
        /// <param name="overFlowtextStyleInfo">The text style info.</param>
        /// <param name="part">The part</param>
        /// <param name="block">The block</param>
        /// <param name="cellState">State of the cell</param>
        /// <param name="g">The graphics</param>
        /// <param name="state">The state</param>
        /// <param name="spans">The spans</param>
        void PaintSheetCell(int r, int c, Windows.Foundation.Rect rect, Windows.Foundation.Rect styleRect, string overFlowText, StyleInfo overFlowtextStyleInfo, GcSheetBlock.PartType part, GcSheetBlock block, CellState cellState, Graphics g, ExporterState state, SpanLayoutData spans)
        {
            Worksheet worksheet;
            SheetArea area;
            StyleInfo style;
            StyleInfo tmStyle;
            if ((rect.Width > 0.0) && (rect.Height > 0.0))
            {
                worksheet = block.Sheet;
                area = block.GetAreaType(part);
                if (area == SheetArea.CornerHeader)
                {
                    SolidColorBrush cornerBrush = null;
                    if (Application.Current.RequestedTheme == ApplicationTheme.Dark)
                    {
                        cornerBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(0xff, 0x3a, 0x3a, 0x3a));
                    }
                    else
                    {
                        cornerBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(0xff, 0xa2, 0x9f, 0x9f));
                    }
                    if (state.BlackAndWhite)
                    {
                        Windows.UI.Color grayColor = Utilities.GetGrayColor(cornerBrush.Color);
                        cornerBrush = new SolidColorBrush(grayColor);
                    }
                    g.FillRectangle(new Windows.Foundation.Rect(rect.Left, rect.Top, rect.Width, rect.Height), cornerBrush);
                }
                else
                {
                    style = worksheet.GetActualStyleInfo(r, c, area);
                    tmStyle = (overFlowtextStyleInfo != null) ? overFlowtextStyleInfo : style;
                    string str = worksheet.GetText(r, c, area);
                    string str2 = string.IsNullOrEmpty(overFlowText) ? str : overFlowText;
                    g.SaveState();
                    g.Translate(rect.X, rect.Y);
                    ContentAlignment alignment = ContentAlignment.Create(tmStyle);
                    if (alignment == null)
                    {
                        alignment = new ContentAlignment();
                    }
                    if (((part == GcSheetBlock.PartType.Header) || (part == GcSheetBlock.PartType.Footer)) || ((block.Type == GcSheetBlock.PartType.Header) || (block.Type == GcSheetBlock.PartType.Footer)))
                    {
                        alignment.HorizontalAlignment = TextHorizontalAlignment.Center;
                        alignment.VerticalAlignment = TextVerticalAlignment.Center;
                    }
                    Windows.Foundation.Rect rect2 = new Windows.Foundation.Rect(rect.X - cellState.BorderOverlapLeft, rect.Y - cellState.BorderOverlapTop, (rect.Width + cellState.BorderOverlapLeft) + cellState.BorderOverlapRight, rect.Height + cellState.BorderOverlapTop);
                    BorderLine gridLine = worksheet.GetGridLine(area);
                    bool flag = (r == 0) && (area == SheetArea.Cells);
                    if ((r > 0) && (spans.Find(GetPreviousVisableRow(r, worksheet, area), c) != null))
                    {
                        flag = true;
                    }
                    if (flag)
                    {
                        LineItem item = new LineItem();
                        Windows.Foundation.Rect rect3 = new Windows.Foundation.Rect(rect2.X, rect2.Top - 2.0, rect2.Right - rect2.Left, 2.0);
                        item.Bounds.Add(rect3);
                        item.Direction = 0;
                        item.Line = MaxBorderLine(this.GetBorderLine(worksheet, r, c, area, 1), this.GetBorderLine(worksheet, r - 1, c, area, 3), gridLine);
                        if (item.Line != null)
                        {
                            item.PreviousLine = MaxBorderLine(this.GetBorderLine(worksheet, r, c - 1, area, 3), this.GetBorderLine(worksheet, r + 1, c - 1, area, 1), gridLine);
                            item.PreviousBreaker2 = MaxBorderLine(this.GetBorderLine(worksheet, r + 1, c, area, 0), this.GetBorderLine(worksheet, r + 1, c - 1, area, 2), gridLine);
                            item.NextLine = MaxBorderLine(this.GetBorderLine(worksheet, r, c + 1, area, 3), this.GetBorderLine(worksheet, r + 1, c + 1, area, 1), gridLine);
                            item.NextBreaker2 = MaxBorderLine(this.GetBorderLine(worksheet, r + 1, c, area, 2), this.GetBorderLine(worksheet, r + 1, c + 1, area, 0), gridLine);
                            cellState.LineItems.Add(item);
                        }
                    }
                    bool flag2 = (c == 0) && (area == SheetArea.Cells);
                    if ((c > 0) && (spans.Find(r, GetPreviousVisableColumn(c, worksheet, area)) != null))
                    {
                        flag2 = true;
                    }
                    if (flag2)
                    {
                        LineItem item2 = new LineItem();
                        Windows.Foundation.Rect rect4 = new Windows.Foundation.Rect(rect2.X - 2.0, rect2.Top, 2.0, rect2.Bottom - rect2.Top);
                        item2.Bounds.Add(rect4);
                        item2.Direction = 1;
                        item2.Line = MaxBorderLine(this.GetBorderLine(worksheet, r, c, area, 0), this.GetBorderLine(worksheet, r, c - 1, area, 2), gridLine);
                        if (item2.Line != null)
                        {
                            item2.PreviousLine = MaxBorderLine(this.GetBorderLine(worksheet, r - 1, c, area, 2), this.GetBorderLine(worksheet, r - 1, c + 1, area, 0), gridLine);
                            item2.PreviousBreaker2 = MaxBorderLine(this.GetBorderLine(worksheet, r, c + 1, area, 1), this.GetBorderLine(worksheet, r - 1, c + 1, area, 3), gridLine);
                            item2.NextLine = MaxBorderLine(this.GetBorderLine(worksheet, r + 1, c, area, 2), this.GetBorderLine(worksheet, r + 1, c + 1, area, 0), gridLine);
                            item2.NextBreaker2 = MaxBorderLine(this.GetBorderLine(worksheet, r, c + 1, area, 3), this.GetBorderLine(worksheet, r + 1, c + 1, area, 1), gridLine);
                            cellState.LineItems.Add(item2);
                        }
                    }
                    bool flag3 = false;
                    CellRange range3 = spans.Find(r, c);
                    if (range3 != null)
                    {
                        flag3 = true;
                    }
                    if (!flag3)
                    {
                        LineItem item3 = new LineItem {
                            Bounds = { rect2 },
                            Direction = 0,
                            Line = MaxBorderLine(this.GetBorderLine(worksheet, r, c, area, 3), this.GetBorderLine(worksheet, r + 1, c, area, 1), gridLine)
                        };
                        if (item3.Line != null)
                        {
                            item3.NextBreaker1 = MaxBorderLine(this.GetBorderLine(worksheet, r, c, area, 2), this.GetBorderLine(worksheet, r, c + 1, area, 0), gridLine);
                            item3.NextBreaker2 = MaxBorderLine(this.GetBorderLine(worksheet, r + 1, c, area, 2), this.GetBorderLine(worksheet, r + 1, c + 1, area, 0), gridLine);
                            item3.NextLine = MaxBorderLine(this.GetBorderLine(worksheet, r, c + 1, area, 3), this.GetBorderLine(worksheet, r + 1, c + 1, area, 1), gridLine);
                            item3.PreviousBreaker1 = MaxBorderLine(this.GetBorderLine(worksheet, r, c, area, 0), this.GetBorderLine(worksheet, r, c - 1, area, 2), gridLine);
                            item3.PreviousBreaker2 = MaxBorderLine(this.GetBorderLine(worksheet, r + 1, c, area, 0), this.GetBorderLine(worksheet, r + 1, c - 1, area, 2), gridLine);
                            item3.PreviousLine = MaxBorderLine(this.GetBorderLine(worksheet, r, c - 1, area, 3), this.GetBorderLine(worksheet, r + 1, c - 1, area, 1), gridLine);
                            cellState.LineItems.Add(item3);
                        }
                        LineItem item4 = new LineItem();
                        item4.Bounds.Add(rect2);
                        item4.Direction = 1;
                        item4.Line = MaxBorderLine(this.GetBorderLine(worksheet, r, c, area, 2), this.GetBorderLine(worksheet, r, c + 1, area, 0), gridLine);
                        if (item4.Line != null)
                        {
                            item4.PreviousBreaker1 = MaxBorderLine(this.GetBorderLine(worksheet, r - 1, c, area, 3), this.GetBorderLine(worksheet, r, c, area, 1), gridLine);
                            item4.PreviousLine = MaxBorderLine(this.GetBorderLine(worksheet, r - 1, c, area, 2), this.GetBorderLine(worksheet, r - 1, c + 1, area, 0), gridLine);
                            item4.PreviousBreaker2 = MaxBorderLine(this.GetBorderLine(worksheet, r, c + 1, area, 1), this.GetBorderLine(worksheet, r - 1, c + 1, area, 3), gridLine);
                            item4.NextBreaker1 = MaxBorderLine(this.GetBorderLine(worksheet, r, c, area, 3), this.GetBorderLine(worksheet, r + 1, c, area, 1), gridLine);
                            item4.NextLine = MaxBorderLine(this.GetBorderLine(worksheet, r + 1, c, area, 2), this.GetBorderLine(worksheet, r + 1, c + 1, area, 0), gridLine);
                            item4.NextBreaker2 = MaxBorderLine(this.GetBorderLine(worksheet, r, c + 1, area, 3), this.GetBorderLine(worksheet, r + 1, c + 1, area, 1), gridLine);
                            cellState.LineItems.Add(item4);
                        }
                    }
                    else
                    {
                        bool flag4 = false;
                        bool flag5 = false;
                        int num = -1;
                        int num2 = -1;
                        if (range3 != null)
                        {
                            num = (range3.Row + range3.RowCount) - 1;
                            num2 = (range3.Column + range3.ColumnCount) - 1;
                        }
                        if (spans.Find(GetNextVisableRow(r, worksheet, area), c) != null)
                        {
                            flag4 = true;
                        }
                        if (spans.Find(r, GetNextVisableColumn(c, worksheet, area)) != null)
                        {
                            flag5 = true;
                        }
                        if (((num == (worksheet.FrozenRowCount - 1)) || (num == ((worksheet.GetRowCount(area) - 1) - worksheet.FrozenTrailingRowCount))) || (num == block.RowEndIndex))
                        {
                            flag4 = true;
                        }
                        if (((num2 == (worksheet.FrozenColumnCount - 1)) || (num2 == ((worksheet.GetColumnCount(area) - 1) - worksheet.FrozenTrailingColumnCount))) || (num2 == block.ColumnEndIndex))
                        {
                            flag5 = true;
                        }
                        if (flag4)
                        {
                            LineItem item5 = new LineItem {
                                Bounds = { rect2 },
                                Direction = 0,
                                Line = MaxBorderLine(this.GetBorderLine(worksheet, num, c, area, 3), this.GetBorderLine(worksheet, num + 1, c, area, 1), gridLine)
                            };
                            if (item5.Line != null)
                            {
                                item5.NextBreaker1 = MaxBorderLine(this.GetBorderLine(worksheet, num, c, area, 2), this.GetBorderLine(worksheet, num, c + 1, area, 0), gridLine);
                                item5.NextBreaker2 = MaxBorderLine(this.GetBorderLine(worksheet, num + 1, c, area, 2), this.GetBorderLine(worksheet, num + 1, c + 1, area, 0), gridLine);
                                item5.NextLine = MaxBorderLine(this.GetBorderLine(worksheet, num, c + 1, area, 3), this.GetBorderLine(worksheet, num + 1, c + 1, area, 1), gridLine);
                                item5.PreviousBreaker1 = MaxBorderLine(this.GetBorderLine(worksheet, num, c, area, 0), this.GetBorderLine(worksheet, num, c - 1, area, 2), gridLine);
                                item5.PreviousBreaker2 = MaxBorderLine(this.GetBorderLine(worksheet, num + 1, c, area, 0), this.GetBorderLine(worksheet, num + 1, c - 1, area, 2), gridLine);
                                item5.PreviousLine = MaxBorderLine(this.GetBorderLine(worksheet, num, c - 1, area, 3), this.GetBorderLine(worksheet, num + 1, c - 1, area, 1), gridLine);
                                cellState.LineItems.Add(item5);
                            }
                        }
                        if (flag5)
                        {
                            LineItem item6 = new LineItem {
                                Bounds = { rect2 },
                                Direction = 1,
                                Line = MaxBorderLine(this.GetBorderLine(worksheet, r, num2, area, 2), this.GetBorderLine(worksheet, r, num2 + 1, area, 0), gridLine)
                            };
                            if (item6.Line != null)
                            {
                                item6.PreviousBreaker1 = MaxBorderLine(this.GetBorderLine(worksheet, r - 1, num2, area, 3), this.GetBorderLine(worksheet, r, num2, area, 1), gridLine);
                                item6.PreviousLine = MaxBorderLine(this.GetBorderLine(worksheet, r - 1, num2, area, 2), this.GetBorderLine(worksheet, r - 1, num2 + 1, area, 0), gridLine);
                                item6.PreviousBreaker2 = MaxBorderLine(this.GetBorderLine(worksheet, r, num2 + 1, area, 1), this.GetBorderLine(worksheet, r - 1, num2 + 1, area, 3), gridLine);
                                item6.NextBreaker1 = MaxBorderLine(this.GetBorderLine(worksheet, r, num2, area, 3), this.GetBorderLine(worksheet, r + 1, num2, area, 1), gridLine);
                                item6.NextLine = MaxBorderLine(this.GetBorderLine(worksheet, r + 1, num2, area, 2), this.GetBorderLine(worksheet, r + 1, num2 + 1, area, 0), gridLine);
                                item6.NextBreaker2 = MaxBorderLine(this.GetBorderLine(worksheet, r, num2 + 1, area, 3), this.GetBorderLine(worksheet, r + 1, num2 + 1, area, 1), gridLine);
                                cellState.LineItems.Add(item6);
                                cellState.LineItems.Add(item6);
                            }
                        }
                    }
                    styleRect.X -= rect.X;
                    styleRect.Y -= rect.Y;
                    rect.X = 0.0;
                    rect.Y = 0.0;
                    if (style.Background == null)
                    {
                        if (area == (SheetArea.CornerHeader | SheetArea.RowHeader))
                        {
                            if (Application.Current.RequestedTheme == ApplicationTheme.Dark)
                            {
                                style.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(0xff, 0x3a, 0x3a, 0x3a));
                            }
                            else
                            {
                                style.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(0xff, 0xcc, 0xcc, 0xcc));
                            }
                        }
                        else if (area == SheetArea.ColumnHeader)
                        {
                            Windows.UI.Color color = Colors.Transparent;
                            LinearGradientBrush brush = new LinearGradientBrush();
                            brush.StartPoint = new Windows.Foundation.Point(0.5, 0.0);
                            brush.EndPoint = new Windows.Foundation.Point(0.5, 1.0);
                            GradientStopCollection stops = new GradientStopCollection();
                            if (Application.Current.RequestedTheme == ApplicationTheme.Dark)
                            {
                                color = Windows.UI.Color.FromArgb(0xff, 0x3a, 0x3a, 0x3a);
                            }
                            else
                            {
                                color = Windows.UI.Color.FromArgb(0xff, 0xcc, 0xcc, 0xcc);
                            }
                            GradientStop stop = new GradientStop();
                            stop.Offset = 0.125;
                            stop.Color = color;
                            GradientStop stop2 = new GradientStop();
                            stop2.Offset = 1.0;
                            stop2.Color = color;
                            stops.Add(stop);
                            stops.Add(stop2);
                            brush.GradientStops = stops;
                            style.Background = brush;
                        }
                    }
                    if ((style.Formatter != null) && (style.Formatter is IPdfOwnerPaintSupport))
                    {
                        ((IPdfOwnerPaintSupport) style.Formatter).PaintCell(g, rect, style, worksheet.GetValue(r, c, area));
                    }
                    else
                    {
                        Brush background = style.Background;
                        if (style.IsBackgroundThemeColorSet())
                        {
                            background = new SolidColorBrush(((IThemeSupport)worksheet).GetThemeColor(style.BackgroundThemeColor));
                        }
                        if (Utilities.HasFillEffect(background) && !background.Equals(FillEffects.White))
                        {
                            bool flag6 = true;
                            if (background is SolidColorBrush)
                            {
                                Windows.UI.Color color = ((SolidColorBrush)background).Color;
                                if ((color.A == 0) || (((color.R == 0xff) && (color.G == 0xff)) && (color.B == 0xff)))
                                {
                                    flag6 = false;
                                }
                                if (state.BlackAndWhite)
                                {
                                    Windows.UI.Color bgColor = Utilities.GetGrayColor(color);
                                    background = new SolidColorBrush(bgColor);
                                }
                            }
                            if (flag6)
                            {
                                g.FillRectangle(new Windows.Foundation.Rect(styleRect.Left - 0.5, styleRect.Top - 0.5, styleRect.Width + 0.1, styleRect.Height + 0.5), background);
                            }
                        }
                        if (true)
                        {
                            bool flag8 = false;
                            object obj2 = worksheet.GetValue(r, c, area);
                            flag8 = obj2 is byte[];
                            if (flag8)
                            {
                                Image instance = null;
                                byte[] data = obj2 as byte[];
                                if (data.Length >= 4)
                                {
                                    try
                                    {
                                        instance = Image.GetInstance(data);
                                    }
                                    catch
                                    {
                                    }
                                }
                                if (instance != null)
                                {
                                    g.AddImage(instance, rect.X, rect.Y, rect.Width, rect.Height);
                                }
                                else
                                {
                                    flag8 = false;
                                }
                            }
                            if (!flag8)
                            {
                                bool flag9 = false;
                                bool flag10 = false;
                                DataBarDrawingObject dataBarObject = null;
                                IconDrawingObject iconObject = null;
                                DrawingObject[] objArray = worksheet.GetDrawingObject(r, c, 1, 1);
                                if (((objArray != null) && (objArray.Length > 0)) && (area == SheetArea.Cells))
                                {
                                    foreach (DrawingObject obj5 in objArray)
                                    {
                                        if (!flag9)
                                        {
                                            DataBarDrawingObject obj6 = obj5 as DataBarDrawingObject;
                                            if (obj6 != null)
                                            {
                                                flag9 = true;
                                                dataBarObject = obj6;
                                                continue;
                                            }
                                        }
                                        if (!flag10)
                                        {
                                            IconDrawingObject obj7 = obj5 as IconDrawingObject;
                                            if (obj7 != null)
                                            {
                                                flag10 = true;
                                                iconObject = obj7;
                                            }
                                        }
                                    }
                                }
                                bool flag11 = true;
                                if (flag9)
                                {
                                    flag11 = !dataBarObject.ShowBarOnly;
                                    this.PaintDataBar(styleRect, g, state, dataBarObject);
                                }
                                if (flag10)
                                {
                                    flag11 = !iconObject.ShowIconOnly;
                                    this.PaintIcon(styleRect, g, iconObject, alignment);
                                }
                                if (area == SheetArea.Cells)
                                {
                                    Sparkline sparkline = worksheet.GetSparkline(r, c);
                                    if (sparkline != null)
                                    {
                                        this.PaintSparkLine(styleRect, g, sparkline, worksheet);
                                    }
                                }
                                Brush foreground = tmStyle.Foreground;
                                if (tmStyle.IsForegroundThemeColorSet())
                                {
                                    if (tmStyle.ForegroundThemeColor == null)
                                    {
                                        foreground = new SolidColorBrush(Colors.Black);
                                    }
                                    else
                                    {
                                        foreground = new SolidColorBrush(((IThemeSupport)worksheet).GetThemeColor(tmStyle.ForegroundThemeColor));
                                    }
                                }

                                foreground = (foreground == null) ? FillEffects.Black : foreground;
                                if (!string.IsNullOrEmpty(str2) && Utilities.HasFillEffect(foreground))
                                {
                                    g.Rectangle(rect);
                                    g.Clip();
                                    if (((obj2 != null) && (alignment != null)) && (alignment.HorizontalAlignment == TextHorizontalAlignment.General))
                                    {
                                        if (Utilities.IsExcelRightAlignmentValue(obj2))
                                        {
                                            alignment.HorizontalAlignment = TextHorizontalAlignment.Right;
                                        }
                                        else if (Utilities.IsExcelCenterAlignmentValue(obj2))
                                        {
                                            alignment.HorizontalAlignment = TextHorizontalAlignment.Center;
                                        }
                                    }
                                    if (tmStyle.IsFontThemeSet())
                                    {
                                        tmStyle.FontFamily = ((IThemeSupport) worksheet).GetThemeFont(tmStyle.FontTheme);
                                    }
                                    if (state.BlackAndWhite && (foreground is SolidColorBrush))
                                    {
                                        Windows.UI.Color grayColor = Utilities.GetGrayColor(((SolidColorBrush)foreground).Color);
                                        foreground = new SolidColorBrush(grayColor);
                                    }
                                    bool fitRect = style.ShrinkToFit && !style.WordWrap;
                                    if (flag11)
                                    {
                                        g.DrawText(str2, Font.Create(tmStyle), alignment, foreground, rect, fitRect);
                                    }
                                }
                            }
                        }
                        if (area == SheetArea.Cells)
                        {
                            if (Utilities.IsPaddingCell(worksheet.GetTag(r, c, area)))
                            {
                                g.DrawLine(rect.X + 5.0, rect.Y, rect.X + 5.0, rect.Y + rect.Height, 1.0, FillEffects.Yellow);
                            }
                            else if (Utilities.IsPaddingFooterCell(worksheet.GetTag(r, c, area)))
                            {
                                g.SetLineJoin(PdfGraphics.LineJoinType.Round);
                                double num3 = rect.Height / 2.0;
                                double x = rect.X + 5.0;
                                g.ApplyFillEffect(FillEffects.Yellow, Windows.Foundation.Rect.Empty, true, false);
                                g.SetLineWidth(1.0);
                                g.NewPath();
                                g.MoveTo(x, rect.Y);
                                g.LineTo(x, rect.Y + num3);
                                g.LineTo(rect.X + rect.Width, rect.Y + num3);
                                g.Stroke();
                            }
                        }
                    }
                    g.RestoreState();
                }
            }
        }

        /// <summary>
        /// Paints the sheet part.
        /// </summary>
        /// <param name="part">The part</param>
        /// <param name="block">The block</param>
        /// <param name="g">The graphics</param>
        /// <param name="state">The state</param>
        void PaintSheetPart(GcSheetBlock.PartType part, GcSheetBlock block, Graphics g, ExporterState state)
        {
            int num;
            int num2;
            int num3;
            int num4;
            PartLayoutData data;
            PartLayoutData data2;
            SpanLayoutData data3;
            g.SaveState();
            Windows.Foundation.Rect rect = block.GetRect(part);
            g.Rectangle(rect);
            g.Clip();
            g.Translate(rect.X, rect.Y);
            block.GetPart(part, out num, out num2, out num3, out num4, out data, out data2, out data3);
            Worksheet sheet = block.Sheet;
            SheetArea areaType = block.GetAreaType(part);
            BorderLine gridLine = sheet.GetGridLine(areaType);
            BorderLine gridline = sheet.GetGridLine(areaType);
            double num5 = 0.0;
            double num6 = 0.0;
            double num7 = 0.0;
            double num8 = 0.0;
            if (block.State.ShowGridLine)
            {
                GetGridLineWidths(gridLine, out num7, out num8);
                GetGridLineWidths(gridline, out num5, out num6);
            }
            bool flag = false;
            bool flag2 = false;
            double borderOverlapLeft = flag2 ? num6 : 0.0;
            double borderOverlapTop = flag2 ? num8 : 0.0;
            SpanCellInfo info = null;
            CellState cellState = new CellState(num5, num6, num7, num8, borderOverlapLeft, 0.0, borderOverlapTop);
            List<CellRange> list = new List<CellRange>();
            double num11 = 0.0;
            double num12 = 0.0;
            for (int i = num; i <= num2; i++)
            {
                int acutalRowIndex = block.GetAcutalRowIndex(i);
                double num15 = Utilities.GetRowHeight(sheet, data, areaType, acutalRowIndex);
                for (int j = num3; j <= num4; j++)
                {
                    double num17 = Utilities.GetColumnWidth(sheet, data2, areaType, j);
                    int rowIndex = acutalRowIndex;
                    int columnIndex = j;
                    double x = num11;
                    double y = num12;
                    double height = num15;
                    double width = num17;
                    num11 += num17;
                    if (data3 != null)
                    {
                        CellRange spanRange = data3.Find(acutalRowIndex, j);
                        if (spanRange != null)
                        {
                            if (data3.PureSpans.Find(acutalRowIndex, j) != null)
                            {
                                if (list.Contains(spanRange))
                                {
                                    continue;
                                }
                                rowIndex = spanRange.Row;
                                columnIndex = SpanLayoutData.GetValueColumn(sheet, areaType, spanRange.Row, spanRange.Column, data3.PureSpans);
                                for (int k = spanRange.Column; k < j; k++)
                                {
                                    x -= Utilities.GetColumnWidth(sheet, data2, areaType, k);
                                }
                                for (int m = spanRange.Row; m < acutalRowIndex; m++)
                                {
                                    y -= Utilities.GetRowHeight(sheet, data, areaType, m);
                                }
                                width = 0.0;
                                height = 0.0;
                                for (int n = spanRange.Column; n < (spanRange.Column + spanRange.ColumnCount); n++)
                                {
                                    width += Utilities.GetColumnWidth(sheet, data2, areaType, n);
                                }
                                for (int num27 = spanRange.Row; num27 < (spanRange.Row + spanRange.RowCount); num27++)
                                {
                                    height += Utilities.GetRowHeight(sheet, data, areaType, num27);
                                }
                                list.Add(spanRange);
                            }
                            else
                            {
                                rowIndex = acutalRowIndex;
                                columnIndex = j;
                                int offsetCindex = SpanLayoutData.GetValueColumn(sheet, areaType, spanRange.Row, spanRange.Column, data3.PureSpans);
                                if ((columnIndex == ((spanRange.Column + spanRange.ColumnCount) - 1)) || ((columnIndex == num4) && (((spanRange.Column + spanRange.ColumnCount) - 1) > num4)))
                                {
                                    flag = true;
                                }
                                if (columnIndex == offsetCindex)
                                {
                                    info = this.GetOverFlowTextCellInfo(x, y, width, height, offsetCindex, acutalRowIndex, rowIndex, columnIndex, offsetCindex, data, data2, spanRange, areaType, sheet);
                                    continue;
                                }
                                if ((columnIndex == num3) && (spanRange.Column < num3))
                                {
                                    info = this.GetOverFlowTextCellInfo(x, y, width, height, num3, acutalRowIndex, rowIndex, columnIndex, offsetCindex, data, data2, spanRange, areaType, sheet);
                                    continue;
                                }
                                if (num4 < offsetCindex)
                                {
                                    info = this.GetOverFlowTextCellInfo(x, y, width, height, num4, acutalRowIndex, rowIndex, columnIndex, offsetCindex, data, data2, spanRange, areaType, sheet);
                                    continue;
                                }
                            }
                        }
                    }
                    cellState.BorderOverlapTop = (i == num) ? 0.0 : borderOverlapTop;
                    cellState.BorderOverlapLeft = (j == num3) ? 0.0 : borderOverlapLeft;
                    this.PaintSheetCell(rowIndex, columnIndex, new Windows.Foundation.Rect(x, y, width, height), new Windows.Foundation.Rect(x, y, width, height), string.Empty, null, part, block, cellState, g, state, data3);
                    if (flag && (info != null))
                    {
                        this.PaintSheetCell(info.RowIndex, info.ColumnIndex, info.TextRect, info.StyleRect, info.Text, info.TextStyle, part, block, cellState, g, state, data3);
                        flag = false;
                        info = null;
                    }
                }
                num11 = 0.0;
                num12 += num15;
                if (flag && (info != null))
                {
                    this.PaintSheetCell(info.RowIndex, info.ColumnIndex, info.TextRect, info.StyleRect, info.Text, info.TextStyle, part, block, cellState, g, state, data3);
                    flag = false;
                    info = null;
                }
            }
            if (block.State.ShowGridLine)
            {
                PaintGridLine(sheet, areaType, gridLine, gridline, num, num2, num3, num4, data, data2, data3, new Windows.Foundation.Rect(0.0, 0.0, rect.Width, rect.Height), cellState.LineItems, g, state);
            }
            foreach (LineItem item in cellState.LineItems)
            {
                IThemeContextSupport support = (item == null) ? null : item.Line;
                IThemeContextSupport support2 = (item.PreviousBreaker1 == null) ? null : item.PreviousBreaker1;
                IThemeContextSupport support3 = (item.PreviousBreaker2 == null) ? null : item.PreviousBreaker2;
                IThemeContextSupport support4 = (item.PreviousLine == null) ? null : item.PreviousLine;
                IThemeContextSupport support5 = (item.NextBreaker1 == null) ? null : item.NextBreaker1;
                IThemeContextSupport support6 = (item.NextBreaker2 == null) ? null : item.NextBreaker2;
                IThemeContextSupport support7 = (item.NextLine == null) ? null : item.NextLine;
                if (support4 != null)
                {
                    support4.SetContext(block.Sheet);
                }
                if (support2 != null)
                {
                    support2.SetContext(block.Sheet);
                }
                if (support3 != null)
                {
                    support3.SetContext(block.Sheet);
                }
                if (support5 != null)
                {
                    support5.SetContext(block.Sheet);
                }
                if (support6 != null)
                {
                    support6.SetContext(block.Sheet);
                }
                if (support7 != null)
                {
                    support7.SetContext(block.Sheet);
                }
                if (support != null)
                {
                    support.SetContext(block.Sheet);
                }
                try
                {
                    DrawBorder(g, state, item, item.Bounds[0]);
                }
                finally
                {
                    if (support != null)
                    {
                        support.SetContext(null);
                    }
                }
            }
            if ((areaType == SheetArea.Cells) && ((block.State.Shapes.Count > 0) || (block.State.StickyNotes.Count > 0)))
            {
                Windows.Foundation.Rect rect2 = new Windows.Foundation.Rect();
                for (int num29 = 0; num29 <= num4; num29++)
                {
                    if (num29 < num3)
                    {
                        rect2.X += Utilities.GetColumnWidth(sheet, data2, areaType, num29);
                    }
                    else
                    {
                        rect2.Width += Utilities.GetColumnWidth(sheet, data2, areaType, num29);
                    }
                }
                for (int num30 = 0; num30 <= num2; num30++)
                {
                    if (num30 < num)
                    {
                        rect2.Y += Utilities.GetRowHeight(sheet, data, areaType, num30);
                    }
                    else
                    {
                        rect2.Height += Utilities.GetRowHeight(sheet, data, areaType, num30);
                    }
                }
                g.SaveState();
                g.Translate(-rect2.X, -rect2.Y);
                g.RestoreState();
            }
            g.RestoreState();
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

