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
            _rowsContainer.Measure(availableSize);
            _borderContainer.Measure(availableSize);

            BuildSelection();
            _selectionContainer.Measure(availableSize);

            if (_formulaSelectionContainer.Children.Count > 0)
                _formulaSelectionContainer.InvalidateMeasure();
            _formulaSelectionContainer.Measure(availableSize);

            _shapeContainer.Measure(availableSize);

            if (_dragFillContainer != null)
            {
                _dragFillContainer.Measure(availableSize);
            }

            if (_decoratinPanel != null)
            {
                _decoratinPanel.InvalidateMeasure();
                _decoratinPanel.Measure(availableSize);
            }

            _dataValidationPanel.InvalidateMeasure();

            _editorPanel.Measure(availableSize);
            AttachEditorForActiveCell();

            if (Excel._formulaSelectionGripperPanel != null)
            {
                Excel._formulaSelectionGripperPanel.InvalidateMeasure();
            }

            _floatingObjectContainerPanel.Measure(availableSize);
            _floatingObjectsMovingResizingContainer.Measure(availableSize);

            return GetViewportSize(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Rect rc = new Rect(0.0, 0.0, finalSize.Width, finalSize.Height);
            _rowsContainer.Arrange(rc);
            _borderContainer.Arrange(rc);
            _selectionContainer.Arrange(rc);
            _formulaSelectionContainer.Arrange(rc);
            _shapeContainer.Arrange(rc);

            if (_dragFillContainer != null)
            {
                _dragFillContainer.Arrange(rc);
            }

            if (_decoratinPanel != null)
            {
                _decoratinPanel.Arrange(rc);
            }
            _dataValidationPanel.Arrange(rc);

            if (IsEditing())
            {
                _editorPanel.ResumeEditor();
            }
            _editorPanel.Arrange(rc);
            _floatingObjectContainerPanel.Arrange(rc);
            _floatingObjectsMovingResizingContainer.Arrange(rc);

            Size viewportSize = GetViewportSize(finalSize);
            RectangleGeometry geometry;
            if (Excel.IsTouching)
            {
                if (Clip == null)
                {
                    geometry = new RectangleGeometry();
                    geometry.Rect = new Rect(new Point(), viewportSize);
                    Clip = geometry;
                }
            }
            else
            {
                geometry = new RectangleGeometry();
                geometry.Rect = new Rect(new Point(), viewportSize);
                Clip = geometry;
            }

            if (Clip != null)
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
                int rowStart = Excel.GetViewportTopRow(RowViewportIndex);
                int rowEnd = Excel.GetViewportBottomRow(RowViewportIndex);
                int columnStart = Excel.GetViewportLeftColumn(ColumnViewportIndex);
                int columnEnd = Excel.GetViewportRightColumn(ColumnViewportIndex);

                if ((rowStart <= rowEnd) && (columnStart <= columnEnd))
                {
                    int num5 = -1;
                    for (int i = rowStart - 1; i > -1; i--)
                    {
                        if (Excel.ActiveSheet.GetActualRowVisible(i, SheetArea.Cells))
                        {
                            num5 = i;
                            break;
                        }
                    }
                    rowStart = num5;
                    int count = GetDataContext().Rows.Count;
                    for (int j = rowEnd + 1; j < count; j++)
                    {
                        if (Excel.ActiveSheet.GetActualRowVisible(j, SheetArea.Cells))
                        {
                            rowEnd = j;
                            break;
                        }
                    }
                    int num9 = -1;
                    for (int k = columnStart - 1; k > -1; k--)
                    {
                        if (Excel.ActiveSheet.GetActualColumnVisible(k, SheetArea.Cells))
                        {
                            num9 = k;
                            break;
                        }
                    }
                    columnStart = num9;
                    int num11 = GetDataContext().Columns.Count;
                    for (int m = columnEnd + 1; m < num11; m++)
                    {
                        if (Excel.ActiveSheet.GetActualColumnVisible(m, SheetArea.Cells))
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
            CellItem editingCell = GetViewportCell(_activeRow, _activeCol, true);
            if (editingCell != null)
            {
                if (((_editorPanel == null) || (_editorPanel.EditingColumnIndex != _activeCol)) || ((_editorPanel.EditingRowIndex != _activeRow) || !editingCell.HasEditingElement()))
                {
                    if (_editorPanel.Editor != null)
                    {
                        object obj2 = editingCell.Excel.ActiveSheet.GetValue(_activeRow, _activeCol);
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

