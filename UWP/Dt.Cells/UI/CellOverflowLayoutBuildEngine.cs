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
        const int _maxCellOverflowDistance = 100;
        Dictionary<int, CellOverflowLayoutModel> _cachedCellOverflowModels = new Dictionary<int, CellOverflowLayoutModel>();
        int _cachedLeftColumn = -1;
        int _cachedRightColumn = -1;

        public CellOverflowLayoutBuildEngine(CellsPanel viewport)
        {
            Viewport = viewport;
            _cachedLeftColumn = viewport.Excel.GetViewportLeftColumn(viewport.ColumnViewportIndex);
            _cachedRightColumn = viewport.Excel.GetViewportRightColumn(viewport.ColumnViewportIndex);
        }

        CellOverflowLayoutModel BuildCellOverflowLayoutModel(int rowIndex)
        {
            if (!Viewport.Excel.CanCellOverflow)
            {
                return null;
            }

            ColumnLayoutModel viewportColumnLayoutModel = Viewport.Excel.GetViewportColumnLayoutModel(Viewport.ColumnViewportIndex);
            if (viewportColumnLayoutModel == null)
            {
                return null;
            }
            object textFormattingMode = null;
            bool useLayoutRounding = Viewport.Excel.UseLayoutRounding;
            SpanGraph cachedSpanGraph = Viewport.CachedSpanGraph;
            CellOverflowLayoutModel result = new CellOverflowLayoutModel();
            CellOverflowLayout layout = BuildHeadingCellOverflowLayoutModel(rowIndex, viewportColumnLayoutModel, textFormattingMode, useLayoutRounding);
            result.HeadingOverflowlayout = layout;
            for (int i = 0; i < viewportColumnLayoutModel.Count; i++)
            {
                Cell cachedCell;
                CellOverflowLayout layout3;
                CellOverflowLayout layout5;
                ColumnLayout layout2 = viewportColumnLayoutModel[i];
                if (layout2.Width > 0.0)
                {
                    cachedCell = Viewport.CellCache.GetCachedCell(rowIndex, layout2.Column);
                    if ((((cachedCell != null) && !string.IsNullOrEmpty(cachedCell.Text)) && (!cachedCell.ActualWordWrap && !cachedCell.ActualShrinkToFit)) && (cachedSpanGraph.GetState(rowIndex, layout2.Column) == 0))
                    {
                        switch (cachedCell.ToHorizontalAlignment())
                        {
                            case HorizontalAlignment.Left:
                            case HorizontalAlignment.Stretch:
                                {
                                    int deadColumnIndex = Enumerable.Last<ColumnLayout>((IEnumerable<ColumnLayout>)viewportColumnLayoutModel).Column + 1;
                                    CellOverflowLayout item = BuildCellOverflowLayoutModelForLeft(cachedCell, rowIndex, false, deadColumnIndex, textFormattingMode, useLayoutRounding);
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
                                    int num3 = Enumerable.Last<ColumnLayout>((IEnumerable<ColumnLayout>)viewportColumnLayoutModel).Column + 1;
                                    CellOverflowLayout layout4 = BuildCellOverflowLayoutModelForLeft(cachedCell, rowIndex, true, num3, textFormattingMode, useLayoutRounding);
                                    num3 = viewportColumnLayoutModel[0].Column - 1;
                                    layout5 = BuildCellOverflowLayoutModelForRight(cachedCell, rowIndex, result, true, num3, textFormattingMode, useLayoutRounding);
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
                                    CellOverflowLayout layout7 = BuildCellOverflowLayoutModelForRight(cachedCell, rowIndex, result, false, num6, textFormattingMode, useLayoutRounding);
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
                    Size textSize = MeasureHelper.MeasureTextInCell(cachedCell, new Size(double.PositiveInfinity, double.PositiveInfinity), (double)Viewport.Excel.ZoomFactor, null, textFormattingMode, useLayoutRounding);
                    layout3.ContentWidth = MeasureHelper.ConvertTextSizeToExcelCellSize(textSize, (double)Viewport.Excel.ZoomFactor).Width;
                    result.Add(layout3);
                }
            }
            result.TrailingOverflowlayout = BuildTrailingCellOverflowLayoutModel(rowIndex, viewportColumnLayoutModel, result, textFormattingMode, useLayoutRounding);
            if (((result.Count <= 0) && (result.HeadingOverflowlayout == null)) && (result.TrailingOverflowlayout == null))
            {
                return null;
            }
            return result;
        }

        CellOverflowLayout BuildCellOverflowLayoutModelForLeft(Cell bindingCell, int rowIndex, bool buildForCenter, int deadColumnIndex, object textFormattingMode, bool useLayoutRounding)
        {
            ICellsSupport dataContext = Viewport.GetDataContext();
            int index = bindingCell.Column.Index;
            if ((index < (dataContext.Columns.Count - 1)) && (index < deadColumnIndex))
            {
                Column column = dataContext.Columns[index];
                CellLayoutModel cellLayoutModel = Viewport.GetCellLayoutModel();
                Column column2 = dataContext.Columns[index + 1];
                int num2 = column2.Index;
                if ((cellLayoutModel != null) && (cellLayoutModel.FindCell(rowIndex, num2) != null))
                {
                    return null;
                }
                Cell cachedCell = Viewport.CellCache.GetCachedCell(rowIndex, num2);
                if ((cachedCell != null) && !string.IsNullOrEmpty(cachedCell.Text))
                {
                    return null;
                }
                float zoomFactor = Viewport.Excel.ZoomFactor;
                Size textSize = MeasureHelper.MeasureTextInCell(bindingCell, new Size(double.PositiveInfinity, double.PositiveInfinity), (double)zoomFactor, null, textFormattingMode, useLayoutRounding);
                double width = MeasureHelper.ConvertTextSizeToExcelCellSize(textSize, (double)zoomFactor).Width;
                double num5 = column.ActualWidth * zoomFactor;
                if (buildForCenter)
                {
                    width /= 2.0;
                    num5 /= 2.0;
                }
                double num6 = num5;
                if (num6 < width)
                {
                    CellOverflowLayout layout2 = new CellOverflowLayout(column.Index, num6)
                    {
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
                        cachedCell = Viewport.CellCache.GetCachedCell(rowIndex, num2);
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
                        layout2.ContentWidth = MeasureHelper.ConvertTextSizeToExcelCellSize(textSize, (double)zoomFactor).Width;
                        layout2.RightBackgroundWidth = num6;
                        return layout2;
                    }
                }
            }
            return null;
        }

        CellOverflowLayout BuildCellOverflowLayoutModelForRight(Cell bindingCell, int rowIndex, CellOverflowLayoutModel result, bool buildForCenter, int deadColumnIndex, object textFormattingMode, bool useLayoutRounding)
        {
            ICellsSupport dataContext = Viewport.GetDataContext();
            int index = bindingCell.Column.Index;
            if (index > deadColumnIndex)
            {
                Column column = dataContext.Columns[index];
                CellLayoutModel cellLayoutModel = Viewport.GetCellLayoutModel();
                Column column2 = dataContext.Columns[index - 1];
                int num2 = column2.Index;
                if ((cellLayoutModel != null) && (cellLayoutModel.FindCell(rowIndex, num2) != null))
                {
                    return null;
                }
                Cell cachedCell = Viewport.CellCache.GetCachedCell(rowIndex, num2);
                if ((cachedCell != null) && !string.IsNullOrEmpty(cachedCell.Text))
                {
                    return null;
                }
                float zoomFactor = Viewport.Excel.ZoomFactor;
                Size textSize = MeasureHelper.MeasureTextInCell(bindingCell, new Size(double.PositiveInfinity, double.PositiveInfinity), (double)zoomFactor, null, textFormattingMode, useLayoutRounding);
                double width = MeasureHelper.ConvertTextSizeToExcelCellSize(textSize, (double)zoomFactor).Width;
                double num5 = column.ActualWidth * zoomFactor;
                if (buildForCenter)
                {
                    width /= 2.0;
                    num5 /= 2.0;
                }
                double num6 = num5;
                if (num6 < width)
                {
                    CellOverflowLayout layout2 = new CellOverflowLayout(column.Index, num6)
                    {
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
                        cachedCell = Viewport.CellCache.GetCachedCell(rowIndex, num2);
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
                        layout2.ContentWidth = MeasureHelper.ConvertTextSizeToExcelCellSize(textSize, (double)zoomFactor).Width;
                        layout2.LeftBackgroundWidth = num6;
                        return layout2;
                    }
                }
            }
            return null;
        }

        CellOverflowLayout BuildHeadingCellOverflowLayoutModel(int rowIndex, ColumnLayoutModel columnLayoutModel, object textFormattingMode, bool useLayoutRounding)
        {
            ColumnLayout layout = Enumerable.FirstOrDefault<ColumnLayout>(columnLayoutModel, delegate (ColumnLayout clm)
            {
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
            SheetSpanModelBase spanModel = Viewport.GetSpanModel();
            ICellsSupport dataContext = Viewport.GetDataContext();
            for (int i = 1; i < _maxCellOverflowDistance; i++)
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
                    Cell cachedCell = Viewport.CellCache.GetCachedCell(rowIndex, column);
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
                                    int deadColumnIndex = Enumerable.Last<ColumnLayout>((IEnumerable<ColumnLayout>)columnLayoutModel).Column + 1;
                                    layout2 = BuildCellOverflowLayoutModelForLeft(cachedCell, rowIndex, false, deadColumnIndex, textFormattingMode, useLayoutRounding);
                                    if ((layout2 == null) || (layout2.EndingColumn < ViewportLeftColumn))
                                    {
                                        return null;
                                    }
                                    return layout2;
                                }
                            case HorizontalAlignment.Center:
                                {
                                    int num4 = Enumerable.Last<ColumnLayout>((IEnumerable<ColumnLayout>)columnLayoutModel).Column + 1;
                                    layout2 = BuildCellOverflowLayoutModelForLeft(cachedCell, rowIndex, true, num4, textFormattingMode, useLayoutRounding);
                                    if ((layout2 == null) || (layout2.EndingColumn < ViewportLeftColumn))
                                    {
                                        return null;
                                    }
                                    layout2.BackgroundWidth += (dataContext.Columns[column].ActualWidth * Viewport.Excel.ZoomFactor) / 2.0;
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

        CellOverflowLayout BuildTrailingCellOverflowLayoutModel(int rowIndex, ColumnLayoutModel columnLayoutModel, CellOverflowLayoutModel existed, object textFormattingMode, bool useLayoutRounding)
        {
            ColumnLayout layout = Enumerable.LastOrDefault<ColumnLayout>(columnLayoutModel, delegate (ColumnLayout clm)
            {
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
                ICellsSupport dataContext = Viewport.GetDataContext();
                SheetSpanModelBase spanModel = Viewport.GetSpanModel();
                for (int i = 1; i < _maxCellOverflowDistance; i++)
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
                        Cell cachedCell = Viewport.CellCache.GetCachedCell(rowIndex, column);
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
                                        layout2 = BuildCellOverflowLayoutModelForRight(cachedCell, rowIndex, existed, true, deadColumnIndex, textFormattingMode, useLayoutRounding);
                                        if ((layout2 == null) || (layout2.StartingColumn > ViewportRightColumn))
                                        {
                                            return null;
                                        }
                                        layout2.BackgroundWidth += (dataContext.Columns[column].ActualWidth * Viewport.Excel.ZoomFactor) / 2.0;
                                        return layout2;
                                    }
                                case HorizontalAlignment.Right:
                                    {
                                        int num5 = columnLayoutModel[0].Column - 1;
                                        layout2 = BuildCellOverflowLayoutModelForRight(cachedCell, rowIndex, existed, false, num5, textFormattingMode, useLayoutRounding);
                                        if ((layout2 == null) || (layout2.StartingColumn > ViewportRightColumn))
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
            _cachedCellOverflowModels.Clear();
        }

        public CellOverflowLayoutModel GetModel(int rowIndex)
        {
            if (Viewport == null || !Viewport.Excel.CanCellOverflow)
            {
                return null;
            }

            CellOverflowLayoutModel model = null;
            if (_cachedCellOverflowModels.TryGetValue(rowIndex, out model))
            {
                if (model == CellOverflowLayoutModel.Empty)
                {
                    return null;
                }
                return model;
            }
            model = BuildCellOverflowLayoutModel(rowIndex);
            if (model == null)
            {
                _cachedCellOverflowModels[rowIndex] = CellOverflowLayoutModel.Empty;
                return model;
            }
            _cachedCellOverflowModels[rowIndex] = model;
            return model;
        }

        public void RemoveModel(int rowIndex)
        {
            _cachedCellOverflowModels.Remove(rowIndex);
        }

        CellsPanel Viewport { get; set; }

        public int ViewportLeftColumn
        {
            get { return _cachedLeftColumn; }
            set
            {
                if (_cachedLeftColumn != value)
                {
                    _cachedLeftColumn = value;
                    _cachedCellOverflowModels.Clear();
                }
            }
        }

        public int ViewportRightColumn
        {
            get { return _cachedRightColumn; }
            set
            {
                if (_cachedRightColumn != value)
                {
                    _cachedRightColumn = value;
                    _cachedCellOverflowModels.Clear();
                }
            }
        }
    }
}

