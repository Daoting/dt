#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-08-05 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Cells.UI
{
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
}

