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
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Input;
using System.Collections.Concurrent;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 对话框容器
    /// </summary>
    [ContentProperty(Name = "Content")]
    public partial class Dlg : Control, IDlgPressed, IDestroy
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
            new PropertyMetadata(0.0, OnLeftChanged));

        public static readonly DependencyProperty TopProperty = DependencyProperty.Register(
            "Top",
            typeof(double),
            typeof(Dlg),
            new PropertyMetadata(0.0, OnTopChanged));

        public static readonly DependencyProperty EdgeMarginProperty = DependencyProperty.Register(
            "EdgeMargin",
            typeof(double),
            typeof(Dlg),
            new PropertyMetadata(0.0));

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

        public static readonly DependencyProperty ShowVeilProperty = DependencyProperty.Register(
            "ShowVeil",
            typeof(bool),
            typeof(Dlg),
            new PropertyMetadata(true, OnShowVeilChanged));

        public static readonly DependencyProperty CanCloseProperty = DependencyProperty.Register(
            "CanClose",
            typeof(bool),
            typeof(Dlg),
            new PropertyMetadata(true));

        public static readonly DependencyProperty AllowRelayPressProperty = DependencyProperty.Register(
            "AllowRelayPress",
            typeof(bool),
            typeof(Dlg),
            new PropertyMetadata(true));

        public static readonly DependencyProperty HeaderButtonTextProperty = DependencyProperty.Register(
            "HeaderButtonText",
            typeof(string),
            typeof(Dlg),
            new PropertyMetadata("\uE009"));

        public static readonly DependencyProperty ContentTransitionsProperty = DependencyProperty.Register(
            "ContentTransitions",
            typeof(TransitionCollection),
            typeof(Dlg),
            new PropertyMetadata(null));

        public static readonly DependencyProperty VeilBrushProperty = DependencyProperty.Register(
            "VeilBrush",
            typeof(SolidColorBrush),
            typeof(Dlg),
            new PropertyMetadata(null, OnShowVeilChanged));

        public static readonly DependencyProperty EnableClosingAnimationProperty = DependencyProperty.Register(
            "EnableClosingAnimation",
            typeof(bool),
            typeof(Dlg),
            new PropertyMetadata(false));

        public static readonly DependencyProperty TopMostProperty = DependencyProperty.Register(
            "TopMost",
            typeof(bool),
            typeof(Dlg),
            new PropertyMetadata(false));

        public static readonly DependencyProperty OwnWinProperty = DependencyProperty.Register(
            "OwnWin",
            typeof(Win),
            typeof(Dlg),
            new PropertyMetadata(null, OnOwnWinChanged));

        static void OnWinPlacementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!Kit.IsPhoneUI)
                ((Dlg)d).OnPlacementChanged();
        }

        static void OnPhonePlacementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (Kit.IsPhoneUI)
                ((Dlg)d).OnPlacementChanged();
        }

        static void OnShowVeilChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Dlg)d).ApplyVeilBrush();
        }

        static void OnLeftChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Canvas.SetLeft((Dlg)d, (double)e.NewValue);
        }

        static void OnTopChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Canvas.SetTop((Dlg)d, (double)e.NewValue);
        }

        static void OnOwnWinChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Dlg dlg = (Dlg)d;
            if (e.OldValue is Win old)
                old.Destroyed -= dlg.OnOwnWinDestroyed;
            if (e.NewValue is Win win)
                win.Destroyed += dlg.OnOwnWinDestroyed;
        }
        #endregion

        #region 成员变量
        static int _normalZIndex = 1;
        static int _topmostZIndex = 100000;
        readonly Canvas _canvas;
        Grid _headerGrid;
        bool _isHeadPressed;
        bool _isResizing;
        ResizeDirection _resizeDirection;
        Point _startPoint;
        Rect _initRect;
        Border _bdResize;
        bool _isRemoving;
        protected TaskCompletionSource<bool> _taskSrc;
        #endregion

        #region 构造方法
        public Dlg()
        {
            // 用处：对话框的背景遮罩、位置
            _canvas = new Canvas();
            _canvas.Children.Add(this);

            // 两种UI模式样式不同，PhoneUI模式为缺省样式
            if (Kit.IsPhoneUI)
                DefaultStyleKey = typeof(Dlg);
            else
                Style = (Style)Res.DialogRes["WinDlg"];
        }
        #endregion

        #region 事件
        /// <summary>
        /// 对话框正在关闭事件，可以取消关闭
        /// </summary>
        public event Action<Dlg, DlgClosingArgs> Closing;

        /// <summary>
        /// 对话框关闭后事件
        /// </summary>
        public event Action<Dlg, bool> Closed;

        /// <summary>
        /// 拖拽调整大小后事件
        /// </summary>
        public event Action Resized;

        /// <summary>
        /// 对话框拖拽移动事件，参数为当前鼠标的位置
        /// </summary>
        public event Action<Dlg, Point> Dragging;

        /// <summary>
        /// 对话框停止拖拽事件，参数为停止时的位置
        /// </summary>
        public event Action<Point> Dropped;
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
        /// WinPlacement为 FromLeft FromTop FromRight FromBottom 时，与所在边的边距
        /// </summary>
        public double EdgeMargin
        {
            get { return (double)GetValue(EdgeMarginProperty); }
            set { SetValue(EdgeMarginProperty, value); }
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
        /// 获取设置是否显示遮罩，win模式默认false，phone模式默认true
        /// </summary>
        public bool ShowVeil
        {
            get { return (bool)GetValue(ShowVeilProperty); }
            set { SetValue(ShowVeilProperty, value); }
        }

        /// <summary>
        /// 获取设置是否显示关闭按钮，默认true
        /// </summary>
        public bool CanClose
        {
            get { return (bool)GetValue(CanCloseProperty); }
            set { SetValue(CanCloseProperty, value); }
        }

        /// <summary>
        /// 无遮罩时是否允许将点击事件传递到下层对话框，默认true
        /// </summary>
        public bool AllowRelayPress
        {
            get { return (bool)GetValue(AllowRelayPressProperty); }
            set { SetValue(AllowRelayPressProperty, value); }
        }

        /// <summary>
        /// 获取设置标题栏按钮字符
        /// </summary>
        public string HeaderButtonText
        {
            get { return (string)GetValue(HeaderButtonTextProperty); }
            set { SetValue(HeaderButtonTextProperty, value); }
        }

        /// <summary>
        /// 获取或设置切换内容时的转换
        /// </summary>
        public TransitionCollection ContentTransitions
        {
            get { return (TransitionCollection)GetValue(ContentTransitionsProperty); }
            set { SetValue(ContentTransitionsProperty, value); }
        }

        /// <summary>
        /// 获取设置遮罩颜色
        /// </summary>
        public SolidColorBrush VeilBrush
        {
            get { return (SolidColorBrush)GetValue(VeilBrushProperty); }
            set { SetValue(VeilBrushProperty, value); }
        }

        /// <summary>
        /// 获取对话框是否已显示
        /// </summary>
        public bool IsOpened
        {
            get { return UITree.ContainsDlg(_canvas); }
        }

        /// <summary>
        /// 是否启用关闭时动画，只Win有效，默认false
        /// </summary>
        public bool EnableClosingAnimation
        {
            get { return (bool)GetValue(EnableClosingAnimationProperty); }
            set { SetValue(EnableClosingAnimationProperty, value); }
        }

        /// <summary>
        /// 是否将对话框置于最上层，默认false
        /// </summary>
        public bool TopMost
        {
            get { return (bool)GetValue(TopMostProperty); }
            set { SetValue(TopMostProperty, value); }
        }

        /// <summary>
        /// 所属Win，和Win生命周期相同，Win关闭后关闭对话框、释放资源
        /// </summary>
        public Win OwnWin
        {
            get { return (Win)GetValue(OwnWinProperty); }
            set { SetValue(OwnWinProperty, value); }
        }
        #endregion

        #region 外部方法
        /// <summary>
        /// 显示对话框
        /// </summary>
        /// <returns>true 正常打开；false 已显示无需再次打开</returns>
        public bool Show()
        {
            if (!UITree.ContainsDlg(_canvas))
            {
                ShowInCanvas();
                return true;
            }

            BringToTop();
            return false;
        }

        /// <summary>
        /// 显示对话框，可异步等待到关闭
        /// </summary>
        /// <returns>返回关闭时(通过Close方法)的参数值</returns>
        public Task<bool> ShowAsync()
        {
            if (!UITree.ContainsDlg(_canvas))
            {
                _taskSrc = new TaskCompletionSource<bool>();
                ShowInCanvas();
                return _taskSrc.Task;
            }

            BringToTop();

            // 确保已显示时也能正常等待
            if (_taskSrc != null && !_taskSrc.Task.IsCompleted)
                return _taskSrc.Task;
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
        /// <param name="e"></param>
        public void OnOK(Mi e)
        {
            Close(true);
        }

        /// <summary>
        /// 确认并关闭对话框，使方法ShowAsync返回true，方便菜单项使用
        /// </summary>
        public void OnOK()
        {
            Close(true);
        }

        /// <summary>
        /// 设置对话框的大小，内部限制在可视区之内
        /// <para>值为0：整个可视区宽度或高度</para>
        /// <para>值为负数：小于可视区大小的值</para>
        /// <para>值超出可视区大小：采用可视区的宽度或高度</para>
        /// </summary>
        /// <param name="p_width">宽度</param>
        /// <param name="p_height">高度</param>
        public void SetSize(double p_width, double p_height)
        {
            if (p_width > Kit.ViewWidth)
                Width = Kit.ViewWidth;
            else if (p_width <= 0)
                Width = Kit.ViewWidth + p_width;
            else
                Width = p_width;

            if (p_height > Kit.ViewHeight)
                Height = Kit.ViewHeight;
            else if (p_height <= 0)
                Height = Kit.ViewHeight + p_height;
            else
                Height = p_height;
        }

        /// <summary>
        /// 置顶对话框
        /// </summary>
        public void BringToTop()
        {
            if (TopMost)
            {
                if (Canvas.GetZIndex(_canvas) != _topmostZIndex)
                    Canvas.SetZIndex(_canvas, ++_topmostZIndex);
            }
            else
            {
                if (Canvas.GetZIndex(_canvas) != _normalZIndex)
                    Canvas.SetZIndex(_canvas, ++_normalZIndex);
            }
        }

        /// <summary>
        /// 加载Tab，PhoneUI直接显示，WinUI外套Tabs
        /// </summary>
        /// <param name="p_tab"></param>
        public void LoadTab(Tab p_tab)
        {
            if (p_tab == null)
                Throw.Msg("Dlg中的Tab不可为空！");

            p_tab.OwnDlg = this;
            if (Kit.IsPhoneUI)
            {
                HideTitleBar = true;
                Content = p_tab;
            }
            else
            {
                Tabs tabs = new Tabs();
                tabs.Items.Add(p_tab);
                Content = tabs;

                // 缺省标题
                if (ReadLocalValue(TitleProperty) == DependencyProperty.UnsetValue)
                    Title = p_tab.Title;
            }
        }

        /// <summary>
        /// 窗口内加载多Tab，PhoneUI外套PhoneTabs，WinUI外套Tabs
        /// </summary>
        /// <param name="p_tabs"></param>
        public void LoadTabs(IList<Tab> p_tabs)
        {
            if (p_tabs == null || p_tabs.Count == 0)
                Throw.Msg("Dlg中的Tab不可为空！");

            if (p_tabs.Count == 1)
            {
                LoadTab(p_tabs[0]);
                return;
            }

            if (Kit.IsPhoneUI)
            {
                HideTitleBar = true;
                var tabs = new PhoneTabs();
                foreach (var tab in p_tabs)
                {
                    tab.OwnDlg = this;
                    tabs.AddItem(tab);
                }

                tabs.Select(0);
                Content = tabs;
            }
            else
            {
                Tabs tabs = new Tabs();
                foreach (var tab in p_tabs)
                {
                    tab.OwnDlg = this;
                    tabs.Items.Add(tab);
                }
                Content = tabs;

                // 缺省标题
                if (ReadLocalValue(TitleProperty) == DependencyProperty.UnsetValue)
                    Title = p_tabs[0].Title;
            }
        }

        /// <summary>
        /// 对话框内部加载窗口，非PhoneUI有效
        /// </summary>
        /// <param name="p_win"></param>
        public void LoadWin(Win p_win)
        {
            if (p_win == null)
                Throw.Msg("Dlg中的Win不可为空！");

            Content = p_win;
            p_win.OwnDlg = this;

            // 缺省标题
            if (ReadLocalValue(TitleProperty) == DependencyProperty.UnsetValue)
                Title = p_win.Title;
        }
        #endregion

        #region 重写方法
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Button btn = (Button)GetTemplateChild("CloseButton");
            if (btn != null)
                btn.Click += (s, e) => Close();

            var rootGrid = (Grid)GetTemplateChild("RootGrid");
            if (rootGrid != null && !Kit.IsPhoneUI)
            {
                rootGrid.PointerPressed += OnRootGridPointerPressed;
                rootGrid.PointerMoved += OnRootGridPointerMoved;
                rootGrid.PointerReleased += OnRootGridPointerReleased;
                rootGrid.PointerExited += OnRootGridPointerExited;

                // 禁止获得焦点时调整显示层次，太乱！
                //rootGrid.GotFocus += (s, e) => BringToTop();
            }

            _headerGrid = (Grid)GetTemplateChild("HeaderGrid");
            if (_headerGrid != null)
            {
                if (Kit.IsPhoneUI)
                {
                    var theme = Kit.GetService<ITheme>();
                    _headerGrid.Background = (theme == null) ? Res.主蓝 : theme.ThemeBrush;
                }
                _headerGrid.PointerPressed += OnHeaderPointerPressed;
                _headerGrid.PointerMoved += OnHeaderPointerMoved;
                _headerGrid.PointerReleased += OnHeaderPointerReleased;
#if WIN
                _headerGrid.RightTapped += OnHeaderRightTapped;
#endif
            }
            ApplyVeilBrush();
        }

        #endregion

        #region 显示
        /// <summary>
        /// 在对话框层显示
        /// </summary>
        void ShowInCanvas()
        {
            DlgPlacement placement = Kit.IsPhoneUI ? PhonePlacement : WinPlacement;

#if WIN
            if (EnableClosingAnimation)
                _canvas.Transitions = null;
#endif

            // 未能正确添加到可视树 或 采用相对位置但未设置目标时
            if (!UITree.AddDlg(_canvas)
                || (placement > DlgPlacement.FromBottom && PlacementTarget == null))
                return;

            if (TopMost)
                Canvas.SetZIndex(_canvas, ++_topmostZIndex);
            else
                Canvas.SetZIndex(_canvas, ++_normalZIndex);
            
            double maxWidth = Kit.ViewWidth;
            double maxHeight = Kit.ViewHeight;

            // 确保底层Canvas占用整个可视区域，遮罩效果
            _canvas.Width = maxWidth;
            _canvas.Height = maxHeight;

            Measure(new Size(maxWidth, maxHeight));
            double actWidth = DesiredSize.Width;
            double actHeight = DesiredSize.Height;

            Rect rcTarget;
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
                    // Desktop时不覆盖任务栏
                    Top = UITree.RootContent is Desktop ? 44 : 0;
                    if (Height != double.NaN && Height < maxHeight - Top)
                    {
                        // 设置高度时垂直居中
                        Top = Math.Ceiling((maxHeight - Top - Height) / 2);
                    }
                    else
                    {
                        // 未设置高度时占用除任务栏的整个高度
                        Height = maxHeight - Top;
                    }
                    Left = EdgeMargin;
                    if (maxWidth < actWidth)
                        Width = maxWidth;
                    break;

                case DlgPlacement.FromLeftTop:
                    // Desktop时不覆盖任务栏
                    Top = UITree.RootContent is Desktop ? 44 : 0;
                    if (Height != double.NaN && Height > maxHeight - Top)
                    {
                        // 未设置高度时占用除任务栏的整个高度
                        Height = maxHeight - Top;
                    }
                    Left = EdgeMargin;
                    if (maxWidth < actWidth)
                        Width = maxWidth;
                    break;

                case DlgPlacement.FromLeftBottom:
                    Top = UITree.RootContent is Desktop ? 44 : 0;
                    if (actHeight < maxHeight - Top)
                    {
                        Top = Math.Ceiling(maxHeight - actHeight);
                    }
                    else
                    {
                        // 未设置高度时占用除任务栏的整个高度
                        Height = maxHeight - Top;
                    }
                    Left = EdgeMargin;
                    if (maxWidth < actWidth)
                        Width = maxWidth;
                    break;

                case DlgPlacement.FromTop:
                    Left = 0;
                    Top = EdgeMargin + (UITree.RootContent is Desktop ? 44 : 0);
                    if (Width != double.NaN && Width < maxWidth)
                    {
                        // 设置宽度时水平居中
                        Left = Math.Ceiling((maxWidth - Width) / 2);
                    }
                    else
                    {
                        // 未设置宽度时占用整个宽度
                        Width = maxWidth;
                    }
                    if (maxHeight < actHeight)
                        Height = maxHeight;
                    break;

                case DlgPlacement.FromTopLeft:
                    Left = 0;
                    Top = EdgeMargin + (UITree.RootContent is Desktop ? 44 : 0);
                    if (actWidth > maxWidth)
                    {
                        // 未设置宽度时占用整个宽度
                        Width = maxWidth;
                    }
                    if (maxHeight < actHeight)
                        Height = maxHeight;
                    break;

                case DlgPlacement.FromTopRight:
                    Top = EdgeMargin + (UITree.RootContent is Desktop ? 44 : 0);
                    if (actWidth < maxWidth)
                    {
                        Left = Math.Ceiling(maxWidth - actWidth);
                    }
                    else
                    {
                        Left = 0;
                        Width = maxWidth;
                    }
                    if (maxHeight < actHeight)
                        Height = maxHeight;
                    break;

                case DlgPlacement.FromRight:
                    // Desktop时不覆盖任务栏
                    Top = UITree.RootContent is Desktop ? 44 : 0;
                    if (Height != double.NaN && Height < maxHeight - Top)
                    {
                        // 设置高度时垂直居中
                        Top = Math.Ceiling((maxHeight - Top - Height) / 2);
                    }
                    else
                    {
                        // 未设置高度时占用除任务栏的整个高度
                        Height = maxHeight - Top;
                    }
                    if (maxWidth < actWidth)
                    {
                        Left = 0;
                        Width = maxWidth;
                    }
                    else
                    {
                        Left = maxWidth - actWidth - EdgeMargin;
                    }
                    break;

                case DlgPlacement.FromRightTop:
                    // Desktop时不覆盖任务栏
                    Top = UITree.RootContent is Desktop ? 44 : 0;
                    if (actHeight > maxHeight - Top)
                    {
                        Height = maxHeight - Top;
                    }
                    if (maxWidth < actWidth)
                    {
                        Left = 0;
                        Width = maxWidth;
                    }
                    else
                    {
                        Left = maxWidth - actWidth - EdgeMargin;
                    }
                    break;

                case DlgPlacement.FromRightBottom:
                    // Desktop时不覆盖任务栏
                    Top = UITree.RootContent is Desktop ? 44 : 0;
                    if (actHeight < maxHeight - Top)
                    {
                        Top = Math.Ceiling(maxHeight - actHeight);
                    }
                    else
                    {
                        Top = 0;
                        Height = maxHeight - Top;
                    }
                    if (maxWidth < actWidth)
                    {
                        Left = 0;
                        Width = maxWidth;
                    }
                    else
                    {
                        Left = maxWidth - actWidth - EdgeMargin;
                    }
                    break;

                case DlgPlacement.FromBottom:
                    Left = 0;
                    if (Width != double.NaN && Width < maxWidth)
                    {
                        // 设置宽度时水平居中
                        Left = Math.Ceiling((maxWidth - Width) / 2);
                    }
                    else
                    {
                        // 未设置宽度时占用整个宽度
                        Width = maxWidth;
                    }
                    if (maxHeight < actHeight)
                    {
                        Top = 0;
                        Height = maxHeight;
                    }
                    else
                    {
                        Top = maxHeight - actHeight - EdgeMargin;
                    }
                    break;

                case DlgPlacement.FromBottomLeft:
                    Left = 0;
                    if (actWidth > maxWidth)
                    {
                        Width = maxWidth;
                    }
                    if (maxHeight < actHeight)
                    {
                        Top = 0;
                        Height = maxHeight;
                    }
                    else
                    {
                        Top = maxHeight - actHeight - EdgeMargin;
                    }
                    break;

                case DlgPlacement.FromBottomRight:
                    Left = 0;
                    if (actWidth < maxWidth)
                    {
                        Left = Math.Ceiling(maxWidth - actWidth);
                    }
                    else
                    {
                        Width = maxWidth;
                    }
                    if (maxHeight < actHeight)
                    {
                        Top = 0;
                        Height = maxHeight;
                    }
                    else
                    {
                        Top = maxHeight - actHeight - EdgeMargin;
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

                case DlgPlacement.TargetOuterBottomRight:
                    rcTarget = PlacementTarget.GetBounds();
                    left = rcTarget.Right - actWidth;
                    if (AutoAdjustPosition)
                    {
                        Left = left < 0 ? 0 : (left + actWidth > maxWidth ? maxWidth - actWidth : left);
                        Top = rcTarget.Bottom < 0 ? 0 : (rcTarget.Bottom + actHeight > maxHeight ? maxHeight - actHeight : rcTarget.Bottom);
                    }
                    else
                    {
                        Left = left;
                        Top = rcTarget.Bottom;
                    }
                    break;

                case DlgPlacement.TargetOverlap:
                    rcTarget = PlacementTarget.GetBounds();
                    Left = rcTarget.Left;
                    Top = rcTarget.Top;
                    Width = rcTarget.Width;
                    Height = rcTarget.Height;
                    break;
            }

            // 禁止获得焦点时调整显示层次，太乱！
            // 设置焦点，防止其他 Dlg 抢焦点造成显示层次变化，如Dlg中有CList时点开后下拉框转到下层！
            //Focus(FocusState.Programmatic);
        }

        /// <summary>
        /// 从对话框层移除
        /// </summary>
        /// <param name="p_ok">对话框关闭时的返回值，返回值传递给ShowAsync OnClosing OnClosed方法 和 Closing Closed事件</param>
        async void RemoveFromCanvas(bool p_ok = false)
        {
            // 屏蔽多次触发移除的情况
            // 如：关闭对话框前弹出确认对话框，点击确认对话框时可能触发OnOuterPressed，出现多次触发Closing事件！
            if (_isRemoving || !UITree.ContainsDlg(_canvas))
                return;

            try
            {
                _isRemoving = true;
                // 关闭前
                if (Closing != null)
                {
                    var args = new DlgClosingArgs() { Result = p_ok };
                    Closing(this, args);
                    await args.EnsureAllCompleted();
                    if (args.Cancel)
                        return;
                }
                if (!await OnClosing(p_ok))
                    return;

#if WIN
                if (EnableClosingAnimation)
                    _canvas.Transitions = new TransitionCollection { new AddDeleteThemeTransition() };
#endif
                UITree.RemoveDlg(_canvas);

                // ShowAsync情况
                if (_taskSrc != null && !_taskSrc.Task.IsCompleted)
                {
                    _taskSrc.SetResult(p_ok);
                    _taskSrc = null;
                }

                // 关闭后
                Closed?.Invoke(this, p_ok);
                OnClosed(p_ok);

                // 遗漏的外框
                if (_bdResize != null)
                {
                    UITree.RemoveDlgResizeFlag(_bdResize);
                    _bdResize = null;
                }
            }
            finally
            {
                _isRemoving = false;
            }
        }

        /// <summary>
        /// 显示位置变化时调整动画
        /// </summary>
        void OnPlacementChanged()
        {
#if WIN
            DlgPlacement placement = Kit.IsPhoneUI ? PhonePlacement : WinPlacement;

            var trans = new TransitionCollection();
            switch (placement)
            {
                case DlgPlacement.FromLeft:
                case DlgPlacement.FromLeftTop:
                case DlgPlacement.FromLeftBottom:
                    trans.Add(new EdgeUIThemeTransition { Edge = EdgeTransitionLocation.Left });
                    break;
                case DlgPlacement.FromTop:
                case DlgPlacement.FromTopLeft:
                case DlgPlacement.FromTopRight:
                    trans.Add(new EdgeUIThemeTransition { Edge = EdgeTransitionLocation.Top });
                    break;
                case DlgPlacement.FromRight:
                case DlgPlacement.FromRightTop:
                case DlgPlacement.FromRightBottom:
                    trans.Add(new EdgeUIThemeTransition { Edge = EdgeTransitionLocation.Right });
                    break;
                case DlgPlacement.FromBottom:
                case DlgPlacement.FromBottomLeft:
                case DlgPlacement.FromBottomRight:
                    trans.Add(new EdgeUIThemeTransition { Edge = EdgeTransitionLocation.Bottom });
                    break;
                default:
                    trans = null;
                    break;
            }
            Transitions = trans;
#endif
        }

        /// <summary>
        /// 应用遮罩
        /// </summary>
        void ApplyVeilBrush()
        {
            if (ShowVeil)
            {
                _canvas.Background = VeilBrush == null ? Res.深暗遮罩 : VeilBrush;
            }
            else if (_canvas.Background != null)
            {
                _canvas.Background = null;
            }
        }

        /// <summary>
        /// 点击对话框
        /// </summary>
        /// <param name="p_point">点击位置点坐标</param>
        /// <returns>是否继续调用下层对话框的 OnPressed</returns>
        bool IDlgPressed.OnPressed(Point p_point)
        {
            if (!this.ContainPoint(p_point))
            {
                OnOuterPressed(p_point);
            }
            else if (ShowVeil)
            {
                // 在对话框内部点击并且有遮罩时，传递到紧挨的下层对话框！！！
                // 比如：对话框有遮罩，点击内容弹出菜单，再点击对话框区域(非菜单内)，确保菜单能关闭
                var dlg = UITree.GetNextLevelDlg(_canvas);
                if (dlg != null)
                {
                    ((IDlgPressed)dlg).OnPressed(p_point);
                }
            }

            // 无遮罩 且 允许传递时 继续调用下层对话框的 OnPressed
            return !ShowVeil && AllowRelayPress;
        }

        /// <summary>
        /// 点击对话框外部
        /// </summary>
        /// <param name="p_point">外部点击位置</param>
        /// <returns>是否继续调用下层对话框的 OnPressed</returns>
        protected virtual void OnOuterPressed(Point p_point)
        {
            if (!IsPinned
                && (ClipElement == null || !ClipElement.ContainPoint(p_point)))
                RemoveFromCanvas();
        }

        /// <summary>
        /// 关闭或后退之前，返回false表示禁止关闭
        /// </summary>
        /// <param name="p_result">对话框关闭时的返回值</param>
        /// <returns>true 表允许关闭</returns>
        protected virtual Task<bool> OnClosing(bool p_result)
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// 关闭或后退之后
        /// </summary>
        /// <param name="p_result">对话框关闭时的返回值</param>
        protected virtual void OnClosed(bool p_result)
        {
        }
        #endregion

        #region 拖拽及调整大小
        /// <summary>
        /// 点击标题栏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void OnHeaderPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (GetResizeDirection(e.GetCurrentPoint(this).Position) == ResizeDirection.None && _headerGrid.CapturePointer(e.Pointer))
            {
                _isHeadPressed = true;
                Point pt = e.GetCurrentPoint(null).Position;
                _startPoint = new Point(pt.X - Left, pt.Y - Top);
                Dragging?.Invoke(this, pt);
            }
        }

        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnHeaderPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (_isHeadPressed)
            {
                var pt = e.GetCurrentPoint(null).Position;
                var left = pt.X - _startPoint.X;
                if (left < -ActualWidth + 40)
                    left = -ActualWidth + 40;
                else if (left > Kit.ViewWidth - 40)
                    left = Kit.ViewWidth - 40;

                var top = pt.Y - _startPoint.Y;
                if (top < 0)
                    top = 0;
                else if (top > Kit.ViewHeight - 40)
                    top = Kit.ViewHeight - 40;

                Left = left;
                Top = top;
                Dragging?.Invoke(this, pt);
            }
        }

        /// <summary>
        /// 抬起
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnHeaderPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (_isHeadPressed)
            {
                _isHeadPressed = false;
                _headerGrid.ReleasePointerCapture(e.Pointer);
                Dropped?.Invoke(e.GetCurrentPoint(null).Position);
            }
        }

        /// <summary>
        /// 点击RootGrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnRootGridPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            BringToTop();
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
                    Point offset = this.TransformToVisual(null).TransformPoint(new Point(0.0, 0.0));
                    _isResizing = true;
                    _resizeDirection = dir;
                    _startPoint = e.GetCurrentPoint(null).Position;
                    _initRect = new Rect(offset.X, offset.Y, ActualWidth, ActualHeight);
                    UpdateMouseCursor(_resizeDirection);

                    // 调整大小的外框
                    _bdResize = new Border
                    {
                        BorderBrush = Res.亮红,
                        Width = ActualWidth,
                        Height = ActualHeight,
                        BorderThickness = new Thickness(2),
                    };
                    Canvas.SetLeft(_bdResize, offset.X);
                    Canvas.SetTop(_bdResize, offset.Y);
                    int zindex = TopMost ? _topmostZIndex : _normalZIndex;
                    Canvas.SetZIndex(_bdResize, zindex + 10);
                    UITree.AddDlgResizeFlag(_bdResize);
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
                    newWidth = Math.Min(maxWidth, Math.Max(minWidth, _initRect.Width + _startPoint.X - pt.X));
                    _bdResize.Width = newWidth;
                    Canvas.SetLeft(_bdResize, _initRect.Right - newWidth);
                    return;

                case ResizeDirection.TopLeft:
                    newWidth = Math.Min(maxWidth, Math.Max(minWidth, _initRect.Width + _startPoint.X - pt.X));
                    _bdResize.Width = newWidth;
                    Canvas.SetLeft(_bdResize, _initRect.Right - newWidth);
                    newHeight = Math.Min(maxHeight, Math.Max(minHeight, _initRect.Height + _startPoint.Y - pt.Y));
                    _bdResize.Height = newHeight;
                    Canvas.SetTop(_bdResize, _initRect.Bottom - newHeight);
                    return;

                case ResizeDirection.Top:
                    newHeight = Math.Min(maxHeight, Math.Max(minHeight, _initRect.Height + _startPoint.Y - pt.Y));
                    _bdResize.Height = newHeight;
                    Canvas.SetTop(_bdResize, _initRect.Bottom - newHeight);
                    return;

                case ResizeDirection.TopRight:
                    newHeight = Math.Min(maxHeight, Math.Max(minHeight, _initRect.Height + _startPoint.Y - pt.Y));
                    _bdResize.Height = newHeight;
                    Canvas.SetTop(_bdResize, _initRect.Bottom - newHeight);
                    _bdResize.Width = Math.Min(maxWidth, Math.Max(minWidth, _initRect.Width + pt.X - _startPoint.X));
                    return;

                case ResizeDirection.Right:
                    _bdResize.Width = Math.Min(maxWidth, Math.Max(minWidth, _initRect.Width + pt.X - _startPoint.X));
                    return;

                case ResizeDirection.BottomRight:
                    _bdResize.Height = Math.Min(maxHeight, Math.Max(minHeight, _initRect.Height + pt.Y - _startPoint.Y));
                    _bdResize.Width = Math.Min(maxWidth, Math.Max(minWidth, _initRect.Width + pt.X - _startPoint.X));
                    return;

                case ResizeDirection.Bottom:
                    _bdResize.Height = Math.Min(maxHeight, Math.Max(minHeight, _initRect.Height + pt.Y - _startPoint.Y));
                    return;

                case ResizeDirection.BottomLeft:
                    newWidth = Math.Min(maxWidth, Math.Max(minWidth, _initRect.Width + _startPoint.X - pt.X));
                    _bdResize.Width = newWidth;
                    Canvas.SetLeft(_bdResize, _initRect.Right - newWidth);
                    _bdResize.Height = Math.Min(maxHeight, Math.Max(minHeight, _initRect.Height + pt.Y - _startPoint.Y));
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
                UITree.RemoveDlgResizeFlag(_bdResize);
                _bdResize = null;

                // 触发事件
                Resized?.Invoke();
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
#if ENABLECURSOR
            switch (direction)
            {
                case ResizeDirection.Left:
                case ResizeDirection.Right:
                    ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.SizeWestEast);
                    break;

                case ResizeDirection.TopLeft:
                case ResizeDirection.BottomRight:
                    ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.SizeNorthwestSoutheast);
                    break;

                case ResizeDirection.Top:
                case ResizeDirection.Bottom:
                    ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.SizeNorthSouth);
                    break;

                case ResizeDirection.TopRight:
                case ResizeDirection.BottomLeft:
                    ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.SizeNortheastSouthwest);
                    break;

                default:
                    ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Arrow);
                    break;
            }
#endif
        }
        #endregion

        #region 标题栏右键菜单
#if WIN
        static Menu _menu;

        async void OnHeaderRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if (_menu == null)
            {
                _menu = new Menu { IsContextMenu = true };

                Mi mi = new Mi { ID = "复制类名" };
                mi.Click += OnCopyDlg;
                _menu.Items.Add(mi);

                mi = new Mi { ID = "复制内容类名" };
                mi.Click += OnCopyContent;
                _menu.Items.Add(mi);

                mi = new Mi { ID = "预览Xaml" };
                mi.Click += OnDlgXaml;
                _menu.Items.Add(mi);

                mi = new Mi { ID = "预览内容Xaml" };
                mi.Click += OnContentXaml;
                _menu.Items.Add(mi);
            }

            _menu.DataContext = this;
            await _menu.OpenContextMenu(e.GetPosition(null));
        }

        static void OnCopyDlg(Mi e)
        {
            var dlg = _menu.DataContext as Dlg;
            Kit.CopyToClipboard(dlg.GetType().FullName, true);
        }

        static void OnCopyContent(Mi e)
        {
            var dlg = _menu.DataContext as Dlg;
            if (dlg.Content != null)
            {
                Kit.CopyToClipboard(dlg.Content.GetType().FullName, true);
            }
            else
            {
                Kit.Warn("Dlg无内容！");
            }
        }

        static void OnDlgXaml(Mi e)
        {
            Type tp = _menu.DataContext.GetType();
            if (tp == typeof(Dlg))
            {
                Kit.Warn("标准的Dlg，无Xaml内容！");
                return;
            }

            string res = Docking.TabHeader.GetSourcePath(tp);
            if (res == null)
            {
                Kit.Warn($"未找到 [{tp.FullName}] 的Xaml内容！");
            }
            else
            {
                Docking.TabHeader.ShowXamlDlg(res, tp);
            }
        }

        static void OnContentXaml(Mi e)
        {
            var dlg = _menu.DataContext as Dlg;
            if (dlg.Content != null)
            {
                Type tp = dlg.Content.GetType();
                string res = Docking.TabHeader.GetSourcePath(tp);
                if (res == null)
                {
                    Kit.Warn($"未找到 [{tp.FullName}] 的Xaml内容！");
                }
                else
                {
                    Docking.TabHeader.ShowXamlDlg(res, tp);
                }
            }
            else
            {
                Kit.Warn("Dlg无内容！");
            }
        }
#endif
        #endregion

        #region 释放资源
        /// <summary>
        /// 实现 IDestroy 接口
        /// </summary>
        public void Destroy()
        {
            _cleaner.Add(this);
        }

        void OnOwnWinDestroyed(Win e)
        {
            e.Destroyed -= OnOwnWinDestroyed;
            Close();
            _cleaner.Add(this);
        }

        static readonly DlgCleaner _cleaner = new DlgCleaner();
        class DlgCleaner
        {
            readonly BlockingCollection<Dlg> _queue;

            public DlgCleaner()
            {
                _queue = new BlockingCollection<Dlg>();
                Task.Run(Clean);
            }

            public bool Add(Dlg p_dlg)
            {
                if (p_dlg == null || p_dlg.Content == null)
                    return false;
                return _queue.TryAdd(p_dlg);
            }

            void Clean()
            {
                while (true)
                {
                    try
                    {
                        var dlg = _queue.Take();
                        Kit.RunSync(() =>
                        {
                            try
                            {
                                if (dlg.Content is IDestroy tc)
                                {
                                    tc.Destroy();
                                }
                                else if (dlg.Content is UIElement elem)
                                {
                                    foreach (var cl in elem.FindChildrenByType<IDestroy>())
                                    {
                                        cl.Destroy();
                                    }
                                }
                            }
                            catch { }

                            dlg.Content = null;
                            dlg = null;
                        });
                    }
                    catch { }
                }
            }
        }
        #endregion
    }
}
