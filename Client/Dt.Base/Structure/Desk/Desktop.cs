#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-05-13 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 桌面容器
    /// </summary>
    public partial class Desktop : Control
    {
        #region 静态成员
        const double _defaultSideSize = 400;
        const double _minSideSize = 120;

        /// <summary>
        /// 内部主窗口
        /// </summary>
        public readonly static DependencyProperty MainWinProperty = DependencyProperty.Register(
            "MainWin",
            typeof(IWin),
            typeof(Desktop),
            new PropertyMetadata(null));

        /// <summary>
        /// 停靠在左侧的窗口
        /// </summary>
        public readonly static DependencyProperty LeftWinProperty = DependencyProperty.Register(
            "LeftWin",
            typeof(IWin),
            typeof(Desktop),
            new PropertyMetadata(null, OnLeftWinChanged));

        /// <summary>
        /// 停靠在右侧的窗口
        /// </summary>
        public readonly static DependencyProperty RightWinProperty = DependencyProperty.Register(
            "RightWin",
            typeof(IWin),
            typeof(Desktop),
            new PropertyMetadata(null, OnRightWinChanged));

        static void OnLeftWinChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Desktop dt = (Desktop)d;
            if (dt._grid == null)
                return;

            IWin win = (IWin)e.NewValue;
            if (win != null)
            {
                if (win == dt.RightWin)
                    dt.RightWin = null;
                dt._spLeft.Visibility = Visibility.Visible;
                dt._grid.ColumnDefinitions[0].Width = new GridLength(dt._leftSize.HasValue ? dt._leftSize.Value : _defaultSideSize);
            }
            else
            {
                dt._spLeft.Visibility = Visibility.Collapsed;
                dt._grid.ColumnDefinitions[0].Width = GridLength.Auto;
                dt._grid.ColumnDefinitions[1].Width = GridLength.Auto;
            }
        }

        static void OnRightWinChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Desktop dt = (Desktop)d;
            if (dt._grid == null)
                return;

            var win = (IWin)e.NewValue;
            if (win != null)
            {
                if (win == dt.LeftWin)
                    dt.LeftWin = null;
                dt._spRight.Visibility = Visibility.Visible;
                dt._grid.ColumnDefinitions[4].Width = new GridLength(dt._rightSize.HasValue ? dt._rightSize.Value : _defaultSideSize);
            }
            else
            {
                dt._spRight.Visibility = Visibility.Collapsed;
                dt._grid.ColumnDefinitions[3].Width = GridLength.Auto;
                dt._grid.ColumnDefinitions[4].Width = GridLength.Auto;
            }
        }

        /// <summary>
        /// 获取桌面实例
        /// </summary>
        public static Desktop Inst { get; internal set; }
        #endregion

        #region 成员变量
        readonly HashSet<IWin> _winCache = new HashSet<IWin>();
        Grid _grid;
        Splitter _spLeft;
        Splitter _spRight;
        double? _leftSize;
        double? _rightSize;
        #endregion

        #region 构造方法
        /// <summary>
        /// 构造方法
        /// </summary>
        public Desktop()
        {
            DefaultStyleKey = typeof(Desktop);
            Inst = this;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 获取设置内部主窗口
        /// </summary>
        public IWin MainWin
        {
            get { return (IWin)GetValue(MainWinProperty); }
            internal set { SetValue(MainWinProperty, value); }
        }

        /// <summary>
        /// 获取主页窗口
        /// </summary>
        public IWin HomeWin { get; internal set; }

        /// <summary>
        /// 获取设置停靠在左侧的窗口
        /// </summary>
        public IWin LeftWin
        {
            get { return (IWin)GetValue(LeftWinProperty); }
            internal set { SetValue(LeftWinProperty, value); }
        }

        /// <summary>
        /// 获取设置停靠在右侧的窗口
        /// </summary>
        public IWin RightWin
        {
            get { return (IWin)GetValue(RightWinProperty); }
            internal set { SetValue(RightWinProperty, value); }
        }
        #endregion

        #region 外部方法
        /// <summary>
        /// 设置左侧分割栏窗口
        /// </summary>
        /// <param name="p_win">窗口</param>
        /// <param name="p_width">宽度，null表默认</param>
        public static void SetLeftWin(IWin p_win, double? p_width = null)
        {
            if (p_win == null)
                return;

            Inst._leftSize = p_width;
            Inst.LeftWin = p_win;
        }

        /// <summary>
        /// 清除左侧分割栏
        /// </summary>
        public static void ClearLeftWin()
        {
            Inst._leftSize = null;
            Inst.LeftWin = null;
        }

        /// <summary>
        /// 设置右侧分割栏窗口
        /// </summary>
        /// <param name="p_win">窗口</param>
        /// <param name="p_width">宽度，null表默认</param>
        public static void SetRightWin(IWin p_win, double? p_width = null)
        {
            if (p_win == null)
                return;

            Inst._rightSize = p_width;
            Inst.RightWin = p_win;
        }

        /// <summary>
        /// 清除右侧分割栏
        /// </summary>
        public static void ClearRightWin()
        {
            Inst._rightSize = null;
            Inst.RightWin = null;
        }

        /// <summary>
        /// 显示新窗口并缓存
        /// </summary>
        /// <param name="p_win">窗口</param>
        internal bool ShowNewWin(IWin p_win)
        {
            if (_winCache.Add(p_win))
            {
                MainWin = p_win;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 激活窗口
        /// </summary>
        /// <param name="p_win">窗口</param>
        /// <returns>是否激活成功</returns>
        internal bool ActiveWin(IWin p_win)
        {
            if (_winCache.Contains(p_win))
            {
                ActiveWinInternal(p_win);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 根据窗口类型和初始参数激活窗口
        /// </summary>
        /// <param name="p_type">窗口类型</param>
        /// <param name="p_params">初始参数</param>
        /// <returns>激活的窗口</returns>
        internal IWin ActiveWin(Type p_type, object p_params)
        {
            foreach (var win in _winCache)
            {
                if (win.GetType() == p_type)
                {
                    if (win.Params == null
                        || (p_params != null
                            && win.Params != null
                            && JsonSerializer.Serialize(win.Params, JsonOptions.UnsafeSerializer) == JsonSerializer.Serialize(p_params, JsonOptions.UnsafeSerializer)))
                    {
                        ActiveWinInternal(win);
                        return win;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 关闭窗口并从缓存中移除
        /// </summary>
        /// <param name="p_win">窗口</param>
        /// <param name="p_nextWin">下一个待激活的窗口</param>
        internal async Task<bool> CloseWin(IWin p_win, IWin p_nextWin)
        {
            if (!_winCache.Contains(p_win))
                return false;

            // 触发关闭前事件，外部判断是否允许关闭
            if (!await p_win.OnClosing())
                return false;

            _winCache.Remove(p_win);

            // 若待关闭的窗口为激活状态
            if (MainWin == p_win)
            {
                if (p_nextWin != null)
                {
                    // 若待激活窗口停靠状态，先移除停靠
                    if (LeftWin == p_nextWin)
                        LeftWin = null;
                    else if (RightWin == p_nextWin)
                        RightWin = null;
                    MainWin = p_nextWin;
                }
                else
                {
                    // 无激活窗口时显示主页
                    ShowHomePage();
                }
            }
            else if (LeftWin == p_win)
            {
                LeftWin = null;
            }
            else if (RightWin == p_win)
            {
                RightWin = null;
            }

            // 关闭后
            p_win.OnClosed();
            GC.Collect();
            return true;
        }

        /// <summary>
        /// 关闭其他窗口
        /// </summary>
        /// <param name="p_win"></param>
        internal async Task<List<IWin>> CloseExcept(IWin p_win)
        {
            LeftWin = null;
            RightWin = null;
            MainWin = p_win;

            List<IWin> ls = new List<IWin>();
            foreach (var win in _winCache)
            {
                if (win == p_win)
                    continue;

                // 触发关闭前事件，外部判断是否允许关闭
                if (await win.OnClosing())
                    ls.Add(win);
            }

            if (ls.Count > 0)
            {
                foreach (var win in ls)
                {
                    _winCache.Remove(win);
                    // 关闭后
                    win.OnClosed();
                }
                GC.Collect();
            }
            return ls;
        }

        /// <summary>
        /// 显示主页
        /// </summary>
        internal void ShowHomePage()
        {
            if (MainWin != HomeWin)
                MainWin = HomeWin;
        }

        /// <summary>
        /// 重置停靠在两侧的窗口宽度
        /// </summary>
        /// <param name="p_win"></param>
        internal void ResetSideWidth(IWin p_win)
        {
            if (_grid != null)
            {
                if (RightWin == p_win)
                    _grid.ColumnDefinitions[4].Width = new GridLength(_defaultSideSize);
                else if (LeftWin == p_win)
                    _grid.ColumnDefinitions[0].Width = new GridLength(_defaultSideSize);
            }
        }
        #endregion

        #region 重写方法
        /// <summary>
        /// 应用模板
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _spLeft = (Splitter)GetTemplateChild("LeftSplitter");
            _spLeft.DraggingCompleted += OnLeftSplitterDraggingCompleted;
            _spRight = (Splitter)GetTemplateChild("RightSplitter");
            _spRight.DraggingCompleted += OnRightSplitterDraggingCompleted;
            _grid = (Grid)GetTemplateChild("ContentGrid");

            // 初始化左右窗口
            if (RightWin != null)
            {
                _spRight.Visibility = Visibility.Visible;
                _grid.ColumnDefinitions[4].Width = new GridLength(_rightSize.HasValue ? _rightSize.Value : _defaultSideSize);
            }
            if (LeftWin != null)
            {
                _spLeft.Visibility = Visibility.Visible;
                _grid.ColumnDefinitions[0].Width = new GridLength(_leftSize.HasValue ? _leftSize.Value : _defaultSideSize);
            }
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 激活指定窗口
        /// </summary>
        /// <param name="p_win"></param>
        void ActiveWinInternal(IWin p_win)
        {
            if (p_win == RightWin)
                RightWin = null;
            else if (p_win == LeftWin)
                LeftWin = null;
            MainWin = p_win;
        }

        /// <summary>
        /// 左侧分隔栏宽度调整结束
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnLeftSplitterDraggingCompleted(object sender, EventArgs e)
        {
            if (_grid == null || LeftWin == null)
                return;

            GridLength width = _grid.ColumnDefinitions[0].Width;
            if (width.IsAbsolute)
            {
                if (width.Value < _minSideSize)
                {
                    // 小于最小尺寸时移除内容
                    LeftWin = null;
                    Windows.UI.Xaml.Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 1);
                }
                else if (width.Value > _defaultSideSize)
                {
                    // 主窗口太小时左变主
                    double mainWidth = _grid.ActualWidth - width.Value - 24;
                    var rightWidth = _grid.ColumnDefinitions[4].Width;
                    if (rightWidth.IsAbsolute)
                        mainWidth = mainWidth - rightWidth.Value - 24;
                    if (mainWidth < _minSideSize)
                    {
                        var win = LeftWin;
                        LeftWin = null;
                        MainWin = win;
                        Taskbar.Inst.ActiveTaskItem(win);
                        Windows.UI.Xaml.Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 1);
                    }
                }
            }
        }

        /// <summary>
        /// 右侧分隔栏宽度调整结束
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnRightSplitterDraggingCompleted(object sender, EventArgs e)
        {
            if (_grid == null || RightWin == null)
                return;

            GridLength width = _grid.ColumnDefinitions[4].Width;
            if (width.IsAbsolute)
            {
                if (width.Value < _minSideSize)
                {
                    RightWin = null;
                    Windows.UI.Xaml.Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 1);
                }
                else if (width.Value > _defaultSideSize)
                {
                    double mainWidth = _grid.ActualWidth - width.Value - 24;
                    var leftWidth = _grid.ColumnDefinitions[0].Width;
                    if (leftWidth.IsAbsolute)
                        mainWidth = mainWidth - leftWidth.Value - 24;
                    if (mainWidth < _minSideSize)
                    {
                        var win = RightWin;
                        RightWin = null;
                        MainWin = win;
                        Taskbar.Inst.ActiveTaskItem(win);
                        Windows.UI.Xaml.Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 1);
                    }
                }
            }
        }
        #endregion
    }
}
