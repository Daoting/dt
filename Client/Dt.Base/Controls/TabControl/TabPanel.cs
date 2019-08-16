#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-09-12 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 处理 TabControl 上的 TabItem 对象的布局
    /// </summary>
    public partial class TabPanel : Panel
    {
        TabControl _parentTab;

#if UWP
        protected override Size MeasureOverride(Size availableSize)
        {
            double width = 0.0;
            double num2 = 0.0;
            if (ParentTab != null)
            {
                if ((ParentTab.TabStripPlacement == ItemPlacement.Bottom)
                    || (ParentTab.TabStripPlacement == ItemPlacement.Top))
                {
                    foreach (UIElement element in base.Children)
                    {
                        if (element != null)
                        {
                            element.Measure(new Size(availableSize.Width, availableSize.Height));
                            width += element.DesiredSize.Width;
                            num2 = Math.Max(num2, element.DesiredSize.Height);
                        }
                    }
                    availableSize = new Size(width, num2);
                    return availableSize;
                }
                foreach (UIElement element2 in base.Children)
                {
                    if (element2 != null)
                    {
                        element2.Measure(new Size(availableSize.Width, availableSize.Height));
                        width = Math.Max(width, element2.DesiredSize.Width);
                        num2 += element2.DesiredSize.Height;
                    }
                }
                availableSize = new Size(width, num2);
            }
            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double y = 0.0;
            double x = 0.0;
            if (ParentTab != null)
            {
                if ((ParentTab.TabStripPlacement == ItemPlacement.Bottom) 
                    || (ParentTab.TabStripPlacement == ItemPlacement.Top))
                {
                    for (int j = 0; j < base.Children.Count; j++)
                    {
                        var element = base.Children[j];
                        if (element != null)
                        {
                            element.Arrange(new Rect(x, y, element.DesiredSize.Width, element.DesiredSize.Height));
                            x += element.DesiredSize.Width + 10.0;
                        }
                    }
                    return finalSize;
                }
                for (int i = 0; i < base.Children.Count; i++)
                {
                    UIElement element2 = base.Children[i];
                    if (element2 != null)
                    {
                        element2.Arrange(new Rect(x, y, element2.DesiredSize.Width, element2.DesiredSize.Height));
                        y += element2.DesiredSize.Height + 10.0;
                    }
                }
            }
            return finalSize;
        }
#endif

        /// <summary>
        /// 获取所属的Tab控件
        /// </summary>
        internal TabControl ParentTab
        {
            get
            {
                if (_parentTab == null)
                {
                    _parentTab = ItemsControl.GetItemsOwner(this) as TabControl;
                }
                return _parentTab;
            }
        }
    }
}
