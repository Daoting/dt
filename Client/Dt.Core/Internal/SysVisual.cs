#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Linq;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 系统可视树管理类，三层：桌面层/页面层、对话框层、提示信息层
    /// </summary>
    internal static class SysVisual
    {
        #region 成员变量
        /// <summary>
        /// Window.Current.Content内容，根Grid
        /// </summary>
        static readonly Grid _rootGrid;

        /// <summary>
        /// 对话框面板
        /// </summary>
        static readonly Canvas _dlgCanvas;

        /// <summary>
        /// 提示信息面板
        /// </summary>
        static readonly StackPanel _notifyPanel;
        static readonly PointerEventHandler _pressedHandler = new PointerEventHandler(OnPanelPointerPressed);
        static readonly SolidColorBrush _veilBrush = new SolidColorBrush(Color.FromArgb(0x66, 0, 0, 0));

        /// <summary>
        /// phone状态栏高度
        /// </summary>
        public static readonly int StatusBarHeight;
        #endregion

        static SysVisual()
        {
            // 根Grid
            _rootGrid = new Grid();

            // 桌面层/页面层，此层调整为动态添加！为uno节省级数！启动时为临时提示信息
            TextBlock tb = new TextBlock
            {
                Text = "正在启动...",
                FontSize = 15,
                Foreground = new SolidColorBrush(Windows.UI.Colors.White),
                Margin = new Thickness(40, 0, 40, 0),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            _rootGrid.Children.Add(tb);

            // 对话框层
            _dlgCanvas = new Canvas();
            _rootGrid.AddHandler(UIElement.PointerPressedEvent, _pressedHandler, true);
            _rootGrid.Children.Add(_dlgCanvas);

            // 提示信息层
            NotifyList = new ItemList<NotifyInfo>();
            _notifyPanel = new StackPanel();
            _notifyPanel.Spacing = 10;
            if (AtSys.IsPhoneUI)
            {
                _notifyPanel.HorizontalAlignment = HorizontalAlignment.Stretch;
                _notifyPanel.VerticalAlignment = VerticalAlignment.Top;
                _rootGrid.Children.Add(_notifyPanel);
            }
            else
            {
                _notifyPanel.Width = 240;
                _notifyPanel.HorizontalAlignment = HorizontalAlignment.Right;
                _notifyPanel.VerticalAlignment = VerticalAlignment.Bottom;
                _rootGrid.Children.Add(_notifyPanel);
            }

#if UWP || WASM
            _rootGrid.SizeChanged += OnSizeChanged;
#else
            // 状态栏边距
            StatusBarHeight = (int)Application.Current.Resources["StatusBarHeight"];
            _rootGrid.Padding = new Thickness(0, StatusBarHeight, 0, 0);
#endif
            // 主题蓝色
            _rootGrid.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(0xFF, 0x1B, 0xA1, 0xE2));
            Window.Current.Content = _rootGrid;
        }

        /// <summary>
        /// 获取设置桌面层/页面层的内容元素，桌面、Frame、登录页面，在最底层
        /// </summary>
        public static UIElement RootContent
        {
            get
            {
                if (_rootGrid.Children.Count > 2)
                    return (UIElement)_rootGrid.Children[0];
                return null;
            }
            set
            {
                if (_rootGrid.Children.Count > 2)
                    _rootGrid.Children.RemoveAt(0);
                if (value != null)
                    _rootGrid.Children.Insert(0, value);
            }
        }

        /// <summary>
        /// PhoneUI模式的根Frame
        /// </summary>
        public static Frame RootFrame
        {
            get
            {
                if (_rootGrid.Children.Count > 2)
                    return (Frame)_rootGrid.Children[0];
                return null;
            }
        }

        /// <summary>
        /// 获取提示信息列表，避免启动时AtKit的初始化
        /// </summary>
        public static ItemList<NotifyInfo> NotifyList { get; }

        /// <summary>
        /// UI模式切换的回调方法，Phone UI 与 PC UI 切换
        /// </summary>
        public static Action UIModeChanged { get; set; }

        /// <summary>
        /// UI调度对象，uno中无法通过CoreApplication.MainView.CoreWindow.Dispatcher获取
        /// </summary>
        public static CoreDispatcher Dispatcher
        {
            get { return _rootGrid.Dispatcher; }
        }

        #region 对话框
        /// <summary>
        /// 是否存在对话框
        /// </summary>
        public static bool ExistDlg
        {
            get { return _dlgCanvas.Children.Count > 0; }
        }

        /// <summary>
        /// 将对话框添加到可视树
        /// </summary>
        /// <param name="p_dlg">对话框</param>
        /// <returns></returns>
        public static bool AddDlg(UIElement p_dlg)
        {
            if (p_dlg == null || !(p_dlg is IDlgOuterPressed))
                return false;

            // Phone模式遮罩
            if (!_dlgCanvas.Children.Contains(p_dlg))
            {
                if (AtSys.IsPhoneUI && _dlgCanvas.Children.Count == 0)
                    _dlgCanvas.Background = _veilBrush;
                _dlgCanvas.Children.Add(p_dlg);
            }
            return true;
        }

        /// <summary>
        /// 从可视树移除对话框
        /// </summary>
        /// <param name="p_dlg">对话框</param>
        public static void RemoveDlg(UIElement p_dlg)
        {
            if (p_dlg == null
                || !(p_dlg is IDlgOuterPressed)
                || !_dlgCanvas.Children.Contains(p_dlg))
                return;

            _dlgCanvas.Children.Remove(p_dlg);
            // 移除Phone模式遮罩
            if (AtSys.IsPhoneUI && _dlgCanvas.Children.Count == 0)
                _dlgCanvas.ClearValue(Panel.BackgroundProperty);
        }

        /// <summary>
        /// 是否存在某对话框
        /// </summary>
        /// <param name="p_dlg">对话框</param>
        /// <returns></returns>
        public static bool ContainsDlg(UIElement p_dlg)
        {
            return p_dlg != null && _dlgCanvas.Children.Contains(p_dlg);
        }

        /// <summary>
        /// 获取最上面的对话框
        /// </summary>
        /// <returns></returns>
        public static UIElement GetTopDlg()
        {
            UIElement elem = null;
            int z = -1;
            foreach (var item in _dlgCanvas.Children.OfType<UIElement>())
            {
                int index = Canvas.GetZIndex(item);
                if (index > z)
                {
                    z = index;
                    elem = item;
                }
            }
            return elem;
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
        /// 始终处理所有点击事件，以便处理点击对话框外部
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void OnPanelPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (_dlgCanvas.Children.Count == 0)
                return;

            var pt = e.GetCurrentPoint(null).Position;
            // 临时列表，循环内部会删除_dlgCanvas的元素！
            var ls = _dlgCanvas.Children.OfType<IDlgOuterPressed>().ToList();
            foreach (var dlg in ls)
            {
                FrameworkElement elem = (FrameworkElement)dlg;
                MatrixTransform trans = elem.TransformToVisual(null) as MatrixTransform;
                if (trans == null)
                    continue;

                double offsetX = trans.Matrix.OffsetX;
                double offsetY = trans.Matrix.OffsetY;
                // 对话框内部不处理
                if (pt.X > offsetX
                    && pt.X < offsetX + elem.ActualWidth
                    && pt.Y > offsetY
                    && pt.Y < offsetY + elem.ActualHeight)
                    continue;

                dlg.OnOuterPressed(pt);
            }
        }
        #endregion

        #region Notify
        /// <summary>
        /// UI添加一条提示信息
        /// </summary>
        /// <param name="p_index"></param>
        /// <param name="p_item"></param>
        public static void InsertNotifyItem(int p_index, UIElement p_item)
        {
            _notifyPanel.Children.Insert(p_index, p_item);
        }

        /// <summary>
        /// UI删除一条提示信息
        /// </summary>
        /// <param name="p_index"></param>
        public static void RemoveNotifyItem(int p_index)
        {
            _notifyPanel.Children.RemoveAt(p_index);
        }

        /// <summary>
        /// UI清空所有提示信息
        /// </summary>
        public static void ClearAllNotify()
        {
            _notifyPanel.Children.Clear();
        }
        #endregion

        #region 可视区域
        /// <summary>
        /// 可视区域宽度
        /// 手机：页面宽度
        /// PC上：除标题栏和外框的窗口内部宽度
        /// </summary>
        public static double ViewWidth
        {
            get { return _rootGrid.ActualWidth; }
        }

        /// <summary>
        /// 可视区域高度
        /// 手机：不包括状态栏的高度
        /// PC上：除标题栏和外框的窗口内部高度
        /// </summary>
        public static double ViewHeight
        {
            get
            {
                // ApplicationView.GetForCurrentView().VisibleBounds在uno中大小不正确！！！
                // Android上高度多50
                return _rootGrid.ActualHeight - StatusBarHeight;
            }
        }
        #endregion

#if UWP || WASM
        /// <summary>
        /// 系统区域大小变化时UI自适应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            bool isPhoneUI = e.NewSize.Width < 600;
            if (isPhoneUI != AtSys.IsPhoneUI)
            {
                AtSys.IsPhoneUI = isPhoneUI;

                // 调整对话框层
                _dlgCanvas.Children.Clear();
                _dlgCanvas.ClearValue(Panel.BackgroundProperty);
                _dlgCanvas.PointerPressed -= _pressedHandler;
                _rootGrid.RemoveHandler(UIElement.PointerPressedEvent, _pressedHandler);
                if (AtSys.IsPhoneUI)
                    _dlgCanvas.PointerPressed += _pressedHandler;
                else
                    _rootGrid.AddHandler(UIElement.PointerPressedEvent, _pressedHandler, true);

                // 调整提示信息层
                if (AtSys.IsPhoneUI)
                {
                    _notifyPanel.Width = double.NaN;
                    _notifyPanel.HorizontalAlignment = HorizontalAlignment.Stretch;
                    _notifyPanel.VerticalAlignment = VerticalAlignment.Top;
                }
                else
                {
                    _notifyPanel.Width = 240;
                    _notifyPanel.HorizontalAlignment = HorizontalAlignment.Right;
                    _notifyPanel.VerticalAlignment = VerticalAlignment.Bottom;
                }

                UIModeChanged?.Invoke();
            }
        }
#endif
    }

    /// <summary>
    /// 对话框处理点击接口
    /// </summary>
    public interface IDlgOuterPressed
    {
        /// <summary>
        /// 点击对话框外部
        /// </summary>
        /// <param name="p_point">点击位置点坐标</param>
        void OnOuterPressed(Point p_point);
    }
}