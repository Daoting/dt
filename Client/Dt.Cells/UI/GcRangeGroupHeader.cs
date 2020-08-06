#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
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
        List<RangeGroupHeaderButtonPresenter> _headButtons;

        public GcRangeGroupHeader(SheetView sheetView) : base(sheetView)
        {
        }

        void ArrangeGroupHeader(Size finalSize)
        {
            if (((GetMaxLevel(Orientation) != -1) && (_headButtons != null)) && (_headButtons.Count > 0))
            {
                double width = CalcMinWidthOrHeight(finalSize, Orientation);
                if (width != 0.0)
                {
                    double num3;
                    double num4;
                    if (Orientation == Windows.UI.Xaml.Controls.Orientation.Horizontal)
                    {
                        num3 = base.Location.X + 2.0;
                        num4 = base.Location.Y + Math.Max((double) 0.0, (double) ((finalSize.Height - width) / 2.0));
                        using (List<RangeGroupHeaderButtonPresenter>.Enumerator enumerator = _headButtons.GetEnumerator())
                        {
                            while (enumerator.MoveNext())
                            {
                                enumerator.Current.Arrange(new Rect(base.PointToClient(new Point(num3, num4)), new Size(width, width)));
                                num3 += width;
                            }
                            return;
                        }
                    }
                    if (Orientation == Windows.UI.Xaml.Controls.Orientation.Vertical)
                    {
                        num3 = base.Location.X + Math.Max((double) 0.0, (double) ((finalSize.Width - width) / 2.0));
                        num4 = base.Location.Y + 2.0;
                        using (List<RangeGroupHeaderButtonPresenter>.Enumerator enumerator2 = _headButtons.GetEnumerator())
                        {
                            while (enumerator2.MoveNext())
                            {
                                enumerator2.Current.Arrange(new Rect(base.PointToClient(new Point(num3, num4)), new Size(width, width)));
                                num4 += width;
                            }
                        }
                    }
                }
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            ArrangeGroupHeader(finalSize);
            return base.ArrangeOverride(finalSize);
        }

        void GroupHeaderButton_Click(object sender, RoutedEventArgs e)
        {
            RangeGroupHeaderButtonPresenter presenter = sender as RangeGroupHeaderButtonPresenter;
            Worksheet sheet = base._sheetView.Worksheet;
            if (((presenter != null) && (sheet != null)) && !base._sheetView.IsEditing)
            {
                int level = presenter.Level - 1;
                if (level >= 0)
                {
                    if (Orientation == Windows.UI.Xaml.Controls.Orientation.Horizontal)
                    {
                        RowGroupHeaderExpandExtent rowGroupHeaderExpandExtent = new RowGroupHeaderExpandExtent(level);
                        if (!base._sheetView.RaiseRangeGroupStateChanging(true, -1, level))
                        {
                            RowGroupHeaderExpandUndoAction command = new RowGroupHeaderExpandUndoAction(sheet, rowGroupHeaderExpandExtent);
                            base._sheetView.DoCommand(command);
                            base._sheetView.RaiseRangeGroupStateChanged(true, -1, level);
                        }
                    }
                    else if (Orientation == Windows.UI.Xaml.Controls.Orientation.Vertical)
                    {
                        ColumnGroupHeaderExpandExtent columnGroupHeaderExpandExtent = new ColumnGroupHeaderExpandExtent(level);
                        ColumnGroupHeaderExpandUndoAction action2 = new ColumnGroupHeaderExpandUndoAction(sheet, columnGroupHeaderExpandExtent);
                        if (!base._sheetView.RaiseRangeGroupStateChanging(false, -1, level))
                        {
                            base._sheetView.DoCommand(action2);
                            base._sheetView.RaiseRangeGroupStateChanged(false, -1, level);
                        }
                    }
                }
            }
        }

        void MeasureGroupHeader(Size availableSize)
        {
            int maxLevel = GetMaxLevel(Orientation);
            if (maxLevel != -1)
            {
                double num2 = CalcMinWidthOrHeight(availableSize, Orientation);
                if (num2 != 0.0)
                {
                    int num3 = maxLevel + 2;
                    for (int i = 0; i < num3; i++)
                    {
                        RangeGroupHeaderButtonPresenter presenter = new RangeGroupHeaderButtonPresenter();
                        presenter.Click += GroupHeaderButton_Click;
                        presenter.Level = i + 1;
                        presenter.Height = num2;
                        presenter.Width = num2;
                        base.Children.Add(presenter);
                        _headButtons.Add(presenter);
                    }
                }
            }
        }

        void MeasureInitialization()
        {
            if ((_headButtons != null) && (_headButtons.Count > 0))
            {
                foreach(RangeGroupHeaderButtonPresenter rbp in _headButtons)
                {
                    rbp.Click -= GroupHeaderButton_Click;
                }
            }
            _headButtons = new List<RangeGroupHeaderButtonPresenter>();
            base.Children.Clear();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            MeasureInitialization();
            MeasureGroupHeader(availableSize);
            MeasureBorderLines(availableSize);
            return base.MeasureOverride(availableSize);
        }

        public Windows.UI.Xaml.Controls.Orientation Orientation { get; set; }
    }
}

