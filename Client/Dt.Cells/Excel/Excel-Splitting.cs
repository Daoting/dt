#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-08-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using Dt.Cells.UI;
using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Base
{
    public partial class Excel
    {
        void StartColumnSplitting()
        {
            if (Workbook.Protect)
                return;

            HitTestInformation savedHitTestInformation = GetHitInfo();
            SheetLayout layout = GetSheetLayout();
            if (!IsTouching)
            {
                IsColumnSplitting = true;
            }
            else
            {
                IsTouchColumnSplitting = true;
            }
            IsWorking = true;
            if (_columnSplittingTracker == null)
            {
                // 动态分割线，创建后始终在可视树，通过Opacity控制是否显示
                _columnSplittingTracker = new Line();
                _columnSplittingTracker.Stroke = BrushRes.BlackBrush;
                _trackersPanel.Children.Add(_columnSplittingTracker);
            }
            int columnViewportIndex = savedHitTestInformation.ColumnViewportIndex;
            _columnSplittingTracker.Opacity = 0.5;
            switch (savedHitTestInformation.HitTestType)
            {
                case HitTestType.RowSplitBar:
                case HitTestType.ColumnSplitBar:
                    _columnSplittingTracker.StrokeThickness = _defaultSplitBarSize;
                    _columnSplittingTracker.X1 = layout.GetHorizontalSplitBarX(columnViewportIndex) + (_defaultSplitBarSize / 2.0);
                    _columnSplittingTracker.Y1 = layout.Y;
                    _columnSplittingTracker.X2 = _columnSplittingTracker.X1;
                    _columnSplittingTracker.Y2 = layout.HeaderY + _availableSize.Height;
                    return;

                case HitTestType.RowSplitBox:
                    return;

                case HitTestType.ColumnSplitBox:
                    _columnSplittingTracker.StrokeThickness = _defaultSplitBarSize;
                    if (ColumnSplitBoxAlignment == SplitBoxAlignment.Leading)
                    {
                        _columnSplittingTracker.X1 = layout.GetViewportX(columnViewportIndex) + (_defaultSplitBarSize / 2.0);
                    }
                    else
                    {
                        _columnSplittingTracker.X1 = (layout.GetViewportX(columnViewportIndex) + layout.GetViewportWidth(columnViewportIndex)) - (_defaultSplitBarSize / 2.0);
                    }

                    _columnSplittingTracker.Y1 = layout.Y;
                    _columnSplittingTracker.X2 = _columnSplittingTracker.X1;
                    _columnSplittingTracker.Y2 = layout.HeaderY + _availableSize.Height;
                    return;
            }
        }

        void ContinueColumnSplitting()
        {
            HitTestInformation savedHitTestInformation = GetHitInfo();
            SheetLayout layout = GetSheetLayout();
            int columnViewportIndex = savedHitTestInformation.ColumnViewportIndex;
            switch (savedHitTestInformation.HitTestType)
            {
                case HitTestType.RowSplitBar:
                case HitTestType.ColumnSplitBar:
                    if (MousePosition.X <= _columnSplittingTracker.X1)
                    {
                        _columnSplittingTracker.X1 = Math.Max(MousePosition.X, layout.GetViewportX(columnViewportIndex) + (layout.GetHorizontalSplitBarWidth(columnViewportIndex) / 2.0));
                        break;
                    }
                    _columnSplittingTracker.X1 = Math.Min(MousePosition.X, (layout.GetViewportX(columnViewportIndex + 1) + layout.GetViewportWidth(columnViewportIndex + 1)) - (layout.GetHorizontalSplitBarWidth(columnViewportIndex) / 2.0));
                    break;

                case HitTestType.ColumnSplitBox:
                    if (MousePosition.X <= _columnSplittingTracker.X1)
                    {
                        _columnSplittingTracker.X1 = Math.Max(MousePosition.X, layout.GetViewportX(columnViewportIndex) + (_defaultSplitBarSize / 2.0));
                        break;
                    }
                    _columnSplittingTracker.X1 = Math.Min(MousePosition.X, (layout.GetViewportX(columnViewportIndex) + layout.GetViewportWidth(columnViewportIndex)) - (_defaultSplitBarSize / 2.0));
                    break;
            }
            _columnSplittingTracker.X2 = _columnSplittingTracker.X1;
        }

        void EndColumnSplitting()
        {
            double num2;
            HitTestInformation savedHitTestInformation = GetHitInfo();
            SheetLayout layout = GetSheetLayout();
            int columnViewportIndex = savedHitTestInformation.ColumnViewportIndex;
            IsWorking = false;
            IsTouchColumnSplitting = false;
            IsColumnSplitting = false;
            switch (savedHitTestInformation.HitTestType)
            {
                case HitTestType.RowSplitBar:
                case HitTestType.ColumnSplitBar:
                    if (MousePosition.X <= layout.GetHorizontalSplitBarX(savedHitTestInformation.ColumnViewportIndex))
                    {
                        num2 = layout.GetHorizontalSplitBarX(savedHitTestInformation.ColumnViewportIndex) - MousePosition.X;
                    }
                    else
                    {
                        num2 = Math.Max((double)0.0, (double)((MousePosition.X - layout.GetHorizontalSplitBarX(savedHitTestInformation.ColumnViewportIndex)) - layout.GetHorizontalSplitBarWidth(savedHitTestInformation.ColumnViewportIndex)));
                    }
                    if (num2 != 0.0)
                    {
                        double deltaViewportWidth = (_columnSplittingTracker.X1 - layout.GetHorizontalSplitBarX(savedHitTestInformation.ColumnViewportIndex)) - (layout.GetHorizontalSplitBarWidth(savedHitTestInformation.ColumnViewportIndex) / 2.0);
                        int viewportIndex = savedHitTestInformation.ColumnViewportIndex;
                        if (!RaiseColumnViewportWidthChanging(viewportIndex, deltaViewportWidth))
                        {
                            AdjustColumnViewport(columnViewportIndex, deltaViewportWidth);
                            RaiseColumnViewportWidthChanged(viewportIndex, deltaViewportWidth);
                        }
                    }
                    break;

                case HitTestType.ColumnSplitBox:
                    if (ColumnSplitBoxAlignment == SplitBoxAlignment.Leading)
                    {
                        num2 = Math.Max(0.0, MousePosition.X - layout.GetViewportX(savedHitTestInformation.ColumnViewportIndex) - _defaultSplitBarSize);
                    }
                    else
                    {
                        num2 = Math.Max(0.0, layout.GetViewportX(savedHitTestInformation.ColumnViewportIndex) + layout.GetViewportWidth(savedHitTestInformation.ColumnViewportIndex) - MousePosition.X - _defaultSplitBarSize);
                    }

                    if (num2 > 0.0)
                    {
                        double num3 = (_columnSplittingTracker.X1 - layout.GetViewportX(columnViewportIndex)) - (_defaultSplitBarSize / 2.0);
                        int num4 = (ColumnSplitBoxAlignment == SplitBoxAlignment.Leading) ? 0 : (GetViewportInfo().ColumnViewportCount - 1);
                        if (!RaiseColumnViewportWidthChanging(num4, num3))
                        {
                            AddColumnViewport(columnViewportIndex, num3);
                            RaiseColumnViewportWidthChanged(num4, num3);
                            ShowCell(GetActiveRowViewportIndex(), GetActiveColumnViewportIndex(), ActiveSheet.ActiveRowIndex, ActiveSheet.ActiveColumnIndex, VerticalPosition.Nearest, HorizontalPosition.Nearest);
                        }
                    }
                    break;

                default:
                    break;
            }
            _columnSplittingTracker.Opacity = 0.0;
        }

        void StartRowSplitting()
        {
            if (Workbook.Protect)
                return;

            HitTestInformation savedHitTestInformation = GetHitInfo();
            SheetLayout layout = GetSheetLayout();
            if (!IsTouching)
            {
                IsRowSplitting = true;
            }
            else
            {
                IsTouchRowSplitting = true;
            }
            IsWorking = true;
            if (_rowSplittingTracker == null)
            {
                // 动态分割线，创建后始终在可视树，通过Opacity控制是否显示
                _rowSplittingTracker = new Line();
                _rowSplittingTracker.Stroke = BrushRes.BlackBrush;
                _trackersPanel.Children.Add(_rowSplittingTracker);
            }
            int rowViewportIndex = savedHitTestInformation.RowViewportIndex;
            _rowSplittingTracker.Opacity = 0.5;
            switch (savedHitTestInformation.HitTestType)
            {
                case HitTestType.RowSplitBar:
                case HitTestType.ColumnSplitBar:
                    _rowSplittingTracker.StrokeThickness = _defaultSplitBarSize;
                    _rowSplittingTracker.Y1 = layout.GetVerticalSplitBarY(rowViewportIndex) + (_defaultSplitBarSize / 2.0);
                    _rowSplittingTracker.X1 = layout.X;
                    _rowSplittingTracker.Y2 = _rowSplittingTracker.Y1;
                    _rowSplittingTracker.X2 = layout.X + _availableSize.Width;
                    return;

                case HitTestType.RowSplitBox:
                    _rowSplittingTracker.StrokeThickness = _defaultSplitBarSize;
                    if (RowSplitBoxAlignment == SplitBoxAlignment.Leading)
                    {
                        _rowSplittingTracker.Y1 = layout.GetViewportY(rowViewportIndex) + (_defaultSplitBarSize / 2.0);
                    }
                    else
                    {
                        _rowSplittingTracker.Y1 = (layout.GetViewportY(rowViewportIndex) + layout.GetViewportHeight(rowViewportIndex)) - (_defaultSplitBarSize / 2.0);
                    }

                    _rowSplittingTracker.X1 = layout.X;
                    _rowSplittingTracker.Y2 = _rowSplittingTracker.Y1;
                    _rowSplittingTracker.X2 = layout.X + _availableSize.Width;
                    return;
            }
        }

        void ContinueRowSplitting()
        {
            HitTestInformation savedHitTestInformation = GetHitInfo();
            SheetLayout layout = GetSheetLayout();
            int rowViewportIndex = savedHitTestInformation.RowViewportIndex;
            switch (savedHitTestInformation.HitTestType)
            {
                case HitTestType.RowSplitBar:
                case HitTestType.ColumnSplitBar:
                    if (MousePosition.Y <= _rowSplittingTracker.Y1)
                    {
                        _rowSplittingTracker.Y1 = Math.Max(MousePosition.Y, layout.GetViewportY(rowViewportIndex) + (layout.GetVerticalSplitBarHeight(rowViewportIndex) / 2.0));
                        break;
                    }
                    _rowSplittingTracker.Y1 = Math.Min(MousePosition.Y, (layout.GetViewportY(rowViewportIndex + 1) + layout.GetViewportHeight(rowViewportIndex + 1)) - (layout.GetVerticalSplitBarHeight(rowViewportIndex) / 2.0));
                    break;

                case HitTestType.RowSplitBox:
                    if (MousePosition.Y <= _rowSplittingTracker.Y1)
                    {
                        _rowSplittingTracker.Y1 = Math.Max(MousePosition.Y, layout.GetViewportY(rowViewportIndex) + (_defaultSplitBarSize / 2.0));
                        break;
                    }
                    _rowSplittingTracker.Y1 = Math.Min(MousePosition.Y, (layout.GetViewportY(rowViewportIndex) + layout.GetViewportHeight(rowViewportIndex)) - (_defaultSplitBarSize / 2.0));
                    break;
            }
            _rowSplittingTracker.Y2 = _rowSplittingTracker.Y1;
        }

        void EndRowSplitting()
        {
            double num2;
            HitTestInformation savedHitTestInformation = GetHitInfo();
            SheetLayout layout = GetSheetLayout();
            int rowViewportIndex = savedHitTestInformation.RowViewportIndex;
            IsWorking = false;
            IsRowSplitting = false;
            IsTouchRowSplitting = false;
            switch (savedHitTestInformation.HitTestType)
            {
                case HitTestType.RowSplitBar:
                case HitTestType.ColumnSplitBar:
                    if (MousePosition.Y <= layout.GetVerticalSplitBarY(rowViewportIndex))
                    {
                        num2 = layout.GetVerticalSplitBarY(rowViewportIndex) - MousePosition.Y;
                    }
                    else
                    {
                        num2 = Math.Max(0.0, ((MousePosition.Y - layout.GetVerticalSplitBarY(rowViewportIndex)) - layout.GetVerticalSplitBarHeight(rowViewportIndex)));
                    }
                    if (num2 != 0.0)
                    {
                        double deltaViewportHeight = (_rowSplittingTracker.Y1 - layout.GetVerticalSplitBarY(rowViewportIndex)) - (layout.GetVerticalSplitBarHeight(rowViewportIndex) / 2.0);
                        int viewportIndex = savedHitTestInformation.RowViewportIndex;
                        if (!RaiseRowViewportHeightChanging(viewportIndex, deltaViewportHeight))
                        {
                            AdjustRowViewport(rowViewportIndex, deltaViewportHeight);
                            RaiseRowViewportHeightChanged(viewportIndex, deltaViewportHeight);
                        }
                    }
                    break;

                case HitTestType.RowSplitBox:
                    if (RowSplitBoxAlignment == SplitBoxAlignment.Leading)
                    {
                        num2 = Math.Max(0.0, ((MousePosition.Y - layout.GetViewportY(rowViewportIndex)) - _defaultSplitBarSize));
                    }
                    else
                    {
                        num2 = Math.Max(0.0, (((layout.GetViewportY(rowViewportIndex) + layout.GetViewportHeight(rowViewportIndex)) - MousePosition.Y) - _defaultSplitBarSize));
                    }
                    if (num2 > 0.0)
                    {
                        double num3 = (_rowSplittingTracker.Y1 - layout.GetViewportY(rowViewportIndex)) - (_defaultSplitBarSize / 2.0);
                        int num4 = (RowSplitBoxAlignment == SplitBoxAlignment.Leading) ? 0 : (GetViewportInfo().RowViewportCount - 1);
                        if (!RaiseRowViewportHeightChanging(num4, num3))
                        {
                            AddRowViewport(rowViewportIndex, num3);
                            RaiseRowViewportHeightChanged(num4, num3);
                            ShowCell(GetActiveRowViewportIndex(), GetActiveColumnViewportIndex(), ActiveSheet.ActiveRowIndex, ActiveSheet.ActiveColumnIndex, VerticalPosition.Nearest, HorizontalPosition.Nearest);
                        }
                    }
                    break;

                default:
                    break;
            }
            _rowSplittingTracker.Opacity = 0.0;
        }

        void UpdateHorizontalSplitBars()
        {
            SheetLayout layout = GetSheetLayout();
            if ((_horizontalSplitBar != null) && ((ActiveSheet == null) || (_horizontalSplitBar.Length != (layout.ColumnPaneCount - 1))))
            {
                foreach (var bar in _horizontalSplitBar)
                {
                    Children.Remove(bar);
                }
                _horizontalSplitBar = null;
            }

            if (((ActiveSheet != null) && (_horizontalSplitBar == null)) && (layout.ColumnPaneCount > 1))
            {
                _horizontalSplitBar = new Rectangle[layout.ColumnPaneCount - 1];
                for (int i = 0; i < _horizontalSplitBar.Length; i++)
                {
                    _horizontalSplitBar[i] = CreateSplitBar();
                    //Canvas.SetZIndex(_horizontalSplitBar[i], 2);
                }
            }
        }

        void UpdateVerticalSplitBars()
        {
            SheetLayout layout = GetSheetLayout();
            if ((_verticalSplitBar != null) && ((ActiveSheet == null) || (_verticalSplitBar.Length != (layout.RowPaneCount - 1))))
            {
                foreach (var bar in _verticalSplitBar)
                {
                    Children.Remove(bar);
                }
                _verticalSplitBar = null;
            }
            if (((ActiveSheet != null) && (_verticalSplitBar == null)) && (layout.RowPaneCount > 1))
            {
                _verticalSplitBar = new Rectangle[layout.RowPaneCount - 1];
                for (int i = 0; i < _verticalSplitBar.Length; i++)
                {
                    _verticalSplitBar[i] = CreateSplitBar();
                    //Canvas.SetZIndex(_verticalSplitBar[i], 2);
                }
            }
        }

        void UpdateCrossSplitBars()
        {
            SheetLayout layout = GetSheetLayout();
            if ((_crossSplitBar != null) && (((ActiveSheet == null) || (_crossSplitBar.GetLength(0) != (layout.RowPaneCount - 1))) || (_crossSplitBar.GetLength(1) != (layout.ColumnPaneCount - 1))))
            {
                for (int i = 0; i < _crossSplitBar.GetLength(0); i++)
                {
                    for (int j = 0; j < _crossSplitBar.GetLength(1); j++)
                    {
                        Children.Remove(_crossSplitBar[i, j]);
                    }
                }
                _crossSplitBar = null;
            }

            if (ActiveSheet != null
                && _crossSplitBar == null
                && layout.RowPaneCount > 1
                && layout.ColumnPaneCount > 1)
            {
                _crossSplitBar = new Rectangle[layout.RowPaneCount - 1, layout.ColumnPaneCount - 1];
                for (int k = 0; k < _crossSplitBar.GetLength(0); k++)
                {
                    for (int m = 0; m < _crossSplitBar.GetLength(1); m++)
                    {
                        _crossSplitBar[k, m] = CreateSplitBar();
                        //Canvas.SetZIndex(_crossSplitBar[k, m], 2);
                    }
                }
            }
        }

        Rectangle CreateSplitBar()
        {
            return new Rectangle { Fill = BrushRes.中灰1, Stroke = BrushRes.中灰2 };
        }


        void UpdateHorizontalSplitBoxes()
        {
            SheetLayout layout = GetSheetLayout();
            if ((_horizontalSplitBox != null) && ((ActiveSheet == null) || (_horizontalSplitBox.Length != layout.ColumnPaneCount)))
            {
                for (int i = 0; i < _horizontalSplitBox.Length; i++)
                {
                    Children.Remove(_horizontalSplitBox[i]);
                }
                _horizontalSplitBox = null;
            }

            if ((ActiveSheet != null) && (_horizontalSplitBox == null))
            {
                _horizontalSplitBox = new Border[layout.ColumnPaneCount];
                for (int j = 0; j < layout.ColumnPaneCount; j++)
                {
                    var bd = new Border
                    {
                        Background = BrushRes.浅灰1,
                        Child = new TextBlock
                        {
                            Text = "\uE016",
                            // 居中在android上位置不对
                            Margin = new Thickness(9, 7, 0, 0),
                            //HorizontalAlignment = HorizontalAlignment.Center,
                            //VerticalAlignment = VerticalAlignment.Center,
                            FontFamily = (FontFamily)Application.Current.Resources["IconFont"],
                            FontSize = 12,
                            Foreground = BrushRes.深灰2,
                        }
                    };
                    _horizontalSplitBox[j] = bd;
                }
            }
        }

        void UpdateVerticalSplitBoxes()
        {
            SheetLayout layout = GetSheetLayout();
            if ((_verticalSplitBox != null) && ((ActiveSheet == null) || (_verticalSplitBox.Length != layout.RowPaneCount)))
            {
                foreach (var box in _verticalSplitBox)
                {
                    Children.Remove(box);
                }
                _verticalSplitBox = null;
            }

            if ((ActiveSheet != null) && (_verticalSplitBox == null))
            {
                _verticalSplitBox = new Border[layout.RowPaneCount];
                for (int i = 0; i < layout.RowPaneCount; i++)
                {
                    var bd = new Border
                    {
                        Background = BrushRes.浅灰1,
                        Child = new TextBlock
                        {
                            Text = "\uE018",
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                            FontFamily = (FontFamily)Application.Current.Resources["IconFont"],
                            FontSize = 12,
                            Foreground = BrushRes.深灰2,
                        }
                    };
                    _verticalSplitBox[i] = bd;
                }
            }
        }

    }
}

