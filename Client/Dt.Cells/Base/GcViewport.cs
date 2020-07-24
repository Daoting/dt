#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using Dt.Cells.CellTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
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

        #region 测量布局
        protected override Size MeasureOverride(Size availableSize)
        {
            BuildSpanGraph();

            if (_rowsContainer != null)
            {
                _rowsContainer.Measure(availableSize);
            }

            if (_borderContainer != null)
            {
                _borderContainer.Measure(availableSize);
            }

            if (_selectionContainer != null)
            {
                BuildSelection();
                _selectionContainer.Measure(availableSize);
                if (_formulaSelectionContainer.Children.Count > 0)
                {
                    _formulaSelectionContainer.InvalidateMeasure();
                }
                _formulaSelectionContainer.Measure(availableSize);
            }

            if (_shapeContainer != null)
            {
                _shapeContainer.Measure(availableSize);
            }

            if (_dragFillContainer != null)
            {
                _dragFillContainer.Measure(availableSize);
            }

            if (_editorPanel != null)
            {
                _editorPanel.Measure(availableSize);
            }

            AttachEditorForActiveCell();

            // hdt
            if (_decoratinPanel != null)
            {
                _decoratinPanel.InvalidateMeasure();
                _decoratinPanel.Measure(availableSize);
            }

            if (_dataValidationPanel != null)
            {
                _dataValidationPanel.InvalidateMeasure();
            }

            if (Sheet._formulaSelectionGripperPanel != null)
            {
                Sheet._formulaSelectionGripperPanel.InvalidateMeasure();
            }

            if (_floatingObjectContainerPanel != null)
            {
                _floatingObjectContainerPanel.Measure(availableSize);
            }

            if (_floatingObjectsMovingResizingContainer != null)
            {
                _floatingObjectsMovingResizingContainer.Measure(availableSize);
            }

            return GetViewportSize(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Rect rc = new Rect(0.0, 0.0, finalSize.Width, finalSize.Height);
            if (_rowsContainer != null)
            {
                _rowsContainer.Arrange(rc);
            }

            if (_borderContainer != null)
            {
                _borderContainer.Arrange(rc);
            }

            if (_selectionContainer != null)
            {
                _selectionContainer.Arrange(rc);
                _formulaSelectionContainer.Arrange(rc);
            }

            if (_shapeContainer != null)
            {
                _shapeContainer.Arrange(rc);
                // hdt
                //Sheet.GetViewportColumnLayoutModel(ColumnViewportIndex);
                //Sheet.GetViewportRowLayoutModel(RowViewportIndex);
            }

            if (_editorPanel != null)
            {
                if (IsEditing())
                {
                    _editorPanel.ResumeEditor();
                }
                _editorPanel.Arrange(rc);
            }

            if (_dragFillContainer != null)
            {
                _dragFillContainer.Arrange(rc);
            }

            // hdt
            if (_decoratinPanel != null)
            {
                _decoratinPanel.InvalidateArrange();
                _decoratinPanel.Arrange(rc);
            }

            if (_dataValidationPanel != null)
            {
                _dataValidationPanel.Arrange(rc);
            }
            // hdt
            //if (((SheetArea == SheetArea.Cells) && (Sheet != null)) && (Sheet.Worksheet != null))
            //{
            //    if (Sheet.Worksheet != null)
            //    {
            //        int activeRowIndex = Sheet.Worksheet.ActiveRowIndex;
            //    }
            //    if (Sheet.Worksheet != null)
            //    {
            //        int activeColumnIndex = Sheet.Worksheet.ActiveColumnIndex;
            //    }
            //}

            Size viewportSize = GetViewportSize(finalSize);

            if (_floatingObjectContainerPanel != null)
            {
                _floatingObjectContainerPanel.Arrange(rc);
            }

            if (_floatingObjectsMovingResizingContainer != null)
            {
                _floatingObjectsMovingResizingContainer.Arrange(rc);
            }

            RectangleGeometry geometry;
            if (Sheet != null && Sheet.IsTouching)
            {
                if (base.Clip == null)
                {
                    geometry = new RectangleGeometry();
                    geometry.Rect = new Rect(new Point(), viewportSize);
                    base.Clip = geometry;
                }
            }
            else
            {
                geometry = new RectangleGeometry();
                geometry.Rect = new Rect(new Point(), viewportSize);
                base.Clip = geometry;
            }

            if (base.Clip != null)
            {
                geometry = new RectangleGeometry();
                geometry.Rect = Clip.Rect;
                _borderContainer.Clip = geometry;
            }
            return viewportSize;
        }

        void BuildSpanGraph()
        {
            _cachedSpanGraph.Reset();
            SheetSpanModelBase spanModel = GetSpanModel();
            if ((spanModel != null) && !spanModel.IsEmpty())
            {
                int rowStart = -1;
                int rowEnd = -1;
                int columnStart = -1;
                int columnEnd = -1;
                switch (SheetArea)
                {
                    case SheetArea.Cells:
                        rowStart = Sheet.GetViewportTopRow(RowViewportIndex);
                        rowEnd = Sheet.GetViewportBottomRow(RowViewportIndex);
                        columnStart = Sheet.GetViewportLeftColumn(ColumnViewportIndex);
                        columnEnd = Sheet.GetViewportRightColumn(ColumnViewportIndex);
                        break;

                    case (SheetArea.CornerHeader | SheetArea.RowHeader):
                        rowStart = Sheet.GetViewportTopRow(RowViewportIndex);
                        rowEnd = Sheet.GetViewportBottomRow(RowViewportIndex);
                        columnStart = 0;
                        columnEnd = Sheet.Worksheet.RowHeader.ColumnCount - 1;
                        break;

                    case SheetArea.ColumnHeader:
                        rowStart = 0;
                        rowEnd = Sheet.Worksheet.ColumnHeader.RowCount - 1;
                        columnStart = Sheet.GetViewportLeftColumn(ColumnViewportIndex);
                        columnEnd = Sheet.GetViewportRightColumn(ColumnViewportIndex);
                        break;
                }
                if ((rowStart <= rowEnd) && (columnStart <= columnEnd))
                {
                    int num5 = -1;
                    for (int i = rowStart - 1; i > -1; i--)
                    {
                        if (Sheet.Worksheet.GetActualRowVisible(i, SheetArea))
                        {
                            num5 = i;
                            break;
                        }
                    }
                    rowStart = num5;
                    int count = GetDataContext().Rows.Count;
                    for (int j = rowEnd + 1; j < count; j++)
                    {
                        if (Sheet.Worksheet.GetActualRowVisible(j, SheetArea))
                        {
                            rowEnd = j;
                            break;
                        }
                    }
                    int num9 = -1;
                    for (int k = columnStart - 1; k > -1; k--)
                    {
                        if (Sheet.Worksheet.GetActualColumnVisible(k, SheetArea))
                        {
                            num9 = k;
                            break;
                        }
                    }
                    columnStart = num9;
                    int num11 = GetDataContext().Columns.Count;
                    for (int m = columnEnd + 1; m < num11; m++)
                    {
                        if (Sheet.Worksheet.GetActualColumnVisible(m, SheetArea))
                        {
                            columnEnd = m;
                            break;
                        }
                    }
                    _cachedSpanGraph.BuildGraph(columnStart, columnEnd, rowStart, rowEnd, GetSpanModel(), CellCache);
                }
            }
        }

        void AttachEditorForActiveCell()
        {
            if (SheetArea == SheetArea.Cells)
            {
                CellPresenterBase editingCell = GetViewportCell(_activeRow, _activeCol, true);
                if (editingCell != null)
                {
                    if (((_editorPanel == null) || (_editorPanel.EditingColumnIndex != _activeCol)) || ((_editorPanel.EditingRowIndex != _activeRow) || !editingCell.CellType.HasEditingElement()))
                    {
                        if (_editorPanel.Editor != null)
                        {
                            object obj2 = editingCell.SheetView.Worksheet.GetValue(_activeRow, _activeCol);
                            if ((obj2 != null) && string.IsNullOrEmpty((_editorPanel.Editor as TextBox).Text))
                            {
                                (_editorPanel.Editor as TextBox).Text = obj2.ToString();
                                (_editorPanel.Editor as TextBox).SelectionStart = obj2.ToString().Length;
                            }
                            editingCell.CellType.SetEditingElement(_editorPanel.Editor);
                        }
                        else
                        {
                            PrepareCellEditing(editingCell);
                        }
                    }
                    if (_editorPanel != null)
                    {
                        SolidColorBrush brush = null;
                        UIAdaptor.InvokeSync(delegate
                        {
                            brush = new SolidColorBrush(Colors.White);
                        });
                        _editorPanel.SetBackground(brush);
                    }
                }
                if ((editingCell == null) && (_editorPanel != null))
                {
                    SolidColorBrush brush = null;
                    UIAdaptor.InvokeSync(delegate
                    {
                        brush = new SolidColorBrush(Colors.Transparent);
                    });
                    _editorPanel.SetBackground(brush);
                }
                _editorPanel.InvalidateMeasure();
            }
        }
        #endregion

        void BuildSelection()
        {
            _cachedSelectionLayout.Clear();
            UIAdaptor.InvokeAsync(() => SelectionContainer.FocusIndicator.Visibility = Visibility.Collapsed);
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
                            UIAdaptor.InvokeAsync(() => SelectionContainer.FocusIndicator.Visibility = Visibility.Visible);
                        }
                        else
                        {
                            UIAdaptor.InvokeAsync(() => SelectionContainer.FocusIndicator.Visibility = Visibility.Collapsed);
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
                            UIAdaptor.InvokeAsync(() => SelectionContainer.FocusIndicator.Visibility = Visibility.Collapsed);
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
                UIAdaptor.InvokeAsync(() =>
                {
                    SelectionContainer.FocusIndicator.InvalidateMeasure();
                    SelectionContainer.FocusIndicator.InvalidateArrange();
                });
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
            Action action = null;
            if (FloatingObjectsPanel != null)
            {
                if (action == null)
                {
                    action = delegate
                    {
                        FloatingObjectsPanel.InvalidateMeasure();
                    };
                }
                UIAdaptor.InvokeAsync(action);
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
            Action action = null;
            if (SupportSelection)
            {
                if (action == null)
                {
                    action = delegate
                    {
                        SelectionContainer.InvalidateMeasure();
                    };
                }
                UIAdaptor.InvokeAsync(action);
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
            Action action = null;
            if (_editorPanel != null)
            {
                if (action == null)
                {
                    action = delegate
                    {
                        _editorPanel.SetBackground(new SolidColorBrush(Colors.White));
                    };
                }
                UIAdaptor.InvokeSync(action);
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
                if (!base.Children.Contains(_editorPanel))
                {
                    base.Children.Add(_editorPanel);
                }
                if (!DesignMode.DesignModeEnabled && ElementTreeHelper.IsKeyboardFocusWithin(Sheet._host))
                {
                    (_editorPanel.Editor as Control).Focus(FocusState.Programmatic);
                }
            }
            _editorPanel.InvalidateMeasure();
            _editorPanel.InvalidateArrange();
        }

        internal void PrepareCellEditing(int row, int column)
        {
            Action action = null;
            CellPresenterBase editingCell = GetViewportCell(row, column, true);
            if (editingCell != null)
            {
                PrepareCellEditing(editingCell);
            }
            else if (_editorPanel != null)
            {
                if (action == null)
                {
                    action = delegate
                    {
                        _editorPanel.SetBackground(new SolidColorBrush(Colors.Transparent));
                    };
                }
                UIAdaptor.InvokeSync(action);
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
            Action action = null;
            if (FloatingObjectsPanel != null)
            {
                if (action == null)
                {
                    action = delegate
                    {
                        FloatingObjectsPanel.Refresh(parameter);
                    };
                }
                UIAdaptor.InvokeAsync(action);
            }
        }

        internal void RefreshFloatingObjectContainerIsSelected()
        {
            Action action = null;
            if (FloatingObjectsPanel != null)
            {
                if (action == null)
                {
                    action = delegate
                    {
                        FloatingObjectsPanel.RefreshContainerIsSelected();
                    };
                }
                UIAdaptor.InvokeAsync(action);
            }
        }

        internal void RefreshFloatingObjectContainerIsSelected(FloatingObject floatingObject)
        {
            Action action = null;
            if (FloatingObjectsPanel != null)
            {
                if (action == null)
                {
                    action = delegate
                    {
                        FloatingObjectsPanel.RefreshContainerIsSelected(floatingObject);
                    };
                }
                UIAdaptor.InvokeAsync(action);
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
            Action action = null;
            if (FloatingObjectsPanel != null)
            {
                if (action == null)
                {
                    action = delegate
                    {
                        FloatingObjectsPanel.Refresh(null);
                    };
                }
                UIAdaptor.InvokeAsync(action);
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

    internal partial class DataValidationPanel : Panel
    {
        DataValidationListButton _dataValidationListButton;
        DataValidationListButtonInfo _dataValidationListButtonInfo;
        DataValidationInputMessagePopUp _inputMessagePopUp;
        Dictionary<int, Dictionary<int, InvalidDataPresenter>> _presenters;
        DataValidator _validator;
        internal const double DATAVALIDATIONLISTBUTTONWIDTH = 16.0;

        public DataValidationPanel(GcViewport parent)
        {
            Action action = null;
            base.IsHitTestVisible = false;
            if (action == null)
            {
                action = delegate
                {
                    base.Background = new SolidColorBrush(Colors.Transparent);
                };
            }
            UIAdaptor.InvokeSync(action);
            ParentViewport = parent;
        }

        public void AddDataValidationListButtonInfo(DataValidationListButtonInfo info)
        {
            if (info != null)
            {
                _dataValidationListButtonInfo = info;
                if (_dataValidationListButton == null)
                {
                    _dataValidationListButton = new DataValidationListButton();
                    base.Children.Add(_dataValidationListButton);
                }
            }
        }

        public void AddInvalidDataPresenterInfo(InvalidDataPresenterInfo info)
        {
            if (_presenters == null)
            {
                _presenters = new Dictionary<int, Dictionary<int, InvalidDataPresenter>>();
            }
            Dictionary<int, InvalidDataPresenter> dictionary = null;
            if (!_presenters.TryGetValue(info.Row, out dictionary))
            {
                dictionary = new Dictionary<int, InvalidDataPresenter>();
                _presenters[info.Row] = dictionary;
            }
            if (!dictionary.ContainsKey(info.Column))
            {
                InvalidDataPresenter presenter = new InvalidDataPresenter();
                base.Children.Add(presenter);
                dictionary[info.Column] = presenter;
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if ((ParentViewport._activeRow >= 0) && (ParentViewport._activeCol >= 0))
            {
                Rect rangeBounds = ParentViewport.GetRangeBounds(new CellRange(ParentViewport._activeRow, ParentViewport._activeCol, 1, 1));
                double height = rangeBounds.Height;
                double width = rangeBounds.Width;
                double x = rangeBounds.X;
                double y = rangeBounds.Y;
                if (double.IsInfinity(rangeBounds.Height))
                {
                    height = 0.0;
                    y = 0.0;
                }
                if (double.IsInfinity(rangeBounds.Width))
                {
                    width = 0.0;
                    x = 0.0;
                }
                if ((_dataValidationListButton != null) && (_dataValidationListButtonInfo != null))
                {
                    _dataValidationListButton.VerticalAlignment = VerticalAlignment.Bottom;
                    if (_dataValidationListButtonInfo.Column == _dataValidationListButtonInfo.DisplayColumn)
                    {
                        _dataValidationListButton.HorizontalAlignment = HorizontalAlignment.Right;
                        Rect rect2 = new Rect(x - 1.0, y - 1.0, width, height);
                        _dataValidationListButton.Arrange(rect2);
                    }
                    else
                    {
                        _dataValidationListButton.HorizontalAlignment = HorizontalAlignment.Left;
                        Rect rect3 = new Rect((x + width) + 1.0, y - 1.0, 16.0, height);
                        _dataValidationListButton.Arrange(rect3);
                    }
                }
                if (_inputMessagePopUp != null)
                {
                    if (!double.IsInfinity(rangeBounds.Height))
                    {
                        y += rangeBounds.Height + 5.0;
                    }
                    if (!double.IsInfinity(rangeBounds.Width))
                    {
                        x += rangeBounds.Width / 2.0;
                    }
                    Size size = (_inputMessagePopUp.Content as Grid).DesiredSize;
                    Rect rect4 = new Rect(x, y, ((size.Width + (2.0 * (_inputMessagePopUp.Padding.Left + _inputMessagePopUp.Padding.Right))) + _inputMessagePopUp.BorderThickness.Left) + _inputMessagePopUp.BorderThickness.Right, ((size.Height + (2.0 * (_inputMessagePopUp.Padding.Bottom + _inputMessagePopUp.Padding.Top))) + _inputMessagePopUp.BorderThickness.Top) + _inputMessagePopUp.BorderThickness.Bottom);
                    _inputMessagePopUp.Arrange(rect4);
                }
                if (_presenters != null)
                {
                    foreach (KeyValuePair<int, Dictionary<int, InvalidDataPresenter>> pair in _presenters)
                    {
                        foreach (KeyValuePair<int, InvalidDataPresenter> pair2 in pair.Value)
                        {
                            if ((pair2.Value != null) && (ParentViewport.GetCellLayoutModel() != null))
                            {
                                InvalidDataPresenter presenter = pair2.Value;
                                rangeBounds = ParentViewport.GetRangeBounds(new CellRange(pair.Key, pair2.Key, 1, 1));
                                height = rangeBounds.Height;
                                width = rangeBounds.Width;
                                x = rangeBounds.X;
                                y = rangeBounds.Y;
                                if (double.IsInfinity(rangeBounds.Height))
                                {
                                    height = 0.0;
                                    y = 0.0;
                                }
                                if (double.IsInfinity(rangeBounds.Width))
                                {
                                    width = 0.0;
                                    x = 0.0;
                                }
                                presenter.Height = height;
                                presenter.Width = width;
                                Rect rect5 = new Rect(x - 1.0, y - 1.0, width, height);
                                presenter.Arrange(rect5);
                            }
                        }
                    }
                }
            }
            return base.ArrangeOverride(finalSize);
        }

        public void CloseInputMessageToolTip()
        {
            if ((_inputMessagePopUp != null) && base.Children.Contains(_inputMessagePopUp))
            {
                base.Children.Remove(_inputMessagePopUp);
                _inputMessagePopUp = null;
                _validator = null;
                base.InvalidateMeasure();
            }
        }

        Grid CreateDataValidationInputMessage(DataValidator validator)
        {
            Grid grid = new Grid
            {
                RowDefinitions = { new RowDefinition() }
            };
            TextBlock block4 = new TextBlock();
            block4.Text = validator.InputMessage;
            TextBlock element = block4;
            element.TextWrapping = TextWrapping.Wrap;
            element.MaxWidth = 240.0;
            grid.Children.Add(element);
            if (!string.IsNullOrEmpty(validator.InputTitle))
            {
                grid.RowDefinitions.Add(new RowDefinition());
                TextBlock block3 = new TextBlock();
                block3.Text = validator.InputTitle;
                block3.FontWeight = FontWeights.Bold;
                TextBlock block2 = block3;
                grid.Children.Add(block2);
                Grid.SetRow(element, 1);
            }
            return grid;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (_presenters != null)
            {
                if (ParentViewport.Sheet.HighlightInvalidData)
                {
                    using (Dictionary<int, Dictionary<int, InvalidDataPresenter>>.Enumerator enumerator = _presenters.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            using (Dictionary<int, InvalidDataPresenter>.Enumerator enumerator2 = enumerator.Current.Value.GetEnumerator())
                            {
                                while (enumerator2.MoveNext())
                                {
                                    enumerator2.Current.Value.Measure(availableSize);
                                }
                                continue;
                            }
                        }
                        goto Label_010D;
                    }
                }
                using (Dictionary<int, Dictionary<int, InvalidDataPresenter>>.Enumerator enumerator3 = _presenters.GetEnumerator())
                {
                    while (enumerator3.MoveNext())
                    {
                        foreach (KeyValuePair<int, InvalidDataPresenter> pair4 in enumerator3.Current.Value)
                        {
                            base.Children.Remove(pair4.Value);
                        }
                    }
                }
                _presenters.Clear();
            }
        Label_010D:
            if (((_inputMessagePopUp != null) && (_inputMessagePopUp.Content != null)) && (_validator != null))
            {
                _inputMessagePopUp.Measure(availableSize);
            }
            if ((_dataValidationListButton != null) && (_dataValidationListButtonInfo != null))
            {
                Rect rangeBounds = ParentViewport.GetRangeBounds(new CellRange(ParentViewport._activeRow, ParentViewport._activeCol, 1, 1));
                double height = rangeBounds.Height;
                double width = rangeBounds.Width;
                if (double.IsInfinity(rangeBounds.Height))
                {
                    height = 0.0;
                }
                if (double.IsInfinity(rangeBounds.Width))
                {
                    width = 0.0;
                }
                Size size = new Size(width, height);
                if (_dataValidationListButtonInfo.Column != _dataValidationListButtonInfo.DisplayColumn)
                {
                    size = new Size(16.0, height);
                }
                _dataValidationListButton.Measure(size);
            }
            return base.MeasureOverride(availableSize);
        }

        public void RemoveDataValidationListButtonInfo()
        {
            if ((_dataValidationListButton != null) && base.Children.Contains(_dataValidationListButton))
            {
                base.Children.Remove(_dataValidationListButton);
            }
            _dataValidationListButton = null;
            _dataValidationListButtonInfo = null;
        }

        public void RemoveInvalidDataPresenterInfo(InvalidDataPresenterInfo info)
        {
            if ((_presenters != null) && _presenters.ContainsKey(info.Row))
            {
                Dictionary<int, InvalidDataPresenter> dictionary = _presenters[info.Row];
                if ((dictionary != null) && dictionary.ContainsKey(info.Column))
                {
                    InvalidDataPresenter presenter = dictionary[info.Column];
                    if (dictionary.Remove(info.Column))
                    {
                        base.Children.Remove(presenter);
                    }
                }
            }
        }

        public void ShowInputMessageToolTip(DataValidator validator)
        {
            if ((validator != null) && !string.IsNullOrEmpty(validator.InputMessage))
            {
                _validator = validator;
                _inputMessagePopUp = new DataValidationInputMessagePopUp();
                _inputMessagePopUp.Content = CreateDataValidationInputMessage(validator);
                base.Children.Add(_inputMessagePopUp);
                base.InvalidateMeasure();
            }
        }

        internal GcViewport ParentViewport { get; set; }
    }

    internal partial class DragFillContainerPanel : Panel
    {
        Rectangle _dragClearRectangle;
        DragFillFrame _dragFillFrame;

        public DragFillContainerPanel()
        {
            Action action = null;
            _dragFillFrame = new DragFillFrame();
            base.Children.Add(_dragFillFrame);
            if (action == null)
            {
                action = delegate
                {
                    Rectangle rectangle = new Rectangle();
                    rectangle.Fill = new SolidColorBrush(Color.FromArgb(200, 110, 110, 110));
                    _dragClearRectangle = rectangle;
                };
            }
            UIAdaptor.InvokeSync(action);
            base.Children.Add(_dragClearRectangle);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (DragFillFrame != null)
            {
                Rect rect = ParentViewport._cachedDragFillFrameRect;
                if ((rect.Width > 0.0) && (rect.Height > 0.0))
                {
                    DragFillFrame.Visibility = Visibility.Visible;
                    DragFillFrame.Arrange(rect);
                    DragFillFrame.InvalidateArrange();
                }
                else
                {
                    DragFillFrame.Visibility = Visibility.Collapsed;
                }
            }
            if (DragClearRectangle != null)
            {
                Rect rect2 = ParentViewport._cachedDragClearRect;
                if ((rect2.Width > 0.0) && (rect2.Height > 0.0))
                {
                    DragClearRectangle.Visibility = Visibility.Visible;
                    DragClearRectangle.Arrange(rect2);
                    DragClearRectangle.InvalidateArrange();
                    return finalSize;
                }
                DragClearRectangle.Visibility = Visibility.Collapsed;
            }
            return finalSize;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (DragFillFrame != null)
            {
                Rect rect = ParentViewport._cachedDragFillFrameRect;
                if ((rect.Width > 0.0) && (rect.Height > 0.0))
                {
                    DragFillFrame.Visibility = Visibility.Visible;
                    DragFillFrame.Measure(new Size(rect.Width, rect.Height));
                }
                else
                {
                    DragFillFrame.Visibility = Visibility.Collapsed;
                }
            }
            if (DragClearRectangle != null)
            {
                Rect rect2 = ParentViewport._cachedDragClearRect;
                if ((rect2.Width > 0.0) && (rect2.Height > 0.0))
                {
                    DragClearRectangle.Visibility = Visibility.Visible;
                    DragClearRectangle.Measure(new Size(rect2.Width, rect2.Height));
                }
                else
                {
                    DragClearRectangle.Visibility = Visibility.Collapsed;
                }
            }
            return ParentViewport.GetViewportSize(availableSize);
        }

        public Rectangle DragClearRectangle
        {
            get { return _dragClearRectangle; }
        }

        public DragFillFrame DragFillFrame
        {
            get { return _dragFillFrame; }
        }

        public GcViewport ParentViewport { get; set; }
    }

    internal partial class EditingPanel : Panel
    {
        Panel _backgroundPanel;
        CellPresenterBase _editingCell;
        FrameworkElement _editor1;
        FrameworkElement _editor2;

        public event EventHandler EditingChanged;

        public EditingPanel(GcViewport parent)
        {
            ParentViewport = parent;
            UIAdaptor.InvokeSync(() => { Background = new SolidColorBrush(Colors.White); });
            _backgroundPanel = new Canvas();
            Children.Add(_backgroundPanel);
            _backgroundPanel.IsHitTestVisible = false;
            IsHitTestVisible = false;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if ((_editingCell != null) && (ParentViewport != null))
            {
                Rect rect = ParentViewport.CalcEditorBounds(EditingRowIndex, EditingColumnIndex, finalSize);
                double left = rect.Left;
                HorizontalAlignment alignment = HorizontalAlignment.Left;
                if (_editingCell.BindingCell != null)
                {
                    float num2 = _editingCell.BindingCell.ActualTextIndent * ParentViewport.Sheet.ZoomFactor;
                    alignment = _editingCell.BindingCell.ToHorizontalAlignment();
                    if (_editingCell.BindingCell.ActualTextIndent > 0)
                    {
                        switch (alignment)
                        {
                            case HorizontalAlignment.Left:
                                left += num2;
                                break;

                            case HorizontalAlignment.Right:
                                left -= num2;
                                break;
                        }
                    }
                }
                Rect rect2 = new Rect(left, rect.Top - 1.0, (rect.Width >= 2.0) ? (rect.Width - 2.0) : 0.0, rect.Height + 1.0);
                if (_editingCell.BindingCell.ActualVerticalAlignment == CellVerticalAlignment.Top)
                {
                    rect2 = new Rect(left, rect.Top - 2.0, (rect.Width >= 2.0) ? (rect.Width - 2.0) : 0.0, rect.Height + 2.0);
                }
                else if (_editingCell.BindingCell.ActualVerticalAlignment == CellVerticalAlignment.Bottom)
                {
                    rect2 = new Rect(left, rect.Top, (rect.Width >= 2.0) ? (rect.Width - 2.0) : 0.0, rect.Height);
                }
                double x = rect2.X;
                double y = rect2.Y;
                IsEditorVisible = (rect2.Width > 0.0) && (rect2.Height > 0.0);
                RectangleGeometry geometry = new RectangleGeometry();
                geometry.Rect = new Rect(rect.X, rect.Y, rect.Width, rect.Height);
                base.Clip = geometry;
                if (_backgroundPanel != null)
                {
                    _backgroundPanel.Width = rect2.Width + 3.0;
                    _backgroundPanel.Height = rect2.Height;
                    _backgroundPanel.Arrange(new Rect(rect2.X - 1.0, rect2.Y, rect2.Width + 2.0, rect2.Height));
                }
                if (Editor != null)
                {
                    if ((rect2.Width > 1.0) && (rect2.Height > 1.0))
                    {
                        if (_editor1 != null)
                        {
                            _editor1.Width = rect2.Width;
                            _editor1.Height = rect2.Height;
                        }
                        if (_editor2 != null)
                        {
                            _editor2.Width = rect2.Width;
                            _editor2.Height = rect2.Height;
                        }
                    }
                    if ((_editor1 != null) && (_editor1.Visibility == Visibility.Visible))
                    {
                        _editor1.Arrange(new Rect(rect2.X, rect2.Y, rect2.Width, rect2.Height));
                    }
                    if ((_editor2 != null) && (_editor2.Visibility == Visibility.Visible))
                    {
                        _editor2.Arrange(new Rect(rect2.X, rect2.Y, rect2.Width, rect2.Height));
                    }
                }
            }
            return base.ArrangeOverride(finalSize);
        }

        void EditorTextChanged(object sender, TextChangedEventArgs e)
        {
            if ((ParentViewport != null) && ParentViewport.IsEditing())
            {
                EditorDirty = true;
                if (EditingChanged != null)
                {
                    EditingChanged(this, EventArgs.Empty);
                }
            }
        }

        internal FrameworkElement GetAvaiableEditor()
        {
            object focusedElement = GetFocusedElement();
            if (object.ReferenceEquals(Editor, _editor1))
            {
                if (_editor2 != null)
                {
                    _editor2.Visibility = Visibility.Visible;
                    if ((focusedElement == _editor1) && (_editor2 != null))
                    {
                        (_editor2 as TextBox).Focus(FocusState.Programmatic);
                    }
                    if (_editor2 is EditingElement)
                    {
                        (_editor2 as EditingElement).Status = EditorStatus.Ready;
                    }
                }
                if (_editor1 != null)
                {
                    if ((((focusedElement == _editor1) && !object.ReferenceEquals(GetFocusedElement(), _editor2)) && ((ParentViewport != null) && (ParentViewport.Sheet != null))) && (ParentViewport.Sheet._host != null))
                    {
                        ParentViewport.Sheet._host.Focus(FocusState.Programmatic);
                    }
                    _editor1.Visibility = Visibility.Collapsed;
                    if (_editor1 is TextBox)
                    {
                        (_editor1 as TextBox).Text = string.Empty;
                    }
                    if (_editor1 is EditingElement)
                    {
                        (_editor1 as EditingElement).Status = EditorStatus.Ready;
                    }
                }
                return _editor2;
            }
            if (!object.ReferenceEquals(Editor, _editor2))
            {
                return Editor;
            }
            if (_editor1 != null)
            {
                _editor1.Visibility = Visibility.Visible;
                if ((focusedElement == _editor2) && (_editor1 != null))
                {
                    (_editor1 as TextBox).Focus(FocusState.Programmatic);
                }
                if (_editor1 is EditingElement)
                {
                    (_editor1 as EditingElement).Status = EditorStatus.Ready;
                }
            }
            if (_editor2 != null)
            {
                _editor2.Visibility = Visibility.Collapsed;
                if (_editor2 is TextBox)
                {
                    (_editor2 as TextBox).Text = string.Empty;
                }
                if (_editor2 is EditingElement)
                {
                    (_editor2 as EditingElement).Status = EditorStatus.Ready;
                }
            }
            return _editor1;
        }

        object GetFocusedElement()
        {
            return FocusManager.GetFocusedElement();
        }

        public void InstallEditor(CellPresenterBase cell, bool startEditing = false)
        {
            if (cell == null)
                return;

            ICellType cellType = cell.CellType;
            FrameworkElement editingElement = cell.GetEditingElement();
            int row = cell.Row;
            int column = cell.Column;
            if (cell.CellLayout != null)
            {
                row = cell.CellLayout.Row;
                column = cell.CellLayout.Column;
            }
            _editingCell = cell;
            EditorDirty = false;
            if (!object.ReferenceEquals(editingElement, _editor2) && (_editor1 == null))
            {
                _editor1 = editingElement;
            }
            else if (!object.ReferenceEquals(editingElement, _editor1) && (_editor2 == null))
            {
                _editor2 = editingElement;
            }
            Editor = editingElement;
            Editor.Visibility = Visibility.Visible;
            if (Editor != null)
            {
                if (!Children.Contains(Editor))
                {
                    Children.Add(Editor);
                }
                EditingColumnIndex = column;
                EditingRowIndex = row;
                if (cellType.DataContext.ActualBackground != null)
                {
                    _backgroundPanel.Background = cellType.DataContext.ActualBackground;
                }
                else
                {
                    UIAdaptor.InvokeSync(() => { _backgroundPanel.Background = new SolidColorBrush(Colors.Transparent); });
                }
                TextBox editor = Editor as TextBox;
                editor.IsHitTestVisible = false;
                // hdt
                Worksheet ws = ParentViewport.Sheet.Worksheet;
                if (ws != null && ws.LockCell)
                    editor.IsEnabled = false;
                else if (!editor.IsEnabled)
                    editor.IsEnabled = true;

                if (IsHitTestVisible)
                    IsHitTestVisible = false;
                UpateScrollViewSize(editor);
                editor.SelectAll();
                if (editor != null)
                {
                    editor.TextChanged -= EditorTextChanged;
                    editor.TextChanged += EditorTextChanged;
                }
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (_editor1 != null)
            {
                _editor1.Measure(availableSize);
            }
            if (_editor2 != null)
            {
                _editor2.Measure(availableSize);
            }
            if (_backgroundPanel != null)
            {
                _backgroundPanel.Measure(availableSize);
            }
            return ParentViewport.GetViewportSize(availableSize);
        }

        public void ResumeEditor()
        {
            if (_editingCell != null)
            {
                CellPresenterBase objA = ParentViewport.GetViewportCell(EditingRowIndex, EditingColumnIndex, true);
                if (objA != null)
                {
                    if (!object.Equals(objA, _editingCell))
                    {
                        Control editor = Editor as Control;
                        if (editor != null)
                        {
                            editor.Focus(FocusState.Programmatic);
                        }
                        _editingCell = objA;
                    }
                    objA.HideForEditing();
                }
            }
        }

        public void SetBackground(Brush brush)
        {
            base.Background = brush;
        }

        internal void SetEditorStatus(EditorStatus status)
        {
            if (Editor is EditingElement)
            {
                (Editor as EditingElement).Status = status;
            }
        }

        internal void UpadateEditor()
        {
            bool isWrap = false;
            if ((_editingCell != null) && (ParentViewport != null))
            {
                Size viewportSize = ParentViewport.GetViewportSize();
                CellPresenterBase base2 = ParentViewport.GetViewportCell(EditingRowIndex, EditingColumnIndex, true);
                if ((_editingCell != null) && (ParentViewport._editorPanel != null))
                {
                    Rect rect = ParentViewport.GetCellBounds(EditingRowIndex, EditingColumnIndex, false);
                    Size cellContentSize = new Size(rect.Width, rect.Height);
                    double height = viewportSize.Height - (rect.Top - ParentViewport.Location.Y);
                    if ((rect.Width != 0.0) && (rect.Height != 0.0))
                    {
                        Cell cachedCell = ParentViewport.CellCache.GetCachedCell(EditingRowIndex, EditingColumnIndex);
                        HorizontalAlignment alignment = cachedCell.ToHorizontalAlignment();
                        switch (alignment)
                        {
                            case HorizontalAlignment.Left:
                                {
                                    float indent = cachedCell.ActualTextIndent * ParentViewport.Sheet.ZoomFactor;
                                    double num3 = (viewportSize.Width - rect.Left) + ParentViewport.Location.X;
                                    num3 = Math.Max(Math.Min(num3, viewportSize.Width), 0.0);
                                    Size maxSize = new Size(num3, height);
                                    isWrap = base2.JudgeWordWrap(maxSize, cellContentSize, alignment, indent);
                                    goto Label_02B5;
                                }
                            case HorizontalAlignment.Right:
                                {
                                    float num4 = cachedCell.ActualTextIndent * ParentViewport.Sheet.ZoomFactor;
                                    double num5 = rect.Right - ParentViewport.Location.X;
                                    num5 = Math.Max(Math.Min(num5, viewportSize.Width), 0.0);
                                    Size size4 = new Size(num5, height);
                                    isWrap = _editingCell.JudgeWordWrap(size4, cellContentSize, alignment, num4);
                                    goto Label_02B5;
                                }
                        }
                        if (alignment == HorizontalAlignment.Center)
                        {
                            double num6 = (rect.Left - ParentViewport.Location.X) + (rect.Width / 2.0);
                            if (num6 < 0.0)
                            {
                                num6 = 0.0;
                            }
                            double num7 = viewportSize.Width - num6;
                            if (num7 < 0.0)
                            {
                                num7 = 0.0;
                            }
                            double width = 2.0 * Math.Min(num6, num7);
                            Size size5 = new Size(width, height);
                            isWrap = _editingCell.JudgeWordWrap(size5, cellContentSize, alignment, 0f);
                        }
                    }
                }
            }
        Label_02B5:
            if (Editor != null)
            {
                if (_editor1 != null)
                {
                    UpdateEditingElement(_editor1, isWrap);
                }
                if (_editor2 != null)
                {
                    UpdateEditingElement(_editor2, isWrap);
                }
            }
        }

        void UpateScrollViewSize(TextBox tb)
        {
            string text = tb.Text;
            tb.Text = "Text";
            tb.Text = text;
        }

        public void Update(CellPresenterBase cell)
        {
            Action action = null;
            int row = cell.Row;
            int column = cell.Column;
            if (cell.CellLayout != null)
            {
                row = cell.CellLayout.Row;
                column = cell.CellLayout.Column;
            }
            _editingCell = cell;
            EditorDirty = false;
            EditingColumnIndex = column;
            EditingRowIndex = row;
            if (cell.CellType.DataContext.ActualBackground != null)
            {
                _backgroundPanel.Background = cell.CellType.DataContext.ActualBackground;
            }
            else
            {
                if (action == null)
                {
                    action = delegate
                    {
                        _backgroundPanel.Background = new SolidColorBrush(Colors.Transparent);
                    };
                }
                UIAdaptor.InvokeSync(action);
            }
        }

        void UpdateEditingElement(FrameworkElement editElement, bool isWrap)
        {
            EditingElement element = editElement as EditingElement;
            if (element != null)
            {
                element.TextWrapping = isWrap ? TextWrapping.Wrap : TextWrapping.NoWrap;
            }
        }

        public int EditingColumnIndex { get; private set; }

        public int EditingRowIndex { get; private set; }

        public FrameworkElement Editor { get; private set; }

        public bool EditorDirty { get; set; }

        internal EditorStatus EditorStatus
        {
            get
            {
                if ((Editor != null) && (Editor is EditingElement))
                {
                    return (Editor as EditingElement).Status;
                }
                return EditorStatus.Ready;
            }
        }

        public bool IsEditorVisible { get; set; }

        public GcViewport ParentViewport { get; set; }
    }

    internal partial class FormulaSelectionContainerPanel : Panel
    {
        DispatcherTimer _timer;

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (FormulaSelectionFrame frame in base.Children)
            {
                CellRange range = frame.SelectionItem.Range;
                int row = (range.Row < 0) ? 0 : range.Row;
                int column = (range.Column < 0) ? 0 : range.Column;
                int rowCount = (range.RowCount < 0) ? ParentViewport.Sheet.Worksheet.RowCount : range.RowCount;
                int columnCount = (range.ColumnCount < 0) ? ParentViewport.Sheet.Worksheet.ColumnCount : range.ColumnCount;
                bool isLeftVisible = false;
                bool isTopVisible = false;
                bool isRightVisible = false;
                bool isBottomVisible = false;
                Rect rect = ParentViewport.GetRangeBounds(new CellRange(row, column, rowCount, columnCount), out isLeftVisible, out isRightVisible, out isTopVisible, out isBottomVisible);
                if ((rect.Width > 0.0) && (rect.Height > 0.0))
                {
                    rect.X--;
                    rect.Y--;
                    rect.Width++;
                    rect.Height++;
                }
                if ((rect.Width > 0.0) && (rect.Height > 0.0))
                {
                    frame.Arrange(rect);
                }
                else
                {
                    frame.Arrange(new Rect(0.0, 0.0, 0.0, 0.0));
                }
            }
            return base.ArrangeOverride(finalSize);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (FormulaSelectionFrame frame in base.Children)
            {
                CellRange range = frame.SelectionItem.Range;
                int row = (range.Row < 0) ? 0 : range.Row;
                int column = (range.Column < 0) ? 0 : range.Column;
                int rowCount = (range.RowCount < 0) ? ParentViewport.Sheet.Worksheet.RowCount : range.RowCount;
                int columnCount = (range.ColumnCount < 0) ? ParentViewport.Sheet.Worksheet.ColumnCount : range.ColumnCount;
                bool isLeftVisible = false;
                bool isTopVisible = false;
                bool isRightVisible = false;
                bool isBottomVisible = false;
                Rect rect = ParentViewport.GetRangeBounds(new CellRange(row, column, rowCount, columnCount), out isLeftVisible, out isRightVisible, out isTopVisible, out isBottomVisible);
                frame.IsLeftVisible = isLeftVisible;
                frame.IsRightVisible = isRightVisible;
                frame.IsTopVisible = isTopVisible;
                frame.IsBottomVisible = isBottomVisible;
                if ((rect.IsEmpty || (rect.Width == 0.0)) || (rect.Height == 0.0))
                {
                    frame.Visibility = Visibility.Collapsed;
                }
                else
                {
                    frame.Visibility = Visibility.Visible;
                }
            }
            foreach (FrameworkElement element in base.Children)
            {
                element.Measure(availableSize);
            }
            return base.MeasureOverride(availableSize);
        }

        public void Refresh()
        {
            base.Children.Clear();
            using (IEnumerator<FormulaSelectionItem> enumerator = ParentViewport.Sheet.FormulaSelections.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    FormulaSelectionFrame frame = new FormulaSelectionFrame(enumerator.Current);
                    base.Children.Add(frame);
                }
            }
            if (base.Children.Count > 0)
            {
                StartTimer();
            }
            else
            {
                EndTimer();
            }
        }

        void StartTimer()
        {
            if (_timer == null)
            {
                _timer = new DispatcherTimer();
                _timer.Interval = new TimeSpan(0, 0, 0, 0, 200);
                _timer.Tick += TimerTick;
            }
            _timer.Start();
        }

        void TimerTick(object sender, object e)
        {
            foreach (FormulaSelectionFrame frame in base.Children)
            {
                frame.OnTick();
            }
        }

        void EndTimer()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer = null;
            }
        }

        public GcViewport ParentViewport { get; set; }
    }

    internal partial class FormulaSelectionFrame : Panel
    {
        Line _bottom;
        Rectangle _bottomBackground;
        bool _canChangeBoundsByUI = true;
        bool _flag;
        Line _innerBottom;
        Line _innerLeft;
        Line _innerRight;
        Line _innerTop;
        bool _isBottomVisible;
        bool _isFlickering = true;
        bool _isLeftVisible;
        bool _isMouseOvering;
        bool _isRightVisible;
        bool _isTopVisible;
        Line _left;
        Rectangle _leftBackground;
        Rectangle _leftBottom;
        Rectangle _leftTop;
        Line _right;
        Rectangle _rightBackground;
        Rectangle _rightBottom;
        Rectangle _rightTop;
        FormulaSelectionItem _selectionItem;
        Line _top;
        Rectangle _topBackground;

        public FormulaSelectionFrame(FormulaSelectionItem selectionItem)
        {
            CreateInnerBorders(selectionItem);
            CreateBackground(selectionItem);
            CreateCorners(selectionItem);
            CreateOutterBorders(selectionItem);
            _selectionItem = selectionItem;
            _selectionItem.PropertyChanged += new PropertyChangedEventHandler(SelectionItemPropertyChanged);
            _isFlickering = selectionItem.IsFlickering;
            _canChangeBoundsByUI = selectionItem.CanChangeBoundsByUI;
            UpdateVisibilities();
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (_isMouseOvering)
            {
                _innerLeft.Y2 = finalSize.Height;
                _innerLeft.Arrange(new Rect(0.0, 0.0, 1.0, finalSize.Height));
                _innerRight.Y2 = finalSize.Height;
                _innerRight.Arrange(new Rect(finalSize.Width - 1.0, 0.0, 1.0, finalSize.Height));
                _innerTop.X2 = finalSize.Width;
                _innerTop.Arrange(new Rect(0.0, 0.0, finalSize.Width, 1.0));
                _innerBottom.X2 = finalSize.Width;
                _innerBottom.Arrange(new Rect(0.0, finalSize.Height - 1.0, finalSize.Width, 1.0));
                _leftBackground.Arrange(new Rect(1.0, 0.0, 2.0, finalSize.Height));
                _rightBackground.Arrange(new Rect(finalSize.Width - 3.0, 0.0, 2.0, finalSize.Height));
                _topBackground.Arrange(new Rect(0.0, 1.0, finalSize.Width, 2.0));
                _bottomBackground.Arrange(new Rect(0.0, finalSize.Height - 3.0, finalSize.Width, 2.0));
            }
            else
            {
                _innerLeft.Y2 = finalSize.Height;
                _innerLeft.Arrange(new Rect(1.0, 0.0, 1.0, finalSize.Height));
                _innerRight.Y2 = finalSize.Height;
                _innerRight.Arrange(new Rect(finalSize.Width - 2.0, 0.0, 1.0, finalSize.Height));
                _innerTop.X2 = finalSize.Width;
                _innerTop.Arrange(new Rect(0.0, 1.0, finalSize.Width, 1.0));
                _innerBottom.X2 = finalSize.Width;
                _innerBottom.Arrange(new Rect(0.0, finalSize.Height - 2.0, finalSize.Width, 1.0));
                _leftBackground.Arrange(new Rect(0.0, 0.0, 1.0, finalSize.Height));
                _rightBackground.Arrange(new Rect(finalSize.Width - 1.0, 0.0, 1.0, finalSize.Height));
                _topBackground.Arrange(new Rect(0.0, 0.0, finalSize.Width, 1.0));
                _bottomBackground.Arrange(new Rect(0.0, finalSize.Height - 1.0, finalSize.Width, 1.0));
            }
            double width = 5.0;
            double height = 5.0;
            _leftTop.Arrange(new Rect(-1.0, -1.0, width, height));
            _rightTop.Arrange(new Rect((finalSize.Width - width) + 1.0, -1.0, width, height));
            _leftBottom.Arrange(new Rect(-1.0, (finalSize.Height - height) + 1.0, width, height));
            _rightBottom.Arrange(new Rect((finalSize.Width - width) + 1.0, (finalSize.Height - height) + 1.0, width, height));
            if (_isMouseOvering)
            {
                _left.Y2 = finalSize.Height;
                _left.Arrange(new Rect(1.0, 0.0, 1.0, finalSize.Height));
                _right.Y2 = finalSize.Height;
                _right.Arrange(new Rect(finalSize.Width - 2.0, 0.0, 1.0, finalSize.Height));
                _top.X2 = finalSize.Width;
                _top.Arrange(new Rect(0.0, 1.0, finalSize.Width, 1.0));
                _bottom.X2 = finalSize.Width;
                _bottom.Arrange(new Rect(0.0, finalSize.Height - 2.0, finalSize.Width, 1.0));
            }
            else
            {
                _left.Y2 = finalSize.Height;
                _left.Arrange(new Rect(0.0, 0.0, 1.0, finalSize.Height));
                _right.Y2 = finalSize.Height;
                _right.Arrange(new Rect(finalSize.Width - 1.0, 0.0, 1.0, finalSize.Height));
                _top.X2 = finalSize.Width;
                _top.Arrange(new Rect(0.0, 0.0, finalSize.Width, 1.0));
                _bottom.X2 = finalSize.Width;
                _bottom.Arrange(new Rect(0.0, finalSize.Height - 1.0, finalSize.Width, 1.0));
            }
            return base.ArrangeOverride(finalSize);
        }

        void CreateBackground(FormulaSelectionItem selectionItem)
        {
            SolidColorBrush brush = new SolidColorBrush(selectionItem.Color);
            Rectangle rectangle = new Rectangle();
            rectangle.Width = 1.0;
            rectangle.Height = base.Height;
            rectangle.Fill = brush;
            _leftBackground = rectangle;
            Rectangle rectangle2 = new Rectangle();
            rectangle2.Width = 1.0;
            rectangle2.Height = base.Height;
            rectangle2.Fill = brush;
            _rightBackground = rectangle2;
            Rectangle rectangle3 = new Rectangle();
            rectangle3.Width = base.Width;
            rectangle3.Height = 1.0;
            rectangle3.Fill = brush;
            _topBackground = rectangle3;
            Rectangle rectangle4 = new Rectangle();
            rectangle4.Width = base.Width;
            rectangle4.Height = 1.0;
            rectangle4.Fill = brush;
            _bottomBackground = rectangle4;
            base.Children.Add(_leftBackground);
            base.Children.Add(_rightBackground);
            base.Children.Add(_topBackground);
            base.Children.Add(_bottomBackground);
        }

        void CreateCorners(FormulaSelectionItem selectionItem)
        {
            SolidColorBrush brush = new SolidColorBrush(selectionItem.Color);
            Rectangle rectangle = new Rectangle();
            rectangle.Width = 5.0;
            rectangle.Height = 5.0;
            rectangle.Fill = brush;
            _leftTop = rectangle;
            Rectangle rectangle2 = new Rectangle();
            rectangle2.Width = 5.0;
            rectangle2.Height = 5.0;
            rectangle2.Fill = brush;
            _rightTop = rectangle2;
            Rectangle rectangle3 = new Rectangle();
            rectangle3.Width = 5.0;
            rectangle3.Height = 5.0;
            rectangle3.Fill = brush;
            _leftBottom = rectangle3;
            Rectangle rectangle4 = new Rectangle();
            rectangle4.Width = 5.0;
            rectangle4.Height = 5.0;
            rectangle4.Fill = brush;
            _rightBottom = rectangle4;
            base.Children.Add(_leftTop);
            base.Children.Add(_rightTop);
            base.Children.Add(_leftBottom);
            base.Children.Add(_rightBottom);
        }

        DoubleCollection CreateDoubleCollection()
        {
            return new DoubleCollection { 1.0, 1.0 };
        }

        void CreateInnerBorders(FormulaSelectionItem selectionItem)
        {
            SolidColorBrush brush = new SolidColorBrush(Colors.Black);
            Line line = new Line();
            line.Stroke = brush;
            line.StrokeThickness = 4.0;
            line.StrokeDashArray = CreateDoubleCollection();
            line.StrokeDashOffset = -0.25;
            _innerLeft = line;
            Line line2 = new Line();
            line2.Stroke = brush;
            line2.StrokeThickness = 4.0;
            line2.StrokeDashArray = CreateDoubleCollection();
            line2.StrokeDashOffset = 0.25;
            _innerRight = line2;
            Line line3 = new Line();
            line3.Stroke = brush;
            line3.StrokeThickness = 4.0;
            line3.StrokeDashArray = CreateDoubleCollection();
            line3.StrokeDashOffset = -0.25;
            _innerTop = line3;
            Line line4 = new Line();
            line4.Stroke = brush;
            line4.StrokeThickness = 4.0;
            line4.StrokeDashArray = CreateDoubleCollection();
            line4.StrokeDashOffset = 0.25;
            _innerBottom = line4;
            base.Children.Add(_innerLeft);
            base.Children.Add(_innerRight);
            base.Children.Add(_innerTop);
            base.Children.Add(_innerBottom);
        }

        void CreateOutterBorders(FormulaSelectionItem selectionItem)
        {
            Color color = selectionItem.Color;
            SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(0xff, (byte)(0xff - color.R), (byte)(0xff - color.G), (byte)(0xff - color.B)));
            Line line = new Line();
            line.Stroke = brush;
            line.StrokeThickness = 4.0;
            line.StrokeDashArray = CreateDoubleCollection();
            _left = line;
            Line line2 = new Line();
            line2.Stroke = brush;
            line2.StrokeThickness = 4.0;
            line2.StrokeDashArray = CreateDoubleCollection();
            _right = line2;
            Line line3 = new Line();
            line3.Stroke = brush;
            line3.StrokeThickness = 4.0;
            line3.StrokeDashArray = CreateDoubleCollection();
            _top = line3;
            Line line4 = new Line();
            line4.Stroke = brush;
            line4.StrokeThickness = 4.0;
            line4.StrokeDashArray = CreateDoubleCollection();
            _bottom = line4;
            base.Children.Add(_left);
            base.Children.Add(_right);
            base.Children.Add(_top);
            base.Children.Add(_bottom);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (FrameworkElement element in base.Children)
            {
                element.Measure(availableSize);
            }
            return base.MeasureOverride(availableSize);
        }

        public void OnTick()
        {
            if (_selectionItem.IsFlickering)
            {
                if (_flag)
                {
                    _left.StrokeDashOffset = 1.0;
                    _top.StrokeDashOffset = 1.0;
                    _right.StrokeDashOffset = 1.0;
                    _bottom.StrokeDashOffset = 1.0;
                    _innerLeft.StrokeDashOffset = 0.75;
                    _innerRight.StrokeDashOffset = -0.75;
                    _innerTop.StrokeDashOffset = 0.75;
                    _innerBottom.StrokeDashOffset = -0.75;
                }
                else
                {
                    _left.StrokeDashOffset = 0.0;
                    _top.StrokeDashOffset = 0.0;
                    _right.StrokeDashOffset = 0.0;
                    _bottom.StrokeDashOffset = 0.0;
                    _innerLeft.StrokeDashOffset = -0.25;
                    _innerRight.StrokeDashOffset = 0.25;
                    _innerTop.StrokeDashOffset = -0.25;
                    _innerBottom.StrokeDashOffset = 0.25;
                }
                _flag = !_flag;
            }
        }

        void SelectionItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Range")
            {
                UIElement parent = VisualTreeHelper.GetParent(this) as UIElement;
                if (parent != null)
                {
                    parent.InvalidateMeasure();
                }
            }
            else if (e.PropertyName == "Color")
            {
                Color color = _selectionItem.Color;
                SolidColorBrush brush = new SolidColorBrush(color);
                _leftBackground.Fill = brush;
                _rightBackground.Fill = brush;
                _topBackground.Fill = brush;
                _bottomBackground.Fill = brush;
                _leftTop.Fill = brush;
                _rightTop.Fill = brush;
                _leftBottom.Fill = brush;
                _rightBottom.Fill = brush;
                brush = new SolidColorBrush(Color.FromArgb(0xff, (byte)(0xff - color.R), (byte)(0xff - color.G), (byte)(0xff - color.B)));
                _left.Stroke = brush;
                _right.Stroke = brush;
                _top.Stroke = brush;
                _bottom.Stroke = brush;
            }
            else if (e.PropertyName == "IsFlickering")
            {
                IsFlickering = _selectionItem.IsFlickering;
            }
            else if (e.PropertyName == "IsMouseOver")
            {
                IsMouseOvering = _selectionItem.IsMouseOver;
            }
            else if (e.PropertyName == "CanChangeBoundsByUI")
            {
                CanChangeBoundsByUI = _selectionItem.CanChangeBoundsByUI;
            }
        }

        void UpdateVisibilities()
        {
            if (IsLeftVisible)
            {
                _leftBackground.Visibility = Visibility.Visible;
                if (IsFlickering)
                {
                    _left.Visibility = Visibility.Visible;
                    _innerLeft.Visibility = Visibility.Visible;
                }
                else
                {
                    _left.Visibility = Visibility.Collapsed;
                    _innerLeft.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                _leftBackground.Visibility = Visibility.Collapsed;
                _left.Visibility = Visibility.Collapsed;
                _innerLeft.Visibility = Visibility.Collapsed;
            }
            if (IsRightVisible)
            {
                _rightBackground.Visibility = Visibility.Visible;
                if (IsFlickering)
                {
                    _right.Visibility = Visibility.Visible;
                    _innerRight.Visibility = Visibility.Visible;
                }
                else
                {
                    _right.Visibility = Visibility.Collapsed;
                    _innerRight.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                _rightBackground.Visibility = Visibility.Collapsed;
                _right.Visibility = Visibility.Collapsed;
                _innerRight.Visibility = Visibility.Collapsed;
            }
            if (IsTopVisible)
            {
                _topBackground.Visibility = Visibility.Visible;
                if (IsFlickering)
                {
                    _top.Visibility = Visibility.Visible;
                    _innerTop.Visibility = Visibility.Visible;
                }
                else
                {
                    _top.Visibility = Visibility.Collapsed;
                    _innerTop.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                _topBackground.Visibility = Visibility.Collapsed;
                _top.Visibility = Visibility.Collapsed;
                _innerTop.Visibility = Visibility.Collapsed;
            }
            if (IsBottomVisible)
            {
                _bottomBackground.Visibility = Visibility.Visible;
                if (IsFlickering)
                {
                    _bottom.Visibility = Visibility.Visible;
                    _innerBottom.Visibility = Visibility.Visible;
                }
                else
                {
                    _bottom.Visibility = Visibility.Collapsed;
                    _innerBottom.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                _bottomBackground.Visibility = Visibility.Collapsed;
                _bottom.Visibility = Visibility.Collapsed;
                _innerBottom.Visibility = Visibility.Collapsed;
            }
            if ((_canChangeBoundsByUI && IsLeftVisible) && IsTopVisible)
            {
                _leftTop.Visibility = Visibility.Visible;
            }
            else
            {
                _leftTop.Visibility = Visibility.Collapsed;
            }
            if ((_canChangeBoundsByUI && IsRightVisible) && IsTopVisible)
            {
                _rightTop.Visibility = Visibility.Visible;
            }
            else
            {
                _rightTop.Visibility = Visibility.Collapsed;
            }
            if ((_canChangeBoundsByUI && IsLeftVisible) && IsBottomVisible)
            {
                _leftBottom.Visibility = Visibility.Visible;
            }
            else
            {
                _leftBottom.Visibility = Visibility.Collapsed;
            }
            if ((_canChangeBoundsByUI && IsRightVisible) && IsBottomVisible)
            {
                _rightBottom.Visibility = Visibility.Visible;
            }
            else
            {
                _rightBottom.Visibility = Visibility.Collapsed;
            }
        }

        public bool CanChangeBoundsByUI
        {
            get { return _canChangeBoundsByUI; }
            set
            {
                if (_canChangeBoundsByUI != value)
                {
                    _canChangeBoundsByUI = value;
                    UpdateVisibilities();
                }
            }
        }

        public bool IsBottomVisible
        {
            get { return _isBottomVisible; }
            set
            {
                if (_isBottomVisible != value)
                {
                    _isBottomVisible = value;
                    UpdateVisibilities();
                }
            }
        }

        public bool IsFlickering
        {
            get { return _isFlickering; }
            set
            {
                if (_isFlickering != value)
                {
                    _isFlickering = value;
                    UpdateVisibilities();
                }
            }
        }

        public bool IsLeftVisible
        {
            get { return _isLeftVisible; }
            set
            {
                if (_isLeftVisible != value)
                {
                    _isLeftVisible = value;
                    UpdateVisibilities();
                }
            }
        }

        public bool IsMouseOvering
        {
            get { return _isMouseOvering; }
            set
            {
                if (_isMouseOvering != value)
                {
                    _isMouseOvering = value;
                    if (_isMouseOvering)
                    {
                        _leftBackground.Width = 2.0;
                        _rightBackground.Width = 2.0;
                        _topBackground.Height = 2.0;
                        _bottomBackground.Height = 2.0;
                    }
                    else
                    {
                        _leftBackground.Width = 1.0;
                        _rightBackground.Width = 1.0;
                        _topBackground.Height = 1.0;
                        _bottomBackground.Height = 1.0;
                    }
                    base.InvalidateArrange();
                }
            }
        }

        public bool IsRightVisible
        {
            get { return _isRightVisible; }
            set
            {
                if (_isRightVisible != value)
                {
                    _isRightVisible = value;
                    UpdateVisibilities();
                }
            }
        }

        public bool IsTopVisible
        {
            get { return _isTopVisible; }
            set
            {
                if (_isTopVisible != value)
                {
                    _isTopVisible = value;
                    UpdateVisibilities();
                }
            }
        }

        public FormulaSelectionItem SelectionItem
        {
            get { return _selectionItem; }
        }
    }


    internal partial class FormulaSelectionGripperContainerPanel : Panel
    {
        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (FormulaSelectionGripperFrame frame in base.Children)
            {
                CellRange range = frame.SelectionItem.Range;
                int row = (range.Row < 0) ? 0 : range.Row;
                int column = (range.Column < 0) ? 0 : range.Column;
                int rowCount = (range.RowCount < 0) ? ParentSheet.Worksheet.RowCount : range.RowCount;
                int columnCount = (range.ColumnCount < 0) ? ParentSheet.Worksheet.ColumnCount : range.ColumnCount;
                bool isLeftVisible = false;
                bool isTopVisible = false;
                bool isRightVisible = false;
                bool isBottomVisible = false;
                int activeRowViewportIndex = ParentSheet.GetActiveRowViewportIndex();
                int activeColumnViewportIndex = ParentSheet.GetActiveColumnViewportIndex();
                GcViewport viewportRowsPresenter = ParentSheet.GetViewportRowsPresenter(activeRowViewportIndex, activeColumnViewportIndex);
                Rect rect = viewportRowsPresenter.GetRangeBounds(new CellRange(row, column, rowCount, columnCount), out isLeftVisible, out isRightVisible, out isTopVisible, out isBottomVisible);
                if (!rect.IsEmpty)
                {
                    rect = viewportRowsPresenter.TransformToVisual(this).TransformBounds(rect);
                }
                if ((rect.Width > 0.0) && (rect.Height > 0.0))
                {
                    rect.X--;
                    rect.Y--;
                    rect.Width++;
                    rect.Height++;
                }
                if ((rect.Width > 0.0) && (rect.Height > 0.0))
                {
                    frame.Arrange(rect);
                }
                else
                {
                    frame.Arrange(new Rect(0.0, 0.0, 0.0, 0.0));
                }
            }
            return base.ArrangeOverride(finalSize);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (FormulaSelectionGripperFrame frame in base.Children)
            {
                CellRange range = frame.SelectionItem.Range;
                int row = (range.Row < 0) ? 0 : range.Row;
                int column = (range.Column < 0) ? 0 : range.Column;
                int rowCount = (range.RowCount < 0) ? ParentSheet.Worksheet.RowCount : range.RowCount;
                int columnCount = (range.ColumnCount < 0) ? ParentSheet.Worksheet.ColumnCount : range.ColumnCount;
                bool isLeftVisible = false;
                bool isTopVisible = false;
                bool isRightVisible = false;
                bool isBottomVisible = false;
                int activeRowViewportIndex = ParentSheet.GetActiveRowViewportIndex();
                int activeColumnViewportIndex = ParentSheet.GetActiveColumnViewportIndex();
                Rect rect = ParentSheet.GetViewportRowsPresenter(activeRowViewportIndex, activeColumnViewportIndex).GetRangeBounds(new CellRange(row, column, rowCount, columnCount), out isLeftVisible, out isRightVisible, out isTopVisible, out isBottomVisible);
                if ((rect.IsEmpty || (rect.Width == 0.0)) || (rect.Height == 0.0))
                {
                    frame.Visibility = Visibility.Collapsed;
                }
                else
                {
                    frame.Visibility = Visibility.Visible;
                    if (isLeftVisible && isTopVisible)
                    {
                        frame.TopLeftVisibility = Visibility.Visible;
                    }
                    else
                    {
                        frame.TopLeftVisibility = Visibility.Collapsed;
                    }
                    if (isRightVisible && isBottomVisible)
                    {
                        frame.BottomRightVisibility = Visibility.Visible;
                    }
                    else
                    {
                        frame.BottomRightVisibility = Visibility.Collapsed;
                    }
                }
            }
            foreach (FrameworkElement element in base.Children)
            {
                element.Measure(availableSize);
            }
            return base.MeasureOverride(availableSize);
        }

        public void Refresh()
        {
            base.Children.Clear();
            using (IEnumerator<FormulaSelectionItem> enumerator = ParentSheet.FormulaSelections.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    FormulaSelectionGripperFrame frame = new FormulaSelectionGripperFrame(enumerator.Current);
                    base.Children.Add(frame);
                }
            }
        }

        public SpreadView ParentSheet { get; set; }

    }

    internal partial class FormulaSelectionGripperFrame : Panel
    {
        Ellipse _bottomRightGripper;
        Visibility _bottomRightVisibility;
        bool _canChangeBoundsByUI = true;
        FormulaSelectionItem _selectionItem;
        Ellipse _topLeftGripper;
        Visibility _topLeftVisibility;

        public FormulaSelectionGripperFrame(FormulaSelectionItem selectionItem)
        {
            _selectionItem = selectionItem;
            _selectionItem.PropertyChanged += new PropertyChangedEventHandler(SelectionItemPropertyChanged);
            _canChangeBoundsByUI = selectionItem.CanChangeBoundsByUI;
            CreateTouchGrippers(selectionItem);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _topLeftGripper.Arrange(new Rect(-_topLeftGripper.Width / 2.0, -_topLeftGripper.Height / 2.0, _topLeftGripper.Width, _topLeftGripper.Height));
            _bottomRightGripper.Arrange(new Rect(finalSize.Width - (_topLeftGripper.Width / 2.0), finalSize.Height - (_topLeftGripper.Height / 2.0), _topLeftGripper.Width, _topLeftGripper.Height));
            return base.ArrangeOverride(finalSize);
        }

        void CreateTouchGrippers(FormulaSelectionItem selectionItem)
        {
            SolidColorBrush brush = new SolidColorBrush(selectionItem.Color);
            Ellipse ellipse = new Ellipse();
            ellipse.Width = 16.0;
            ellipse.Height = 16.0;
            ellipse.Fill = new SolidColorBrush(Colors.White);
            ellipse.StrokeThickness = 2.0;
            ellipse.Stroke = brush;
            _topLeftGripper = ellipse;
            Ellipse ellipse2 = new Ellipse();
            ellipse2.Width = 16.0;
            ellipse2.Height = 16.0;
            ellipse2.Fill = new SolidColorBrush(Colors.White);
            ellipse2.StrokeThickness = 2.0;
            ellipse2.Stroke = brush;
            _bottomRightGripper = ellipse2;
            UpdateGripperVisibility();
            base.Children.Add(_topLeftGripper);
            base.Children.Add(_bottomRightGripper);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (FrameworkElement element in base.Children)
            {
                element.Measure(availableSize);
            }
            return base.MeasureOverride(availableSize);
        }

        void SelectionItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Range")
            {
                UIElement parent = VisualTreeHelper.GetParent(this) as UIElement;
                if (parent != null)
                {
                    parent.InvalidateMeasure();
                }
            }
            else if (e.PropertyName == "Color")
            {
                SolidColorBrush brush = new SolidColorBrush(_selectionItem.Color);
                _topLeftGripper.Stroke = brush;
                _bottomRightGripper.Stroke = brush;
            }
            else if (e.PropertyName == "CanChangeBoundsByUI")
            {
                CanChangeBoundsByUI = _selectionItem.CanChangeBoundsByUI;
            }
            else if (e.PropertyName == "IsResizing")
            {
                if (_selectionItem.IsResizing)
                {
                    _topLeftGripper.Visibility = Visibility.Collapsed;
                    _bottomRightGripper.Visibility = Visibility.Collapsed;
                }
                else
                {
                    UpdateGripperVisibility();
                }
            }
        }

        void UpdateGripperVisibility()
        {
            if (SheetView.FormulaSelectionFeature.IsTouching && _canChangeBoundsByUI)
            {
                _topLeftGripper.Visibility = _topLeftVisibility;
                _bottomRightGripper.Visibility = _bottomRightVisibility;
            }
            else
            {
                _topLeftGripper.Visibility = Visibility.Collapsed;
                _bottomRightGripper.Visibility = Visibility.Collapsed;
            }
        }

        public Visibility BottomRightVisibility
        {
            get { return _bottomRightVisibility; }
            set
            {
                if (_bottomRightVisibility != value)
                {
                    _bottomRightVisibility = value;
                    UpdateGripperVisibility();
                }
            }
        }

        public bool CanChangeBoundsByUI
        {
            get { return _canChangeBoundsByUI; }
            set
            {
                if (_canChangeBoundsByUI != value)
                {
                    _canChangeBoundsByUI = value;
                    UpdateGripperVisibility();
                }
            }
        }

        public FormulaSelectionItem SelectionItem
        {
            get { return _selectionItem; }
        }

        public Visibility TopLeftVisibility
        {
            get { return _topLeftVisibility; }
            set
            {
                if (_topLeftVisibility != value)
                {
                    _topLeftVisibility = value;
                    UpdateGripperVisibility();
                }
            }
        }
    }

    internal partial class RowsPanel : Panel
    {
        HashSet<RowPresenter> _cachedChildren;
        internal int _normalZIndexBase;
        Dictionary<int, RowPresenter> _rows;
        internal int _spanRowZIndexBase;

        public RowsPanel()
        {
            Action action = null;
            _normalZIndexBase = 0x2710;
            _spanRowZIndexBase = 0x4e20;
            _rows = new Dictionary<int, RowPresenter>();
            _cachedChildren = new HashSet<RowPresenter>();
            if (action == null)
            {
                action = delegate
                {
                    base.HorizontalAlignment = HorizontalAlignment.Left;
                    base.VerticalAlignment = VerticalAlignment.Top;
                    base.Background = new SolidColorBrush(Colors.White);
                };
            }
            UIAdaptor.InvokeSync(action);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            RowLayoutModel rowLayoutModel = ParentViewport.GetRowLayoutModel();
            double y = 0.0;
            double rowWidth = 0.0;
            foreach (RowLayout layout in rowLayoutModel)
            {
                if (layout.Height >= 0.0)
                {
                    double width = finalSize.Width;
                    double height = layout.Height;
                    if (_rows.ContainsKey(layout.Row))
                    {
                        RowPresenter presenter = _rows[layout.Row];
                        presenter.Arrange(new Rect(new Windows.Foundation.Point(0.0, y), new Size(width, height)));
                        if (rowWidth == 0.0)
                        {
                            rowWidth = presenter.RowWidth;
                        }
                    }
                    y += height;
                }
            }
            rowWidth = Math.Min(ParentViewport.GetViewportSize().Width, rowWidth);
            Size size = new Size(rowWidth, y);
            RectangleGeometry geometry = new RectangleGeometry();
            geometry.Rect = new Rect(new Windows.Foundation.Point(0.0, 0.0), size);
            base.Clip = geometry;
            return size;
        }

        RowPresenter GetNewRowWithRecyclingSupport(int rowIndex)
        {
            RowPresenter recycledRow = null;
            recycledRow = GetRecycledRow();
            if (recycledRow == null)
            {
                recycledRow = ParentViewport.GenerateNewRow();
            }
            recycledRow.Row = rowIndex;
            recycledRow.OwningPresenter = ParentViewport;
            return recycledRow;
        }

        internal RowPresenter GetRecycledRow()
        {
            RowPresenter presenter = null;
            while ((RecycledRows.Count > 0) && (presenter == null))
            {
                RowPresenter presenter2 = RecycledRows[0];
                if (presenter2 != null)
                {
                    RecycledRows.Remove(presenter2);
                    if (presenter2.IsRecyclable)
                    {
                        presenter = presenter2;
                    }
                }
            }
            return presenter;
        }

        internal RowPresenter GetRow(int row)
        {
            RowPresenter presenter = null;
            _rows.TryGetValue(row, out presenter);
            return presenter;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (ParentViewport.SupportCellOverflow && ParentViewport.Sheet.CanCellOverflow)
            {
                int viewportLeftColumn = ParentViewport.Sheet.GetViewportLeftColumn(ParentViewport.ColumnViewportIndex);
                ParentViewport.CellOverflowLayoutBuildEngine.ViewportLeftColumn = viewportLeftColumn;
                int viewportRightColumn = ParentViewport.Sheet.GetViewportRightColumn(ParentViewport.ColumnViewportIndex);
                ParentViewport.CellOverflowLayoutBuildEngine.ViewportRightColumn = viewportRightColumn;
            }
            double x = ParentViewport.Location.X;
            double y = ParentViewport.Location.Y;
            RowLayoutModel rowLayoutModel = ParentViewport.GetRowLayoutModel();
            Dictionary<int, RowPresenter> rows = _rows;
            _rows = new Dictionary<int, RowPresenter>();
            foreach (RowPresenter presenter in Enumerable.ToArray<RowPresenter>((IEnumerable<RowPresenter>)rows.Values))
            {
                if ((rowLayoutModel.FindRow(presenter.Row) == null) && !TryRecycleRow(presenter))
                {
                    presenter.CleanUpBeforeDiscard();
                    if (_cachedChildren.Remove(presenter))
                    {
                        base.Children.Remove(presenter);
                        rows.Remove(presenter.Row);
                    }
                }
            }
            double num5 = 0.0;
            foreach (RowLayout layout in rowLayoutModel)
            {
                if (layout.Height >= 0.0)
                {
                    RowPresenter element = null;
                    int row = layout.Row;
                    if (rows.TryGetValue(row, out element))
                    {
                        rows.Remove(row);
                        if (layout.Height > 0.0)
                        {
                            element.UpdateDisplayedCells();
                        }
                    }
                    else
                    {
                        element = GetNewRowWithRecyclingSupport(row);
                        if (layout.Height > 0.0)
                        {
                            if (!_cachedChildren.Contains(element))
                            {
                                base.Children.Add(element);
                                _cachedChildren.Add(element);
                                element.UpdateDisplayedCells();
                            }
                            else
                            {
                                element.UpdateDisplayedCells(true);
                            }
                        }
                    }
                    if (layout.Height > 0.0)
                    {
                        int num7 = _normalZIndexBase + element.Row;
                        if (element.ContainsSpanCell)
                        {
                            num7 = _spanRowZIndexBase + element.Row;
                        }
                        num7 = num7 % 0x7ffe;
                        Canvas.SetZIndex(element, num7);
                        _rows.Add(row, element);
                        element.Location = new Windows.Foundation.Point(x, y);
                        element.Measure(new Size(availableSize.Width, layout.Height));
                        y += layout.Height;
                        num5 = Math.Max(num5, element.DesiredSize.Width);
                    }
                    else
                    {
                        if (_cachedChildren.Remove(element))
                        {
                            base.Children.Remove(element);
                        }
                        TryRecycleRow(element);
                    }
                }
            }
            foreach (RowPresenter presenter3 in RecycledRows)
            {
                if (_cachedChildren.Remove(presenter3))
                {
                    base.Children.Remove(presenter3);
                    foreach (CellPresenterBase base2 in presenter3.Children)
                    {
                        base2.RemoveInvalidDataPresenter();
                    }
                }
            }
            rows.Clear();
            return new Size(num5 + ParentViewport.Location.X, y);
        }

        bool TryRecycleRow(RowPresenter objRow)
        {
            if (objRow.IsRecyclable)
            {
                RecycledRows.Add(objRow);
                objRow.CellsDirty = true;
                return true;
            }
            return false;
        }

        public GcViewport ParentViewport { get; set; }

        internal List<RowPresenter> RecycledRows
        {
            get { return ParentViewport.RecycledRows; }
        }

        internal List<RowPresenter> Rows
        {
            get { return new List<RowPresenter>((IEnumerable<RowPresenter>)_rows.Values); }
        }
    }

    internal partial class SelectionContainerPanel : Panel
    {
        List<Rect> _activeSelectionLayouts;
        List<Rectangle> _activeSelectionRectangles;
        SelectionFrame _focusIndicator;
        Brush _selectionBackground;
        Path _selectionPath;
        const int ACTIVE_SELECTION_RECTANGLE_NUMBER = 4;

        public SelectionContainerPanel(GcViewport parentViewport)
        {
            _activeSelectionRectangles = new List<Rectangle>();
            _activeSelectionLayouts = new List<Rect>();
            ParentViewport = parentViewport;

            _selectionBackground = parentViewport.Sheet.Worksheet.SelectionBackground;
            _selectionPath = new Path();
            if (_selectionBackground != null)
                _selectionPath.Fill = _selectionBackground;
            else
                _selectionPath.Fill = new SolidColorBrush(Color.FromArgb(60, 180, 180, 200));

            GeometryGroup group = new GeometryGroup();
            group.FillRule = FillRule.Nonzero;
            _selectionPath.Data = group;
            Children.Add(_selectionPath);

            for (int i = 0; i < 4; i++)
            {
                Rectangle rectangle = new Rectangle();
                rectangle.Fill = _selectionBackground;
                _activeSelectionRectangles.Add(rectangle);
                Children.Add(rectangle);
            }

            _focusIndicator = new SelectionFrame(parentViewport);
            Children.Add(_focusIndicator);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (ParentViewport.Sheet.HideSelectionWhenPrinting)
            {
                return availableSize;
            }

            UpdateActiveSelectionLayouts();
            for (int i = 0; i < _activeSelectionRectangles.Count; i++)
            {
                Rect rect = _activeSelectionLayouts[i];
                _activeSelectionRectangles[i].InvalidateMeasure();
                if ((rect.Width > 0.0) && (rect.Height > 0.0))
                {
                    _activeSelectionRectangles[i].Measure(new Size(rect.Width, rect.Height));
                }
                else
                {
                    _activeSelectionRectangles[i].Measure(new Size(0.0, 0.0));
                }
            }

            GeometryGroup data = _selectionPath.Data as GeometryGroup;
            if (data != null)
            {
                data.Children.Clear();
                for (int j = 0; j < ParentViewport._cachedSelectionLayout.Count; j++)
                {
                    Rect rect2 = ParentViewport._cachedSelectionLayout[j];
                    if (ParentViewport.IsActived)
                    {
                        Rect rect3 = ParentViewport._cachedActiveSelectionLayout;
                        if (!rect2.IsEmpty)
                        {
                            if (rect3.IsEmpty)
                            {
                                RectangleGeometry geometry = new RectangleGeometry();
                                geometry.Rect = rect2;
                                data.Children.Add(geometry);
                            }
                            else if (!ContainsRect(rect2, rect3) && !ContainsRect(rect3, rect2))
                            {
                                RectangleGeometry geometry2 = new RectangleGeometry();
                                geometry2.Rect = rect2;
                                data.Children.Add(geometry2);
                            }
                        }
                    }
                    else if (!rect2.IsEmpty)
                    {
                        RectangleGeometry geometry3 = new RectangleGeometry();
                        geometry3.Rect = rect2;
                        data.Children.Add(geometry3);
                    }
                }
            }

            _selectionPath.InvalidateMeasure();
            if (ParentViewport.Sheet.Worksheet.SelectionBackground == null)
            {
                _selectionBackground = new SolidColorBrush(Color.FromArgb(60, 180, 180, 200));
            }
            else
            {
                _selectionBackground = ParentViewport.Sheet.Worksheet.SelectionBackground;
            }
            foreach (Rectangle item in _activeSelectionRectangles)
            {
                UIAdaptor.InvokeAsync(() => { item.Fill = _selectionBackground; });
            }
            _selectionPath.Measure(availableSize);

            if (FocusIndicator != null)
            {
                Rect rect4 = ParentViewport._cachedSelectionFrameLayout;
                if (!IsAnchorCellInSelection)
                {
                    rect4 = ParentViewport._cachedFocusCellLayout;
                }
                if ((FocusIndicator.Visibility == Visibility.Visible) && (ParentViewport.IsActived || (ParentViewport._cachedSelectionLayout.Count <= 1)))
                {
                    if ((rect4.Width > 0.0) && (rect4.Height > 0.0))
                    {
                        FocusIndicator.Measure(new Size(rect4.Width, rect4.Height));
                    }
                }
                else
                {
                    FocusIndicator.Visibility = Visibility.Collapsed;
                }
            }
            return ParentViewport.GetViewportSize(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (!ParentViewport.Sheet.HideSelectionWhenPrinting)
            {
                Rect rect = new Rect(new Windows.Foundation.Point(), finalSize);
                for (int i = 0; i < _activeSelectionRectangles.Count; i++)
                {
                    _activeSelectionRectangles[i].InvalidateArrange();
                    if ((_activeSelectionLayouts[i].Width > 0.0) && (_activeSelectionLayouts[i].Height > 0.0))
                    {
                        _activeSelectionRectangles[i].Arrange(_activeSelectionLayouts[i]);
                    }
                    else
                    {
                        _activeSelectionRectangles[i].Arrange(new Rect(0.0, 0.0, 0.0, 0.0));
                    }
                }
                _selectionPath.Arrange(rect);
                if (FocusIndicator == null)
                {
                    return finalSize;
                }
                Rect rect2 = ParentViewport._cachedSelectionFrameLayout;
                if (!IsAnchorCellInSelection)
                {
                    rect2 = ParentViewport._cachedFocusCellLayout;
                }
                if ((FocusIndicator.Visibility == Visibility.Visible) && (ParentViewport.IsActived || (ParentViewport._cachedSelectionLayout.Count <= 1)))
                {
                    if ((rect2.Width > 0.0) && (rect2.Height > 0.0))
                    {
                        FocusIndicator.Arrange(rect2);
                        return finalSize;
                    }
                    FocusIndicator.Visibility = Visibility.Collapsed;
                    return finalSize;
                }
                FocusIndicator.Visibility = Visibility.Collapsed;
            }
            return finalSize;
        }

        bool ContainsRect(Rect rect1, Rect rect2)
        {
            return ((((rect2.Left >= rect1.Left) && (rect2.Right <= rect1.Right)) && (rect2.Top >= rect1.Top)) && (rect2.Bottom <= rect1.Bottom));
        }

        CellRange GetViewportRange()
        {
            int viewportTopRow = ParentViewport.Sheet.GetViewportTopRow(ParentViewport.RowViewportIndex);
            int viewportLeftColumn = ParentViewport.Sheet.GetViewportLeftColumn(ParentViewport.ColumnViewportIndex);
            int viewportBottomRow = ParentViewport.Sheet.GetViewportBottomRow(ParentViewport.RowViewportIndex);
            int viewportRightColumn = ParentViewport.Sheet.GetViewportRightColumn(ParentViewport.ColumnViewportIndex);
            return new CellRange(viewportTopRow, viewportLeftColumn, (viewportBottomRow - viewportTopRow) + 1, (viewportRightColumn - viewportLeftColumn) + 1);
        }

        internal void ResetSelectionFrameStroke()
        {
            _focusIndicator.ResetSelectionFrameStoke();
        }

        internal void SetSelectionFrameStroke(Brush brush)
        {
            _focusIndicator.SetSelectionFrameStroke(brush);
        }

        void UpdateActiveSelectionLayouts()
        {
            Rect rect = ParentViewport._cachedActiveSelectionLayout;
            Rect rect2 = ParentViewport._cachedFocusCellLayout;
            CellRange range = null;
            _activeSelectionLayouts = new List<Rect>();
            CellRange viewportRange = GetViewportRange();

            if (ParentViewport.IsActived && (ParentViewport.Sheet.Worksheet.ActiveCell != null))
            {
                Worksheet ws = ParentViewport.Sheet.Worksheet;
                range = new CellRange(ws.ActiveRowIndex, ws.ActiveColumnIndex, 1, 1);
                CellRange range3 = ws.SpanModel.Find(range.Row, range.Column);
                if ((range3 != null) && viewportRange.Intersects(range3.Row, range3.Column, range3.RowCount, range3.ColumnCount))
                {
                    range = CellRange.GetIntersect(viewportRange, range3, viewportRange.RowCount, viewportRange.ColumnCount);
                }
            }

            if (ParentViewport.IsActived)
            {
                if ((viewportRange.RowCount == 0) || (viewportRange.ColumnCount == 0))
                {
                    _activeSelectionLayouts.Add(Rect.Empty);
                    _activeSelectionLayouts.Add(Rect.Empty);
                    _activeSelectionLayouts.Add(Rect.Empty);
                    _activeSelectionLayouts.Add(Rect.Empty);
                }
                else if ((range != null) && !viewportRange.Contains(range))
                {
                    _activeSelectionLayouts.Add(rect);
                    _activeSelectionLayouts.Add(Rect.Empty);
                    _activeSelectionLayouts.Add(Rect.Empty);
                    _activeSelectionLayouts.Add(Rect.Empty);
                }
                else if (ParentViewport._cachedActiveSelection != null
                    && range != null
                    && IsActiveCellBoundsValid
                    && ParentViewport._cachedActiveSelection != range
                    && ParentViewport._cachedActiveSelection.Contains(range))
                {
                    Rect rect3 = new Rect(rect.X, rect.Y, rect.Width, rect2.Y - rect.Y);
                    Rect rect4 = new Rect(rect.X, rect2.Y, rect2.X - rect.X, rect2.Height);
                    double width = rect.Right - rect2.Right;
                    if (width < 0.0)
                    {
                        width = 0.0;
                    }
                    Rect rect5 = new Rect(rect2.Right, rect2.Y, width, rect2.Height);
                    double height = rect.Bottom - rect2.Bottom;
                    if (height < 0.0)
                    {
                        height = 0.0;
                    }
                    Rect rect6 = new Rect(rect.X, rect2.Bottom, rect.Width, height);
                    _activeSelectionLayouts.Add(rect3);
                    _activeSelectionLayouts.Add(rect4);
                    _activeSelectionLayouts.Add(rect5);
                    _activeSelectionLayouts.Add(rect6);
                }
                else
                {
                    _activeSelectionLayouts.Add(Rect.Empty);
                    _activeSelectionLayouts.Add(Rect.Empty);
                    _activeSelectionLayouts.Add(Rect.Empty);
                    _activeSelectionLayouts.Add(Rect.Empty);
                }
            }
            else if ((viewportRange.RowCount == 0) || (viewportRange.ColumnCount == 0))
            {
                _activeSelectionLayouts.Add(Rect.Empty);
                _activeSelectionLayouts.Add(Rect.Empty);
                _activeSelectionLayouts.Add(Rect.Empty);
                _activeSelectionLayouts.Add(Rect.Empty);
            }
            else
            {
                _activeSelectionLayouts.Add(rect);
                _activeSelectionLayouts.Add(Rect.Empty);
                _activeSelectionLayouts.Add(Rect.Empty);
                _activeSelectionLayouts.Add(Rect.Empty);
            }
        }

        internal SelectionFrame FocusIndicator
        {
            get { return _focusIndicator; }
        }

        bool IsActiveCellBoundsValid
        {
            get { return ((!ParentViewport._cachedFocusCellLayout.IsEmpty && (ParentViewport._cachedFocusCellLayout.Width > 0.0)) && (ParentViewport._cachedFocusCellLayout.Height > 0.0)); }
        }

        public bool IsAnchorCellInSelection { get; set; }

        public GcViewport ParentViewport { get; set; }
    }

    internal partial class SelectionFrame : Panel
    {
        [ThreadStatic]
        static readonly Brush DefaultSelectionBorderBrush = new SolidColorBrush(Color.FromArgb(220, 0, 0, 0));
        const int BORDER_OFFSET = 4;
        internal const int FILL_WEIGHT = 5;
        const int SelectionBorderThickness = 3;

        Rectangle _bottomRectangle;
        Color _currentBorderColor;
        Rectangle _fillIndicator;
        Rect _fillIndicatorBounds = Rect.Empty;
        FillIndicatorPosition _fillIndicatorPosition;
        bool _isBottomVisible = true;
        bool _isFillIndicatorVisible = true;
        bool _isLeftVisible = true;
        bool _isRightVisible = true;
        bool _isTopVisible = true;
        Rectangle _leftRectangle;
        Rectangle _rightRectangle;
        Color _selectionBorderColor;
        double _thickNess = 3.0;
        Rectangle _topRectangle;

        public SelectionFrame(GcViewport owingViewport)
        {
            OwingViewport = owingViewport;
            _selectionBorderColor = GetSelectionBorderColor(OwingViewport.Sheet.Worksheet.SelectionBorderColor, OwingViewport.Sheet.Worksheet.SelectionBorderThemeColor);
            _currentBorderColor = _selectionBorderColor;
            _leftRectangle = new Rectangle();
            _leftRectangle.Fill = DefaultSelectionBorderBrush;
            _leftRectangle.Stroke = null;
            _leftRectangle.StrokeThickness = 0.0;
            _leftRectangle.Width = 3.0;
            base.Children.Add(_leftRectangle);
            _topRectangle = new Rectangle();
            _topRectangle.Fill = DefaultSelectionBorderBrush;
            _topRectangle.Stroke = null;
            _topRectangle.StrokeThickness = 0.0;
            _topRectangle.Height = 3.0;
            base.Children.Add(_topRectangle);
            _rightRectangle = new Rectangle();
            _rightRectangle.Fill = DefaultSelectionBorderBrush;
            _rightRectangle.Stroke = null;
            _rightRectangle.StrokeThickness = 0.0;
            _rightRectangle.Width = 3.0;
            base.Children.Add(_rightRectangle);
            _bottomRectangle = new Rectangle();
            _bottomRectangle.Fill = DefaultSelectionBorderBrush;
            _bottomRectangle.Stroke = null;
            _bottomRectangle.StrokeThickness = 0.0;
            _bottomRectangle.Height = 3.0;
            base.Children.Add(_bottomRectangle);
            IsLeftVisible = true;
            IsRightVisible = true;
            IsTopVisible = true;
            IsBottomVisible = true;
            _fillIndicator = new Rectangle();
            base.Children.Add(_fillIndicator);
            SetSelectionStyle(_currentBorderColor);
            _fillIndicatorPosition = FillIndicatorPosition.BottomRight;
        }

        Rect ArrangeFillRect(Size finalSize)
        {
            Rect empty = Rect.Empty;
            switch (_fillIndicatorPosition)
            {
                case FillIndicatorPosition.TopRight:
                    empty = new Rect((finalSize.Width - 5.0) + 1.0, 3.0, 5.0, 5.0);
                    break;

                case FillIndicatorPosition.BottomLeft:
                    empty = new Rect(3.0, (finalSize.Height - 5.0) + 1.0, 5.0, 5.0);
                    break;

                case FillIndicatorPosition.BottomRight:
                    empty = new Rect((finalSize.Width - 5.0) + 1.0, (finalSize.Height - 5.0) + 1.0, 5.0, 5.0);
                    break;
            }
            _fillIndicator.Arrange(empty);
            return empty;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (!finalSize.IsEmpty)
            {
                Rect empty = Rect.Empty;
                Rect rect2 = Rect.Empty;
                Rect rect3 = Rect.Empty;
                Rect rect4 = Rect.Empty;
                if (IsLeftVisible)
                {
                    if (IsFillIndicatorVisible && (FillIndicatorPosition == FillIndicatorPosition.BottomLeft))
                    {
                        double height = finalSize.Height - 4.0;
                        if (height < 0.0)
                        {
                            height = 0.0;
                        }
                        empty = new Rect(0.0, 0.0, Thickness, height);
                    }
                    else
                    {
                        empty = new Rect(0.0, 0.0, Thickness, finalSize.Height);
                    }
                }
                if (IsRightVisible)
                {
                    if (IsFillIndicatorVisible)
                    {
                        if (_fillIndicatorPosition == FillIndicatorPosition.TopRight)
                        {
                            double num2 = (finalSize.Height - 5.0) - 4.0;
                            if (num2 < 0.0)
                            {
                                num2 = 0.0;
                            }
                            rect2 = new Rect(finalSize.Width - Thickness, 9.0, Thickness, num2);
                        }
                        else if (_fillIndicatorPosition == FillIndicatorPosition.BottomRight)
                        {
                            double num3 = finalSize.Height - 5.0;
                            if (num3 < 0.0)
                            {
                                num3 = 0.0;
                            }
                            rect2 = new Rect(finalSize.Width - Thickness, 0.0, Thickness, num3);
                        }
                        else
                        {
                            rect2 = new Rect(finalSize.Width - Thickness, 0.0, Thickness, finalSize.Height);
                        }
                    }
                    else
                    {
                        rect2 = new Rect(finalSize.Width - Thickness, 0.0, Thickness, finalSize.Height);
                    }
                }
                if (IsTopVisible)
                {
                    if (IsFillIndicatorVisible && (FillIndicatorPosition == FillIndicatorPosition.TopRight))
                    {
                        double width = finalSize.Width - 4.0;
                        if (width < 0.0)
                        {
                            width = 0.0;
                        }
                        rect3 = new Rect(0.0, 0.0, width, Thickness);
                    }
                    else
                    {
                        rect3 = new Rect(0.0, 0.0, finalSize.Width, Thickness);
                    }
                }
                if (IsBottomVisible)
                {
                    if (IsFillIndicatorVisible)
                    {
                        if (_fillIndicatorPosition == FillIndicatorPosition.BottomLeft)
                        {
                            double num5 = (finalSize.Width - 5.0) - 4.0;
                            if (num5 < 0.0)
                            {
                                num5 = 0.0;
                            }
                            rect4 = new Rect(9.0, finalSize.Height - Thickness, num5, Thickness);
                        }
                        else if (_fillIndicatorPosition == FillIndicatorPosition.BottomRight)
                        {
                            double num6 = finalSize.Width - 5.0;
                            if (num6 < 0.0)
                            {
                                num6 = 0.0;
                            }
                            rect4 = new Rect(0.0, finalSize.Height - Thickness, num6, Thickness);
                        }
                        else
                        {
                            rect4 = new Rect(0.0, finalSize.Height - Thickness, finalSize.Width, Thickness);
                        }
                    }
                    else
                    {
                        rect4 = new Rect(0.0, finalSize.Height - Thickness, finalSize.Width, Thickness);
                    }
                }
                if (IsLeftVisible)
                {
                    _leftRectangle.Arrange(empty);
                }
                if (IsTopVisible)
                {
                    _topRectangle.Arrange(rect3);
                }
                if (IsRightVisible)
                {
                    _rightRectangle.Arrange(rect2);
                }
                if (IsBottomVisible)
                {
                    _bottomRectangle.Arrange(rect4);
                }
                if (IsFillIndicatorVisible)
                {
                    _fillIndicatorBounds = ArrangeFillRect(finalSize);
                }
                (OwingViewport.Sheet as SpreadView).UpdateTouchSelectionGripper();
            }
            return finalSize;
        }

        Color GetSelectionBorderColor(Color selectionBorderColor, string themeColor)
        {
            if (themeColor != null)
            {
                return OwingViewport.Sheet.Worksheet.Workbook.GetThemeColor(themeColor);
            }
            return selectionBorderColor;
        }

        internal bool IsMouseInFillIndicator(Windows.Foundation.Point viewportPoint)
        {
            return _fillIndicatorBounds.Contains(viewportPoint);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            _selectionBorderColor = GetSelectionBorderColor(OwingViewport.Sheet.Worksheet.SelectionBorderColor, OwingViewport.Sheet.Worksheet.SelectionBorderThemeColor);
            if (_currentBorderColor != _selectionBorderColor)
            {
                _currentBorderColor = _selectionBorderColor;
                SetSelectionStyle(_currentBorderColor);
            }
            _leftRectangle.Measure(new Size(Thickness, availableSize.Height));
            _rightRectangle.Measure(new Size(Thickness, availableSize.Height));
            _topRectangle.Measure(new Size(availableSize.Width, Thickness));
            _bottomRectangle.Measure(new Size(availableSize.Width, Thickness));
            _fillIndicator.Measure(new Size(5.0, 5.0));
            return availableSize;
        }

        internal async void ResetSelectionFrameStoke()
        {
            _leftRectangle.Fill = DefaultSelectionBorderBrush;
            _topRectangle.Fill = DefaultSelectionBorderBrush;
            _rightRectangle.Fill = DefaultSelectionBorderBrush;
            _bottomRectangle.Fill = DefaultSelectionBorderBrush;
            await base.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(InvalidateMeasure));
        }

        internal async void SetSelectionFrameStroke(Brush brush)
        {
            _leftRectangle.Fill = brush;
            _topRectangle.Fill = brush;
            _rightRectangle.Fill = brush;
            _bottomRectangle.Fill = brush;
            await base.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(InvalidateMeasure));
        }

        void SetSelectionStyle(Color currentBorderColor)
        {
            Action action = null;
            SolidColorBrush brush = new SolidColorBrush(currentBorderColor);
            _leftRectangle.Fill = brush;
            _topRectangle.Fill = brush;
            _rightRectangle.Fill = brush;
            _bottomRectangle.Fill = brush;
            if (action == null)
            {
                action = delegate
                {
                    _fillIndicator.Fill = new SolidColorBrush(currentBorderColor);
                };
            }
            UIAdaptor.InvokeSync(action);
        }

        public Rect FillIndicatorBounds
        {
            get { return _fillIndicatorBounds; }
        }

        public FillIndicatorPosition FillIndicatorPosition
        {
            get { return _fillIndicatorPosition; }
            set
            {
                if (FillIndicatorPosition != value)
                {
                    _fillIndicatorPosition = value;
                    base.InvalidateMeasure();
                    base.InvalidateArrange();
                }
            }
        }

        public bool IsBottomVisible
        {
            get { return _isBottomVisible; }
            set
            {
                if (_isBottomVisible != value)
                {
                    _isBottomVisible = value;
                    if (value)
                    {
                        _bottomRectangle.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        _bottomRectangle.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        public bool IsFillIndicatorVisible
        {
            get { return _isFillIndicatorVisible; }
            set
            {
                if (IsFillIndicatorVisible != value)
                {
                    _isFillIndicatorVisible = value;
                    _fillIndicator.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                }
            }
        }

        public bool IsLeftVisible
        {
            get { return _isLeftVisible; }
            set
            {
                if (_isLeftVisible != value)
                {
                    _isLeftVisible = value;
                    if (value)
                    {
                        _leftRectangle.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        _leftRectangle.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        public bool IsRightVisible
        {
            get { return _isRightVisible; }
            set
            {
                if (_isRightVisible != value)
                {
                    _isRightVisible = value;
                    if (value)
                    {
                        _rightRectangle.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        _rightRectangle.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        public bool IsTopVisible
        {
            get { return _isTopVisible; }
            set
            {
                if (_isTopVisible != value)
                {
                    _isTopVisible = value;
                    if (value)
                    {
                        _topRectangle.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        _topRectangle.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        public GcViewport OwingViewport { get; set; }

        public double Thickness
        {
            get { return _thickNess; }
            set
            {
                if (Thickness != value)
                {
                    _thickNess = value;
                    _leftRectangle.Width = value;
                    _rightRectangle.Width = value;
                    _topRectangle.Height = value;
                    _bottomRectangle.Height = value;
                }
            }
        }
    }
}

