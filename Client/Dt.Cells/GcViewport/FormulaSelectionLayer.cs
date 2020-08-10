#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-08-05 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Cells.UI
{
    internal partial class FormulaSelectionLayer : Panel
    {
        DispatcherTimer _timer;

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (FormulaSelectionFrame frame in base.Children)
            {
                CellRange range = frame.SelectionItem.Range;
                int row = (range.Row < 0) ? 0 : range.Row;
                int column = (range.Column < 0) ? 0 : range.Column;
                int rowCount = (range.RowCount < 0) ? ParentViewport.Sheet.ActiveSheet.RowCount : range.RowCount;
                int columnCount = (range.ColumnCount < 0) ? ParentViewport.Sheet.ActiveSheet.ColumnCount : range.ColumnCount;
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
                int rowCount = (range.RowCount < 0) ? ParentViewport.Sheet.ActiveSheet.RowCount : range.RowCount;
                int columnCount = (range.ColumnCount < 0) ? ParentViewport.Sheet.ActiveSheet.ColumnCount : range.ColumnCount;
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

        public CellsPanel ParentViewport { get; set; }
    }
}

