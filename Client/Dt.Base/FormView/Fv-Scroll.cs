#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-10-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.FormView;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Input;
using Windows.System;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 滚动相关
    /// </summary>
    public partial class Fv
    {
        /// <summary>
        /// 滚动到最顶端
        /// </summary>
        public void ScrollTop()
        {
            if (_panel.Children.Count > 0)
                ScrollInto((FrameworkElement)_panel.Children[0]);
        }

        /// <summary>
        /// 滚动到最底端
        /// </summary>
        public void ScrollBottom()
        {
            // 末尾为边框
            if (_panel.Children.Count > 1)
                ScrollInto((FrameworkElement)_panel.Children[_panel.Children.Count - 2]);
        }

        /// <summary>
        /// 将指定格滚动到可视区域
        /// </summary>
        /// <param name="p_index">格索引</param>
        public void ScrollInto(int p_index)
        {
            if (p_index >= 0 && p_index < _panel.Children.Count)
                ScrollInto((FrameworkElement)_panel.Children[p_index]);
        }

        /// <summary>
        /// 将指定单元格滚动到可视范围
        /// </summary>
        /// <param name="p_elem"></param>
        public void ScrollInto(FrameworkElement p_elem)
        {
            if (_scroll == null || p_elem == null)
                return;

            // 单元格相对面板位置
            Point pt = p_elem.TransformToVisual(_panel).TransformPoint(new Point());
            if (_scroll.Content as Panel == _panel)
            {
                // 内部滚动栏
                if (pt.Y < _scroll.VerticalOffset)
                {
                    // 超出上部
                    _scroll.ChangeView(null, pt.Y, null);
                }
                else if ((pt.Y + p_elem.ActualHeight) > (_scroll.VerticalOffset + _scroll.ViewportHeight))
                {
                    // 超出下部
                    _scroll.ChangeView(null, pt.Y + p_elem.ActualHeight - _scroll.ViewportHeight, null);
                }
            }
            else
            {
                // 外部滚动栏
                // 面板相对滚动栏位置
                Point ptScroll = _panel.TransformToVisual(_scroll).TransformPoint(new Point());
                if (pt.Y + ptScroll.Y < 0)
                {
                    // 超出上部
                    _scroll.ChangeView(null, pt.Y + ptScroll.Y + _scroll.VerticalOffset, null);
                }
                else if (pt.Y + ptScroll.Y + p_elem.ActualHeight > _scroll.ViewportHeight)
                {
                    // 超出下部
                    _scroll.ChangeView(null, pt.Y + ptScroll.Y + p_elem.ActualHeight + _scroll.VerticalOffset - _scroll.ViewportHeight, null);
                }
            }
        }
    }
}