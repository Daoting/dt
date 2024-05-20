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
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Internal only.
    /// </summary>
    internal partial class GcPaintHelper
    {
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
                    offsetX = Math.Max((double)0.0, (double)((page.Rectangles.CropRectangle.Width - empty.Width) - empty.X)) / 2.0;
                    break;

                case Centering.Vertical:
                    offsetY = Math.Max((double)0.0, (double)((height - empty.Height) - empty.Y)) / 2.0;
                    break;

                case Centering.Both:
                    offsetX = Math.Max((double)0.0, (double)((page.Rectangles.CropRectangle.Width - empty.Width) - empty.X)) / 2.0;
                    offsetY = Math.Max((double)0.0, (double)((height - empty.Height) - empty.Y)) / 2.0;
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
                    this.Paint((GcRangeBlock)block2, g, state);
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
                    this.Paint((GcRangeBlock)block3, g, state);
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
                        ((GcPageInfo)block.Data).CurrentPage = state.CurrentPageNumber;
                        ((GcPageInfo)block.Data).CurrentHPage = state.CurrentHPageNumber;
                        ((GcPageInfo)block.Data).CurrentVPage = state.CurrentVPageNumber;
                        ((GcPageInfo)block.Data).PageCount = state.PageCount;
                        ((GcPageInfo)block.Data).DateTime = state.DateTime;
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
                    Bookmark bookmark = new Bookmark
                    {
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
                    g.FillRectangle(new Windows.Foundation.Rect(rect.Left, rect.Top, rect.Width, rect.Height), _cornerBrush);
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
                        LineItem item3 = new LineItem
                        {
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
                            LineItem item5 = new LineItem
                            {
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
                            LineItem item6 = new LineItem
                            {
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
                    
                    // 行头列头背景
                    if (style.Background == null
                        && (area == SheetArea.ColumnHeader || area == SheetArea.RowHeader))
                    {
                        style.Background = _headerBackgroud;
                    }
                    
                    if ((style.Formatter != null) && (style.Formatter is IPdfOwnerPaintSupport))
                    {
                        ((IPdfOwnerPaintSupport)style.Formatter).PaintCell(g, rect, style, worksheet.GetValue(r, c, area));
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
                                        tmStyle.FontFamily = ((IThemeSupport)worksheet).GetThemeFont(tmStyle.FontTheme);
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
        
        static SolidColorBrush _headerBackgroud = new SolidColorBrush(Windows.UI.Color.FromArgb(0xff, 0xf1, 0xf1, 0xf1));
        static SolidColorBrush _cornerBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(0xff, 0xe0, 0xe0, 0xe0));
    }
}

