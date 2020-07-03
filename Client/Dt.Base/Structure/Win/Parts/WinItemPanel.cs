#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-03-03 创建
******************************************************************************/
#endregion

#region 引用命名
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
        Control _centerItem = null;

        /// <summary>
        /// 获取采用填充方式的WinItem
        /// </summary>
        public Control CenterItem
        {
            get { return _centerItem; }
            set
            {
                if (_centerItem != value && value != null)
                {
                    _centerItem = value;
                    if (!Children.Contains(_centerItem))
                    {
                        _centerItem.Width = _centerItem.Height = double.NaN;
                        Children.Add(_centerItem);
                    }
                }
            }
        }

        /// <summary>
        /// 清空除中部区域的子元素
        /// </summary>
        internal void Clear()
        {
            Children.Clear();
            if (_centerItem != null)
                Children.Add(_centerItem);
        }

        /// <summary>
        /// 测量
        /// </summary>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            var otherChildren = from item in Children.OfType<WinItem>()
                                where item != _centerItem
                                select item;
            foreach (WinItem item in otherChildren)
            {
                switch (item.DockState)
                {
                    case WinItemState.DockedLeft:
                    case WinItemState.DockedRight:
                        {
                            if (double.IsNaN(item.Width))
                            {
                                if (double.IsNaN(item.InitWidth) && !double.IsInfinity(availableSize.Width))
                                    item.Width = availableSize.Width / 2;
                                else
                                    item.Width = item.InitWidth;
                            }
                            continue;
                        }
                    case WinItemState.DockedTop:
                    case WinItemState.DockedBottom:
                        {
                            if (double.IsNaN(item.Height))
                            {
                                // 未设置初始高度时自动占一半
                                if (double.IsNaN(item.InitHeight) && !double.IsInfinity(availableSize.Height))
                                    item.Height = availableSize.Height / 2;
                                else
                                    item.Height = item.InitHeight;
                            }
                            continue;
                        }
                }
            }

            double num = 0.0;
            double num2 = 0.0;
            double num3 = 0.0;
            double num4 = 0.0;
            int num5 = 0;
            int count = Children.Count;
            while (num5 < count)
            {
                WinItem item = Children[num5] as WinItem;
                if (item != null)
                {
                    Size remainingSize = new Size(Math.Max(0.0, availableSize.Width - num3), Math.Max(0.0, availableSize.Height - num4));
                    item.Measure(remainingSize);
                    Size desiredSize = item.DesiredSize;
                    switch (item.DockState)
                    {
                        case WinItemState.DockedLeft:
                        case WinItemState.DockedRight:
                            num2 = Math.Max(num2, num4 + desiredSize.Height);
                            num3 += desiredSize.Width;
                            break;

                        case WinItemState.DockedTop:
                        case WinItemState.DockedBottom:
                            num = Math.Max(num, num3 + desiredSize.Width);
                            num4 += desiredSize.Height;
                            break;
                    }
                }
                num5++;
            }
            return new Size(Math.Max(num, num3), Math.Max(num2, num4));
        }

        /// <summary>
        /// 布局
        /// </summary>
        /// <param name="finalSize"></param>
        /// <returns></returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            double x = 0.0;
            double y = 0.0;
            double num5 = 0.0;
            double num6 = 0.0;
            foreach (WinItem item in Children.OfType<WinItem>())
            {
                Size desiredSize = item.DesiredSize;
                Rect finalRect = new Rect(x, y, Math.Max(0.0, (finalSize.Width - (x + num5))), Math.Max(0.0, (finalSize.Height - (y + num6))));
                if (item != _centerItem)
                {
                    switch (item.DockState)
                    {
                        case WinItemState.DockedLeft:
                            x += desiredSize.Width;
                            finalRect.Width = desiredSize.Width;
                            break;

                        case WinItemState.DockedTop:
                            y += desiredSize.Height;
                            finalRect.Height = desiredSize.Height;
                            break;

                        case WinItemState.DockedRight:
                            num5 += desiredSize.Width;
                            finalRect.X = Math.Max(0.0, finalSize.Width - num5);
                            finalRect.Width = desiredSize.Width;
                            break;

                        case WinItemState.DockedBottom:
                            num6 += desiredSize.Height;
                            finalRect.Y = Math.Max(0.0, finalSize.Height - num6);
                            finalRect.Height = desiredSize.Height;
                            break;
                    }
                }
                item.Arrange(finalRect);
            }
            return finalSize;
        }
    }
}