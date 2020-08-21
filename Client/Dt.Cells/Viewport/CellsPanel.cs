#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Cells.Data;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// 单元格视口
    /// </summary>
    internal partial class CellsPanel : Panel
    {
        internal int _activeCol;
        internal int _activeRow;
        Panel _borderLayer;
        CellOverflowLayoutBuildEngine _buildEngine;
        internal CellRange _cachedActiveSelection;
        internal Rect _cachedActiveSelectionLayout;
        internal Rect[] _cachedChartShapeMovingRects;
        internal Rect[] _cachedChartShapeResizingRects;
        internal Rect _cachedDragClearRect;
        internal Rect _cachedDragFillFrameRect;
        internal Rect _cachedFocusCellLayout;
        internal Rect _cachedSelectionFrameLayout;
        internal List<Rect> _cachedSelectionLayout;
        SpanGraph _cachedSpanGraph;
        CellCachePool _cellCachePool;
        DataValidationLayer _dataValidationLayer;
        DragFillLayer _dragFillLayer;
        Rect _editorBounds;
        internal EditingLayer _editorLayer;
        FloatingObjectLayer _floatingObjectLayer;
        FloatingObjectMovingLayer _floatingObjectsMovingResizingLayer;
        FormulaSelectionLayer _formulaSelectionLayer;
        RowsLayer _rowsLayer;
        SelectionLayer _selectionLayer;
        Panel _shapeLayer;
        DecorationLayer _decorationLayer;

        public CellsPanel(Excel p_excel)
        {
            Excel = p_excel;

            _cachedSelectionLayout = new List<Rect>();
            _cachedActiveSelectionLayout = Rect.Empty;
            _cachedSelectionFrameLayout = new Rect();
            _cachedFocusCellLayout = new Rect();
            _editorBounds = new Rect();
            _cachedDragFillFrameRect = Rect.Empty;
            _cachedDragClearRect = Rect.Empty;
            _cellCachePool = new CellCachePool(Excel.ActiveSheet);
            _cachedSpanGraph = new SpanGraph();

            // 1 行数据层
            _rowsLayer = new RowsLayer(this);
            Children.Add(_rowsLayer);

            // 2 网格层，调整为只用在内容区域，行/列头不再使用
            _borderLayer = new BorderLayer(this);
            Children.Add(_borderLayer);

            // 3 选择状态层
            _selectionLayer = new SelectionLayer(this);
            Children.Add(_selectionLayer);

            // 4 公式选择层
            _formulaSelectionLayer = new FormulaSelectionLayer { ParentViewport = this };
            Children.Add(_formulaSelectionLayer);

            // 5 图形层
            _shapeLayer = new Canvas();
            Children.Add(_shapeLayer);

            // 6 拖拽复制层
            if (Excel.CanUserDragFill)
            {
                _dragFillLayer = new DragFillLayer { ParentViewport = this };
                Children.Add(_dragFillLayer);
            }

            // 7 新增修饰层
            if (p_excel.ShowDecoration)
            {
                _decorationLayer = new DecorationLayer(this);
                Children.Add(_decorationLayer);
            }

            // 8 数据校验层
            _dataValidationLayer = new DataValidationLayer(this);
            Children.Add(_dataValidationLayer);

            // 9 编辑层
            _editorLayer = new EditingLayer(this);
            Children.Add(_editorLayer);

            // 10 浮动对象层
            _floatingObjectLayer = new FloatingObjectLayer(this);
            Children.Add(_floatingObjectLayer);

            // 11 浮动对象编辑层
            _floatingObjectsMovingResizingLayer = new FloatingObjectMovingLayer(this);
            Children.Add(_floatingObjectsMovingResizingLayer);

            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;
            Loaded += GcViewport_Loaded;
        }

        internal void AddDataValidationInvalidDataPresenterInfo(InvalidDataPresenterInfo info)
        {
            _dataValidationLayer.AddInvalidDataPresenterInfo(info);
        }

        void BuildSelection()
        {
            _cachedSelectionLayout.Clear();
            _selectionLayer.FocusIndicator.Visibility = Visibility.Collapsed;
            _cachedFocusCellLayout = Rect.Empty;
            _cachedSelectionFrameLayout = Rect.Empty;
            _cachedActiveSelectionLayout = Rect.Empty;
            _cachedActiveSelection = null;
            _activeRow = Excel.ActiveSheet.ActiveRowIndex;
            _activeCol = Excel.ActiveSheet.ActiveColumnIndex;
            _selectionLayer.IsAnchorCellInSelection = false;
            CellRange activeCellRange = GetActiveCellRange();
            List<CellRange> list = new List<CellRange>((IEnumerable<CellRange>)Excel.ActiveSheet.Selections);
            if (list.Count == 0)
            {
                list.Add(activeCellRange);
            }
            int num = list.Count;
            RowLayoutModel rowLayoutModel = GetRowLayoutModel();
            Size viewportSize = GetViewportSize();
            Rect rect = new Rect(0.0, 0.0, viewportSize.Width, viewportSize.Height);
            if ((rowLayoutModel != null) && (rowLayoutModel.Count > 0))
            {
                ColumnLayoutModel viewportColumnLayoutModel = Excel.GetViewportColumnLayoutModel(ColumnViewportIndex);
                if ((viewportColumnLayoutModel != null) && (viewportColumnLayoutModel.Count > 0))
                {
                    int row = rowLayoutModel[0].Row;
                    int num3 = rowLayoutModel[rowLayoutModel.Count - 1].Row;
                    int column = viewportColumnLayoutModel[0].Column;
                    int num5 = viewportColumnLayoutModel[viewportColumnLayoutModel.Count - 1].Column;
                    for (int i = 0; i < num; i++)
                    {
                        CellRange range = list[i];
                        if (range.Contains(_activeRow, _activeCol))
                        {
                            _selectionLayer.IsAnchorCellInSelection = true;
                        }
                        int num7 = (range.Row < 0) ? 0 : range.Row;
                        int num8 = (range.Column < 0) ? 0 : range.Column;
                        int rowCount = (range.RowCount < 0) ? Excel.ActiveSheet.RowCount : range.RowCount;
                        int columnCount = (range.ColumnCount < 0) ? Excel.ActiveSheet.ColumnCount : range.ColumnCount;
                        range = new CellRange(num7, num8, rowCount, columnCount);
                        Rect rect2 = GetRangeBounds(range);
                        rect2.Intersect(rect);
                        if (!rect2.IsEmpty)
                        {
                            _cachedSelectionLayout.Add(new Rect(rect2.Left + 1.0, rect2.Top + 1.0, Math.Max((double)0.0, (double)(rect2.Width - 3.0)), Math.Max((double)0.0, (double)(rect2.Height - 3.0))));
                            if (range.Contains(activeCellRange))
                            {
                                Rect rect3 = new Rect(rect2.Left + 1.0, rect2.Top + 1.0, Math.Max((double)0.0, (double)(rect2.Width - 3.0)), Math.Max((double)0.0, (double)(rect2.Height - 3.0)));
                                if (_cachedActiveSelectionLayout.IsEmpty || (num == 1))
                                {
                                    _cachedActiveSelectionLayout = rect3;
                                    _cachedActiveSelection = range;
                                }
                                else
                                {
                                    Rect rect4 = new Rect(rect3.Left, rect3.Top, rect3.Width, rect3.Height);
                                    rect4.Intersect(_cachedActiveSelectionLayout);
                                    if (rect4.IsEmpty)
                                    {
                                        _cachedActiveSelectionLayout = rect3;
                                        _cachedActiveSelection = range;
                                    }
                                    else if (ContainsRect(rect3, _cachedActiveSelectionLayout))
                                    {
                                        _cachedActiveSelectionLayout = rect3;
                                        _cachedActiveSelection = range;
                                    }
                                }
                            }
                        }
                    }
                    Rect rangeBounds = GetRangeBounds(activeCellRange);
                    if (!rangeBounds.IsEmpty)
                    {
                        rangeBounds = new Rect(rangeBounds.Left + 1.0, rangeBounds.Top + 1.0, Math.Max((double)0.0, (double)(rangeBounds.Width - 3.0)), Math.Max((double)0.0, (double)(rangeBounds.Height - 3.0)));
                    }
                    _cachedFocusCellLayout = rangeBounds;
                    if (num == 1)
                    {
                        CellRange range3 = list[0];
                        if (!_selectionLayer.IsAnchorCellInSelection)
                        {
                            range3 = activeCellRange;
                        }
                        Rect rect6 = GetRangeBounds(range3);
                        rect6.Intersect(rect);
                        if (!rect6.IsEmpty)
                        {
                            if ((range3.Row == -1) && (range3.Column == -1))
                            {
                                _selectionLayer.FocusIndicator.Thickness = 1.0;
                                _cachedSelectionFrameLayout = rect6;
                            }
                            else if (!_selectionLayer.IsAnchorCellInSelection)
                            {
                                _selectionLayer.FocusIndicator.Thickness = 1.0;
                                _cachedSelectionFrameLayout = new Rect(rect6.Left, rect6.Top, rect6.Width, rect6.Height);
                            }
                            else
                            {
                                _selectionLayer.FocusIndicator.Thickness = 3.0;
                                _cachedSelectionFrameLayout = rect6.IsEmpty ? rect6 : new Rect(rect6.Left - 2.0, rect6.Top - 2.0, rect6.Width + 3.0, rect6.Height + 3.0);
                            }
                            if (!Excel.IsDraggingFill)
                            {
                                if (range3.Intersects(row, column, rowLayoutModel.Count, viewportColumnLayoutModel.Count))
                                {
                                    if (range3.Row == -1)
                                    {
                                        _selectionLayer.FocusIndicator.IsTopVisible = row == 0;
                                        _selectionLayer.FocusIndicator.IsBottomVisible = num3 == (Excel.ActiveSheet.RowCount - 1);
                                    }
                                    else
                                    {
                                        _selectionLayer.FocusIndicator.IsTopVisible = (range3.Row >= row) && (range3.Row <= num3);
                                        int num11 = (range3.Row + range3.RowCount) - 1;
                                        _selectionLayer.FocusIndicator.IsBottomVisible = (num11 >= row) && (num11 <= num3);
                                    }
                                    if (range3.Column == -1)
                                    {
                                        _selectionLayer.FocusIndicator.IsLeftVisible = column == 0;
                                        _selectionLayer.FocusIndicator.IsRightVisible = num5 == (Excel.ActiveSheet.ColumnCount - 1);
                                    }
                                    else
                                    {
                                        _selectionLayer.FocusIndicator.IsLeftVisible = (range3.Column >= column) && (range3.Column <= num5);
                                        int num12 = (range3.Column + range3.ColumnCount) - 1;
                                        _selectionLayer.FocusIndicator.IsRightVisible = (num12 >= column) && (num12 <= num5);
                                    }
                                }
                                else
                                {
                                    _selectionLayer.FocusIndicator.IsTopVisible = false;
                                    _selectionLayer.FocusIndicator.IsBottomVisible = false;
                                    _selectionLayer.FocusIndicator.IsLeftVisible = false;
                                    _selectionLayer.FocusIndicator.IsRightVisible = false;
                                }
                            }
                            if (Excel.CanUserDragFill)
                            {
                                if (!Excel.IsDraggingFill)
                                {
                                    if (((rect6.Width == 0.0) || (rect6.Height == 0.0)) || (_selectionLayer.FocusIndicator.Thickness == 1.0))
                                    {
                                        _selectionLayer.FocusIndicator.IsFillIndicatorVisible = false;
                                    }
                                    else if ((range3.Row != -1) && (range3.Column != -1))
                                    {
                                        bool flag = _selectionLayer.FocusIndicator.IsRightVisible && _selectionLayer.FocusIndicator.IsBottomVisible;
                                        if (Excel.InputDeviceType == InputDeviceType.Touch)
                                        {
                                            flag = false;
                                        }
                                        _selectionLayer.FocusIndicator.IsFillIndicatorVisible = flag;
                                        if (flag)
                                        {
                                            _selectionLayer.FocusIndicator.FillIndicatorPosition = FillIndicatorPosition.BottomRight;
                                        }
                                    }
                                    else if ((range3.Row != -1) && (range3.Column == -1))
                                    {
                                        ViewportInfo viewportInfo = Excel.GetViewportInfo();
                                        bool flag2 = false;
                                        if (Excel.ActiveSheet.FrozenColumnCount == 0)
                                        {
                                            flag2 = (ColumnViewportIndex >= 0) && (ColumnViewportIndex < viewportInfo.ColumnViewportCount);
                                        }
                                        else
                                        {
                                            flag2 = (ColumnViewportIndex == -1) || ((ColumnViewportIndex >= 1) && (ColumnViewportIndex < viewportInfo.ColumnViewportCount));
                                        }
                                        flag2 = flag2 && _selectionLayer.FocusIndicator.IsBottomVisible;
                                        if (Excel.InputDeviceType == InputDeviceType.Touch)
                                        {
                                            flag2 = false;
                                        }
                                        _selectionLayer.FocusIndicator.IsFillIndicatorVisible = flag2;
                                        if (flag2)
                                        {
                                            _selectionLayer.FocusIndicator.FillIndicatorPosition = FillIndicatorPosition.BottomLeft;
                                        }
                                    }
                                    else if ((range3.Column != -1) && (range3.Row == -1))
                                    {
                                        ViewportInfo info2 = Excel.GetViewportInfo();
                                        bool flag3 = false;
                                        if (Excel.ActiveSheet.FrozenRowCount == 0)
                                        {
                                            flag3 = (RowViewportIndex >= 0) && (RowViewportIndex < info2.RowViewportCount);
                                        }
                                        else
                                        {
                                            flag3 = (RowViewportIndex == -1) || ((RowViewportIndex >= 1) && (RowViewportIndex < info2.RowViewportCount));
                                        }
                                        flag3 = flag3 && _selectionLayer.FocusIndicator.IsRightVisible;
                                        if (Excel.InputDeviceType == InputDeviceType.Touch)
                                        {
                                            flag3 = false;
                                        }
                                        _selectionLayer.FocusIndicator.IsFillIndicatorVisible = flag3;
                                        if (flag3)
                                        {
                                            _selectionLayer.FocusIndicator.FillIndicatorPosition = FillIndicatorPosition.TopRight;
                                        }
                                    }
                                    else
                                    {
                                        _selectionLayer.FocusIndicator.IsFillIndicatorVisible = false;
                                    }
                                }
                            }
                            else
                            {
                                _selectionLayer.FocusIndicator.IsFillIndicatorVisible = false;
                            }
                            _selectionLayer.FocusIndicator.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            _selectionLayer.FocusIndicator.Visibility = Visibility.Collapsed;
                        }
                    }
                    else
                    {
                        Rect rect7 = GetRangeBounds(activeCellRange);
                        if (!rect7.IsEmpty)
                        {
                            _selectionLayer.FocusIndicator.Thickness = 1.0;
                            _selectionLayer.FocusIndicator.Visibility = Visibility.Visible;
                            _cachedSelectionFrameLayout = rect7;
                            _cachedSelectionFrameLayout.Width = Math.Max((double)0.0, (double)(_cachedSelectionFrameLayout.Width - 1.0));
                            _cachedSelectionFrameLayout.Height = Math.Max((double)0.0, (double)(_cachedSelectionFrameLayout.Height - 1.0));
                            int num13 = 0;
                            int num14 = 0;
                            int num15 = 0;
                            int num16 = 0;
                            for (int j = 0; j < num; j++)
                            {
                                CellRange range4 = list[j];
                                if (range4 != null)
                                {
                                    if (activeCellRange.Row == range4.Row)
                                    {
                                        num14 = 1;
                                    }
                                    if (activeCellRange.Column == range4.Column)
                                    {
                                        num13 = 1;
                                    }
                                    if (activeCellRange.Row == ((range4.Row + range4.RowCount) - 1))
                                    {
                                        num16 = 1;
                                    }
                                    if (activeCellRange.Column == ((range4.Column + range4.ColumnCount) - 1))
                                    {
                                        num15 = 1;
                                    }
                                }
                            }
                            _cachedSelectionFrameLayout.Y += num14;
                            _cachedSelectionFrameLayout.Height = Math.Max((double)0.0, (double)(_cachedSelectionFrameLayout.Height - (num14 + num16)));
                            _cachedSelectionFrameLayout.X += num13;
                            _cachedSelectionFrameLayout.Width = Math.Max((double)0.0, (double)(_cachedSelectionFrameLayout.Width - (num13 + num15)));
                        }
                        else
                        {
                            _selectionLayer.FocusIndicator.Visibility = Visibility.Collapsed;
                        }
                        _selectionLayer.FocusIndicator.IsBottomVisible = true;
                        _selectionLayer.FocusIndicator.IsTopVisible = true;
                        _selectionLayer.FocusIndicator.IsLeftVisible = true;
                        _selectionLayer.FocusIndicator.IsRightVisible = true;
                        _selectionLayer.FocusIndicator.IsFillIndicatorVisible = false;
                    }
                }
            }
            if (_selectionLayer.FocusIndicator != null)
            {
                _selectionLayer.FocusIndicator.InvalidateMeasure();
                _selectionLayer.FocusIndicator.InvalidateArrange();
            }
        }

        bool ContainsRect(Rect rect1, Rect rect2)
        {
            return ((((rect2.Left >= rect1.Left) && (rect2.Right <= rect1.Right)) && (rect2.Top >= rect1.Top)) && (rect2.Bottom <= rect1.Bottom));
        }

        internal bool FocusContent()
        {
            // 手机上已在Excel.FocusInternal屏蔽
            if (_editorLayer.Editor != null)
            {
                _editorLayer.Editor.Focus(FocusState.Programmatic);
                return true;
            }
            return false;
        }

        void GcViewport_Loaded(object sender, RoutedEventArgs e)
        {
            int activeColumnViewportIndex = Excel.GetActiveColumnViewportIndex();
            int activeRowViewportIndex = Excel.GetActiveRowViewportIndex();
            if (((ColumnViewportIndex == activeColumnViewportIndex) && (RowViewportIndex == activeRowViewportIndex)) && (Excel.CanSelectFormula && (Excel.ActiveSheet.Workbook.ActiveSheetIndex == Excel.EditorConnector.SheetIndex)))
            {
                Excel.SetActiveCell(Excel.EditorInfo.RowIndex, Excel.EditorInfo.ColumnIndex, true);
                Excel.StartCellEditing(false, "=" + Excel.EditorConnector.GetText(), EditorStatus.Edit);
                _editorLayer.EditorDirty = true;
                if (!Excel.EditorConnector.ActivateEditor)
                {
                    Excel.EditorConnector.ActivateEditor = true;
                    Excel.StopCellEditing(false);
                    SpreadActions.CommitInputNavigationDown(Excel, new ActionEventArgs());
                }
            }
            RefreshFormulaSelection();
            if (Excel != null)
            {
                Excel.RefreshFormulaSelectionGrippers();
            }
        }

        CellRange GetActiveCellRange()
        {
            if ((_activeRow < 0) || (_activeCol < 0))
            {
                return new CellRange(0, 0, 0, 0);
            }
            CellRange range = new CellRange(_activeRow, _activeCol, 1, 1);
            var sheet = Excel.ActiveSheet;
            if ((((sheet != null) && (sheet.SpanModel != null)) && (!sheet.SpanModel.IsEmpty() && (range.RowCount == 1))) && (range.ColumnCount == 1))
            {
                CellRange range2 = sheet.SpanModel.Find(range.Row, range.Column);
                if (range2 != null)
                {
                    range = range2;
                }
            }
            return range;
        }

        internal Rect GetCellBounds(int p_row, int p_column, bool p_ignoreMerged)
        {
            if (Excel == null)
                return new Rect();

            // 包含合并情况
            CellLayoutModel model = null;
            if (!p_ignoreMerged)
            {
                model = Excel.GetCellLayoutModel(RowViewportIndex, ColumnViewportIndex, SheetArea.Cells);
            }
            CellLayout layout = (model == null) ? null : model.FindCell(p_row, p_column);
            if (layout != null)
            {
                return new Rect(layout.X, layout.Y, layout.Width, layout.Height);
            }

            RowLayoutModel rowLayoutModel = Excel.GetRowLayoutModel(RowViewportIndex, SheetArea.Cells);
            ColumnLayoutModel columnLayoutModel = Excel.GetColumnLayoutModel(ColumnViewportIndex, SheetArea.Cells);
            if (rowLayoutModel == null || columnLayoutModel == null)
                return new Rect();

            RowLayout layout2 = rowLayoutModel.Find(p_row);
            ColumnLayout layout3 = columnLayoutModel.Find(p_column);
            double x = -1.0;
            double y = -1.0;
            double width = 0.0;
            double height = 0.0;
            if (layout2 != null)
            {
                y = layout2.Y;
                height = layout2.Height;
            }
            if (layout3 != null)
            {
                x = layout3.X;
                width = layout3.Width;
            }
            return new Rect(x, y, width, height);
        }

        internal CellLayoutModel GetCellLayoutModel()
        {
            return Excel.GetViewportCellLayoutModel(RowViewportIndex, ColumnViewportIndex);
        }

        internal CellOverflowLayoutModel GetCellOverflowLayoutModel(int rowIndex)
        {
            return CellOverflowLayoutBuildEngine.GetModel(rowIndex);
        }

        internal ICellsSupport GetDataContext()
        {
            return Excel.ActiveSheet;
        }

        internal object GetEditorValue()
        {
            var editor = _editorLayer.Editor;
            if (editor != null)
                return editor.Text;
            return null;
        }

        public int GetFlotingObjectZIndex(string name)
        {
            return FloatingObjectsPanel.GetFlotingObjectZIndex(name);
        }

        internal Rect GetRangeBounds(CellRange range)
        {
            return GetRangeBounds(range, SheetArea.Cells);
        }

        internal Rect GetRangeBounds(CellRange range, SheetArea area)
        {
            SheetSpanModel spanModel = null;
            if (Excel.ActiveSheet != null)
            {
                switch (area)
                {
                    case SheetArea.Cells:
                        spanModel = Excel.ActiveSheet.SpanModel;
                        break;

                    case (SheetArea.CornerHeader | SheetArea.RowHeader):
                        spanModel = Excel.ActiveSheet.RowHeaderSpanModel;
                        break;

                    case SheetArea.ColumnHeader:
                        spanModel = Excel.ActiveSheet.ColumnHeaderSpanModel;
                        break;
                }
            }
            if (spanModel != null)
            {
                CellRange range2 = spanModel.Find(range.Row, range.Column);
                if (((range2 != null) && (range2.RowCount >= range.RowCount)) && (range2.ColumnCount >= range.ColumnCount))
                {
                    range = range2;
                }
            }
            RowLayoutModel rowLayoutModel = Excel.GetRowLayoutModel(RowViewportIndex, area);
            ColumnLayoutModel columnLayoutModel = Excel.GetColumnLayoutModel(ColumnViewportIndex, area);
            int row = (rowLayoutModel.Count > 0) ? rowLayoutModel[0].Row : -1;
            int num2 = (rowLayoutModel.Count > 0) ? rowLayoutModel[rowLayoutModel.Count - 1].Row : -1;
            int column = (columnLayoutModel.Count > 0) ? columnLayoutModel[0].Column : -1;
            int num4 = (columnLayoutModel.Count > 0) ? columnLayoutModel[columnLayoutModel.Count - 1].Column : -1;
            if (!range.Intersects(row, column, (num2 - row) + 1, (num4 - column) + 1))
            {
                return Rect.Empty;
            }
            int index = Math.Max(range.Row, row);
            int num6 = Math.Max(range.Column, column);
            int num7 = Math.Min((range.Row + range.RowCount) - 1, num2);
            int num8 = Math.Min((range.Column + range.ColumnCount) - 1, num4);
            RowLayout layout = rowLayoutModel.Find(index);
            RowLayout layout2 = rowLayoutModel.Find(num7);
            ColumnLayout layout3 = columnLayoutModel.Find(num6);
            ColumnLayout layout4 = columnLayoutModel.Find(num8);
            double x = -1.0;
            double y = -1.0;
            double height = 0.0;
            double width = 0.0;
            if ((layout != null) && (layout2 != null))
            {
                y = layout.Y;
                height = (layout2.Y + layout2.Height) - layout.Y;
            }
            else if (rowLayoutModel.Count > 0)
            {
                y = rowLayoutModel[0].Y;
                height = (rowLayoutModel[rowLayoutModel.Count - 1].Y + rowLayoutModel[rowLayoutModel.Count - 1].Height) - y;
            }
            if ((layout3 != null) && (layout4 != null))
            {
                x = layout3.X;
                width = (layout4.X + layout4.Width) - layout3.X;
            }
            else if (columnLayoutModel.Count > 0)
            {
                x = columnLayoutModel[0].X;
                width = (columnLayoutModel[columnLayoutModel.Count - 1].X + columnLayoutModel[columnLayoutModel.Count - 1].Width) - x;
            }
            return new Rect(PointToClient(new Point(x, y)), new Size(width, height));
        }

        internal Rect GetRangeBounds(CellRange range, out bool isLeftVisible, out bool isRightVisible, out bool isTopVisible, out bool isBottomVisible)
        {
            isLeftVisible = true;
            isRightVisible = true;
            isTopVisible = true;
            isBottomVisible = true;
            RowLayoutModel rowLayoutModel = GetRowLayoutModel();
            ColumnLayoutModel viewportColumnLayoutModel = Excel.GetViewportColumnLayoutModel(ColumnViewportIndex);
            int row = (rowLayoutModel.Count > 0) ? rowLayoutModel[0].Row : -1;
            int num2 = (rowLayoutModel.Count > 0) ? rowLayoutModel[rowLayoutModel.Count - 1].Row : -1;
            int column = (viewportColumnLayoutModel.Count > 0) ? viewportColumnLayoutModel[0].Column : -1;
            int num4 = (viewportColumnLayoutModel.Count > 0) ? viewportColumnLayoutModel[viewportColumnLayoutModel.Count - 1].Column : -1;
            if (!range.Intersects(row, column, (num2 - row) + 1, (num4 - column) + 1))
            {
                return Rect.Empty;
            }
            int index = Math.Max(range.Row, row);
            int num6 = Math.Max(range.Column, column);
            int num7 = Math.Min((range.Row + range.RowCount) - 1, num2);
            int num8 = Math.Min((range.Column + range.ColumnCount) - 1, num4);
            RowLayout layout = rowLayoutModel.Find(index);
            RowLayout layout2 = rowLayoutModel.Find(num7);
            ColumnLayout layout3 = viewportColumnLayoutModel.Find(num6);
            ColumnLayout layout4 = viewportColumnLayoutModel.Find(num8);
            double x = -1.0;
            double y = -1.0;
            double height = 0.0;
            double width = 0.0;
            if ((layout != null) && (layout2 != null))
            {
                y = layout.Y;
                height = (layout2.Y + layout2.Height) - layout.Y;
            }
            else if (rowLayoutModel.Count > 0)
            {
                y = rowLayoutModel[0].Y;
                height = (rowLayoutModel[rowLayoutModel.Count - 1].Y + rowLayoutModel[rowLayoutModel.Count - 1].Height) - y;
            }
            if ((layout3 != null) && (layout4 != null))
            {
                x = layout3.X;
                width = (layout4.X + layout4.Width) - layout3.X;
            }
            else if (viewportColumnLayoutModel.Count > 0)
            {
                x = viewportColumnLayoutModel[0].X;
                width = (viewportColumnLayoutModel[viewportColumnLayoutModel.Count - 1].X + viewportColumnLayoutModel[viewportColumnLayoutModel.Count - 1].Width) - x;
            }
            if (range.Column < column)
            {
                isLeftVisible = false;
            }
            if (range.Row < row)
            {
                isTopVisible = false;
            }
            if ((num4 != -1) && (((range.Column + range.ColumnCount) - 1) > num4))
            {
                isRightVisible = false;
            }
            if ((num2 != -1) && (((range.Row + range.RowCount) - 1) > num2))
            {
                isBottomVisible = false;
            }
            return new Rect(PointToClient(new Point(x, y)), new Size(width, height));
        }

        internal RowItem GetRow(int row)
        {
            return RowsContainer.GetRow(row);
        }

        internal RowLayoutModel GetRowLayoutModel()
        {
            return Excel.GetViewportRowLayoutModel(RowViewportIndex);
        }

        internal SheetSpanModelBase GetSpanModel()
        {
            return Excel.ActiveSheet.SpanModel;
        }

        internal SpreadChartView GetSpreadChartView(string chartName)
        {
            if (FloatingObjectsPanel != null)
            {
                FloatingObjectContainer floatingObjectContainer = FloatingObjectsPanel.GetFloatingObjectContainer(chartName);
                if ((floatingObjectContainer != null) && (floatingObjectContainer is SpreadChartContainer))
                {
                    return (floatingObjectContainer as SpreadChartContainer).SpreadChartView;
                }
            }
            return null;
        }

        internal Rect GetViewportBounds()
        {
            return new Rect(Location, GetViewportSize());
        }

        internal CellItem GetViewportCell(int row, int column, bool containsSpan)
        {
            CellItem cell = null;
            RowItem presenter = RowsContainer.GetRow(row);
            if (presenter != null)
            {
                cell = presenter.GetCell(column);
            }
            if (containsSpan && (cell == null))
            {
                foreach (RowItem presenter2 in RowsContainer.Rows)
                {
                    if (presenter2 != null)
                    {
                        foreach (CellItem base3 in presenter2.Cells.Values)
                        {
                            if (((base3 != null) && (base3.CellLayout != null)) && ((base3.CellLayout.Row == row) && (base3.CellLayout.Column == column)))
                            {
                                return base3;
                            }
                        }
                    }
                }
            }
            return cell;
        }

        internal Size GetViewportSize()
        {
            return GetViewportSize(new Size(double.PositiveInfinity, double.PositiveInfinity));
        }

        internal Size GetViewportSize(Size availableSize)
        {
            double viewportWidth = Excel.GetViewportWidth(ColumnViewportIndex);
            double viewportHeight = Excel.GetViewportHeight(RowViewportIndex);
            return new Size(Math.Min(viewportWidth, availableSize.Width), Math.Min(viewportHeight, availableSize.Height));
        }

        internal void InvalidateBordersMeasureState()
        {
            _borderLayer?.InvalidateMeasure();
        }

        internal void InvalidateFloatingObjectMeasureState(FloatingObject floatingObject)
        {
            InvalidateFloatingObjectsMeasureState();
        }

        internal void InvalidateFloatingObjectsMeasureState()
        {
            if (FloatingObjectsPanel != null)
            {
                FloatingObjectsPanel.InvalidateMeasure();
            }
        }

        internal void InvalidateRowsMeasureState(bool forceInvalidateRows)
        {
            if (RowsContainer != null)
            {
                RowsContainer.InvalidateMeasure();
                if (forceInvalidateRows)
                {
                    CellCache.ClearAll();
                    CellOverflowLayoutBuildEngine.Clear();
                    foreach (UIElement elem in RowsContainer.Children)
                    {
                        elem.InvalidateMeasure();
                    }
                }
            }
        }

        internal void InvalidateSelectionMeasureState()
        {
            _selectionLayer?.InvalidateMeasure();
        }

        internal bool IsCurrentEditingCell(int row, int column)
        {
            if (!IsEditing())
            {
                return false;
            }
            return ((_editorLayer.EditingRowIndex == row) && (_editorLayer.EditingColumnIndex == column));
        }

        public Point PointToClient(Point point)
        {
            return new Point(point.X - Location.X, point.Y - Location.Y);
        }

        void RefreshChartsResizingFramesLayouts()
        {
            Rect[] floatingObjectsResizingRects = Excel.GetFloatingObjectsResizingRects(RowViewportIndex, ColumnViewportIndex);
            if ((floatingObjectsResizingRects != null) && (floatingObjectsResizingRects.Length != 0))
            {
                List<Rect> list = new List<Rect>();
                foreach (Rect rect in floatingObjectsResizingRects)
                {
                    list.Add(new Rect(rect.X - Location.X, rect.Y - Location.Y, rect.Width, rect.Height));
                }
                _cachedChartShapeResizingRects = list.ToArray();
            }
        }

        internal void RefreshDataValidationInvalidCircles()
        {
            if (_dataValidationLayer != null)
            {
                _dataValidationLayer.InvalidateMeasure();
            }
        }

        void RefreshDragClearRect()
        {
            CellRange dragClearRange = Excel.GetDragClearRange();
            if (dragClearRange != null)
            {
                _cachedDragClearRect = GetRangeBounds(dragClearRange);
                if (!_cachedDragClearRect.IsEmpty)
                {
                    _cachedDragClearRect = new Rect(_cachedDragClearRect.Left - 2.0, _cachedDragClearRect.Top - 2.0, _cachedDragClearRect.Width + 3.0, _cachedDragClearRect.Height + 3.0);
                }
            }
            else
            {
                _cachedDragClearRect = Rect.Empty;
            }
        }

        internal void RefreshDragFill()
        {
            RefreshDragFillFrame();
            RefreshDragClearRect();
            DragFillContainer.InvalidateMeasure();
            DragFillContainer.InvalidateArrange();
        }

        void RefreshDragFillFrame()
        {
            RefreshDragFillFrameLayouts();
            RefreshDragFillFrameBorders();
        }

        void RefreshDragFillFrameBorders()
        {
            CellRange dragFillFrameRange = Excel.GetDragFillFrameRange();
            RowLayoutModel rowLayoutModel = GetRowLayoutModel();
            if ((rowLayoutModel != null) && (rowLayoutModel.Count > 0))
            {
                ColumnLayoutModel viewportColumnLayoutModel = Excel.GetViewportColumnLayoutModel(ColumnViewportIndex);
                if ((viewportColumnLayoutModel != null) && (viewportColumnLayoutModel.Count > 0))
                {
                    int row = rowLayoutModel[0].Row;
                    int num2 = rowLayoutModel[rowLayoutModel.Count - 1].Row;
                    int column = viewportColumnLayoutModel[0].Column;
                    int num4 = viewportColumnLayoutModel[viewportColumnLayoutModel.Count - 1].Column;
                    if (dragFillFrameRange.Row == -1)
                    {
                        DragFillContainer.DragFillFrame.IsTopVisibie = row == 0;
                        DragFillContainer.DragFillFrame.IsBottomVisibe = num2 == (Excel.ActiveSheet.RowCount - 1);
                    }
                    else
                    {
                        DragFillContainer.DragFillFrame.IsTopVisibie = (dragFillFrameRange.Row >= row) && (dragFillFrameRange.Row <= num2);
                        int num5 = (dragFillFrameRange.Row + dragFillFrameRange.RowCount) - 1;
                        DragFillContainer.DragFillFrame.IsBottomVisibe = (num5 >= row) && (num5 <= num2);
                    }
                    if (dragFillFrameRange.Column == -1)
                    {
                        DragFillContainer.DragFillFrame.IsLeftVisibe = column == 0;
                        DragFillContainer.DragFillFrame.IsRightVisibe = num4 == (Excel.ActiveSheet.ColumnCount - 1);
                    }
                    else
                    {
                        DragFillContainer.DragFillFrame.IsLeftVisibe = (dragFillFrameRange.Column >= column) && (dragFillFrameRange.Column <= num4);
                        int num6 = (dragFillFrameRange.Column + dragFillFrameRange.ColumnCount) - 1;
                        DragFillContainer.DragFillFrame.IsRightVisibe = (num6 >= column) && (num6 <= num4);
                    }
                }
            }
        }

        void RefreshDragFillFrameLayouts()
        {
            CellRange dragFillFrameRange = Excel.GetDragFillFrameRange();
            _cachedDragFillFrameRect = GetRangeBounds(dragFillFrameRange);
            if (!_cachedDragFillFrameRect.IsEmpty)
            {
                _cachedDragFillFrameRect = new Rect(_cachedDragFillFrameRect.Left - 2.0, _cachedDragFillFrameRect.Top - 2.0, _cachedDragFillFrameRect.Width + 3.0, _cachedDragFillFrameRect.Height + 3.0);
            }
        }

        internal void RefreshFlaotingObjectResizingFrames()
        {
            RefreshChartsResizingFramesLayouts();
            FloatingObjectsMovingResizingContainer.InvalidateMeasure();
            FloatingObjectsMovingResizingContainer.InvalidateArrange();
        }

        internal void RefreshFloatingObject(object parameter)
        {
            if (FloatingObjectsPanel != null)
            {
                FloatingObjectsPanel.Refresh(parameter);
            }
        }

        internal void RefreshFloatingObjectContainerIsSelected()
        {
            if (FloatingObjectsPanel != null)
            {
                FloatingObjectsPanel.RefreshContainerIsSelected();
            }
        }

        internal void RefreshFloatingObjectContainerIsSelected(FloatingObject floatingObject)
        {
            if (FloatingObjectsPanel != null)
            {
                FloatingObjectsPanel.RefreshContainerIsSelected(floatingObject);
            }
        }

        internal void RefreshFloatingObjectMovingFrames()
        {
            RefreshFloatingObjectMovingFramesLayouts();
            FloatingObjectsMovingResizingContainer.InvalidateMeasure();
            FloatingObjectsMovingResizingContainer.InvalidateArrange();
        }

        void RefreshFloatingObjectMovingFramesLayouts()
        {
            Rect[] floatingObjectsMovingFrameRects = Excel.GetFloatingObjectsMovingFrameRects(RowViewportIndex, ColumnViewportIndex);
            if ((floatingObjectsMovingFrameRects != null) && (floatingObjectsMovingFrameRects.Length != 0))
            {
                List<Rect> list = new List<Rect>();
                foreach (Rect rect in floatingObjectsMovingFrameRects)
                {
                    list.Add(new Rect(rect.X - Location.X, rect.Y - Location.Y, rect.Width, rect.Height));
                }
                _cachedChartShapeMovingRects = list.ToArray();
            }
        }

        internal void RefreshFloatingObjects()
        {
            if (FloatingObjectsPanel != null)
            {
                FloatingObjectsPanel.Refresh(null);
            }
        }

        public void RefreshFormulaSelection()
        {
            _formulaSelectionLayer?.Refresh();
        }

        public void RefreshSelection()
        {
            if (_selectionLayer != null)
            {
                FloatingObject[] allSelectedFloatingObjects = Excel.GetAllSelectedFloatingObjects();
                if ((allSelectedFloatingObjects != null) && (allSelectedFloatingObjects.Length > 0))
                {
                    _selectionLayer.Visibility = Visibility.Collapsed;
                    if (Excel.InputDeviceType != InputDeviceType.Touch)
                    {
                        Excel.InvalidateMeasure();
                    }
                }
                else
                {
                    _selectionLayer.Visibility = Visibility.Visible;
                    BuildSelection();
                    InvalidateSelectionMeasureState();
                }
            }
        }

        internal void RemoveCellOverflowLayoutModel(int rowIndex)
        {
            CellOverflowLayoutBuildEngine.RemoveModel(rowIndex);
        }

        internal void RemoveDataValidationInvalidDataPresenterInfo(InvalidDataPresenterInfo info)
        {
            _dataValidationLayer.RemoveInvalidDataPresenterInfo(info);
        }

        internal void RemoveDataValidationUI()
        {
            if (_dataValidationLayer != null)
            {
                _dataValidationLayer.CloseInputMessageToolTip();
                _dataValidationLayer.RemoveDataValidationListButtonInfo();
            }
        }

        internal void ResetDragFill()
        {
            _cachedDragFillFrameRect = Rect.Empty;
            _cachedDragClearRect = Rect.Empty;
            DragFillContainer.InvalidateMeasure();
            DragFillContainer.InvalidateArrange();
        }

        internal void ResetFloatingObjectovingFrames()
        {
            _cachedChartShapeMovingRects = null;
            FloatingObjectsMovingResizingContainer.InvalidateMeasure();
            FloatingObjectsMovingResizingContainer.InvalidateArrange();
        }

        internal void ResetFloatingObjectResizingFrames()
        {
            _cachedChartShapeResizingRects = null;
            FloatingObjectsMovingResizingContainer.InvalidateMeasure();
            FloatingObjectsMovingResizingContainer.InvalidateArrange();
        }

        internal void ResumeFloatingObjectsInvalidate(bool forceInvalidate)
        {
            if (FloatingObjectsPanel != null)
            {
                FloatingObjectsPanel.ResumeFloatingObjectInvalidate(forceInvalidate);
            }
        }

        public void RetryEditing()
        {
            var tb = _editorLayer.Editor;
            if (tb != null)
            {
                tb.SelectAll();
                tb.Focus(FocusState.Programmatic);
            }
            else
            {
                Excel.FocusInternal();
            }
        }

        internal void SendFirstKey(string c, bool replace)
        {
            foreach (char ch in c)
            {
                if (((ch != '\r') && (ch != '\n')) && char.IsControl(ch))
                {
                    return;
                }
            }
            var box = _editorLayer.Editor;
            if ((box != null) && !box.IsReadOnly)
            {
                if (replace)
                {
                    box.Text = c;
                    box.SelectionStart = box.Text.Length;
                }
                else
                {
                    int selectionStart = box.SelectionStart;
                    int startIndex = selectionStart;
                    string text = box.Text;
                    if (!string.IsNullOrEmpty(box.Text))
                    {
                        MatchCollection matchs = Regex.Matches(box.Text.Replace("\n", "").Substring(0, selectionStart), "\r");
                        if (matchs != null)
                        {
                            startIndex += matchs.Count;
                        }
                    }
                    if (startIndex <= box.Text.Length)
                    {
                        box.Text = box.Text.Insert(startIndex, c);
                        box.SelectionStart = selectionStart + 1;
                        EditingContainer.InvalidateMeasure();
                        EditingContainer.InvalidateArrange();
                    }
                }
            }
        }

        public void SetFlotingObjectZIndex(string name, int zIndex)
        {
            FloatingObjectsPanel.SetFlotingObjectZIndex(name, zIndex);
        }

        async void ShowSheetCell(int row, int column)
        {
            await Excel.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Excel.ShowCell(RowViewportIndex, ColumnViewportIndex, row, column, VerticalPosition.Nearest, HorizontalPosition.Nearest);
            });
        }

        internal void SuspendFloatingObjectsInvalidate()
        {
            if (FloatingObjectsPanel != null)
            {
                FloatingObjectsPanel.SuspendFloatingObjectInvalidate();
            }
        }

        internal void SynChartShapeThemes()
        {
            if (FloatingObjectsPanel != null)
            {
                FloatingObjectsPanel.SyncChartShapeTheme();
            }
        }

        internal void UpdateDataValidationUI(int row, int column)
        {
            RemoveDataValidationUI();
            if (((Excel != null) && (Excel.ActiveSheet != null)) && ((RowViewportIndex == Excel.GetActiveRowViewportIndex()) && (ColumnViewportIndex == Excel.GetActiveColumnViewportIndex())))
            {
                DataValidator actualDataValidator = Excel.ActiveSheet[row, column].ActualDataValidator;
                if ((actualDataValidator != null) && (GetViewportCell(row, column, true) != null))
                {
                    DataValidationListButtonInfo info = Excel.GetDataValidationListButtonInfo(row, column, SheetArea.Cells);
                    if (info != null)
                    {
                        if (_dataValidationLayer != null)
                        {
                            _dataValidationLayer.AddDataValidationListButtonInfo(info);
                        }
                    }
                    else if (_dataValidationLayer != null)
                    {
                        _dataValidationLayer.RemoveDataValidationListButtonInfo();
                    }
                    if ((actualDataValidator.ShowInputMessage && !string.IsNullOrEmpty(actualDataValidator.InputMessage)) && (_dataValidationLayer != null))
                    {
                        _dataValidationLayer.ShowInputMessageToolTip(actualDataValidator);
                    }
                }
            }
        }

        internal SpanGraph CachedSpanGraph
        {
            get { return _cachedSpanGraph; }
        }

        internal CellCachePool CellCache
        {
            get { return _cellCachePool; }
        }

        internal CellOverflowLayoutBuildEngine CellOverflowLayoutBuildEngine
        {
            get
            {
                if (_buildEngine == null)
                {
                    _buildEngine = new CellOverflowLayoutBuildEngine(this);
                }
                return _buildEngine;
            }
        }

        public int ColumnViewportIndex { get; set; }

        internal DragFillLayer DragFillContainer
        {
            get
            {
                if ((_dragFillLayer == null) && Excel.CanUserDragFill)
                {
                    DragFillLayer panel = new DragFillLayer
                    {
                        ParentViewport = this
                    };
                    _dragFillLayer = panel;
                }
                return _dragFillLayer;
            }
        }

        internal EditingLayer EditingContainer
        {
            get { return _editorLayer; }
        }

        internal Rect EditorBounds
        {
            get { return _editorBounds; }
        }

        internal bool EditorDirty
        {
            get { return _editorLayer.EditorDirty; }
        }

        FloatingObjectMovingLayer FloatingObjectsMovingResizingContainer
        {
            get { return _floatingObjectsMovingResizingLayer; }
        }

        internal FloatingObjectLayer FloatingObjectsPanel
        {
            get { return _floatingObjectLayer; }
        }

        internal FormulaSelectionLayer FormulaSelectionContainer
        {
            get { return _formulaSelectionLayer; }
        }

        internal bool IsActived
        {
            get { return ((Excel == null) || ((Excel.GetActiveColumnViewportIndex() == ColumnViewportIndex) && (Excel.GetActiveRowViewportIndex() == RowViewportIndex))); }
        }

        public Point Location { get; set; }

        internal RowsLayer RowsContainer
        {
            get { return _rowsLayer; }
        }

        public int RowViewportIndex { get; set; }

        internal SelectionLayer SelectionContainer
        {
            get { return _selectionLayer; }
        }

        protected Panel ShapesContainer
        {
            get { return _shapeLayer; }
        }

        public Excel Excel { get; private set; }

        internal bool SupportCellOverflow
        {
            get { return true; }
        }

    }

    internal sealed class CellCachePool : ICellSupport
    {
        Dictionary<ulong, Cell> _cache = new Dictionary<ulong, Cell>();
        ICellsSupport _cellsSupport;

        public CellCachePool(ICellsSupport p_cellsSupport)
        {
            _cellsSupport = p_cellsSupport;
        }

        public void ClearAll()
        {
            foreach (Cell cell in _cache.Values)
            {
                if (cell != null)
                {
                    cell.CacheStyleObject(false);
                }
            }
            _cache.Clear();
        }

        public void ClearRow(int rowIndex)
        {
            foreach (ulong num in _cache.Keys)
            {
                int num2 = (int)(num >> 0x20);
                if (num2 == rowIndex)
                {
                    _cache[num].CacheStyleObject(false);
                    _cache.Remove(num);
                }
            }
        }

        public Cell GetCachedCell(int rowIndex, int columnIndex)
        {
            ulong num = (ulong)rowIndex;
            num = num << 0x20;
            num += (ulong)columnIndex;
            Cell cell = null;
            if (_cache.TryGetValue(num, out cell))
            {
                return cell;
            }
            return Add(rowIndex, columnIndex);
        }

        Cell Add(int rowIndex, int columnIndex)
        {
            if ((rowIndex < 0)
                || (rowIndex >= _cellsSupport.Rows.Count)
                || (columnIndex < 0)
                || (columnIndex >= _cellsSupport.Columns.Count))
            {
                return null;
            }

            Cell cell = _cellsSupport.Cells[rowIndex, columnIndex];
            ulong num = (ulong)rowIndex;
            num = num << 0x20;
            num += (ulong)columnIndex;
            _cache[num] = cell;
            cell.CacheStyleObject(true);
            return cell;
        }

        Cell ICellSupport.GetCell(int row, int column)
        {
            return GetCachedCell(row, column);
        }
    }
}
