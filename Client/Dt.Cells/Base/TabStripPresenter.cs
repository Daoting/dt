#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.UI
{
    internal partial class TabStripPresenter : Panel
    {
        private TabStripNavigator _navigationBar = new TabStripNavigator();
        private TabsPresenter _tabPresenter;
        private const double _TABSTRIPNAVIGATOR_WIDTH = 80.0;

        public TabStripPresenter()
        {
            this._navigationBar.ButtonClick += new EventHandler<NavigationButtonClickEventArgs>(this.NavigationBar_ButtonClick);
            base.Children.Add(this._navigationBar);
            this._tabPresenter = new TabsPresenter();
            this._tabPresenter.HorizontalAlignment = HorizontalAlignment.Stretch;
            base.Children.Add(this._tabPresenter);
        }

        protected override Windows.Foundation.Size ArrangeOverride(Windows.Foundation.Size finalSize)
        {
            double width = finalSize.Width;
            double x = 0.0;
            TabStripNavigator navigationBar = this.NavigationBar;
            if (navigationBar != null)
            {
                double num3 = navigationBar.DesiredSize.Width;
                RectangleGeometry geometry = new RectangleGeometry();
                geometry.Rect = new Windows.Foundation.Rect(new Windows.Foundation.Point(), new Windows.Foundation.Size(Math.Min(num3, finalSize.Width), finalSize.Height));
                navigationBar.Clip = geometry;
                this.NavigationBar.Arrange(new Windows.Foundation.Rect(x, 0.0, num3, finalSize.Height));
                width -= num3;
                x += num3;
            }
            TabsPresenter tabPresenter = this.TabPresenter;
            if (tabPresenter != null)
            {
                tabPresenter.Arrange(new Windows.Foundation.Rect(x, 0.0, Math.Max(width, 0.0), finalSize.Height));
            }
            return finalSize;
        }

        internal int GetStartIndexToBringTabIntoView(int tabIndex)
        {
            return this._tabPresenter.GetStartIndexToBringTabIntoView(tabIndex);
        }

        protected override Windows.Foundation.Size MeasureOverride(Windows.Foundation.Size availableSize)
        {
            Windows.Foundation.Size size = new Windows.Foundation.Size();
            TabStripNavigator navigationBar = this.NavigationBar;
            navigationBar.InvalidateMeasure();
            navigationBar.Measure(new Windows.Foundation.Size(80.0, availableSize.Height));
            size.Height = Math.Max(size.Height, navigationBar.DesiredSize.Height);
            size.Width += navigationBar.DesiredSize.Width;
            TabsPresenter tabPresenter = this.TabPresenter;
            tabPresenter.InvalidateMeasure();
            tabPresenter.Measure(new Windows.Foundation.Size(Math.Max((double) 0.0, (double) (availableSize.Width - 80.0)), availableSize.Height));
            size.Height = Math.Max(size.Height, navigationBar.DesiredSize.Height);
            size.Width += navigationBar.DesiredSize.Width;
            size.Width = Math.Min(size.Width, availableSize.Width);
            size.Height = Math.Min(size.Height, availableSize.Height);
            return size;
        }

        private void NavigationBar_ButtonClick(object sender, NavigationButtonClickEventArgs e)
        {
            switch (e.TabButton)
            {
                case ButtonType.First:
                    this._tabPresenter.NavigateToFirst();
                    return;

                case ButtonType.Previous:
                    this._tabPresenter.NavigateToPrevious();
                    return;

                case ButtonType.Next:
                    this._tabPresenter.NavigateToNext();
                    return;

                case ButtonType.Last:
                    this._tabPresenter.NavigateToLast();
                    return;
            }
        }

        public TabStripNavigator NavigationBar
        {
            get { return  this._navigationBar; }
        }

        public TabsPresenter TabPresenter
        {
            get { return  this._tabPresenter; }
        }
    }
}

