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
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
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
        Size _arrangeSize = new Size();
        List<double> _cachedTabsWidth = null;
        int _startIndex = 0;
        const double _OVERLAP_OFFSET = 4.0;
        static Rect _rcEmpty = new Rect();

        internal event EventHandler<PropertyChangedEventArgs> PropertyChanged;

        public TabsPresenter()
        {
            Margin = new Thickness(10, 0, 0, 0);
        }

        internal int GetNextVisibleIndex(int tabIndex)
        {
            if ((tabIndex >= 0) && (tabIndex <= (Children.Count - 1)))
            {
                for (int i = tabIndex + 1; i < Children.Count; i++)
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
            if ((tabIndex >= 0) && (tabIndex <= (Children.Count - 1)))
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
            if (((Children.Count != 0) && (tabIndex >= 0)) && (tabIndex < Children.Count))
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
                        if (i != (Children.Count - 1))
                        {
                            num5 -= Math.Max((double)0.0, (double)(_arrangeSize.Height - 4.0));
                        }
                        num2 += num5;
                        int preVisibleIndex = GetPreVisibleIndex(i);
                        if (((preVisibleIndex == -1) && (num2 > width)) || ((preVisibleIndex != -1) && ((num2 + 8.0) > width)))
                        {
                            startIndex = Math.Min((int)(Children.Count - 1), (int)(i + 1));
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

        List<double> GetTabsWidth()
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

        void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        internal bool ReCalculateStartIndex(int startViewIndex, int endViewIndex)
        {
            if (((Children.Count != 0) && (endViewIndex >= 0)) && (endViewIndex < Children.Count))
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
                        if (i != (Children.Count - 1))
                        {
                            num4 -= Math.Max((double)0.0, (double)(_arrangeSize.Height - 4.0));
                        }
                        num2 += num4;
                        int preVisibleIndex = GetPreVisibleIndex(i);
                        if (((preVisibleIndex == -1) && (num2 > width)) || ((preVisibleIndex != -1) && ((num2 + 8.0) > width)))
                        {
                            int num6 = Math.Min((int)(Children.Count - 1), (int)(i + 1));
                            if ((StartIndex != num6) && (num6 < (Children.Count - 1)))
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

        internal int FirstScrollableSheetIndex
        {
            get
            {
                for (int i = 0; i < Children.Count; i++)
                {
                    if ((Children[i] as SheetTab).Visible)
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
                for (int i = Children.Count - 1; i >= 0; i--)
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
            double totalWidth = 0.0;
            double maxHeight = 0.0;
            foreach (var tab in Children.OfType<SheetTab>())
            {
                tab.Measure(availableSize);
                totalWidth += tab.DesiredSize.Width;
                maxHeight = Math.Max(maxHeight, tab.DesiredSize.Height);
                list.Add(tab.DesiredSize.Width);
            }
            _cachedTabsWidth = list;
            totalWidth = Math.Min(totalWidth, availableSize.Width);
            return new Size(totalWidth, Math.Min(maxHeight, availableSize.Height));
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            IsLastSheetVisible = false;
            if ((StartIndex < 0) || (StartIndex >= Children.Count))
            {
                StartIndex = 0;
            }

            if (Children.Count > 0)
            {
                for (int i = 0; (i < (StartIndex - 1)) && (i < Children.Count); i++)
                {
                    (Children[i] as SheetTab).Arrange(_rcEmpty);
                }

                double x = 0.0;
                x += Offset;
                double height = arrangeSize.Height;
                double width;
                int num5 = StartIndex - 1;
                if (num5 >= 0)
                {
                    SheetTab tab = Children[num5] as SheetTab;
                    if (tab.Visible)
                    {
                        width = tab.DesiredSize.Width;
                        tab.Arrange(new Rect(-width, 0.0, width, height));
                        x = 8.0 + Offset;
                    }
                    else
                    {
                        tab.Arrange(_rcEmpty);
                    }
                }

                for (int j = StartIndex; j < Children.Count; j++)
                {
                    SheetTab tab = Children[j] as SheetTab;
                    if (tab.Visible)
                    {
                        width = tab.DesiredSize.Width;
                        tab.Arrange(new Rect(x, 0.0, width, height));
                        x += width;
                    }
                    else
                    {
                        tab.Arrange(_rcEmpty);
                    }

                    if (((x + _cachedTabsWidth[_cachedTabsWidth.Count - 1]) + 4.0) < RenderSize.Width)
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
            Clip = geometry;
            return arrangeSize;
        }
    }
}

