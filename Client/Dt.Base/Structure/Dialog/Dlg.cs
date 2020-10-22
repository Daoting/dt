#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-10-09 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.Docking;
using Dt.Core;
using System;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media.Animation;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 对话框容器
    /// </summary>
    [ContentProperty(Name = "Content")]
    public partial class Dlg : Control, IDlgOuterPressed
    {
        #region 静态成员
        public readonly static DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title",
            typeof(string),
            typeof(Dlg),
           new PropertyMetadata("无标题"));

        public static readonly DependencyProperty MenuProperty = DependencyProperty.Register(
            "Menu",
            typeof(Menu),
            typeof(Dlg),
            new PropertyMetadata(null));

        public readonly static DependencyProperty HideTitleBarProperty = DependencyProperty.Register(
            "HideTitleBar",
            typeof(bool),
            typeof(Dlg),
            new PropertyMetadata(false));

        public static readonly DependencyProperty WinPlacementProperty = DependencyProperty.Register(
            "WinPlacement",
            typeof(DlgPlacement),
            typeof(Dlg),
            new PropertyMetadata(DlgPlacement.CenterScreen, OnWinPlacementChanged));

        public static readonly DependencyProperty PhonePlacementProperty = DependencyProperty.Register(
            "PhonePlacement",
            typeof(DlgPlacement),
            typeof(Dlg),
            new PropertyMetadata(DlgPlacement.Maximized, OnPhonePlacementChanged));

        public static readonly DependencyProperty ClipElementProperty = DependencyProperty.Register(
            "ClipElement",
            typeof(FrameworkElement),
            typeof(Dlg),
            new PropertyMetadata(null));

        public static readonly DependencyProperty PlacementTargetProperty = DependencyProperty.Register(
            "PlacementTarget",
            typeof(FrameworkElement),
            typeof(Dlg),
            new PropertyMetadata(null));

        public readonly static DependencyProperty AutoAdjustPositionProperty = DependencyProperty.Register(
            "AutoAdjustPosition",
            typeof(bool),
            typeof(Dlg),
            new PropertyMetadata(true));

        public static readonly DependencyProperty LeftProperty = DependencyProperty.Register(
            "Left",
            typeof(double),
            typeof(Dlg),
            new PropertyMetadata(0.0, OnLeftPropertyChanged));

        public static readonly DependencyProperty TopProperty = DependencyProperty.Register(
            "Top",
            typeof(double),
            typeof(Dlg),
            new PropertyMetadata(0.0, OnTopPropertyChanged));

        public static readonly DependencyProperty ResizeableProperty = DependencyProperty.Register(
            "Resizeable",
            typeof(bool),
            typeof(Dlg),
            new PropertyMetadata(true));

        public static readonly DependencyProperty IsPinnedProperty = DependencyProperty.Register(
            "IsPinned",
            typeof(bool),
            typeof(Dlg),
            new PropertyMetadata(false));

        public readonly static DependencyProperty ContentProperty = DependencyProperty.Register(
            "Content",
            typeof(object),
            typeof(Dlg),
            new PropertyMetadata(null));

        public static readonly DependencyProperty ShowWinVeilProperty = DependencyProperty.Register(
            "ShowWinVeil",
            typeof(bool),
            typeof(Dlg),
            new PropertyMetadata(false, OnShowWinVeilChanged));

        static void OnWinPlacementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!AtSys.IsPhoneUI)
                ((Dlg)d).OnPlacementChanged();
        }

        static void OnPhonePlacementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (AtSys.IsPhoneUI)
                ((Dlg)d).OnPlacementChanged();
        }

        static void OnLeftPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Canvas.SetLeft((Dlg)d, (double)e.NewValue);
        }

        static void OnTopPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Canvas.SetTop((Dlg)d, (double)e.NewValue);
        }

        static void OnShowWinVeilChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!AtSys.IsPhoneUI)
                SysVisual.ToggleDlgWinVeil((Dlg)d);
        }
        #endregion

        #region 成员变量
        static int _currentZIndex = 1;
        Grid _headerGrid;
        bool _isHeadPressed;
        bool _isResizing;
        ResizeDirection _resizeDirection;
        Point _startPoint;
        Rect _initRect;
        TaskCompletionSource<bool> _taskSrc;
        Border _bdResize;
        bool _isRemoving;
        #endregion

        #region 构造方法
        public Dlg()
        {
            // 两种UI模式样式不同，PhoneUI模式为缺省样式
            if (AtSys.IsPhoneUI)
                DefaultStyleKey = typeof(Dlg);
            else
                Style = (Style)Application.Current.Resources["WinDlg"];
        }
        #endregion

        #region 事件
        /// <summary>
        /// 对话框正在关闭事件，可以取消关闭
        /// </summary>
        public event EventHandler<AsyncCancelEventArgs> Closing;

        /// <summary>
        /// 对话框关闭后事件
        /// </summary>
        public event EventHandler Closed;
        #endregion

        #region 属性
        /// <summary>
        /// 获取设置标题文字
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// 获取设置菜单
        /// </summary>
        public Menu Menu
        {
            get { return (Menu)GetValue(MenuProperty); }
            set { SetValue(MenuProperty, value); }
        }

        /// <summary>
        /// 获取设置是否隐藏标题栏
        /// </summary>
        public bool HideTitleBar
        {
            get { return (bool)GetValue(HideTitleBarProperty); }
            set { SetValue(HideTitleBarProperty, value); }
        }

        /// <summary>
        /// 获取设置windows模式的显示位置，默认居中CenterScreen
        /// </summary>
        public DlgPlacement WinPlacement
        {
            get { return (DlgPlacement)GetValue(WinPlacementProperty); }
            set { SetValue(WinPlacementProperty, value); }
        }

        /// <summary>
        /// 获取设置phone模式的显示位置，默认最大化Maximized
        /// </summary>
        public DlgPlacement PhonePlacement
        {
            get { return (DlgPlacement)GetValue(PhonePlacementProperty); }
            set { SetValue(PhonePlacementProperty, value); }
        }

        /// <summary>
        /// 获取设置要裁剪的元素，在该元素区域点击时不自动关闭（如TabControl中弹出式标签）
        /// </summary>
        public FrameworkElement ClipElement
        {
            get { return (FrameworkElement)GetValue(ClipElementProperty); }
            set { SetValue(ClipElementProperty, value); }
        }

        /// <summary>
        /// 获取设置采用相对位置显示时的目标元素
        /// </summary>
        public FrameworkElement PlacementTarget
        {
            get { return (FrameworkElement)GetValue(PlacementTargetProperty); }
            set { SetValue(PlacementTargetProperty, value); }
        }

        /// <summary>
        /// 获取设置是否自动调整最终显示位置，如相对目标显示时对话框在不可见区域时自动调整，默认true
        /// </summary>
        public bool AutoAdjustPosition
        {
            get { return (bool)GetValue(AutoAdjustPositionProperty); }
            set { SetValue(AutoAdjustPositionProperty, value); }
        }

        /// <summary>
        /// 获取设置面板x位置
        /// </summary>
#if ANDROID
        new
#endif
        public double Left
        {
            get { return (double)GetValue(LeftProperty); }
            set { SetValue(LeftProperty, value); }
        }

        /// <summary>
        /// 获取设置面板y位置
        /// </summary>
#if ANDROID
        new
#endif
        public double Top
        {
            get { return (double)GetValue(TopProperty); }
            set { SetValue(TopProperty, value); }
        }

        /// <summary>
        /// 获取设置是否固定对话框，固定时点击外部不自动关闭，默认为false
        /// </summary>
        public bool IsPinned
        {
            get { return (bool)GetValue(IsPinnedProperty); }
            set { SetValue(IsPinnedProperty, value); }
        }

        /// <summary>
        /// 获取设置是否可调节大小，默认为true
        /// </summary>
        public bool Resizeable
        {
            get { return (bool)GetValue(ResizeableProperty); }
            set { SetValue(ResizeableProperty, value); }
        }

        /// <summary>
        /// 获取设置对话框内容
        /// </summary>
        public object Content
        {
            get { return GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        /// <summary>
        /// 获取设置win模式是否显示遮罩，默认为false
        /// </summary>
        public bool ShowWinVeil
        {
            get { return (bool)GetValue(ShowWinVeilProperty); }
            set { SetValue(ShowWinVeilProperty, value); }
        }

        /// <summary>
        /// 获取对话框是否已显示
        /// </summary>
        public bool IsOpened
        {
            get { return SysVisual.ContainsDlg(this); }
        }
        #endregion

        #region 外部方法
        /// <summary>
        /// 显示对话框
        /// </summary>
        /// <returns>true 正常打开；false 已显示无需再次打开</returns>
        public bool Show()
        {
            if (!SysVisual.ContainsDlg(this))
            {
                ShowInCanvas();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 显示对话框，可异步等待到关闭
        /// </summary>
        /// <returns>返回关闭时(通过Close方法)的参数值</returns>
        public Task<bool> ShowAsync()
        {
            if (!SysVisual.ContainsDlg(this))
            {
                _taskSrc = new TaskCompletionSource<bool>();
                ShowInCanvas();
                return _taskSrc.Task;
            }
            return Task.FromResult(false);
        }

        /// <summary>
        /// 关闭对话框
        /// </summary>
        /// <param name="p_ok">传递给异步等待对话框关闭方法的返回值(通过ShowAsync方法)</param>
        public void Close(bool p_ok = false)
        {
            RemoveFromCanvas(p_ok);
        }

        /// <summary>
        /// 确认并关闭对话框，使方法ShowAsync返回true，方便菜单项使用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnOK(object sender, Mi e)
        {
            Close(true);
        }
        #endregion

        #region 重写方法
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Button btn = (Button)GetTemplateChild("CloseButton");
            if (btn != null)
                btn.Click += (s, e) => Close();

            if (AtSys.System == TargetSystem.Windows || AtSys.System == TargetSystem.Web)
            {
                var grid = (Grid)GetTemplateChild("RootGrid");
                if (grid != null)
                {
                    grid.PointerPressed += OnRootGridPointerPressed;
                    grid.PointerMoved += OnRootGridPointerMoved;
                    grid.PointerReleased += OnRootGridPointerReleased;
                    grid.PointerExited += OnRootGridPointerExited;
                }
            }

            _headerGrid = (Grid)GetTemplateChild("HeaderGrid");
            if (_headerGrid != null)
            {
                _headerGrid.PointerPressed += OnHeaderPointerPressed;
                _headerGrid.PointerMoved += OnHeaderPointerMoved;
                _headerGrid.PointerReleased += OnHeaderPointerReleased;
            }
        }

        /// <summary>
        /// 获取焦点
        /// </summary>
        /// <param name="e"></param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            BringToTop();
        }

        /// <summary>
        /// 按下
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            base.OnPointerPressed(e);
            BringToTop();
        }
        #endregion

        #region 显示
        /// <summary>
        /// 在对话框层显示
        /// </summary>
        void ShowInCanvas()
        {
            DlgPlacement placement = AtSys.IsPhoneUI ? PhonePlacement : WinPlacement;

            // 未能正确添加到可视树或采用相对位置时未设置目标
            if (!SysVisual.AddDlg(this)
                || ((placement > DlgPlacement.FromBottom && PlacementTarget == null)))
                return;

            Rect rcTarget;
            Canvas.SetZIndex(this, ++_currentZIndex);
            double maxWidth = SysVisual.ViewWidth;
            double maxHeight = SysVisual.ViewHeight;
            Measure(new Size(maxWidth, maxHeight));
            double actWidth = DesiredSize.Width;
            double actHeight = DesiredSize.Height;

            switch (placement)
            {
                case DlgPlacement.CenterScreen:
                    // 居中显示
                    if (maxWidth < actWidth)
                        actWidth = Width = maxWidth;

                    if (maxHeight < actHeight)
                        actHeight = Height = maxHeight;

                    // 外部未设置时计算居中位置
                    if (ReadLocalValue(LeftProperty) == DependencyProperty.UnsetValue)
                        Left = Math.Ceiling(Math.Floor((maxWidth - actWidth) / 2.0));
                    if (ReadLocalValue(TopProperty) == DependencyProperty.UnsetValue)
                        Top = Math.Ceiling(Math.Floor((maxHeight - actHeight) / 2.0));
                    break;

                case DlgPlacement.Maximized:
                    // 最大化显示
                    if (ReadLocalValue(LeftProperty) == DependencyProperty.UnsetValue)
                        Left = 0;
                    if (ReadLocalValue(TopProperty) == DependencyProperty.UnsetValue)
                        Top = 0;
                    Width = maxWidth;
                    Height = maxHeight;
                    break;

                case DlgPlacement.FromLeft:
                    Top = 0;
                    Left = 0;
                    Height = maxHeight;
                    if (maxWidth < actWidth)
                        Width = maxWidth;
                    break;

                case DlgPlacement.FromTop:
                    Left = 0;
                    Top = 0;
                    Width = maxWidth;
                    if (maxHeight < actHeight)
                        Height = maxHeight;
                    break;

                case DlgPlacement.FromRight:
                    Top = 0;
                    Height = maxHeight;
                    if (maxWidth < actWidth)
                    {
                        Left = 0;
                        Width = maxWidth;
                    }
                    else
                    {
                        Left = maxWidth - actWidth;
                    }
                    break;

                case DlgPlacement.FromBottom:
                    Left = 0;
                    Width = maxWidth;
                    if (maxHeight < actHeight)
                    {
                        Top = 0;
                        Height = maxHeight;
                    }
                    else
                    {
                        Top = maxHeight - actHeight;
                    }
                    break;

                // 相对目标元素
                case DlgPlacement.TargetTopLeft:
                    rcTarget = PlacementTarget.GetBounds();
                    if (AutoAdjustPosition)
                    {
                        Left = rcTarget.Left < 0 ? 0 : (rcTarget.Left + actWidth > maxWidth ? maxWidth - actWidth : rcTarget.Left);
                        Top = rcTarget.Top < 0 ? 0 : (rcTarget.Top + actHeight > maxHeight ? maxHeight - actHeight : rcTarget.Top);
                    }
                    else
                    {
                        Left = rcTarget.Left;
                        Top = rcTarget.Top;
                    }
                    break;

                case DlgPlacement.TargetTopRight:
                    rcTarget = PlacementTarget.GetBounds();
                    if (AutoAdjustPosition)
                    {
                        Left = rcTarget.Right < 0 ? 0 : (rcTarget.Right + actWidth > maxWidth ? maxWidth - actWidth : rcTarget.Right);
                        Top = rcTarget.Top < 0 ? 0 : (rcTarget.Top + actHeight > maxHeight ? maxHeight - actHeight : rcTarget.Top);
                    }
                    else
                    {
                        Left = rcTarget.Right;
                        Top = rcTarget.Top;
                    }
                    break;

                case DlgPlacement.TargetCenter:
                    rcTarget = PlacementTarget.GetBounds();
                    Left = rcTarget.Left + (rcTarget.Width - actWidth) / 2;
                    Top = rcTarget.Top + (rcTarget.Height - actHeight) / 2;
                    break;

                case DlgPlacement.TargetBottomLeft:
                    rcTarget = PlacementTarget.GetBounds();
                    if (AutoAdjustPosition)
                    {
                        Left = rcTarget.Left < 0 ? 0 : (rcTarget.Left + actWidth > maxWidth ? maxWidth - actWidth : rcTarget.Left);
                        Top = rcTarget.Bottom < 0 ? 0 : (rcTarget.Bottom + actHeight > maxHeight ? rcTarget.Top - actHeight : rcTarget.Bottom);
                    }
                    else
                    {
                        Left = rcTarget.Left;
                        Top = rcTarget.Bottom;
                    }
                    break;

                case DlgPlacement.TargetBottomRight:
                    rcTarget = PlacementTarget.GetBounds();
                    if (AutoAdjustPosition)
                    {
                        Left = rcTarget.Right < 0 ? 0 : (rcTarget.Right + actWidth > maxWidth ? maxWidth - actWidth : rcTarget.Right);
                        Top = rcTarget.Bottom < 0 ? 0 : (rcTarget.Bottom + actHeight > maxHeight ? rcTarget.Top - actHeight : rcTarget.Bottom);
                    }
                    else
                    {
                        Left = rcTarget.Right;
                        Top = rcTarget.Bottom;
                    }
                    break;

                case DlgPlacement.TargetOuterLeftTop:
                    rcTarget = PlacementTarget.GetBounds();
                    double left = rcTarget.Left - actWidth;
                    if (AutoAdjustPosition)
                    {
                        Left = left < 0 ? rcTarget.Right : (left + actWidth > maxWidth ? maxWidth - actWidth : left);
                        Top = rcTarget.Top < 0 ? 0 : (rcTarget.Top + actHeight > maxHeight ? maxHeight - actHeight : rcTarget.Top);
                    }
                    else
                    {
                        Left = left;
                        Top = rcTarget.Top;
                    }
                    break;

                case DlgPlacement.TargetOuterTop:
                    rcTarget = PlacementTarget.GetBounds();
                    double top = rcTarget.Top - actHeight;
                    if (AutoAdjustPosition)
                    {
                        Left = rcTarget.Left < 0 ? 0 : (rcTarget.Left + actWidth > maxWidth ? maxWidth - actWidth : rcTarget.Left);
                        Top = top < 0 ? rcTarget.Bottom : (top + actHeight > maxHeight ? maxHeight - actHeight : top);
                    }
                    else
                    {
                        Left = rcTarget.Left;
                        Top = top;
                    }
                    break;
            }
        }

        /// <summary>
        /// 从对话框层移除
        /// </summary>
        /// <param name="p_ok">传递给异步等待对话框关闭方法的返回值(通过ShowAsync方法)</param>
        async void RemoveFromCanvas(bool p_ok = false)
        {
            // 屏蔽多次触发移除的情况
            // 如：关闭对话框前弹出确认对话框，点击确认对话框时可能触发OnOuterPressed，出现多次触发Closing事件！
            if (_isRemoving)
                return;

            try
            {
                _isRemoving = true;
                // 关闭前
                if (Closing != null)
                {
                    var args = new AsyncCancelEventArgs();
                    Closing(this, args);
                    await args.EnsureAllCompleted();
                    if (args.Cancel)
                        return;
                }
                if (!await OnClosing())
                    return;

                SysVisual.RemoveDlg(this);

                // ShowAsync情况
                if (_taskSrc != null && !_taskSrc.Task.IsCompleted)
                {
                    _taskSrc.SetResult(p_ok);
                    _taskSrc = null;
                }

                // 关闭后
                Closed?.Invoke(this, EventArgs.Empty);
                OnClosed();

                // 遗漏的外框
                if (_bdResize != null)
                {
                    SysVisual.RemoveDlgResizeFlag(_bdResize);
                    _bdResize = null;
                }
            }
            finally
            {
                _isRemoving = false;
            }
        }

        /// <summary>
        /// 置顶对话框
        /// </summary>
        void BringToTop()
        {
            if (Canvas.GetZIndex(this) != _currentZIndex)
                Canvas.SetZIndex(this, ++_currentZIndex);
        }

        /// <summary>
        /// 显示位置变化时调整动画
        /// </summary>
        void OnPlacementChanged()
        {
            DlgPlacement placement = AtSys.IsPhoneUI ? PhonePlacement : WinPlacement;

            Transitions = new TransitionCollection();
            switch (placement)
            {
                case DlgPlacement.FromLeft:
                    Transitions.Add(new EdgeUIThemeTransition { Edge = EdgeTransitionLocation.Left });
                    break;
                case DlgPlacement.FromTop:
                    Transitions.Add(new EdgeUIThemeTransition { Edge = EdgeTransitionLocation.Top });
                    break;
                case DlgPlacement.FromRight:
                    Transitions.Add(new EdgeUIThemeTransition { Edge = EdgeTransitionLocation.Right });
                    break;
                case DlgPlacement.FromBottom:
                    Transitions.Add(new EdgeUIThemeTransition { Edge = EdgeTransitionLocation.Bottom });
                    break;
                default:
                    Transitions.Add(new PopupThemeTransition());
                    break;
            }
        }

        /// <summary>
        /// 点击对话框外部
        /// </summary>
        /// <param name="p_point">外部点击位置</param>
        void IDlgOuterPressed.OnOuterPressed(Point p_point)
        {
            OnOuterPressed(p_point);
        }

        /// <summary>
        /// 点击对话框外部
        /// </summary>
        /// <param name="p_point">外部点击位置</param>
        protected virtual void OnOuterPressed(Point p_point)
        {
            if (!IsPinned
                && (ClipElement == null || !ClipElement.ContainPoint(p_point)))
                RemoveFromCanvas();
        }

        /// <summary>
        /// 关闭或后退之前，返回false表示禁止关闭
        /// </summary>
        /// <returns>true 表允许关闭</returns>
        protected virtual Task<bool> OnClosing()
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// 关闭或后退之后
        /// </summary>
        protected virtual void OnClosed()
        {
        }
        #endregion

        #region 拖拽及调整大小
        /// <summary>
        /// 点击标题栏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnHeaderPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (GetResizeDirection(e.GetCurrentPoint(this).Position) == ResizeDirection.None && _headerGrid.CapturePointer(e.Pointer))
            {
                _isHeadPressed = true;
                Point pt = e.GetCurrentPoint(null).Position;
                _startPoint = new Point(pt.X - Left, pt.Y - Top);
            }
        }

        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="e"></param>
        void OnHeaderPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (_isHeadPressed)
            {
                var pt = e.GetCurrentPoint(null).Position;
                Left = pt.X - _startPoint.X;
                Top = pt.Y - _startPoint.Y;
            }
        }

        /// <summary>
        /// 抬起
        /// </summary>
        /// <param name="e"></param>
        void OnHeaderPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (_isHeadPressed)
            {
                _isHeadPressed = false;
                _headerGrid.ReleasePointerCapture(e.Pointer);
            }
        }

        /// <summary>
        /// 点击RootGrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnRootGridPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (!Resizeable)
                return;

            var dir = GetResizeDirection(e.GetCurrentPoint(this).Position);
            if (dir != ResizeDirection.None)
            {
                if (_isHeadPressed)
                {
                    _isHeadPressed = false;
                    _headerGrid.ReleasePointerCapture(e.Pointer);
                }

                if (((Grid)sender).CapturePointer(e.Pointer))
                {
                    Point offset = TransformToVisual(null).TransformPoint(new Point(0.0, 0.0));
                    _isResizing = true;
                    _resizeDirection = dir;
                    _startPoint = e.GetCurrentPoint(null).Position;
                    _initRect = new Rect(offset.X, offset.Y, ActualWidth, ActualHeight);
                    UpdateMouseCursor(_resizeDirection);

                    // 调整大小的外框
                    _bdResize = new Border
                    {
                        BorderBrush = AtRes.醒目红色,
                        Width = ActualWidth,
                        Height = ActualHeight,
                        BorderThickness = new Thickness(2),
                    };
                    Canvas.SetLeft(_bdResize, offset.X);
                    Canvas.SetTop(_bdResize, offset.Y);
                    Canvas.SetZIndex(_bdResize, _currentZIndex + 10);
                    SysVisual.AddDlgResizeFlag(_bdResize);
                }
            }
        }

        /// <summary>
        /// 移动RootGrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnRootGridPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (!Resizeable)
                return;

            if (!_isResizing)
            {
                UpdateMouseCursor(GetResizeDirection(e.GetCurrentPoint(this).Position));
                return;
            }

            double newWidth;
            double newHeight;
            double minWidth = double.IsNaN(MinWidth) ? 0.0 : MinWidth;
            double minHeight = double.IsNaN(MinHeight) ? 0.0 : MinHeight;
            double maxWidth = double.IsNaN(MaxWidth) ? double.PositiveInfinity : MaxWidth;
            double maxHeight = double.IsNaN(MaxHeight) ? double.PositiveInfinity : MaxHeight;
            Point pt = e.GetCurrentPoint(null).Position;
            switch (_resizeDirection)
            {
                case ResizeDirection.Left:
                    newWidth = Math.Min(maxWidth, Math.Max(minWidth, (_initRect.Width + _startPoint.X) - pt.X));
                    _bdResize.Width = newWidth;
                    Canvas.SetLeft(_bdResize, _initRect.Right - newWidth);
                    return;

                case ResizeDirection.TopLeft:
                    newWidth = Math.Min(maxWidth, Math.Max(minWidth, (_initRect.Width + _startPoint.X) - pt.X));
                    _bdResize.Width = newWidth;
                    Canvas.SetLeft(_bdResize, _initRect.Right - newWidth);
                    newHeight = Math.Min(maxHeight, Math.Max(minHeight, (_initRect.Height + _startPoint.Y) - pt.Y));
                    _bdResize.Height = newHeight;
                    Canvas.SetTop(_bdResize, _initRect.Bottom - newHeight);
                    return;

                case ResizeDirection.Top:
                    newHeight = Math.Min(maxHeight, Math.Max(minHeight, (_initRect.Height + _startPoint.Y) - pt.Y));
                    _bdResize.Height = newHeight;
                    Canvas.SetTop(_bdResize, _initRect.Bottom - newHeight);
                    return;

                case ResizeDirection.TopRight:
                    newHeight = Math.Min(maxHeight, Math.Max(minHeight, (_initRect.Height + _startPoint.Y) - pt.Y));
                    _bdResize.Height = newHeight;
                    Canvas.SetTop(_bdResize, _initRect.Bottom - newHeight);
                    _bdResize.Width = Math.Min(maxWidth, Math.Max(minWidth, (_initRect.Width + pt.X) - _startPoint.X));
                    return;

                case ResizeDirection.Right:
                    _bdResize.Width = Math.Min(maxWidth, Math.Max(minWidth, (_initRect.Width + pt.X) - _startPoint.X));
                    return;

                case ResizeDirection.BottomRight:
                    _bdResize.Height = Math.Min(maxHeight, Math.Max(minHeight, (_initRect.Height + pt.Y) - _startPoint.Y));
                    _bdResize.Width = Math.Min(maxWidth, Math.Max(minWidth, (_initRect.Width + pt.X) - _startPoint.X));
                    return;

                case ResizeDirection.Bottom:
                    _bdResize.Height = Math.Min(maxHeight, Math.Max(minHeight, (_initRect.Height + pt.Y) - _startPoint.Y));
                    return;

                case ResizeDirection.BottomLeft:
                    newWidth = Math.Min(maxWidth, Math.Max(minWidth, (_initRect.Width + _startPoint.X) - pt.X));
                    _bdResize.Width = newWidth;
                    Canvas.SetLeft(_bdResize, _initRect.Right - newWidth);
                    _bdResize.Height = Math.Min(maxHeight, Math.Max(minHeight, (_initRect.Height + pt.Y) - _startPoint.Y));
                    return;
            }
        }

        /// <summary>
        /// 取消点击RootGrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnRootGridPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (_isResizing)
            {
                UpdateMouseCursor(ResizeDirection.None);
                _isResizing = false;
                ((Grid)sender).ReleasePointerCapture(e.Pointer);

                Width = _bdResize.Width;
                Height = _bdResize.Height;
                Left = Canvas.GetLeft(_bdResize);
                Top = Canvas.GetTop(_bdResize);
                SysVisual.RemoveDlgResizeFlag(_bdResize);
                _bdResize = null;
            }
        }

        void OnRootGridPointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (!_isResizing && Resizeable)
                UpdateMouseCursor(ResizeDirection.None);
        }

        /// <summary>
        /// 获取当前位置状态
        /// </summary>
        /// <param name="p_position"></param>
        /// <returns></returns>
        ResizeDirection GetResizeDirection(Point p_position)
        {
            double resizerSize = 7.0;
            if ((p_position.X >= 0.0) && (p_position.X < resizerSize))
            {
                if ((p_position.Y >= 0.0) && (p_position.Y < resizerSize))
                {
                    return ResizeDirection.TopLeft;
                }
                if ((p_position.Y > (ActualHeight - resizerSize)) && (p_position.Y <= ActualHeight))
                {
                    return ResizeDirection.BottomLeft;
                }
                return ResizeDirection.Left;
            }
            if ((p_position.X > (ActualWidth - resizerSize)) && (p_position.X <= ActualWidth))
            {
                if ((p_position.Y >= 0.0) && (p_position.Y < resizerSize))
                {
                    return ResizeDirection.TopRight;
                }
                if ((p_position.Y > (ActualHeight - resizerSize)) && (p_position.Y <= ActualHeight))
                {
                    return ResizeDirection.BottomRight;
                }
                return ResizeDirection.Right;
            }
            if ((p_position.Y >= 0.0) && (p_position.Y < resizerSize))
            {
                return ResizeDirection.Top;
            }
            if ((p_position.Y > (ActualHeight - resizerSize)) && (p_position.Y <= ActualHeight))
            {
                return ResizeDirection.Bottom;
            }
            return ResizeDirection.None;
        }

        /// <summary>
        /// 切换鼠标样式
        /// </summary>
        /// <param name="direction"></param>
        void UpdateMouseCursor(ResizeDirection direction)
        {
            switch (direction)
            {
                case ResizeDirection.Left:
                case ResizeDirection.Right:
                    Windows.UI.Xaml.Window.Current.CoreWindow.PointerCursor = Cursors.GetCursor(CoreCursorType.SizeWestEast);
                    break;

                case ResizeDirection.TopLeft:
                case ResizeDirection.BottomRight:
                    Windows.UI.Xaml.Window.Current.CoreWindow.PointerCursor = Cursors.GetCursor(CoreCursorType.SizeNorthwestSoutheast);
                    break;

                case ResizeDirection.Top:
                case ResizeDirection.Bottom:
                    Windows.UI.Xaml.Window.Current.CoreWindow.PointerCursor = Cursors.GetCursor(CoreCursorType.SizeNorthSouth);
                    break;

                case ResizeDirection.TopRight:
                case ResizeDirection.BottomLeft:
                    Windows.UI.Xaml.Window.Current.CoreWindow.PointerCursor = Cursors.GetCursor(CoreCursorType.SizeNortheastSouthwest);
                    break;

                default:
                    Windows.UI.Xaml.Window.Current.CoreWindow.PointerCursor = Cursors.DefaultCursor;
                    break;
            }
        }
        #endregion
    }
}
