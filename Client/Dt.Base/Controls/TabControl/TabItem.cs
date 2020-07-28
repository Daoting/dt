#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-03-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// TabControl 中的可选标签项
    /// </summary>
    [ContentProperty(Name = "Content")]
    public partial class TabItem : DtControl
    {
        #region 静态内容
        /// <summary>
        /// 头内容
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title",
            typeof(string),
            typeof(TabItem),
            new PropertyMetadata(null));

        /// <summary>
        /// 面板内容的宽度
        /// </summary>
        public static readonly DependencyProperty PopWidthProperty = DependencyProperty.Register(
            "PopWidth",
            typeof(double),
            typeof(TabItem),
            new PropertyMetadata(double.NaN));

        /// <summary>
        /// 面板内容的高度
        /// </summary>
        public static readonly DependencyProperty PopHeightProperty = DependencyProperty.Register(
            "PopHeight",
            typeof(double),
            typeof(TabItem),
            new PropertyMetadata(double.NaN));

        /// <summary>
        /// 当前是否被选择
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
            "IsSelected",
            typeof(bool),
            typeof(TabItem),
            new PropertyMetadata(false, OnIsSelectedChanged));

        /// <summary>
        /// Tab标签位置
        /// </summary>
        public static readonly DependencyProperty TabStripPlacementProperty = DependencyProperty.Register(
            "TabStripPlacement",
            typeof(ItemPlacement),
            typeof(TabItem),
            new PropertyMetadata(ItemPlacement.Bottom, OnStripPlacementChanged));

        /// <summary>
        /// 标签内容
        /// </summary>
        public readonly static DependencyProperty ContentProperty = DependencyProperty.Register(
            "Content",
            typeof(object),
            typeof(TabItem),
            new PropertyMetadata(null, OnContentPropertyChanged));

        static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabItem item = (TabItem)d;
            item.ChangeVisualState();
            if (item.Owner != null)
                item.Owner.NotifyIsSelectedChanged(item, item.IsSelected);
        }

        static void OnStripPlacementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabItem item = (TabItem)d;
            if (item._isLoaded)
                item.ApplyStripPlacement();
        }

        static void OnContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TabItem)d).OnContentChanged();
        }
        #endregion

        #region 成员变量
        uint? _pointerID;
        MouseVisualState _state;
        protected bool _isLoaded;

        RotateContent _header;
        Rectangle _rcOuter;
        Rectangle _rcSelected;
        Rectangle _rcLine;
        #endregion

        #region 构造方法
        public TabItem()
        {
            DefaultStyleKey = typeof(TabItem);
        }
        #endregion

        #region 属性
        /// <summary>
        /// 获取设置标签标题
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// 获取设置弹出面板的宽度
        /// </summary>
        public double PopWidth
        {
            get { return (double)GetValue(PopWidthProperty); }
            set { SetValue(PopWidthProperty, value); }
        }

        /// <summary>
        /// 获取设置弹出面板的高度
        /// </summary>
        public double PopHeight
        {
            get { return (double)GetValue(PopHeightProperty); }
            set { SetValue(PopHeightProperty, value); }
        }

        /// <summary>
        /// 获取设置当前标签是否被选择
        /// </summary>
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        /// <summary>
        /// 获取标签位置
        /// </summary>
        public ItemPlacement TabStripPlacement
        {
            get { return (ItemPlacement)GetValue(TabStripPlacementProperty); }
            internal set { SetValue(TabStripPlacementProperty, value); }
        }

        /// <summary>
        /// 获取设置标签内容
        /// 未使用ContentControl因为样式中有多个ContentPresenter时有bug！
        /// </summary>
        public object Content
        {
            get { return GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        /// <summary>
        /// 获取所属TabControl
        /// </summary>
        public TabControl Owner { get; internal set; }
        #endregion

        #region 加载过程
        protected override void OnLoadTemplate()
        {
            _isLoaded = true;
            _header = (RotateContent)GetTemplateChild("ElementHeader");
            _rcOuter = (Rectangle)GetTemplateChild("NormalBorder");
            _rcSelected = (Rectangle)GetTemplateChild("SelectedBorder");
            _rcLine = (Rectangle)GetTemplateChild("ElementLine");
            ApplyStripPlacement();
            ChangeVisualState();
        }
        #endregion

        #region 重写方法
        /// <summary>
        /// 鼠标进入状态
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            base.OnPointerEntered(e);
            if (Owner != null && !IsSelected && _pointerID == null)
            {
                _state = MouseVisualState.Enter;
                ChangeVisualState();
            }
        }

        /// <summary>
        /// 鼠标按下
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            base.OnPointerPressed(e);
            if (Owner != null && CapturePointer(e.Pointer))
            {
                e.Handled = true;
                _pointerID = e.Pointer.PointerId;
                _state = MouseVisualState.Pressed;
                Focus(FocusState.Programmatic);

                if (!IsSelected)
                    IsSelected = true;
                else if (Owner.AllowSwapItem)
                    ChangeVisualState();
            }
        }

        /// <summary>
        /// 拖动过程
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPointerMoved(PointerRoutedEventArgs e)
        {
            base.OnPointerMoved(e);
            if (_pointerID == e.Pointer.PointerId
                && Owner != null
                && Owner.AllowSwapItem)
            {
                e.Handled = true;
                if (!Owner.DoSwap(this, e.GetCurrentPoint(null).Position))
                {
                    // 不在有效区域
                    ReleaseCapture();
                    OnStartDrag(e);
                }
            }
        }

        /// <summary>
        /// 鼠标释放
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            base.OnPointerReleased(e);
            if (_pointerID != e.Pointer.PointerId || Owner == null)
                return;

            e.Handled = true;
            ReleaseCapture();
        }

        /// <summary>
        /// 鼠标离开
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            base.OnPointerExited(e);
            if (Owner != null && _pointerID == null)
            {
                _state = MouseVisualState.Normal;
                ChangeVisualState();
            }
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 切换内容
        /// </summary>
        protected virtual void OnContentChanged()
        {
            if (Owner != null && IsSelected)
                Owner.SelectedContent = Content;
        }

        /// <summary>
        /// 开始拖动标签
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnStartDrag(PointerRoutedEventArgs e)
        {
        }

        /// <summary>
        /// 释放拖拽
        /// </summary>
        void ReleaseCapture()
        {
            ReleasePointerCaptures();
            _pointerID = null;
            _state = MouseVisualState.Normal;
            ChangeVisualState();
        }

        /// <summary>
        /// 调整标签位置，放在样式状态组中造成多次重新测量布局！！！
        /// </summary>
        void ApplyStripPlacement()
        {
            if (TabStripPlacement == ItemPlacement.Left)
            {
                _header.Rotate = ContentRotate.RotatedTop;
                _rcOuter.Margin = new Thickness(-1, 0, 0, -1);
                _rcSelected.Margin = new Thickness(0, 1, 1, -1);
                _rcLine.VerticalAlignment = VerticalAlignment.Top;
            }
            else if (TabStripPlacement == ItemPlacement.Right)
            {
                _header.Rotate = ContentRotate.RotatedBottom;
                _rcOuter.Margin = new Thickness(0, 0, -1, -1);
                _rcSelected.Margin = new Thickness(1, 1, 0, -1);
                _rcLine.VerticalAlignment = VerticalAlignment.Top;
            }
            else if (TabStripPlacement == ItemPlacement.Bottom)
            {
                _header.ClearValue(RotateContent.RotateProperty);
                _rcOuter.Margin = new Thickness(0, -1, -1, 0);
                if (Owner != null && Owner.IsOutlookStyle)
                    _rcSelected.Margin = new Thickness(1, 0, 0, 1);
                else
                    _rcSelected.Margin = new Thickness(1, -1, 0, 1);
                _rcLine.VerticalAlignment = VerticalAlignment.Bottom;
            }
            else
            {
                _header.ClearValue(RotateContent.RotateProperty);
                _rcOuter.Margin = new Thickness(0, 0, -1, -1);
                if (Owner != null && Owner.IsOutlookStyle)
                    _rcSelected.Margin = new Thickness(1, 1, 0, 0);
                else
                    _rcSelected.Margin = new Thickness(1, 1, 0, -1);
                _rcLine.VerticalAlignment = VerticalAlignment.Top;
            }
        }

        /// <summary>
        /// 切换状态
        /// </summary>
        void ChangeVisualState()
        {
            if (_state == MouseVisualState.Enter)
            {
                VisualStateManager.GoToState(this, "PointerOver", true);
            }
            else if (_state == MouseVisualState.Pressed)
            {
                VisualStateManager.GoToState(this, "Pressed", true);
            }
            else
            {
                if (IsSelected)
                    VisualStateManager.GoToState(this, "Selected", true);
                else
                    VisualStateManager.GoToState(this, "UnSelected", true);
            }
        }
        #endregion

        enum MouseVisualState
        {
            /// <summary>
            /// 普遍状态
            /// </summary>
            Normal,

            /// <summary>
            /// 鼠标进入
            /// </summary>
            Enter,

            /// <summary>
            /// 按下
            /// </summary>
            Pressed
        }
    }
}
