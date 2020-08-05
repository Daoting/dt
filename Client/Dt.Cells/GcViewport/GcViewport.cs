#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.CellTypes;
using Dt.Cells.Data;
using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Used within the template of a <see cref="T:GcSpreadSheet" /> to specify the
    /// location in the control's visual tree where the rows are to be added.
    /// </summary>
    internal partial class GcViewport : Panel
    {
        internal int _activeCol;
        internal int _activeRow;
        Panel _borderContainer;
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
        DataValidationPanel _dataValidationPanel;
        DragFillContainerPanel _dragFillContainer;
        Rect _editorBounds;
        internal EditingPanel _editorPanel;
        FloatingObjectContainerPanel _floatingObjectContainerPanel;
        FloatingObjectMovingResizingContainerPanel _floatingObjectsMovingResizingContainer;
        FormulaSelectionContainerPanel _formulaSelectionContainer;
        List<RowPresenter> _recycledRows;
        RowsPanel _rowsContainer;
        SelectionContainerPanel _selectionContainer;
        Panel _shapeContainer;
        protected SheetArea _sheetArea;
        bool _supportShapes;
        DecorationPanel _decoratinPanel;

        public GcViewport(SheetView sheet)
            : this(sheet, SheetArea.Cells, true)
        {
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;
            Loaded += GcViewport_Loaded;
        }

        public GcViewport(SheetView sheet, SheetArea sheetArea, bool supportShapes)
        {
            _cachedSelectionLayout = new List<Rect>();
            _cachedActiveSelectionLayout = Rect.Empty;
            _cachedSelectionFrameLayout = new Rect();
            _cachedFocusCellLayout = new Rect();
            _supportShapes = true;
            _editorBounds = new Rect();
            _recycledRows = new List<RowPresenter>();
            _cachedDragFillFrameRect = Rect.Empty;
            _cachedDragClearRect = Rect.Empty;
            _sheetArea = sheetArea;
            Sheet = sheet;
            _supportShapes = supportShapes;

            _rowsContainer = new RowsPanel { ParentViewport = this };
            Children.Add(_rowsContainer);

            _borderContainer = new GcBorders(this);
            Children.Add(_borderContainer);

            if (SupportSelection)
            {
                _selectionContainer = new SelectionContainerPanel(this);
                Children.Add(_selectionContainer);
                _formulaSelectionContainer = new FormulaSelectionContainerPanel { ParentViewport = this };
                Children.Add(_formulaSelectionContainer);
            }

            if (supportShapes)
            {
                _shapeContainer = new Canvas();
                Children.Add(_shapeContainer);
            }

            if (Sheet.CanUserDragFill)
            {
                _dragFillContainer = new DragFillContainerPanel { ParentViewport = this };
                Children.Add(_dragFillContainer);
            }

            if (SheetArea == SheetArea.Cells)
            {
                // hdt 新增修饰层
                if (sheet.ShowDecoration)
                {
                    _decoratinPanel = new DecorationPanel(this);
                    Children.Add(_decoratinPanel);
                }

                _dataValidationPanel = new DataValidationPanel(this);
                Children.Add(_dataValidationPanel);

                _editorPanel = new EditingPanel(this);
                Children.Add(_editorPanel);

                _floatingObjectContainerPanel = new FloatingObjectContainerPanel(this);
                Children.Add(_floatingObjectContainerPanel);

                _floatingObjectsMovingResizingContainer = new FloatingObjectMovingResizingContainerPanel(this);
                Children.Add(_floatingObjectsMovingResizingContainer);
            }

            _cellCachePool = new CellCachePool(this);
            _cachedSpanGraph = new SpanGraph();
        }

        void _editorPanel_EdtingChanged(object sender, EventArgs e)
        {
            if (_editorPanel != null)
            {
                _editorPanel.InvalidateMeasure();
                _editorPanel.InvalidateArrange();
            }
            Sheet.RaiseEditChange(_activeRow, _activeCol);
        }

        internal void AddDataValidationInvalidDataPresenterInfo(InvalidDataPresenterInfo info)
        {
            _dataValidationPanel.AddInvalidDataPresenterInfo(info);
        }

        void BuildSelection()
        {
            _cachedSelectionLayout.Clear();
            SelectionContainer.FocusIndicator.Visibility = Visibility.Collapsed;
            _cachedFocusCellLayout = Rect.Empty;
            _cachedSelectionFrameLayout = Rect.Empty;
            _cachedActiveSelectionLayout = Rect.Empty;
            _cachedActiveSelection = null;
            _activeRow = Sheet.Worksheet.ActiveRowIndex;
            _activeCol = Sheet.Worksheet.ActiveColumnIndex;
            SelectionContainer.IsAnchorCellInSelection = false;
            CellRange activeCellRange = GetActiveCellRange();
            List<CellRange> list = new List<CellRange>((IEnumerable<CellRange>)Sheet.Worksheet.Selections);
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
                ColumnLayoutModel viewportColumnLayoutModel = Sheet.GetViewportColumnLayoutModel(ColumnViewportIndex);
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
                            SelectionContainer.IsAnchorCellInSelection = true;
                        }
                        int num7 = (range.Row < 0) ? 0 : range.Row;
                        int num8 = (range.Column < 0) ? 0 : range.Column;
                        int rowCount = (range.RowCount < 0) ? Sheet.Worksheet.RowCount : range.RowCount;
                        int columnCount = (range.ColumnCount < 0) ? Sheet.Worksheet.ColumnCount : range.ColumnCount;
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
                        if (!SelectionContainer.IsAnchorCellInSelection)
                        {
                            range3 = activeCellRange;
                        }
                        Rect rect6 = GetRangeBounds(range3);
                        rect6.Intersect(rect);
                        if (!rect6.IsEmpty)
                        {
                            if ((range3.Row == -1) && (range3.Column == -1))
                            {
                                SelectionContainer.FocusIndicator.Thickness = 1.0;
                                _cachedSelectionFrameLayout = rect6;
                            }
                            else if (!SelectionContainer.IsAnchorCellInSelection)
                            {
                                SelectionContainer.FocusIndicator.Thickness = 1.0;
                                _cachedSelectionFrameLayout = new Rect(rect6.Left, rect6.Top, rect6.Width, rect6.Height);
                            }
                            else
                            {
                                SelectionContainer.FocusIndicator.Thickness = 3.0;
                                _cachedSelectionFrameLayout = rect6.IsEmpty ? rect6 : new Rect(rect6.Left - 2.0, rect6.Top - 2.0, rect6.Width + 3.0, rect6.Height + 3.0);
                            }
                            if (!Sheet.IsDraggingFill)
                            {
                                if (range3.Intersects(row, column, rowLayoutModel.Count, viewportColumnLayoutModel.Count))
                                {
                                    if (range3.Row == -1)
                                    {
                                        SelectionContainer.FocusIndicator.IsTopVisible = row == 0;
                                        SelectionContainer.FocusIndicator.IsBottomVisible = num3 == (Sheet.Worksheet.RowCount - 1);
                                    }
                                    else
                                    {
                                        SelectionContainer.FocusIndicator.IsTopVisible = (range3.Row >= row) && (range3.Row <= num3);
                                        int num11 = (range3.Row + range3.RowCount) - 1;
                                        SelectionContainer.FocusIndicator.IsBottomVisible = (num11 >= row) && (num11 <= num3);
                                    }
                                    if (range3.Column == -1)
                                    {
                                        SelectionContainer.FocusIndicator.IsLeftVisible = column == 0;
                                        SelectionContainer.FocusIndicator.IsRightVisible = num5 == (Sheet.Worksheet.ColumnCount - 1);
                                    }
                                    else
                                    {
                                        SelectionContainer.FocusIndicator.IsLeftVisible = (range3.Column >= column) && (range3.Column <= num5);
                                        int num12 = (range3.Column + range3.ColumnCount) - 1;
                                        SelectionContainer.FocusIndicator.IsRightVisible = (num12 >= column) && (num12 <= num5);
                                    }
                                }
                                else
                                {
                                    SelectionContainer.FocusIndicator.IsTopVisible = false;
                                    SelectionContainer.FocusIndicator.IsBottomVisible = false;
                                    SelectionContainer.FocusIndicator.IsLeftVisible = false;
                                    SelectionContainer.FocusIndicator.IsRightVisible = false;
                                }
                            }
                            if (Sheet.CanUserDragFill)
                            {
                                if (!Sheet.IsDraggingFill)
                                {
                                    if (((rect6.Width == 0.0) || (rect6.Height == 0.0)) || (SelectionContainer.FocusIndicator.Thickness == 1.0))
                                    {
                                        SelectionContainer.FocusIndicator.IsFillIndicatorVisible = false;
                                    }
                                    else if ((range3.Row != -1) && (range3.Column != -1))
                                    {
                                        bool flag = SelectionContainer.FocusIndicator.IsRightVisible && SelectionContainer.FocusIndicator.IsBottomVisible;
                                        if (Sheet.InputDeviceType == InputDeviceType.Touch)
                                        {
                                            flag = false;
                                        }
                                        SelectionContainer.FocusIndicator.IsFillIndicatorVisible = flag;
                                        if (flag)
                                        {
                                            SelectionContainer.FocusIndicator.FillIndicatorPosition = FillIndicatorPosition.BottomRight;
                                        }
                                    }
                                    else if ((range3.Row != -1) && (range3.Column == -1))
                                    {
                                        ViewportInfo viewportInfo = Sheet.GetViewportInfo();
                                        bool flag2 = false;
                                        if (Sheet.Worksheet.FrozenColumnCount == 0)
                                        {
                                            flag2 = (ColumnViewportIndex >= 0) && (ColumnViewportIndex < viewportInfo.ColumnViewportCount);
                                        }
                                        else
                                        {
                                            flag2 = (ColumnViewportIndex == -1) || ((ColumnViewportIndex >= 1) && (ColumnViewportIndex < viewportInfo.ColumnViewportCount));
                                        }
                                        flag2 = flag2 && SelectionContainer.FocusIndicator.IsBottomVisible;
                                        if (Sheet.InputDeviceType == InputDeviceType.Touch)
                                        {
                                            flag2 = false;
                                        }
                                        SelectionContainer.FocusIndicator.IsFillIndicatorVisible = flag2;
                                        if (flag2)
                                        {
                                            SelectionContainer.FocusIndicator.FillIndicatorPosition = FillIndicatorPosition.BottomLeft;
                                        }
                                    }
                                    else if ((range3.Column != -1) && (range3.Row == -1))
                                    {
                                        ViewportInfo info2 = Sheet.GetViewportInfo();
                                        bool flag3 = false;
                                        if (Sheet.Worksheet.FrozenRowCount == 0)
                                        {
                                            flag3 = (RowViewportIndex >= 0) && (RowViewportIndex < info2.RowViewportCount);
                                        }
                                        else
                                        {
                                            flag3 = (RowViewportIndex == -1) || ((RowViewportIndex >= 1) && (RowViewportIndex < info2.RowViewportCount));
                                        }
                                        flag3 = flag3 && SelectionContainer.FocusIndicator.IsRightVisible;
                                        if (Sheet.InputDeviceType == InputDeviceType.Touch)
                                        {
                                            flag3 = false;
                                        }
                                        SelectionContainer.FocusIndicator.IsFillIndicatorVisible = flag3;
                                        if (flag3)
                                        {
                                            SelectionContainer.FocusIndicator.FillIndicatorPosition = FillIndicatorPosition.TopRight;
                                        }
                                    }
                                    else
                                    {
                                        SelectionContainer.FocusIndicator.IsFillIndicatorVisible = false;
                                    }
                                }
                            }
                            else
                            {
                                SelectionContainer.FocusIndicator.IsFillIndicatorVisible = false;
                            }
                            SelectionContainer.FocusIndicator.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            SelectionContainer.FocusIndicator.Visibility = Visibility.Collapsed;
                        }
                    }
                    else
                    {
                        Rect rect7 = GetRangeBounds(activeCellRange);
                        if (!rect7.IsEmpty)
                        {
                            SelectionContainer.FocusIndicator.Thickness = 1.0;
                            SelectionContainer.FocusIndicator.Visibility = Visibility.Visible;
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
                            SelectionContainer.FocusIndicator.Visibility = Visibility.Collapsed;
                        }
                        SelectionContainer.FocusIndicator.IsBottomVisible = true;
                        SelectionContainer.FocusIndicator.IsTopVisible = true;
                        SelectionContainer.FocusIndicator.IsLeftVisible = true;
                        SelectionContainer.FocusIndicator.IsRightVisible = true;
                        SelectionContainer.FocusIndicator.IsFillIndicatorVisible = false;
                    }
                }
            }
            if (SelectionContainer.FocusIndicator != null)
            {
                SelectionContainer.FocusIndicator.InvalidateMeasure();
                SelectionContainer.FocusIndicator.InvalidateArrange();
            }
        }

        internal Rect CalcEditorBounds(int row, int column, Size viewportSize)
        {
            CellPresenterBase base2 = GetViewportCell(row, column, true);
            Rect rect = new Rect();
            if ((base2 == null) || (_editorPanel == null))
            {
                return rect;
            }
            Rect rect2 = GetCellBounds(row, column, false);
            rect2.Width--;
            rect2.Height--;
            Rect rect3 = new Rect(Location, viewportSize);
            rect2.Intersect(rect3);
            if (rect2.IsEmpty)
            {
                return rect;
            }
            Size cellContentSize = new Size(rect2.Width, rect2.Height);
            double x = rect2.X;
            double height = viewportSize.Height - (rect2.Top - Location.Y);
            if ((rect2.Width == 0.0) || (rect2.Height == 0.0))
            {
                return rect;
            }
            Cell cachedCell = CellCache.GetCachedCell(row, column);
            HorizontalAlignment alignment = cachedCell.ToHorizontalAlignment();
            switch (alignment)
            {
                case HorizontalAlignment.Left:
                    {
                        float indent = cachedCell.ActualTextIndent * Sheet.ZoomFactor;
                        double num4 = (viewportSize.Width - rect2.Left) + Location.X;
                        num4 = Math.Max(Math.Min(num4, viewportSize.Width), 0.0);
                        Size maxSize = new Size(num4, height);
                        return new Rect(PointToClient(new Windows.Foundation.Point(x, rect2.Y)), base2.GetPreferredEditorSize(maxSize, cellContentSize, alignment, indent));
                    }
                case HorizontalAlignment.Right:
                    {
                        float num5 = cachedCell.ActualTextIndent * Sheet.ZoomFactor;
                        double num6 = rect2.Right - Location.X;
                        num6 = Math.Max(Math.Min(num6, viewportSize.Width), 0.0);
                        Size size4 = new Size(num6, height);
                        Size size = base2.GetPreferredEditorSize(size4, cellContentSize, alignment, num5);
                        Windows.Foundation.Point point = new Windows.Foundation.Point(rect2.Right - size.Width, rect2.Top);
                        return new Rect(PointToClient(point), size);
                    }
                case HorizontalAlignment.Center:
                    {
                        double num7 = (rect2.Left - Location.X) + (rect2.Width / 2.0);
                        if (num7 < 0.0)
                        {
                            num7 = 0.0;
                        }
                        double num8 = viewportSize.Width - num7;
                        if (num8 < 0.0)
                        {
                            num8 = 0.0;
                        }
                        double width = 2.0 * Math.Min(num7, num8);
                        Size size6 = new Size(width, height);
                        Size size7 = base2.GetPreferredEditorSize(size6, cellContentSize, alignment, 0f);
                        if (size7.Width > rect2.Width)
                        {
                            x -= (size7.Width - rect2.Width) / 2.0;
                        }
                        return new Rect(PointToClient(new Windows.Foundation.Point(x, rect2.Y)), size7);
                    }
            }
            Windows.Foundation.Point location = PointToClient(new Windows.Foundation.Point(rect2.X, rect2.Y));
            return new Rect(location, new Size(rect2.Width, rect2.Height));
        }

        bool ContainsRect(Rect rect1, Rect rect2)
        {
            return ((((rect2.Left >= rect1.Left) && (rect2.Right <= rect1.Right)) && (rect2.Top >= rect1.Top)) && (rect2.Bottom <= rect1.Bottom));
        }

        internal bool FocusContent()
        {
            // 手机上已在SheetView.FocusInternal屏蔽
            if ((_editorPanel != null) && (_editorPanel.Editor != null))
            {
                (_editorPanel.Editor as Control).Focus(FocusState.Programmatic);
                return true;
            }
            return false;
        }

        void GcViewport_Loaded(object sender, RoutedEventArgs e)
        {
            int activeColumnViewportIndex = Sheet.GetActiveColumnViewportIndex();
            int activeRowViewportIndex = Sheet.GetActiveRowViewportIndex();
            if (((ColumnViewportIndex == activeColumnViewportIndex) && (RowViewportIndex == activeRowViewportIndex)) && (Sheet.CanSelectFormula && (Sheet.Worksheet.Workbook.ActiveSheetIndex == Sheet.EditorConnector.SheetIndex)))
            {
                Sheet.SetActiveCell(Sheet.EditorInfo.RowIndex, Sheet.EditorInfo.ColumnIndex, true);
                Sheet.StartCellEditing(false, "=" + Sheet.EditorConnector.GetText(), EditorStatus.Edit);
                if (_editorPanel != null)
                {
                    _editorPanel.EditorDirty = true;
                }
                if (!Sheet.EditorConnector.ActivateEditor)
                {
                    Sheet.EditorConnector.ActivateEditor = true;
                    Sheet.StopCellEditing(false);
                    SpreadActions.CommitInputNavigationDown(Sheet, new ActionEventArgs());
                }
            }
            RefreshFormulaSelection();
            if (Sheet != null)
            {
                Sheet.RefreshFormulaSelectionGrippers();
            }
        }

        internal virtual RowPresenter GenerateNewRow()
        {
            return new RowPresenter(this);
        }

        CellRange GetActiveCellRange()
        {
            if ((_activeRow < 0) || (_activeCol < 0))
            {
                return new CellRange(0, 0, 0, 0);
            }
            CellRange range = new CellRange(_activeRow, _activeCol, 1, 1);
            if ((((Sheet.Worksheet != null) && (Sheet.Worksheet.SpanModel != null)) && (!Sheet.Worksheet.SpanModel.IsEmpty() && (range.RowCount == 1))) && (range.ColumnCount == 1))
            {
                CellRange range2 = Sheet.Worksheet.SpanModel.Find(range.Row, range.Column);
                if (range2 != null)
                {
                    range = range2;
                }
            }
            return range;
        }

        internal Rect GetCellBounds(int p_row, int p_column, bool p_ignoreMerged)
        {
            if (Sheet == null)
                return new Rect();

            // 包含合并情况
            CellLayoutModel model = null;
            if (!p_ignoreMerged)
            {
                model = Sheet.GetCellLayoutModel(RowViewportIndex, ColumnViewportIndex, SheetArea);
            }
            CellLayout layout = (model == null) ? null : model.FindCell(p_row, p_column);
            if (layout != null)
            {
                return new Rect(layout.X, layout.Y, layout.Width, layout.Height);
            }

            RowLayoutModel rowLayoutModel = Sheet.GetRowLayoutModel(RowViewportIndex, SheetArea);
            ColumnLayoutModel columnLayoutModel = Sheet.GetColumnLayoutModel(ColumnViewportIndex, SheetArea);
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

        internal virtual CellLayoutModel GetCellLayoutModel()
        {
            return Sheet.GetViewportCellLayoutModel(RowViewportIndex, ColumnViewportIndex);
        }

        internal CellOverflowLayoutModel GetCellOverflowLayoutModel(int rowIndex)
        {
            return CellOverflowLayoutBuildEngine.GetModel(rowIndex);
        }

        internal virtual ICellsSupport GetDataContext()
        {
            return Sheet.Worksheet;
        }

        internal object GetEditorValue()
        {
            if (_editorPanel != null)
            {
                TextBox editor = _editorPanel.Editor as TextBox;
                if (editor != null)
                {
                    return editor.Text;
                }
            }
            return null;
        }

        public int GetFlotingObjectZIndex(string name)
        {
            return FloatingObjectsPanel.GetFlotingObjectZIndex(name);
        }

        internal Rect GetRangeBounds(CellRange range)
        {
            return GetRangeBounds(range, _sheetArea);
        }

        internal Rect GetRangeBounds(CellRange range, SheetArea area)
        {
            SheetSpanModel spanModel = null;
            if (Sheet.Worksheet != null)
            {
                switch (area)
                {
                    case SheetArea.Cells:
                        spanModel = Sheet.Worksheet.SpanModel;
                        break;

                    case (SheetArea.CornerHeader | SheetArea.RowHeader):
                        spanModel = Sheet.Worksheet.RowHeaderSpanModel;
                        break;

                    case SheetArea.ColumnHeader:
                        spanModel = Sheet.Worksheet.ColumnHeaderSpanModel;
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
            RowLayoutModel rowLayoutModel = Sheet.GetRowLayoutModel(RowViewportIndex, area);
            ColumnLayoutModel columnLayoutModel = Sheet.GetColumnLayoutModel(ColumnViewportIndex, area);
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
            return new Rect(PointToClient(new Windows.Foundation.Point(x, y)), new Size(width, height));
        }

        internal Rect GetRangeBounds(CellRange range, out bool isLeftVisible, out bool isRightVisible, out bool isTopVisible, out bool isBottomVisible)
        {
            isLeftVisible = true;
            isRightVisible = true;
            isTopVisible = true;
            isBottomVisible = true;
            RowLayoutModel rowLayoutModel = GetRowLayoutModel();
            ColumnLayoutModel viewportColumnLayoutModel = Sheet.GetViewportColumnLayoutModel(ColumnViewportIndex);
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
            return new Rect(PointToClient(new Windows.Foundation.Point(x, y)), new Size(width, height));
        }

        internal virtual RowPresenter GetRow(int row)
        {
            return RowsContainer.GetRow(row);
        }

        internal virtual RowLayoutModel GetRowLayoutModel()
        {
            return Sheet.GetViewportRowLayoutModel(RowViewportIndex);
        }

        internal virtual SheetSpanModelBase GetSpanModel()
        {
            return Sheet.Worksheet.SpanModel;
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

        internal CellPresenterBase GetViewportCell(int row, int column, bool containsSpan)
        {
            CellPresenterBase cell = null;
            RowPresenter presenter = RowsContainer.GetRow(row);
            if (presenter != null)
            {
                cell = presenter.GetCell(column);
            }
            if (((cell == null) && (CurrentRow != null)) && (row == Sheet.Worksheet.ActiveRowIndex))
            {
                cell = CurrentRow.GetCell(column);
            }
            if (containsSpan && (cell == null))
            {
                foreach (RowPresenter presenter2 in RowsContainer.Rows)
                {
                    if (presenter2 != null)
                    {
                        foreach (CellPresenterBase base3 in presenter2.Cells.Values)
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

        internal virtual Size GetViewportSize(Size availableSize)
        {
            double viewportWidth = Sheet.GetViewportWidth(ColumnViewportIndex);
            double viewportHeight = Sheet.GetViewportHeight(RowViewportIndex);
            return new Size(Math.Min(viewportWidth, availableSize.Width), Math.Min(viewportHeight, availableSize.Height));
        }

        internal void InvalidateBordersMeasureState()
        {
            BorderContainer.InvalidateMeasure();
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
            if (SupportSelection)
            {
                SelectionContainer.InvalidateMeasure();
            }
        }

        internal bool IsCurrentEditingCell(int row, int column)
        {
            if (!IsEditing())
            {
                return false;
            }
            return ((_editorPanel.EditingRowIndex == row) && (_editorPanel.EditingColumnIndex == column));
        }

        public bool IsEditing()
        {
            return ((_editorPanel != null) && (_editorPanel.Opacity == 1.0));
        }

        public Windows.Foundation.Point PointToClient(Windows.Foundation.Point point)
        {
            return new Windows.Foundation.Point(point.X - Location.X, point.Y - Location.Y);
        }

        internal void PrepareCellEditing(CellPresenterBase editingCell)
        {
            if (_editorPanel != null)
            {
                _editorPanel.SetBackground(new SolidColorBrush(Colors.White));
            }

            if (_editorPanel == null)
            {
                _editorPanel = new EditingPanel(this);
                _editorPanel.InstallEditor(editingCell, false);
            }
            else if (((_editorPanel != null) && (_editorPanel.Editor != null)) && (editingCell != null))
            {
                _editorPanel.Update(editingCell);
                if (editingCell.CellType is BaseCellType)
                {
                    FrameworkElement avaiableEditor = _editorPanel.GetAvaiableEditor();
                    object obj2 = Sheet.Worksheet.GetValue(editingCell.Row, editingCell.Column);
                    if (obj2 != null)
                    {
                        if (avaiableEditor == null)
                        {
                            avaiableEditor = editingCell.GetEditingElement();
                        }
                        TextBox box = avaiableEditor as TextBox;
                        if ((box != null) && (box.Text != obj2.ToString()))
                        {
                            (avaiableEditor as TextBox).Text = obj2.ToString();
                            (avaiableEditor as TextBox).SelectionStart = (avaiableEditor as TextBox).Text.Length;
                        }
                        (avaiableEditor as TextBox).SelectAll();
                    }
                    (editingCell.CellType as BaseCellType).SetEditingElement(avaiableEditor);
                }
                _editorPanel.InstallEditor(editingCell, false);
            }
            else if ((_editorPanel != null) && (_editorPanel.Editor == null))
            {
                _editorPanel.InstallEditor(editingCell, false);
            }

            if (_editorPanel.Editor != null)
            {
                _editorPanel.Opacity = 0.0;
                if (!Children.Contains(_editorPanel))
                {
                    Children.Add(_editorPanel);
                }
                // hdt 不需要
                //if (!DesignMode.DesignModeEnabled && ElementTreeHelper.IsKeyboardFocusWithin(Sheet._host))
                //{
                //    (_editorPanel.Editor as Control).Focus(FocusState.Programmatic);
                //}
            }
            _editorPanel.InvalidateMeasure();
            _editorPanel.InvalidateArrange();
        }

        internal void PrepareCellEditing(int row, int column)
        {
            CellPresenterBase editingCell = GetViewportCell(row, column, true);
            if (editingCell != null)
            {
                PrepareCellEditing(editingCell);
            }
            else if (_editorPanel != null)
            {
                _editorPanel.SetBackground(new SolidColorBrush(Colors.Transparent));
            }
        }

        void RefreshChartsResizingFramesLayouts()
        {
            Rect[] floatingObjectsResizingRects = Sheet.GetFloatingObjectsResizingRects(RowViewportIndex, ColumnViewportIndex);
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
            if (_dataValidationPanel != null)
            {
                _dataValidationPanel.InvalidateMeasure();
            }
        }

        void RefreshDragClearRect()
        {
            CellRange dragClearRange = Sheet.GetDragClearRange();
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
            CellRange dragFillFrameRange = Sheet.GetDragFillFrameRange();
            RowLayoutModel rowLayoutModel = GetRowLayoutModel();
            if ((rowLayoutModel != null) && (rowLayoutModel.Count > 0))
            {
                ColumnLayoutModel viewportColumnLayoutModel = Sheet.GetViewportColumnLayoutModel(ColumnViewportIndex);
                if ((viewportColumnLayoutModel != null) && (viewportColumnLayoutModel.Count > 0))
                {
                    int row = rowLayoutModel[0].Row;
                    int num2 = rowLayoutModel[rowLayoutModel.Count - 1].Row;
                    int column = viewportColumnLayoutModel[0].Column;
                    int num4 = viewportColumnLayoutModel[viewportColumnLayoutModel.Count - 1].Column;
                    if (dragFillFrameRange.Row == -1)
                    {
                        DragFillContainer.DragFillFrame.IsTopVisibie = row == 0;
                        DragFillContainer.DragFillFrame.IsBottomVisibe = num2 == (Sheet.Worksheet.RowCount - 1);
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
                        DragFillContainer.DragFillFrame.IsRightVisibe = num4 == (Sheet.Worksheet.ColumnCount - 1);
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
            CellRange dragFillFrameRange = Sheet.GetDragFillFrameRange();
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
            Rect[] floatingObjectsMovingFrameRects = Sheet.GetFloatingObjectsMovingFrameRects(RowViewportIndex, ColumnViewportIndex);
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
            if (SupportSelection)
            {
                FormulaSelectionContainer.Refresh();
            }
        }

        public void RefreshSelection()
        {
            if (SupportSelection)
            {
                FloatingObject[] allSelectedFloatingObjects = Sheet.GetAllSelectedFloatingObjects();
                if ((allSelectedFloatingObjects != null) && (allSelectedFloatingObjects.Length > 0))
                {
                    SelectionContainer.Visibility = Visibility.Collapsed;
                    if (Sheet.InputDeviceType != InputDeviceType.Touch)
                    {
                        (Sheet as SpreadView).InvalidateMeasure();
                    }
                }
                else
                {
                    SelectionContainer.Visibility = Visibility.Visible;
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
            _dataValidationPanel.RemoveInvalidDataPresenterInfo(info);
        }

        internal void RemoveDataValidationUI()
        {
            if (_dataValidationPanel != null)
            {
                _dataValidationPanel.CloseInputMessageToolTip();
                _dataValidationPanel.RemoveDataValidationListButtonInfo();
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
            TextBox tb = ((_editorPanel == null) ? null : ((TextBox)_editorPanel.Editor)) as TextBox;
            if (tb != null)
            {
                tb.SelectAll();
                tb.Focus(FocusState.Programmatic);
            }
            else
            {
                Sheet.FocusInternal();
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
            TextBox box = ((_editorPanel == null) ? null : ((TextBox)_editorPanel.Editor)) as TextBox;
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

        public bool StartCellEditing(int row, int column, bool selectAll)
        {
            return StartCellEditing(row, column, selectAll, string.Empty, EditorStatus.Edit);
        }

        public bool StartCellEditing(int row, int column, bool selectAll, string defaultText, EditorStatus status)
        {
            return StartTextInput(row, column, status, true, selectAll, defaultText);
        }

        internal bool StartTextInput(int row, int column, EditorStatus status)
        {
            return StartTextInput(row, column, status, false, false, null);
        }

        internal bool StartTextInput(int row, int column, EditorStatus status, bool canModifyTextBox, bool selectAll = false, string defaultText = null)
        {
            //hdt
            if (IsEditing())
            {
                return true;
            }
            CellPresenterBase cell = GetViewportCell(row, column, true);
            if (cell != null)
            {
                ShowSheetCell(row, column);
                if (_editorPanel == null)
                {
                    _editorPanel = new EditingPanel(this);
                }
                if ((cell.CellType != null) && !cell.CellType.HasEditingElement())
                {
                    if ((!IsEditing() && (_editorPanel.Editor != null)) && ((_editorPanel.EditingRowIndex == row) && (_editorPanel.EditingColumnIndex == column)))
                    {
                        cell.CellType.SetEditingElement(_editorPanel.Editor);
                    }
                    else
                    {
                        FrameworkElement avaiableEditor = _editorPanel.GetAvaiableEditor();
                        object obj2 = Sheet.Worksheet.GetValue(cell.Row, cell.Column);
                        if (obj2 != null)
                        {
                            if (avaiableEditor == null)
                            {
                                avaiableEditor = cell.GetEditingElement();
                            }
                            TextBox box = avaiableEditor as TextBox;
                            if (box.Text != obj2.ToString())
                            {
                                (avaiableEditor as TextBox).Text = obj2.ToString();
                                (avaiableEditor as TextBox).SelectionStart = (avaiableEditor as TextBox).Text.Length;
                            }
                        }
                        cell.CellType.SetEditingElement(avaiableEditor);
                    }
                }
                _editorPanel.InstallEditor(cell, true);
                _editorPanel.EditingChanged += new EventHandler(_editorPanel_EdtingChanged);
                _editorPanel.Opacity = 1.0;
                if (Sheet.RaiseEditStarting(row, column))
                {
                    _editorPanel.EditingChanged -= new EventHandler(_editorPanel_EdtingChanged);
                    return false;
                }
                if (_editorPanel.Editor != null)
                {
                    if (!base.Children.Contains(_editorPanel))
                    {
                        base.Children.Add(_editorPanel);
                    }
                    _editorPanel.SetEditorStatus(status);
                    TextBox editor = _editorPanel.Editor as TextBox;
                    if (editor != null)
                    {
                        editor.IsReadOnly = false;
                        _editorPanel.IsHitTestVisible = true;

                        // 双击显示光标，IsHitTestVisible为false可控制光标不显示
                        editor.IsHitTestVisible = true;
                        editor.Focus(FocusState.Programmatic);

                        if (canModifyTextBox)
                        {
                            if (defaultText != null)
                            {
                                editor.Text = defaultText;
                            }
                            if (!selectAll && !string.IsNullOrEmpty(editor.SelectedText))
                            {
                                editor.SelectionStart = editor.Text.Length;
                            }
                            else if (selectAll)
                            {
                                if (Sheet._isIMEEnterEditing)
                                {
                                    editor.Text = null;
                                }
                                else
                                {
                                    editor.SelectAll();
                                }
                            }
                            else if (editor.Text != null)
                            {
                                editor.SelectionStart = editor.Text.Length;
                            }
                        }
                    }
                    cell.HideForEditing();
                    _editorPanel.InvalidateMeasure();
                    _editorPanel.InvalidateArrange();
                    return true;
                }
            }
            return false;
        }

        async void ShowSheetCell(int row, int column)
        {
            await Sheet.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Sheet.ShowCell(RowViewportIndex, ColumnViewportIndex, row, column, VerticalPosition.Nearest, HorizontalPosition.Nearest);
            });
        }

        internal bool StartTextInputForWinRT(int row, int column, EditorStatus status)
        {
            //hdt
            if (IsEditing())
            {
                return true;
            }
            CellPresenterBase base2 = GetViewportCell(row, column, true);
            if (base2 != null)
            {
                ShowSheetCell(row, column);
                if (_editorPanel == null)
                {
                    _editorPanel = new EditingPanel(this);
                }
                _editorPanel.EditingChanged += new EventHandler(_editorPanel_EdtingChanged);
                _editorPanel.Opacity = 1.0;
                if (Sheet.RaiseEditStarting(row, column))
                {
                    _editorPanel.EditingChanged -= new EventHandler(_editorPanel_EdtingChanged);
                    return false;
                }
                if (_editorPanel.Editor != null)
                {
                    if (!base.Children.Contains(_editorPanel))
                    {
                        base.Children.Add(_editorPanel);
                    }
                    _editorPanel.SetEditorStatus(status);
                    TextBox editor = _editorPanel.Editor as TextBox;
                    if (editor != null)
                    {
                        _editorPanel.IsHitTestVisible = true;
                        editor.IsHitTestVisible = true;
                        editor.Focus(FocusState.Programmatic);
                    }
                    base2.HideForEditing();
                    _editorPanel.InvalidateMeasure();
                    _editorPanel.InvalidateArrange();
                    return true;
                }
            }
            return false;
        }

        public void StopCellEditing(bool cancel)
        {
            if ((_editorPanel != null) && (_editorPanel.Editor != null))
            {
                _editorPanel.SetEditorStatus(EditorStatus.Ready);
                _editorPanel.EditingChanged -= new EventHandler(_editorPanel_EdtingChanged);
            }
            _editorBounds = new Rect();
            CellPresenterBase cell = GetViewportCell(_activeRow, _activeCol, true);
            if (cell != null)
            {
                cell.UnHideForEditing();
            }
            if (_editorPanel != null)
            {
                _editorPanel.InstallEditor(cell, false);
                _editorPanel.Opacity = 0.0;
                Sheet.FocusInternal();
            }
            Sheet.RaiseEditEnd(_activeRow, _activeCol);
            _editorPanel.InvalidateMeasure();
            _editorPanel.InvalidateArrange();
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

        void UpdateCellState(int row, int column)
        {
            RowPresenter presenter = RowsContainer.GetRow(row);
            if (presenter != null)
            {
                CellPresenterBase cell = presenter.GetCell(column);
                if (cell != null)
                {
                    cell.ApplyState();
                }
            }
        }

        internal void UpdateDataValidationUI(int row, int column)
        {
            RemoveDataValidationUI();
            if (((Sheet != null) && (Sheet.Worksheet != null)) && ((RowViewportIndex == Sheet.GetActiveRowViewportIndex()) && (ColumnViewportIndex == Sheet.GetActiveColumnViewportIndex())))
            {
                DataValidator actualDataValidator = Sheet.Worksheet[row, column].ActualDataValidator;
                if ((actualDataValidator != null) && (GetViewportCell(row, column, true) != null))
                {
                    DataValidationListButtonInfo info = Sheet.GetDataValidationListButtonInfo(row, column, SheetArea.Cells);
                    if (info != null)
                    {
                        if (_dataValidationPanel != null)
                        {
                            _dataValidationPanel.AddDataValidationListButtonInfo(info);
                        }
                    }
                    else if (_dataValidationPanel != null)
                    {
                        _dataValidationPanel.RemoveDataValidationListButtonInfo();
                    }
                    if ((actualDataValidator.ShowInputMessage && !string.IsNullOrEmpty(actualDataValidator.InputMessage)) && (_dataValidationPanel != null))
                    {
                        _dataValidationPanel.ShowInputMessageToolTip(actualDataValidator);
                    }
                }
            }
        }

        protected Panel BorderContainer
        {
            get { return _borderContainer; }
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

        internal RowPresenter CurrentRow { get; set; }

        internal DragFillContainerPanel DragFillContainer
        {
            get
            {
                if ((_dragFillContainer == null) && Sheet.CanUserDragFill)
                {
                    DragFillContainerPanel panel = new DragFillContainerPanel
                    {
                        ParentViewport = this
                    };
                    _dragFillContainer = panel;
                }
                return _dragFillContainer;
            }
        }

        internal EditingPanel EditingContainer
        {
            get { return _editorPanel; }
        }

        internal Rect EditorBounds
        {
            get { return _editorBounds; }
        }

        internal bool EditorDirty
        {
            get { return ((_editorPanel != null) && _editorPanel.EditorDirty); }
        }

        FloatingObjectMovingResizingContainerPanel FloatingObjectsMovingResizingContainer
        {
            get { return _floatingObjectsMovingResizingContainer; }
        }

        internal FloatingObjectContainerPanel FloatingObjectsPanel
        {
            get { return _floatingObjectContainerPanel; }
        }

        internal FormulaSelectionContainerPanel FormulaSelectionContainer
        {
            get { return _formulaSelectionContainer; }
        }

        internal bool IsActived
        {
            get { return ((Sheet == null) || ((Sheet.GetActiveColumnViewportIndex() == ColumnViewportIndex) && (Sheet.GetActiveRowViewportIndex() == RowViewportIndex))); }
        }

        public Windows.Foundation.Point Location { get; set; }

        internal virtual List<RowPresenter> RecycledRows
        {
            get
            {
                if (_recycledRows == null)
                {
                    _recycledRows = new List<RowPresenter>();
                }
                return _recycledRows;
            }
        }

        internal RowsPanel RowsContainer
        {
            get { return _rowsContainer; }
        }

        public int RowViewportIndex { get; set; }

        internal SelectionContainerPanel SelectionContainer
        {
            get { return _selectionContainer; }
        }

        protected Panel ShapesContainer
        {
            get { return _shapeContainer; }
        }

        public SheetView Sheet { get; private set; }

        internal SheetArea SheetArea
        {
            get { return _sheetArea; }
        }

        internal virtual bool SupportCellOverflow
        {
            get { return true; }
        }

        protected virtual bool SupportSelection
        {
            get { return true; }
        }

        internal sealed class CellCachePool : ICellSupport
        {
            Dictionary<ulong, Cell> _cache = new Dictionary<ulong, Cell>();
            GcViewport _parent;

            public CellCachePool(GcViewport parentViewport)
            {
                _parent = parentViewport;
            }

            Cell Add(int rowIndex, int columnIndex)
            {
                ICellsSupport dataContext = ParentViewport.GetDataContext();
                if ((rowIndex < 0) || (rowIndex >= dataContext.Rows.Count))
                {
                    return null;
                }
                if ((columnIndex < 0) || (columnIndex >= dataContext.Columns.Count))
                {
                    return null;
                }
                Cell cell = dataContext.Cells[rowIndex, columnIndex];
                ulong num = (ulong)rowIndex;
                num = num << 0x20;
                num += (ulong)columnIndex;
                _cache[num] = cell;
                cell.CacheStyleObject(true);
                return cell;
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
                foreach (ulong num in Enumerable.ToArray<ulong>((IEnumerable<ulong>)_cache.Keys))
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

            Cell ICellSupport.GetCell(int row, int column)
            {
                return GetCachedCell(row, column);
            }

            GcViewport ParentViewport
            {
                get { return _parent; }
            }
        }
    }
}

