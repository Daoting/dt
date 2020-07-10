#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-03-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Base.Docking
{
    /// <summary>
    /// WinItem布局面板，内部子元素为 WinItem
    /// </summary>
    public partial class WinItemPanel : Panel
    {
        // 中部主区，始终在Children的0位置
        WinItem _centerItem;

        internal void Init(WinItem p_centerItem)
        {
            _centerItem = p_centerItem;
            Children.Add(_centerItem);
        }

        /// <summary>
        /// 清空除中部区域的子元素
        /// </summary>
        internal void Clear()
        {
            Children.Clear();
            // 始终在Children的0位置
            if (_centerItem != null)
                Children.Add(_centerItem);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            double width = double.IsInfinity(availableSize.Width) ? SysVisual.ViewWidth : availableSize.Width;
            double height = double.IsInfinity(availableSize.Height) ? SysVisual.ViewWidth - 50 : availableSize.Height;
            double usedLeft = 0.0;
            double usedRight = 0.0;
            double usedTop = 0.0;
            double usedBottom = 0.0;

            // 初始化时Children已按 中左右上下 的顺序，但拖拽调整后顺序打乱，只有_centerItem始终0位置
            for (int i = 1; i < Children.Count; i++)
            {
                WinItem item = (WinItem)Children[i];

                // 根据停靠位置设置WinItem宽或高
                switch (item.DockState)
                {
                    case WinItemState.DockedLeft:
                    case WinItemState.DockedRight:
                        if (double.IsNaN(item.Width))
                        {
                            if (double.IsNaN(item.InitWidth) && !double.IsInfinity(width))
                                item.Width = width / 2;
                            else
                                item.Width = item.InitWidth;
                        }
                        break;
                    case WinItemState.DockedTop:
                    case WinItemState.DockedBottom:
                        if (double.IsNaN(item.Height))
                        {
                            // 未设置初始高度时自动占一半
                            if (double.IsNaN(item.InitHeight) && !double.IsInfinity(height))
                                item.Height = height / 2;
                            else
                                item.Height = item.InitHeight;
                        }
                        break;
                }

                item.Measure(new Size(Math.Max(0.0, width - usedLeft - usedRight), Math.Max(0.0, height - usedTop - usedBottom)));
                switch (item.DockState)
                {
                    case WinItemState.DockedLeft:
                        item.Bounds = new Rect(usedLeft, usedTop, item.Width, height - usedTop - usedBottom);
                        usedLeft += item.DesiredSize.Width;
                        break;
                    case WinItemState.DockedRight:
                        usedRight += item.DesiredSize.Width;
                        item.Bounds = new Rect(Math.Max(0.0, width - usedRight), usedTop, item.Width, height - usedTop - usedBottom);
                        break;
                    case WinItemState.DockedTop:
                        item.Bounds = new Rect(usedLeft, usedTop, width - usedLeft - usedRight, item.Height);
                        usedTop += item.DesiredSize.Height;
                        break;
                    case WinItemState.DockedBottom:
                        usedBottom += item.DesiredSize.Height;
                        item.Bounds = new Rect(usedLeft, Math.Max(0.0, height - usedBottom), width - usedLeft - usedRight, item.Height);
                        break;
                }
            }

            // 中部主区填充剩余区域，始终在Children的0位置
            Size leaveSize = new Size(Math.Max(0.0, width - usedLeft - usedRight), Math.Max(0.0, height - usedTop - usedBottom));
            _centerItem.Measure(leaveSize);
            _centerItem.Bounds = new Rect(new Point(usedLeft, usedTop), leaveSize);
            return new Size(width, height);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            for (int i = 0; i < Children.Count; i++)
            {
                WinItem item = (WinItem)Children[i];
                item.Arrange(item.Bounds);
            }
            return finalSize;
        }
    }
}