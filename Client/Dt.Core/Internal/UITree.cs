#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Windows.Foundation;
using Windows.UI;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 系统可视树管理类，三层：桌面层/页面层、对话框层、提示信息层
    /// uwp的完整UI树：ScrollViewer - Border - app UI
    /// uno的完整UI树：Grid - Border - app UI
    /// app UI：Grid - 桌面层/页面层
    ///              - 对话框层
    ///              - 提示信息层
    /// </summary>
    internal static class UITree
    {
        #region 成员变量
        //**************************************************************************
        // 响应式设计：4种界面宽度与布局方式，默认左右面板400px，主区最小宽度480px
        // 1. 界面宽度 > 1280px，三列或更多列面板
        // 2. 界面宽度在 881px ~ 1280px，最多两列面板，先停靠左侧面板
        // 3. 界面宽度 641px ~ 880px，只一列面板，左右侧面板都停靠
        // 4. 界面宽度 <= 640px，PhoneUI模式
        // Win中的LayoutManager已对后两种宽度自动处理
        //**************************************************************************

        /// <summary>
        /// PhoneUI模式的最大宽度
        /// </summary>
        const double _maxPhoneUIWidth = 640;

        /// <summary>
        /// 对话框面板
        /// </summary>
        static readonly Canvas _dlgCanvas;

        static PointerEventHandler _pressedHandler = new PointerEventHandler(OnPanelPointerPressed);

        /// <summary>
        /// 内容元素的容器
        /// </summary>
        static readonly Border _contentBorder;

        #endregion

        #region 静态构造
        static UITree()
        {
#if WIN
            // WinUI中Window.Current为null
            MainWin = new Window { Title = Kit.Title };
            CustomWin();
            MainWin.Activate();
#else
            // uno中若新创建，Window.Bounds始终为(0, 0)！
            MainWin = Window.Current;
            MainWin.Title = Kit.Title;
#endif

            // iOS android 启动就崩溃的现象已提uno讨论：https://github.com/unoplatform/uno/discussions/8933
            // 浪费十多天生命，怎能以一个操字了得！根本原因是项目类型过多引起uno异常，uno认为是.net6.0的原因
            // 解决方法：uno增加常量 UNO_DISABLE_KNOWN_MISSING_TYPES , 控制调试状态不检查未绑定类型
            // iOS 在 debug release 状态都不再崩溃
            // android 在 uno4.5 后只联调状态正常，单独运行仍然启动就崩溃
            // 仍然采用老的解决方法：必须最早创建Frame，不能注掉！！！
            // 升级.net8.0后android已不再崩溃，不再崩溃，UNO_DISABLE_KNOWN_MISSING_TYPES会造成编译错误，已无用！
            //#if ANDROID
            //          new Frame().Navigate(typeof(Page));
            //#endif

            // 背景画刷，默认主蓝
            var theme = Kit.GetService<ITheme>();
            Brush bgBrush = (theme == null) ?
                new SolidColorBrush(Color.FromArgb(0xFF, 0x1B, 0xA1, 0xE2))
                : theme.ThemeBrush;

            // 根Grid，背景为主题画刷
            RootGrid = new Grid { Background = bgBrush };

            // 最低层，不可见，截图用
            SnapBorder = new Border();
            RootGrid.Children.Add(SnapBorder);

            // 桌面层/页面层，容器为Border，首页加载快时不显示进度动画！
            _contentBorder = new Border();
            _contentBorder.Child = new ProgressRing
            {
                Height = 80,
                Width = 80,
                IsActive = true,
                Background = bgBrush,
                Foreground = new SolidColorBrush(Colors.White),
            };
            RootGrid.Children.Add(_contentBorder);

            // 对话框层
            _dlgCanvas = new Canvas();
            RootGrid.AddHandler(UIElement.PointerPressedEvent, _pressedHandler, true);
            RootGrid.Children.Add(_dlgCanvas);

            // 提示信息层
            NotifyPanel = new StackPanel { Spacing = 10 };
            RootGrid.Children.Add(NotifyPanel);

#if IOS
            // 状态栏边距
#pragma warning disable CA1422 // 类型或成员已过时
            StatusBarHeight = (int)UIKit.UIApplication.SharedApplication.StatusBarFrame.Height;
#pragma warning restore CA1422
            RootGrid.Padding = new Thickness(0, StatusBarHeight, 0, 0);
#elif ANDROID
            // .net6.0 后 Window.Bounds 不包含顶部状态栏高度！！！
            // Android上已设置不占用顶部状态栏和底部导航栏，但 Window.Bounds 包含顶部状态栏高度！
            // uno5.5.95后以上设置无效，全屏，用边距确保不占用状态栏，
            var res = Android.App.Application.Context.Resources;
            int resourceId = res.GetIdentifier("status_bar_height", "dimen", "android");
            if (resourceId > 0)
            {
                StatusBarHeight = (int)(res.GetDimensionPixelSize(resourceId) / res.DisplayMetrics.Density);
                RootGrid.Padding = new Thickness(0, StatusBarHeight, 0, 0);
            }
#endif

            // 桌面版系统支持Phone模式和Win模式自适应，ios android 不支持，只Phone模式
            // 因无法区分linux和wasm所在的OS，都当作桌面版处理，跑在手机端的wasm也可自适应
#if WIN
            MainWin.SizeChanged += OnWindowSizeChanged;
            Kit.IsPhoneUI = MainWin.Bounds.Width < _maxPhoneUIWidth;
#elif WASM || SKIA
            // skia 和 wasm 上Window.Bounds初始为(0,0)
            // wasm 上 MainWin.SizeChanged 事件初次不触发，触发顺序：OnRootSizeChanged OnWindowSizeChanged
            // skia 上 MainWin.SizeChanged 事件初次触发两次，第一次(1,1)，触发顺序：OnWindowSizeChanged OnRootSizeChanged
            // 故只附加 RootGrid.SizeChanged
            RootGrid.SizeChanged += OnRootSizeChanged;
#endif

            ApplyNotifyStyle();
            MainWin.Content = RootGrid;
            MainWin.Activate();
            Kit.Debug("创建可视树");
        }

        /// <summary>
        /// 创建窗口及整个系统可视树，在静态构造方法中完成，避免重复创建
        /// </summary>
        internal static void Init()
        { }
        #endregion

        #region 基础
        /// <summary>
        /// 主窗口
        /// </summary>
        public static readonly Window MainWin;

        /// <summary>
        /// Window.Content内容，根Grid
        /// </summary>
        public static readonly Grid RootGrid;

        /// <summary>
        /// 在最低层，不可见，截图用的Border容器
        /// </summary>
        public static readonly Border SnapBorder;

        /// <summary>
        /// 获取设置桌面层/页面层的内容元素，桌面、Frame、登录页面，在最底层
        /// </summary>
        public static UIElement RootContent
        {
            get { return _contentBorder.Child; }
            set
            {
                SetDefaultStyle(value as Control);
                _contentBorder.Child = value;

#if WIN
                // 因非调试状态切换两UI模式时造成闪退！Win上始终自定义标题栏
                if (value is Frame frame)
                {
                    // 顶部空出标题栏
                    _contentBorder.Padding = new Thickness(0, MainWin.AppWindow.TitleBar.Height, 0, 0);
                    // 清除标题栏可Passthrough区域
                    var nonClient = Microsoft.UI.Input.InputNonClientPointerSource.GetForWindowId(Kit.MainWin.AppWindow.Id);
                    nonClient.ClearRegionRects(Microsoft.UI.Input.NonClientRegionKind.Passthrough);
                }
                else
                {
                    _contentBorder.Padding = new Thickness();
                }
#endif
            }
        }

        /// <summary>
        /// PhoneUI模式的根Frame
        /// </summary>
        public static Frame RootFrame
        {
            get { return (Frame)_contentBorder.Child; }
        }

        /// <summary>
        /// phone状态栏高度
        /// </summary>
        public static int StatusBarHeight = 0;

        /// <summary>
        /// 内容元素的容器
        /// </summary>
        internal static Border ContentBorder => _contentBorder;
        #endregion

        #region 对话框
        /// <summary>
        /// 是否存在对话框
        /// </summary>
        public static bool ExistDlg
        {
            get { return _dlgCanvas.Children.Count > 0; }
        }

        /// <summary>
        /// 将对话框添加到可视树，Canvas作为对话框背景遮罩
        /// </summary>
        /// <param name="p_cvs">对话框遮罩容器</param>
        /// <returns></returns>
        public static bool AddDlg(Canvas p_cvs)
        {
            if (p_cvs == null
                || _dlgCanvas.Children.Contains(p_cvs)
                || p_cvs.Children.Count != 1)
                return false;

            SetDefaultStyle(p_cvs.Children[0] as Control);
            _dlgCanvas.Children.Add(p_cvs);
            return true;
        }

        /// <summary>
        /// 从可视树移除对话框
        /// </summary>
        /// <param name="p_cvs">对话框遮罩容器</param>
        public static void RemoveDlg(Canvas p_cvs)
        {
            _dlgCanvas.Children.Remove(p_cvs);
        }

        /// <summary>
        /// 是否存在某对话框
        /// </summary>
        /// <param name="p_cvs">对话框遮罩容器</param>
        /// <returns></returns>
        public static bool ContainsDlg(Canvas p_cvs)
        {
            return p_cvs != null && _dlgCanvas.Children.Contains(p_cvs);
        }

        /// <summary>
        /// 获取最上面的对话框
        /// </summary>
        /// <returns></returns>
        public static UIElement GetTopDlg()
        {
            Canvas cvs = null;
            int z = -1;
            foreach (var item in _dlgCanvas.Children.OfType<Canvas>())
            {
                int index = Canvas.GetZIndex(item);
                if (index > z)
                {
                    z = index;
                    cvs = item;
                }
            }

            if (cvs != null && cvs.Children.Count > 0)
                return cvs.Children[0];
            return null;
        }

        /// <summary>
        /// 获取紧挨着的下层对话框，ZIndex小一
        /// </summary>
        /// <param name="p_cvs"></param>
        /// <returns></returns>
        public static UIElement GetNextLevelDlg(Canvas p_cvs)
        {
            int index = Canvas.GetZIndex(p_cvs);
            foreach (var item in _dlgCanvas.Children.OfType<Canvas>())
            {
                if (Canvas.GetZIndex(item) + 1 == index
                    && item.Children.Count > 0)
                {
                    return item.Children[0];
                }
            }
            return null;
        }

        /// <summary>
        /// 将调整对话框大小的外框添加到可视树
        /// </summary>
        /// <param name="p_border">外框</param>
        public static void AddDlgResizeFlag(UIElement p_border)
        {
            if (p_border != null)
                _dlgCanvas.Children.Add(p_border);
        }

        /// <summary>
        /// 从可视树移除调整大小的外框
        /// </summary>
        /// <param name="p_border">外框</param>
        public static void RemoveDlgResizeFlag(UIElement p_border)
        {
            if (p_border != null)
                _dlgCanvas.Children.Remove(p_border);
        }

        /// <summary>
        /// 对话框个数
        /// </summary>
        public static int DlgCount
        {
            get { return _dlgCanvas.Children.Count; }
        }

        /// <summary>
        /// 始终处理所有点击事件，以便处理点击对话框外部
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void OnPanelPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (_dlgCanvas.Children.Count == 0)
                return;

            // 将对话框按从上层到下层(ZIndex)的顺序保存到临时列表，因循环过程中会删除_dlgCanvas的元素！
            var ls = _dlgCanvas.Children.OfType<Canvas>().OrderByDescending((cvs) => Canvas.GetZIndex(cvs)).ToList();
            var pt = e.GetCurrentPoint(null).Position;
            foreach (var cvs in ls)
            {
                if (cvs.Children.Count > 0
                    && cvs.Children[0] is IDlgPressed dlg
                    && !dlg.OnPressed(pt))
                {
                    break;
                }
            }
        }
        #endregion

        #region Notify
        /// <summary>
        /// 提示信息面板
        /// </summary>
        public static readonly StackPanel NotifyPanel;

        /// <summary>
        /// 调整提示信息层样式
        /// </summary>
        static void ApplyNotifyStyle()
        {
            if (Kit.IsPhoneUI)
            {
                NotifyPanel.Width = double.NaN;
                NotifyPanel.HorizontalAlignment = HorizontalAlignment.Stretch;
                NotifyPanel.VerticalAlignment = VerticalAlignment.Top;
            }
            else
            {
                NotifyPanel.Width = 240;
                NotifyPanel.HorizontalAlignment = HorizontalAlignment.Right;
                NotifyPanel.VerticalAlignment = VerticalAlignment.Bottom;
            }
        }
        #endregion

        #region UI自适应
        // 系统区域大小变化时UI自适应
#if WIN
        static void OnWindowSizeChanged(object sender, Microsoft.UI.Xaml.WindowSizeChangedEventArgs e)
        {
            OnWindowSizeChanged(e.Size.Width);
        }

        static void OnWindowSizeChanged(double p_width)
        {
            bool isPhoneUI = p_width < _maxPhoneUIWidth;
            if (isPhoneUI == Kit.IsPhoneUI)
                return;

            Kit.IsPhoneUI = isPhoneUI;

            // 切换更丝滑
            MainWin.DispatcherQueue.TryEnqueue(new DispatcherQueueHandler(() =>
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine("切换UI模式");
#endif
                ApplyNotifyStyle();

                // 登录之前无UI自适应！有向导对话框时造成关闭
                var tp = RootContent.GetType().Name;
                if (tp != "Frame" && tp != "Desktop")
                    return;

                // 调整对话框层
                _dlgCanvas.Children.Clear();
                Kit.OnUIModeChanged();

                // 在非调试状态，切换两UI模式时造成闪退崩溃！！！
                //MainWin.ExtendsContentIntoTitleBar = !isPhoneUI;
            }));
        }
#elif WASM || SKIA
        static void OnRootSizeChanged(object sender, SizeChangedEventArgs e)
        {
            bool isPhoneUI = e.NewSize.Width < _maxPhoneUIWidth;
            if (isPhoneUI == Kit.IsPhoneUI)
                return;

            Kit.IsPhoneUI = isPhoneUI;
            ApplyNotifyStyle();

            // 登录之前无UI自适应！有向导对话框时造成关闭
            var tp = RootContent.GetType().Name;
            if (tp != "Frame" && tp != "Desktop")
                return;

            // 调整对话框层
            _dlgCanvas.Children.Clear();
            Kit.OnUIModeChanged();
        }
#endif
        #endregion

        #region 窗口
#if WIN
        const string _maximizeFlagFile = "maximize.flag";
        const string _phoneUIFlagFile = "phoneui.flag";

        /// <summary>
        /// 调整主窗口大小
        /// </summary>
        /// <param name="p_width"></param>
        /// <param name="p_height"></param>
        public static void ChangeMainWinSize(int p_width, int p_height)
        {
            // 取消窗口大小变化事件，因触发多次
            MainWin.SizeChanged -= OnWindowSizeChanged;

            // 获取屏幕居中位置
            var area = Microsoft.UI.Windowing.DisplayArea.Primary;
            int left = ((int)area.OuterBounds.Width - p_width) / 2;
            int top = ((int)area.OuterBounds.Height - p_height) / 2;
            
            // 用 MoveAndResize 不准，宽高莫名少一丢丢，未发现规律
            MainWin.AppWindow.MoveAndResize(new Windows.Graphics.RectInt32(left, top, p_width + 16, p_height + 8));

            MainWin.SizeChanged += OnWindowSizeChanged;
            OnWindowSizeChanged(p_width);
        }

        /// <summary>
        /// 自定义窗口
        /// </summary>
        static void CustomWin()
        {
            // 自定义窗口标题
            MainWin.ExtendsContentIntoTitleBar = true;
            MainWin.Closed += OnSaveWinState;

            // 加载历史窗口大小状态：最大化、PhoneUI、普通，普通设置 1280 X 720
            if (File.Exists(Path.Combine(Kit.CachePath, _maximizeFlagFile)))
            {
                var presenter = (Microsoft.UI.Windowing.OverlappedPresenter)MainWin.AppWindow.Presenter;
                presenter.Maximize();
            }
            else if (File.Exists(Path.Combine(Kit.CachePath, _phoneUIFlagFile)))
            {
                ChangeMainWinSize(480, 850);
            }
            else
            {
                ChangeMainWinSize(1280, 720);
            }
        }

        /// <summary>
        /// 窗口关闭时，记录窗口大小状态：最大化、PhoneUI、普通
        /// </summary>
        static void OnSaveWinState(object sender, WindowEventArgs args)
        {
            var maxFile = Path.Combine(Kit.CachePath, _maximizeFlagFile);
            bool existMaxFile = File.Exists(maxFile);
            var isMaximized = ((Microsoft.UI.Windowing.OverlappedPresenter)MainWin.AppWindow.Presenter).State == Microsoft.UI.Windowing.OverlappedPresenterState.Maximized;
            if (isMaximized)
            {
                if (!existMaxFile)
                {
                    // 空的标志文件
                    File.Create(maxFile).Close();
                }
                return;
            }

            if (existMaxFile)
            {
                try
                {
                    File.Delete(maxFile);
                }
                catch { }
            }

            var phoneFile = Path.Combine(Kit.CachePath, _phoneUIFlagFile);
            bool existPhoneFile = File.Exists(phoneFile);
            if (Kit.IsPhoneUI)
            {
                if (!existPhoneFile)
                {
                    // 空的标志文件
                    File.Create(phoneFile).Close();
                }
            }
            else if (existPhoneFile)
            {
                try
                {
                    File.Delete(phoneFile);
                }
                catch { }
            }
        }
#endif
        #endregion

        #region 设置默认样式
        static void SetDefaultStyle(Control p_con)
        {
            if (p_con != null)
            {
                // 统一设置默认字体大小
                // 原系统的默认大小：Control为11，TextBlock为14，Frame为15
                p_con.FontSize = 16;
            }
        }
        #endregion
    }

    /// <summary>
    /// 对话框处理点击接口
    /// </summary>
    public interface IDlgPressed
    {
        /// <summary>
        /// 点击对话框
        /// </summary>
        /// <param name="p_point">点击位置点坐标</param>
        /// <returns>是否继续调用下层对话框的 OnPressed</returns>
        bool OnPressed(Point p_point);
    }
}