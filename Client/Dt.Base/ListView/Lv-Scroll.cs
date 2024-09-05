#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-11-15 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.ListView;
using Microsoft.UI.Xaml;
using Windows.Foundation;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 滚动
    /// </summary>
    public partial class Lv
    {
        public static readonly DependencyProperty AutoScrollBottomProperty = DependencyProperty.Register(
            "AutoScrollBottom",
            typeof(bool),
            typeof(Lv),
            new PropertyMetadata(false));

        /// <summary>
        /// 获取设置数据变化时是否自动滚动到最底端，默认false
        /// </summary>
        public bool AutoScrollBottom
        {
            get { return (bool)GetValue(AutoScrollBottomProperty); }
            set { SetValue(AutoScrollBottomProperty, value); }
        }

        /// <summary>
        /// 滚动到最顶端
        /// </summary>
        public void ScrollTop()
        {
            if (_panel != null)
                _panel.ScrollInto(0);
        }

        /// <summary>
        /// 滚动到最底端
        /// </summary>
        public void ScrollBottom()
        {
            if (_panel != null)
                _panel.ScrollInto(Rows.Count - 1);
        }

        /// <summary>
        /// 将指定行滚动到可视区域
        /// </summary>
        /// <param name="p_index">行索引</param>
        public void ScrollInto(int p_index)
        {
            if (_panel != null)
                _panel.ScrollInto(p_index);
        }

        /// <summary>
        /// 滚动到指定的数据行
        /// </summary>
        /// <param name="p_row"></param>
        public void ScrollInto(object p_row)
        {
            if (_panel != null)
            {
                for (int i = 0; i < _rows.Count; i++)
                {
                    if (_rows[i].Data == p_row)
                    {
                        _panel.ScrollInto(i);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 点击分组导航头链接，滚动到指定的分组
        /// </summary>
        /// <param name="p_group"></param>
        internal void ScrollIntoGroup(GroupRow p_group)
        {
            if (IsInnerScroll)
            {
                // 启用动画会界面抖动！
                // 16为分组行上部的间隔高度
                Scroll.ChangeView(null, p_group.IsFirst ? 0 : p_group.Top + LvPanel.GroupSeparatorHeight, null, true);
            }
            else
            {
                // 不能用p_group计算相对位置，因不可见时被布局在空区域
                var pt = _panel.TransformToVisual(Scroll).TransformPoint(new Point());
                double y = Scroll.VerticalOffset + pt.Y + p_group.Top;
                if (!p_group.IsFirst)
                    y += LvPanel.GroupSeparatorHeight;
                Scroll.ChangeView(null, y, null, true);
            }
        }
    }
}