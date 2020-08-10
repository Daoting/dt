#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.UI
{
    internal partial class CellsPanel : Panel
    {
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
            //if (((SheetArea == SheetArea.Cells) && (Sheet != null)) && (Sheet.ActiveSheet != null))
            //{
            //    if (Sheet.ActiveSheet != null)
            //    {
            //        int activeRowIndex = Sheet.ActiveSheet.ActiveRowIndex;
            //    }
            //    if (Sheet.ActiveSheet != null)
            //    {
            //        int activeColumnIndex = Sheet.ActiveSheet.ActiveColumnIndex;
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
                if (_borderContainer != null)
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
                        columnEnd = Sheet.ActiveSheet.RowHeader.ColumnCount - 1;
                        break;

                    case SheetArea.ColumnHeader:
                        rowStart = 0;
                        rowEnd = Sheet.ActiveSheet.ColumnHeader.RowCount - 1;
                        columnStart = Sheet.GetViewportLeftColumn(ColumnViewportIndex);
                        columnEnd = Sheet.GetViewportRightColumn(ColumnViewportIndex);
                        break;
                }
                if ((rowStart <= rowEnd) && (columnStart <= columnEnd))
                {
                    int num5 = -1;
                    for (int i = rowStart - 1; i > -1; i--)
                    {
                        if (Sheet.ActiveSheet.GetActualRowVisible(i, SheetArea))
                        {
                            num5 = i;
                            break;
                        }
                    }
                    rowStart = num5;
                    int count = GetDataContext().Rows.Count;
                    for (int j = rowEnd + 1; j < count; j++)
                    {
                        if (Sheet.ActiveSheet.GetActualRowVisible(j, SheetArea))
                        {
                            rowEnd = j;
                            break;
                        }
                    }
                    int num9 = -1;
                    for (int k = columnStart - 1; k > -1; k--)
                    {
                        if (Sheet.ActiveSheet.GetActualColumnVisible(k, SheetArea))
                        {
                            num9 = k;
                            break;
                        }
                    }
                    columnStart = num9;
                    int num11 = GetDataContext().Columns.Count;
                    for (int m = columnEnd + 1; m < num11; m++)
                    {
                        if (Sheet.ActiveSheet.GetActualColumnVisible(m, SheetArea))
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
                    if (((_editorPanel == null) || (_editorPanel.EditingColumnIndex != _activeCol)) || ((_editorPanel.EditingRowIndex != _activeRow) || !editingCell.HasEditingElement()))
                    {
                        if (_editorPanel.Editor != null)
                        {
                            object obj2 = editingCell.SheetView.ActiveSheet.GetValue(_activeRow, _activeCol);
                            if ((obj2 != null) && string.IsNullOrEmpty((_editorPanel.Editor as TextBox).Text))
                            {
                                (_editorPanel.Editor as TextBox).Text = obj2.ToString();
                                (_editorPanel.Editor as TextBox).SelectionStart = obj2.ToString().Length;
                            }
                            editingCell.SetEditingElement(_editorPanel.Editor);
                        }
                        else
                        {
                            PrepareCellEditing(editingCell);
                        }
                    }
                    if (_editorPanel != null)
                    {
                        SolidColorBrush brush = new SolidColorBrush(Colors.White);
                        _editorPanel.SetBackground(brush);
                    }
                }
                if ((editingCell == null) && (_editorPanel != null))
                {
                    SolidColorBrush brush = new SolidColorBrush(Colors.Transparent);
                    _editorPanel.SetBackground(brush);
                }
                _editorPanel.InvalidateMeasure();
            }
        }
    }
}

