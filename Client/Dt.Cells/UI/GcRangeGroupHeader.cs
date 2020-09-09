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
using Dt.Cells.UndoRedo;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Cells.UI
{
    internal partial class GcRangeGroupHeader : GcGroupBase
    {
        List<GroupHeaderButton> _headButtons;
        int _maxLevel = -1;
        double _lastSize = 0;

        public GcRangeGroupHeader(Excel p_excel) : base(p_excel)
        {
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            int maxLevel = GetMaxLevel(Orientation);
            double size = CalcMinWidthOrHeight(availableSize, Orientation);
            if (maxLevel == -1 || size == 0.0)
            {
                _maxLevel = -1;
                _lastSize = 0;
                return availableSize;
            }

            if (_maxLevel == maxLevel && _lastSize == size)
            {
                foreach (UIElement elem in Children)
                {
                    elem.Measure(availableSize);
                }
                return availableSize;
            }

            _maxLevel = maxLevel;
            _lastSize = size;
            Children.Clear();
            if ((_headButtons != null) && (_headButtons.Count > 0))
            {
                foreach (GroupHeaderButton rbp in _headButtons)
                {
                    rbp.Click -= GroupHeaderButton_Click;
                }
            }

            _headButtons = new List<GroupHeaderButton>();
            for (int i = 0; i < maxLevel + 2; i++)
            {
                GroupHeaderButton presenter = new GroupHeaderButton();
                presenter.Click += GroupHeaderButton_Click;
                presenter.Level = (i + 1).ToString();
                presenter.Height = size;
                presenter.Width = size;
                Children.Add(presenter);
                _headButtons.Add(presenter);
            }

            MeasureBorderLines(availableSize);
            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (_maxLevel == -1 || _headButtons == null)
                return finalSize;

            double num3;
            double num4;
            if (Orientation == Windows.UI.Xaml.Controls.Orientation.Horizontal)
            {
                num3 = Location.X + 2.0;
                num4 = Location.Y + Math.Max(0.0, (finalSize.Height - _lastSize) / 2.0);
                using (List<GroupHeaderButton>.Enumerator enumerator = _headButtons.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        enumerator.Current.Arrange(new Rect(PointToClient(new Point(num3, num4)), new Size(_lastSize, _lastSize)));
                        num3 += _lastSize;
                    }
                }
            }
            else if (Orientation == Windows.UI.Xaml.Controls.Orientation.Vertical)
            {
                num3 = Location.X + Math.Max(0.0, (finalSize.Width - _lastSize) / 2.0);
                num4 = Location.Y + 2.0;
                using (List<GroupHeaderButton>.Enumerator enumerator2 = _headButtons.GetEnumerator())
                {
                    while (enumerator2.MoveNext())
                    {
                        enumerator2.Current.Arrange(new Rect(PointToClient(new Point(num3, num4)), new Size(_lastSize, _lastSize)));
                        num4 += _lastSize;
                    }
                }
            }
            return finalSize;
        }

        void GroupHeaderButton_Click(object sender, RoutedEventArgs e)
        {
            GroupHeaderButton presenter = sender as GroupHeaderButton;
            Worksheet sheet = _excel.ActiveSheet;
            if (((presenter != null) && (sheet != null)) && !_excel.IsEditing)
            {
                int level = int.Parse(presenter.Level) - 1;
                if (level >= 0)
                {
                    _excel.InvalidateRangeGroup();
                    if (Orientation == Windows.UI.Xaml.Controls.Orientation.Horizontal)
                    {
                        RowGroupHeaderExpandExtent rowGroupHeaderExpandExtent = new RowGroupHeaderExpandExtent(level);
                        if (!_excel.RaiseRangeGroupStateChanging(true, -1, level))
                        {
                            RowGroupHeaderExpandUndoAction command = new RowGroupHeaderExpandUndoAction(sheet, rowGroupHeaderExpandExtent);
                            _excel.DoCommand(command);
                            _excel.RaiseRangeGroupStateChanged(true, -1, level);
                        }
                    }
                    else if (Orientation == Windows.UI.Xaml.Controls.Orientation.Vertical)
                    {
                        ColumnGroupHeaderExpandExtent columnGroupHeaderExpandExtent = new ColumnGroupHeaderExpandExtent(level);
                        ColumnGroupHeaderExpandUndoAction action2 = new ColumnGroupHeaderExpandUndoAction(sheet, columnGroupHeaderExpandExtent);
                        if (!_excel.RaiseRangeGroupStateChanging(false, -1, level))
                        {
                            _excel.DoCommand(action2);
                            _excel.RaiseRangeGroupStateChanged(false, -1, level);
                        }
                    }
                }
            }
        }

        public Windows.UI.Xaml.Controls.Orientation Orientation { get; set; }
    }
}

