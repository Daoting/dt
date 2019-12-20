#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.Foundation;
using Windows.UI.Xaml;
#endregion

namespace Dt.Cells.UI
{
    internal class CellOverflowLayoutBuildEngine
    {
        private Dictionary<int, CellOverflowLayoutModel> _cachedCellOverflowModels = new Dictionary<int, CellOverflowLayoutModel>();
        private int _cachedLeftColumn = -1;
        private int _cachedRightColumn = -1;

        public CellOverflowLayoutBuildEngine(GcViewport viewport)
        {
            this.Viewport = viewport;
            this._cachedLeftColumn = viewport.Sheet.GetViewportLeftColumn(viewport.ColumnViewportIndex);
            this._cachedRightColumn = viewport.Sheet.GetViewportRightColumn(viewport.ColumnViewportIndex);
        }

        private CellOverflowLayoutModel BuildCellOverflowLayoutModel(int rowIndex)
        {
            if (!this.Viewport.Sheet.CanCellOverflow)
            {
                return null;
            }
            if (this.Viewport.Sheet.MaxCellOverflowDistance == 1)
            {
                return null;
            }
            ColumnLayoutModel viewportColumnLayoutModel = this.Viewport.Sheet.GetViewportColumnLayoutModel(this.Viewport.ColumnViewportIndex);
            if (viewportColumnLayoutModel == null)
            {
                return null;
            }
            object textFormattingMode = null;
            bool useLayoutRounding = this.Viewport.Sheet.UseLayoutRounding;
            SpanGraph cachedSpanGraph = this.Viewport.CachedSpanGraph;
            CellOverflowLayoutModel result = new CellOverflowLayoutModel();
            CellOverflowLayout layout = this.BuildHeadingCellOverflowLayoutModel(rowIndex, viewportColumnLayoutModel, textFormattingMode, useLayoutRounding);
            result.HeadingOverflowlayout = layout;
            for (int i = 0; i < viewportColumnLayoutModel.Count; i++)
            {
                Cell cachedCell;
                CellOverflowLayout layout3;
                CellOverflowLayout layout5;
                ColumnLayout layout2 = viewportColumnLayoutModel[i];
                if (layout2.Width > 0.0)
                {
                    cachedCell = this.Viewport.CellCache.GetCachedCell(rowIndex, layout2.Column);
                    if ((((cachedCell != null) && !string.IsNullOrEmpty(cachedCell.Text)) && (!cachedCell.ActualWordWrap && !cachedCell.ActualShrinkToFit)) && (cachedSpanGraph.GetState(rowIndex, layout2.Column) == 0))
                    {
                        switch (cachedCell.ToHorizontalAlignment())
                        {
                            case HorizontalAlignment.Left:
                            {
                                int deadColumnIndex = Enumerable.Last<ColumnLayout>((IEnumerable<ColumnLayout>) viewportColumnLayoutModel).Column + 1;
                                CellOverflowLayout item = this.BuildCellOverflowLayoutModelForLeft(cachedCell, rowIndex, false, deadColumnIndex, textFormattingMode, useLayoutRounding);
                                if (item != null)
                                {
                                    result.Add(item);
                                    int index = viewportColumnLayoutModel.IndexOf(viewportColumnLayoutModel.FindColumn(item.EndingColumn));
                                    if (index > -1)
                                    {
                                        i = index;
                                    }
                                }
                                break;
                            }
                            case HorizontalAlignment.Center:
                            {
                                layout3 = new CellOverflowLayout(layout2.Column, 0.0);
                                int num3 = Enumerable.Last<ColumnLayout>((IEnumerable<ColumnLayout>) viewportColumnLayoutModel).Column + 1;
                                CellOverflowLayout layout4 = this.BuildCellOverflowLayoutModelForLeft(cachedCell, rowIndex, true, num3, textFormattingMode, useLayoutRounding);
                                num3 = viewportColumnLayoutModel[0].Column - 1;
                                layout5 = this.BuildCellOverflowLayoutModelForRight(cachedCell, rowIndex, result, true, num3, textFormattingMode, useLayoutRounding);
                                if (layout4 == null)
                                {
                                    goto Label_01C1;
                                }
                                layout3.EndingColumn = layout4.EndingColumn;
                                layout3.BackgroundWidth += layout4.BackgroundWidth;
                                layout3.RightBackgroundWidth = layout4.RightBackgroundWidth;
                                goto Label_01E0;
                            }
                            case HorizontalAlignment.Right:
                            {
                                int num6 = viewportColumnLayoutModel[0].Column - 1;
                                CellOverflowLayout layout7 = this.BuildCellOverflowLayoutModelForRight(cachedCell, rowIndex, result, false, num6, textFormattingMode, useLayoutRounding);
                                if (layout7 != null)
                                {
                                    result.Add(layout7);
                                }
                                break;
                            }
                        }
                    }
                }
                continue;
            Label_01C1:
                layout3.BackgroundWidth += layout2.Width / 2.0;
            Label_01E0:
                if (layout5 != null)
                {
                    layout3.StartingColumn = layout5.StartingColumn;
                    layout3.BackgroundWidth += layout5.BackgroundWidth;
                    layout3.LeftBackgroundWidth = layout5.LeftBackgroundWidth;
                }
                else
                {
                    layout3.BackgroundWidth += layout2.Width / 2.0;
                }
                if (layout3.BackgroundWidth > layout2.Width)
                {
                    Windows.Foundation.Size textSize = MeasureHelper.MeasureTextInCell(cachedCell, new Windows.Foundation.Size(double.PositiveInfinity, double.PositiveInfinity), (double) this.Viewport.Sheet.ZoomFactor, this.Viewport.Sheet.InheritedControlFontFamily, textFormattingMode, useLayoutRounding);
                    layout3.ContentWidth = MeasureHelper.ConvertTextSizeToExcelCellSize(textSize, (double) this.Viewport.Sheet.ZoomFactor).Width;
                    result.Add(layout3);
                }
            }
            result.TrailingOverflowlayout = this.BuildTrailingCellOverflowLayoutModel(rowIndex, viewportColumnLayoutModel, result, textFormattingMode, useLayoutRounding);
            if (((result.Count <= 0) && (result.HeadingOverflowlayout == null)) && (result.TrailingOverflowlayout == null))
            {
                return null;
            }
            return result;
        }

        private CellOverflowLayout BuildCellOverflowLayoutModelForLeft(Cell bindingCell, int rowIndex, bool buildForCenter, int deadColumnIndex, object textFormattingMode, bool useLayoutRounding)
        {
            ICellsSupport dataContext = this.Viewport.GetDataContext();
            int index = bindingCell.Column.Index;
            if ((index < (dataContext.Columns.Count - 1)) && (index < deadColumnIndex))
            {
                Column column = dataContext.Columns[index];
                CellLayoutModel cellLayoutModel = this.Viewport.GetCellLayoutModel();
                Column column2 = dataContext.Columns[index + 1];
                int num2 = column2.Index;
                if ((cellLayoutModel != null) && (cellLayoutModel.FindCell(rowIndex, num2) != null))
                {
                    return null;
                }
                Cell cachedCell = this.Viewport.CellCache.GetCachedCell(rowIndex, num2);
                if ((cachedCell != null) && !string.IsNullOrEmpty(cachedCell.Text))
                {
                    return null;
                }
                float zoomFactor = this.Viewport.Sheet.ZoomFactor;
                Windows.Foundation.Size textSize = MeasureHelper.MeasureTextInCell(bindingCell, new Windows.Foundation.Size(double.PositiveInfinity, double.PositiveInfinity), (double) zoomFactor, this.Viewport.Sheet.InheritedControlFontFamily, textFormattingMode, useLayoutRounding);
                double width = MeasureHelper.ConvertTextSizeToExcelCellSize(textSize, (double) zoomFactor).Width;
                double num5 = column.ActualWidth * zoomFactor;
                if (buildForCenter)
                {
                    width /= 2.0;
                    num5 /= 2.0;
                }
                double num6 = num5;
                if (num6 < width)
                {
                    CellOverflowLayout layout2 = new CellOverflowLayout(column.Index, num6) {
                        EndingColumn = column.Index
                    };
                    for (int i = index + 1; (i < dataContext.Columns.Count) && (i <= deadColumnIndex); i++)
                    {
                        column2 = dataContext.Columns[i];
                        num2 = column2.Index;
                        if ((cellLayoutModel != null) && (cellLayoutModel.FindCell(rowIndex, num2) != null))
                        {
                            layout2.BackgroundWidth = num6;
                            layout2.EndingColumn = dataContext.Columns[i - 1].Index;
                            break;
                        }
                        cachedCell = this.Viewport.CellCache.GetCachedCell(rowIndex, num2);
                        if ((cachedCell != null) && !string.IsNullOrEmpty(cachedCell.Text))
                        {
                            layout2.BackgroundWidth = num6;
                            layout2.EndingColumn = dataContext.Columns[i - 1].Index;
                            break;
                        }
                        double num8 = column2.ActualWidth * zoomFactor;
                        num6 += num8;
                        if (((num6 >= width) || (i == (dataContext.Columns.Count - 1))) || (i >= deadColumnIndex))
                        {
                            layout2.BackgroundWidth = num6;
                            layout2.EndingColumn = column2.Index;
                            break;
                        }
                    }
                    if (layout2.EndingColumn != column.Index)
                    {
                        layout2.ContentWidth = MeasureHelper.ConvertTextSizeToExcelCellSize(textSize, (double) zoomFactor).Width;
                        layout2.RightBackgroundWidth = num6;
                        return layout2;
                    }
                }
            }
            return null;
        }

        private CellOverflowLayout BuildCellOverflowLayoutModelForRight(Cell bindingCell, int rowIndex, CellOverflowLayoutModel result, bool buildForCenter, int deadColumnIndex, object textFormattingMode, bool useLayoutRounding)
        {
            ICellsSupport dataContext = this.Viewport.GetDataContext();
            int index = bindingCell.Column.Index;
            if (index > deadColumnIndex)
            {
                Column column = dataContext.Columns[index];
                CellLayoutModel cellLayoutModel = this.Viewport.GetCellLayoutModel();
                Column column2 = dataContext.Columns[index - 1];
                int num2 = column2.Index;
                if ((cellLayoutModel != null) && (cellLayoutModel.FindCell(rowIndex, num2) != null))
                {
                    return null;
                }
                Cell cachedCell = this.Viewport.CellCache.GetCachedCell(rowIndex, num2);
                if ((cachedCell != null) && !string.IsNullOrEmpty(cachedCell.Text))
                {
                    return null;
                }
                float zoomFactor = this.Viewport.Sheet.ZoomFactor;
                Windows.Foundation.Size textSize = MeasureHelper.MeasureTextInCell(bindingCell, new Windows.Foundation.Size(double.PositiveInfinity, double.PositiveInfinity), (double) zoomFactor, this.Viewport.Sheet.InheritedControlFontFamily, textFormattingMode, useLayoutRounding);
                double width = MeasureHelper.ConvertTextSizeToExcelCellSize(textSize, (double) zoomFactor).Width;
                double num5 = column.ActualWidth * zoomFactor;
                if (buildForCenter)
                {
                    width /= 2.0;
                    num5 /= 2.0;
                }
                double num6 = num5;
                if (num6 < width)
                {
                    CellOverflowLayout layout2 = new CellOverflowLayout(column.Index, num6) {
                        StartingColumn = column.Index
                    };
                    for (int i = index - 1; (i >= 0) && (i >= deadColumnIndex); i--)
                    {
                        column2 = dataContext.Columns[i];
                        num2 = column2.Index;
                        if ((cellLayoutModel != null) && (cellLayoutModel.FindCell(rowIndex, num2) != null))
                        {
                            layout2.BackgroundWidth = num6;
                            layout2.StartingColumn = dataContext.Columns[i + 1].Index;
                            break;
                        }
                        if (((cachedCell != null) && (result != null)) && result.Contains(column2.Index))
                        {
                            layout2.BackgroundWidth = num6;
                            layout2.StartingColumn = dataContext.Columns[i + 1].Index;
                            break;
                        }
                        cachedCell = this.Viewport.CellCache.GetCachedCell(rowIndex, num2);
                        if ((cachedCell != null) && !string.IsNullOrEmpty(cachedCell.Text))
                        {
                            layout2.BackgroundWidth = num6;
                            layout2.StartingColumn = dataContext.Columns[i + 1].Index;
                            break;
                        }
                        num6 += column2.ActualWidth * zoomFactor;
                        if (((num6 >= width) || (i == 0)) || (i <= deadColumnIndex))
                        {
                            layout2.BackgroundWidth = num6;
                            layout2.StartingColumn = column2.Index;
                            break;
                        }
                    }
                    if (layout2.StartingColumn != column.Index)
                    {
                        layout2.ContentWidth = MeasureHelper.ConvertTextSizeToExcelCellSize(textSize, (double) zoomFactor).Width;
                        layout2.LeftBackgroundWidth = num6;
                        return layout2;
                    }
                }
            }
            return null;
        }

        private CellOverflowLayout BuildHeadingCellOverflowLayoutModel(int rowIndex, ColumnLayoutModel columnLayoutModel, object textFormattingMode, bool useLayoutRounding)
        {
            ColumnLayout layout = Enumerable.FirstOrDefault<ColumnLayout>(columnLayoutModel, delegate (ColumnLayout clm) {
                return clm.Width > 0.0;
            });
            if (layout == null)
            {
                if (columnLayoutModel.Count == 0)
                {
                    return null;
                }
                layout = columnLayoutModel[0];
            }
            CellOverflowLayout layout2 = new CellOverflowLayout(layout.Column, 0.0);
            int maxCellOverflowDistance = this.Viewport.Sheet.MaxCellOverflowDistance;
            SheetSpanModelBase spanModel = this.Viewport.GetSpanModel();
            ICellsSupport dataContext = this.Viewport.GetDataContext();
            for (int i = 1; i < maxCellOverflowDistance; i++)
            {
                int column = layout.Column - i;
                if (column < 0)
                {
                    return null;
                }
                if (dataContext.Columns[column].ActualWidth > 0.0)
                {
                    if ((spanModel != null) && (spanModel.Find(rowIndex, column) != null))
                    {
                        return null;
                    }
                    Cell cachedCell = this.Viewport.CellCache.GetCachedCell(rowIndex, column);
                    if (!string.IsNullOrEmpty(cachedCell.Text))
                    {
                        if (cachedCell.ActualWordWrap)
                        {
                            return null;
                        }
                        if (cachedCell.ActualShrinkToFit)
                        {
                            return null;
                        }
                        switch (cachedCell.ToHorizontalAlignment())
                        {
                            case HorizontalAlignment.Left:
                            case HorizontalAlignment.Stretch:
                            {
                                int deadColumnIndex = Enumerable.Last<ColumnLayout>((IEnumerable<ColumnLayout>) columnLayoutModel).Column + 1;
                                layout2 = this.BuildCellOverflowLayoutModelForLeft(cachedCell, rowIndex, false, deadColumnIndex, textFormattingMode, useLayoutRounding);
                                if ((layout2 == null) || (layout2.EndingColumn < this.ViewportLeftColumn))
                                {
                                    return null;
                                }
                                return layout2;
                            }
                            case HorizontalAlignment.Center:
                            {
                                int num4 = Enumerable.Last<ColumnLayout>((IEnumerable<ColumnLayout>) columnLayoutModel).Column + 1;
                                layout2 = this.BuildCellOverflowLayoutModelForLeft(cachedCell, rowIndex, true, num4, textFormattingMode, useLayoutRounding);
                                if ((layout2 == null) || (layout2.EndingColumn < this.ViewportLeftColumn))
                                {
                                    return null;
                                }
                                layout2.BackgroundWidth += (dataContext.Columns[column].ActualWidth * this.Viewport.Sheet.ZoomFactor) / 2.0;
                                return layout2;
                            }
                            case HorizontalAlignment.Right:
                                return null;
                        }
                        break;
                    }
                }
            }
            return null;
        }

        private CellOverflowLayout BuildTrailingCellOverflowLayoutModel(int rowIndex, ColumnLayoutModel columnLayoutModel, CellOverflowLayoutModel existed, object textFormattingMode, bool useLayoutRounding)
        {
            ColumnLayout layout = Enumerable.LastOrDefault<ColumnLayout>(columnLayoutModel, delegate (ColumnLayout clm) {
                return clm.Width > 0.0;
            });
            if (layout == null)
            {
                if (columnLayoutModel.Count == 0)
                {
                    return null;
                }
                layout = columnLayoutModel[0];
            }
            if (!existed.Contains(layout.Column))
            {
                CellOverflowLayout layout2 = new CellOverflowLayout(layout.Column, 0.0);
                int maxCellOverflowDistance = this.Viewport.Sheet.MaxCellOverflowDistance;
                ICellsSupport dataContext = this.Viewport.GetDataContext();
                SheetSpanModelBase spanModel = this.Viewport.GetSpanModel();
                for (int i = 1; i < maxCellOverflowDistance; i++)
                {
                    int column = layout.Column + i;
                    if (column >= dataContext.Columns.Count)
                    {
                        return null;
                    }
                    if (dataContext.Columns[column].ActualWidth > 0.0)
                    {
                        if ((spanModel != null) && (spanModel.Find(rowIndex, column) != null))
                        {
                            return null;
                        }
                        Cell cachedCell = this.Viewport.CellCache.GetCachedCell(rowIndex, column);
                        if (!string.IsNullOrEmpty(cachedCell.Text))
                        {
                            if (cachedCell.ActualWordWrap)
                            {
                                return null;
                            }
                            if (cachedCell.ActualShrinkToFit)
                            {
                                return null;
                            }
                            switch (cachedCell.ToHorizontalAlignment())
                            {
                                case HorizontalAlignment.Left:
                                case HorizontalAlignment.Stretch:
                                    return null;

                                case HorizontalAlignment.Center:
                                {
                                    int deadColumnIndex = columnLayoutModel[0].Column - 1;
                                    layout2 = this.BuildCellOverflowLayoutModelForRight(cachedCell, rowIndex, existed, true, deadColumnIndex, textFormattingMode, useLayoutRounding);
                                    if ((layout2 == null) || (layout2.StartingColumn > this.ViewportRightColumn))
                                    {
                                        return null;
                                    }
                                    layout2.BackgroundWidth += (dataContext.Columns[column].ActualWidth * this.Viewport.Sheet.ZoomFactor) / 2.0;
                                    return layout2;
                                }
                                case HorizontalAlignment.Right:
                                {
                                    int num5 = columnLayoutModel[0].Column - 1;
                                    layout2 = this.BuildCellOverflowLayoutModelForRight(cachedCell, rowIndex, existed, false, num5, textFormattingMode, useLayoutRounding);
                                    if ((layout2 == null) || (layout2.StartingColumn > this.ViewportRightColumn))
                                    {
                                        return null;
                                    }
                                    return layout2;
                                }
                            }
                            break;
                        }
                    }
                }
            }
            return null;
        }

        public void Clear()
        {
            this._cachedCellOverflowModels.Clear();
        }

        public CellOverflowLayoutModel GetModel(int rowIndex)
        {
            if (this.Viewport == null)
            {
                return null;
            }
            if (!this.Viewport.Sheet.CanCellOverflow)
            {
                return null;
            }
            if (!this.Viewport.SupportCellOverflow)
            {
                return null;
            }
            if (this.Viewport.Sheet.MaxCellOverflowDistance == 1)
            {
                return null;
            }
            CellOverflowLayoutModel model = null;
            if (this._cachedCellOverflowModels.TryGetValue(rowIndex, out model))
            {
                if (model == CellOverflowLayoutModel.Empty)
                {
                    return null;
                }
                return model;
            }
            model = this.BuildCellOverflowLayoutModel(rowIndex);
            if (model == null)
            {
                this._cachedCellOverflowModels[rowIndex] = CellOverflowLayoutModel.Empty;
                return model;
            }
            this._cachedCellOverflowModels[rowIndex] = model;
            return model;
        }

        public void RemoveModel(int rowIndex)
        {
            this._cachedCellOverflowModels.Remove(rowIndex);
        }

        private GcViewport Viewport { get; set; }

        public int ViewportLeftColumn
        {
            get { return  this._cachedLeftColumn; }
            set
            {
                if (this._cachedLeftColumn != value)
                {
                    this._cachedLeftColumn = value;
                    this._cachedCellOverflowModels.Clear();
                }
            }
        }

        public int ViewportRightColumn
        {
            get { return  this._cachedRightColumn; }
            set
            {
                if (this._cachedRightColumn != value)
                {
                    this._cachedRightColumn = value;
                    this._cachedCellOverflowModels.Clear();
                }
            }
        }
    }
}

