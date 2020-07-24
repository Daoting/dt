#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// hdt 大调整
    /// </summary>
    internal partial class TabsPresenter : Panel
    {
        internal bool IsLastSheetVisible;
        private Size _arrangeSize = new Size();
        private List<double> _cachedTabsWidth = null;
        private int _startIndex = 0;
        private const double _OVERLAP_OFFSET = 4.0;

#if ANDROID || IOS
        new
#endif
        internal event EventHandler<PropertyChangedEventArgs> PropertyChanged;

        internal int GetNextVisibleIndex(int tabIndex)
        {
            if ((tabIndex >= 0) && (tabIndex <= (Count - 1)))
            {
                for (int i = tabIndex + 1; i < Count; i++)
                {
                    if ((base.Children[i] as SheetTab).Visible)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        internal int GetPreVisibleIndex(int tabIndex)
        {
            if ((tabIndex >= 0) && (tabIndex <= (Count - 1)))
            {
                for (int i = tabIndex - 1; i >= 0; i--)
                {
                    if ((base.Children[i] as SheetTab).Visible)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        internal int GetStartIndexToBringTabIntoView(int tabIndex)
        {
            if (((Count != 0) && (tabIndex >= 0)) && (tabIndex < Count))
            {
                if (tabIndex <= StartIndex)
                {
                    return tabIndex;
                }
                double width = _arrangeSize.Width;
                double num2 = 0.0;
                List<double> tabsWidth = GetTabsWidth();
                int startIndex = StartIndex;
                for (int i = tabIndex; i >= 0; i--)
                {
                    if ((base.Children[i] as SheetTab).Visible)
                    {
                        double num5 = 0.0;
                        if ((i >= 0) && (i < tabsWidth.Count))
                        {
                            num5 = tabsWidth[i];
                        }
                        if (i != (Count - 1))
                        {
                            num5 -= Math.Max((double)0.0, (double)(_arrangeSize.Height - 4.0));
                        }
                        num2 += num5;
                        int preVisibleIndex = GetPreVisibleIndex(i);
                        if (((preVisibleIndex == -1) && (num2 > width)) || ((preVisibleIndex != -1) && ((num2 + 8.0) > width)))
                        {
                            startIndex = Math.Min((int)(Count - 1), (int)(i + 1));
                            break;
                        }
                    }
                }
                if (startIndex > StartIndex)
                {
                    return startIndex;
                }
            }
            return StartIndex;
        }

        private List<double> GetTabsWidth()
        {
            if (_cachedTabsWidth == null)
            {
                _cachedTabsWidth = new List<double>();
                foreach (SheetTab tab in base.Children)
                {
                    if ((tab.DesiredSize.Width == 0.0) && (tab.DesiredSize.Height == 0.0))
                    {
                        tab.Measure(new Size(double.PositiveInfinity, _arrangeSize.Height));
                    }
                    _cachedTabsWidth.Add(tab.DesiredSize.Width);
                }
            }
            return _cachedTabsWidth;
        }

        internal void NavigateToFirst()
        {
            if ((StartIndex != 0) && (Count > 0))
            {
                StartIndex = 0;
                if (!(base.Children[0] as SheetTab).Visible)
                {
                    int nextVisibleIndex = GetNextVisibleIndex(0);
                    if (nextVisibleIndex != -1)
                    {
                        StartIndex = nextVisibleIndex;
                    }
                }
                base.InvalidateMeasure();
                base.InvalidateArrange();
            }
        }

        internal void NavigateToLast()
        {
            if (ReCalculateStartIndex(0, Count - 1))
            {
                base.InvalidateMeasure();
                base.InvalidateArrange();
            }
        }

        internal void NavigateToNext()
        {
            if (StartIndex < (Count - 1))
            {
                int startIndex = StartIndex;
                if (ReCalculateStartIndex(0, Count - 1))
                {
                    int nextVisibleIndex = GetNextVisibleIndex(startIndex);
                    if (nextVisibleIndex != -1)
                    {
                        StartIndex = nextVisibleIndex;
                        base.InvalidateMeasure();
                        base.InvalidateArrange();
                    }
                }
            }
        }

        internal void NavigateToPrevious()
        {
            if (StartIndex > 0)
            {
                int preVisibleIndex = GetPreVisibleIndex(StartIndex);
                if (preVisibleIndex != -1)
                {
                    StartIndex = preVisibleIndex;
                    base.InvalidateMeasure();
                    base.InvalidateArrange();
                }
            }
        }

#if ANDROID  || IOS
        new
#endif
        private void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        internal bool ReCalculateStartIndex(int startViewIndex, int endViewIndex)
        {
            if (((Count != 0) && (endViewIndex >= 0)) && (endViewIndex < Count))
            {
                double width = _arrangeSize.Width;
                double num2 = 0.0;
                List<double> tabsWidth = GetTabsWidth();
                for (int i = endViewIndex; i >= startViewIndex; i--)
                {
                    if ((base.Children[i] as SheetTab).Visible)
                    {
                        double num4 = 0.0;
                        if ((i >= 0) && (i < tabsWidth.Count))
                        {
                            num4 = tabsWidth[i];
                        }
                        if (i != (Count - 1))
                        {
                            num4 -= Math.Max((double)0.0, (double)(_arrangeSize.Height - 4.0));
                        }
                        num2 += num4;
                        int preVisibleIndex = GetPreVisibleIndex(i);
                        if (((preVisibleIndex == -1) && (num2 > width)) || ((preVisibleIndex != -1) && ((num2 + 8.0) > width)))
                        {
                            int num6 = Math.Min((int)(Count - 1), (int)(i + 1));
                            if ((StartIndex != num6) && (num6 < (Count - 1)))
                            {
                                StartIndex = num6;
                                return true;
                            }
                            return false;
                        }
                    }
                }
            }
            return false;
        }

        internal void SetStartSheet(int startIndex)
        {
            if (startIndex != StartIndex)
            {
                _startIndex = startIndex;
                base.InvalidateMeasure();
                base.InvalidateArrange();
            }
        }

        internal void Update()
        {
            _cachedTabsWidth = null;
        }

        internal int Count
        {
            get { return base.Children.Count; }
        }

        internal int FirstScrollableSheetIndex
        {
            get
            {
                for (int i = 0; i < Count; i++)
                {
                    if ((base.Children[i] as SheetTab).Visible)
                    {
                        return i;
                    }
                }
                return StartIndex;
            }
        }

        internal double FirstSheetTabWidth
        {
            get { return Math.Max((double)0.0, (double)(GetTabsWidth()[StartIndex] - 22.0)); }
        }

        internal int LastScrollableSheetIndex
        {
            get
            {
                for (int i = Count - 1; i >= 0; i--)
                {
                    if ((base.Children[i] as SheetTab).Visible)
                    {
                        return i;
                    }
                }
                return FirstScrollableSheetIndex;
            }
        }

        internal double Offset { get; set; }

        internal int StartIndex
        {
            get { return _startIndex; }
            set
            {
                if (_startIndex != value)
                {
                    _startIndex = value;
                    RaisePropertyChanged("StartIndex");
                }
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            List<double> list = new List<double>();
            double num = 0.0;
            double num2 = 0.0;
            Size size = availableSize;
            size.Width = double.PositiveInfinity;
            int count = Count;
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    SheetTab tab = base.Children[i] as SheetTab;
                    tab.InvalidateMeasure();
                    tab.Measure(size);
                    num += tab.DesiredSize.Width;
                    num2 = Math.Max(num2, tab.DesiredSize.Height);
                    list.Add(tab.DesiredSize.Width);
                }
            }
            _cachedTabsWidth = list;
            num = Math.Min(num, availableSize.Width);
            return new Size(num, Math.Min(num2, availableSize.Height));
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            IsLastSheetVisible = false;
            if ((StartIndex < 0) || (StartIndex >= Count))
            {
                StartIndex = 0;
            }
            if (Count > 0)
            {
                for (int i = 0; (i < (StartIndex - 1)) && (i < Count); i++)
                {
                    Rect rect = new Rect();
                    (base.Children[i] as SheetTab).Arrange(rect);
                }
                double x = 0.0;
                x += Offset;
                double height = arrangeSize.Height;
                double width = 0.0;
                int num5 = StartIndex - 1;
                if (num5 >= 0)
                {
                    SheetTab tab = base.Children[num5] as SheetTab;
                    if (tab.Visible)
                    {
                        width = tab.DesiredSize.Width;
                        tab.Arrange(new Rect(-width, 0.0, width, height));
                        x = 8.0 + Offset;
                    }
                    else
                    {
                        tab.Arrange(new Rect());
                    }
                }
                for (int j = StartIndex; j < Count; j++)
                {
                    SheetTab tab2 = base.Children[j] as SheetTab;
                    if (tab2.Visible)
                    {
                        width = tab2.DesiredSize.Width;
                        tab2.Arrange(new Rect(x, 0.0, width, height));
                        x += width;
                    }
                    else
                    {
                        Rect rect3 = new Rect();
                        tab2.Arrange(rect3);
                    }
                    if (((x + _cachedTabsWidth[_cachedTabsWidth.Count - 1]) + 4.0) < base.RenderSize.Width)
                    {
                        IsLastSheetVisible = true;
                    }
                    else
                    {
                        IsLastSheetVisible = false;
                    }
                }
            }
            _arrangeSize = arrangeSize;
            RectangleGeometry geometry = new RectangleGeometry();
            geometry.Rect = new Rect(0.0, 0.0, arrangeSize.Width, arrangeSize.Height);
            base.Clip = geometry;
            return arrangeSize;
        }
    }
}

