#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-06-04 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
#endregion

namespace Dt.Base.ListView
{
    /// <summary>
    /// 分组导航格
    /// </summary>
    public partial class GroupHeaderCell : Control
    {
        #region 静态内容
        public readonly static DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title",
            typeof(string),
            typeof(GroupHeaderCell),
            new PropertyMetadata(""));

        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
            "IsSelected",
            typeof(bool),
            typeof(GroupHeaderCell),
            new PropertyMetadata(false, OnIsSelectedChanged));

        static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // uno中Arrange中修改IsSelected时有多次布局警告
            Kit.RunAsync(() => VisualStateManager.GoToState((GroupHeaderCell)d, ((bool)e.NewValue) ? "Selected" : "Normal", true));
        }
        #endregion

        #region 成员变量
        GroupHeader _owner;
        uint? _pointerID;
        Point _ptLast;
        #endregion

        #region 构造方法
        public GroupHeaderCell(GroupRow p_group, GroupHeader p_owner)
        {
            DefaultStyleKey = typeof(GroupHeaderCell);
            Group = p_group;
            _owner = p_owner;
            Title = p_group.Data.ToString();
        }
        #endregion

        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// 获取当前是否为选择状态
        /// </summary>
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        internal GroupRow Group { get; }

        /// <summary>
        /// 水平位置
        /// </summary>
#if ANDROID
        new
#endif
        internal double Left { get; set; }

        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            if (CapturePointer(e.Pointer))
            {
                e.Handled = true;
                _pointerID = e.Pointer.PointerId;
                _ptLast = e.GetCurrentPoint(null).Position;

                if (e.IsMouse() && !IsSelected)
                    VisualStateManager.GoToState(this, "Pressed", true);
            }
        }

        protected override void OnPointerMoved(PointerRoutedEventArgs e)
        {
            // 触摸模式滚动
            if (e.IsTouch() && _pointerID == e.Pointer.PointerId)
            {
                Point cur = e.GetCurrentPoint(null).Position;
                _owner.DoHorScroll(cur.X - _ptLast.X);
                _ptLast = cur;
            }
        }

        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            if (_pointerID != e.Pointer.PointerId)
                return;

            ReleasePointerCapture(e.Pointer);
            e.Handled = true;
            _pointerID = null;
        }

        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            _owner.Lv.ScrollIntoGroup(Group);
        }

        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            if (e.IsMouse() && !IsSelected)
            {
                e.Handled = true;
                VisualStateManager.GoToState(this, "PointerOver", true);
            }
        }

        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            if (e.IsMouse() && !IsSelected)
            {
                e.Handled = true;
                VisualStateManager.GoToState(this, "Normal", true);
            }
        }
    }
}
