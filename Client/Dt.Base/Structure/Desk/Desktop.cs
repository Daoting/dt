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
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 桌面容器
    /// </summary>
    public partial class Desktop : Control
    {
        #region 静态成员
        const double _minSideSize = 200;
        const double _maxItemWidth = 260;
        const double _deltaSplitterWidth = 200;

        /// <summary>
        /// 内部主窗口
        /// </summary>
        public readonly static DependencyProperty MainWinProperty = DependencyProperty.Register(
            "MainWin",
            typeof(Win),
            typeof(Desktop),
            new PropertyMetadata(null, OnMainWinChanged));

        /// <summary>
        /// 停靠在左侧的窗口
        /// </summary>
        public readonly static DependencyProperty LeftWinProperty = DependencyProperty.Register(
            "LeftWin",
            typeof(Win),
            typeof(Desktop),
            new PropertyMetadata(null, OnLeftWinChanged));

        /// <summary>
        /// 停靠在右侧的窗口
        /// </summary>
        public readonly static DependencyProperty RightWinProperty = DependencyProperty.Register(
            "RightWin",
            typeof(Win),
            typeof(Desktop),
            new PropertyMetadata(null, OnRightWinChanged));

        static void OnMainWinChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Win win = (Win)e.OldValue;
            if (win != null)
                win.IsActived = false;

            win = (Win)e.NewValue;
            if (win != null)
                win.IsActived = true;
        }

        static void OnLeftWinChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Desktop dt = (Desktop)d;
            if (dt._grid == null)
                return;

            var spLeft = (Splitter)dt.GetTemplateChild("LeftSplitter");
            Win win = (Win)e.NewValue;
            if (win != null)
            {
                if (win == dt.RightWin)
                    dt.RightWin = null;
                spLeft.Visibility = Visibility.Visible;
                double width = win.GetSplitWidth();
                if (width > _minSideSize)
                    dt._grid.ColumnDefinitions[0].Width = new GridLength(width);
                else
                    dt._grid.ColumnDefinitions[0].Width = new GridLength(SysVisual.ViewWidth / 2 - _deltaSplitterWidth);
            }
            else
            {
                spLeft.Visibility = Visibility.Collapsed;
                dt._grid.ColumnDefinitions[0].Width = GridLength.Auto;
            }
            dt._grid.ColumnDefinitions[1].Width = GridLength.Auto;
        }

        static void OnRightWinChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Desktop dt = (Desktop)d;
            if (dt._grid == null)
                return;

            var spRight = (Splitter)dt.GetTemplateChild("RightSplitter");
            var win = (Win)e.NewValue;
            if (win != null)
            {
                if (win == dt.LeftWin)
                    dt.LeftWin = null;
                spRight.Visibility = Visibility.Visible;
                double width = win.GetSplitWidth();
                if (width > _minSideSize)
                    dt._grid.ColumnDefinitions[4].Width = new GridLength(width);
                else
                    dt._grid.ColumnDefinitions[4].Width = new GridLength(SysVisual.ViewWidth / 2 - _deltaSplitterWidth);
            }
            else
            {
                spRight.Visibility = Visibility.Collapsed;
                dt._grid.ColumnDefinitions[4].Width = GridLength.Auto;
            }
            dt._grid.ColumnDefinitions[3].Width = GridLength.Auto;
        }

        /// <summary>
        /// 获取桌面实例
        /// </summary>
        public static Desktop Inst { get; internal set; }
        #endregion

        #region 成员变量
        Grid _grid;
        StackPanel _taskbarPanel;
        Win _homeWin;
        #endregion

        #region 构造方法
        /// <summary>
        /// 构造方法
        /// </summary>
        public Desktop()
        {
            DefaultStyleKey = typeof(Desktop);
            Inst = this;
#if !UWP
            Loaded += OnLoaded;
#endif
        }
        #endregion

        #region 属性
        /// <summary>
        /// 获取设置内部主窗口
        /// </summary>
        public Win MainWin
        {
            get { return (Win)GetValue(MainWinProperty); }
            internal set { SetValue(MainWinProperty, value); }
        }

        /// <summary>
        /// 获取主页窗口
        /// </summary>
        public Win HomeWin
        {
            get
            {
                if (_homeWin == null)
                {
                    _homeWin = new Win();
                    _homeWin.Items.Add(new TextBlock { Text = "无主页", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center });
                }
                return _homeWin;
            }
            internal set
            {
                if (_homeWin != value && value != null)
                    _homeWin = value;
            }
        }

        /// <summary>
        /// 获取设置停靠在左侧的窗口
        /// </summary>
        public Win LeftWin
        {
            get { return (Win)GetValue(LeftWinProperty); }
            set { SetValue(LeftWinProperty, value); }
        }

        /// <summary>
        /// 获取设置停靠在右侧的窗口
        /// </summary>
        public Win RightWin
        {
            get { return (Win)GetValue(RightWinProperty); }
            set { SetValue(RightWinProperty, value); }
        }

        /// <summary>
        /// 获取窗口集合
        /// </summary>
        public ItemList<Win> Items { get; } = new ItemList<Win>();
        #endregion

        #region 外部方法
        /// <summary>
        /// 显示新窗口并缓存
        /// </summary>
        /// <param name="p_win">窗口</param>
        internal void ShowNewWin(Win p_win)
        {
            // 不再判断是否包含
            MainWin = p_win;
            Items.Add(p_win);
        }

        /// <summary>
        /// 激活窗口
        /// </summary>
        /// <param name="p_win">窗口</param>
        /// <returns>是否激活成功</returns>
        internal bool ActiveWin(Win p_win)
        {
            if (Items.Contains(p_win))
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
        internal Win ActiveWin(Type p_type, object p_params)
        {
            foreach (var win in Items)
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
        /// 关闭窗口并从缓存中移除，激活下一窗口
        /// </summary>
        /// <param name="p_win">窗口</param>
        internal async Task<bool> CloseWin(Win p_win)
        {
            int index = Items.IndexOf(p_win);
            if (index < 0)
                return false;

            // 触发关闭前事件，外部判断是否允许关闭
            if (!await p_win.OnClosing())
                return false;

            Items.RemoveAt(index);

            // 若待关闭的窗口为激活状态
            if (MainWin == p_win)
            {
                if (Items.Count > 0)
                {
                    // 激活下一窗口
                    Win nextWin = index < Items.Count ? Items[index] : Items[Items.Count - 1];

                    // 若待激活窗口停靠状态，先移除停靠
                    if (LeftWin == nextWin)
                        LeftWin = null;
                    else if (RightWin == nextWin)
                        RightWin = null;
                    MainWin = nextWin;
                }
                else
                {
                    // 无激活窗口时显示主页
                    MainWin = HomeWin;
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
        internal async void CloseExcept(Win p_win)
        {
            LeftWin = null;
            RightWin = null;
            MainWin = p_win;

            foreach (var win in Items)
            {
                if (win == p_win)
                    continue;

                // 触发关闭前事件，外部判断是否允许关闭
                if (await win.OnClosing())
                {
                    Items.Remove(win);
                    // 关闭后
                    win.OnClosed();
                }
            }
            GC.Collect();
        }

        /// <summary>
        /// 拖拽任务栏项
        /// </summary>
        /// <param name="p_src"></param>
        /// <param name="e"></param>
        internal void DoSwap(TaskbarItem p_src, PointerRoutedEventArgs e)
        {
            Point pt = e.GetCurrentPoint(null).Position;
            TaskbarItem tgt = (from item in _taskbarPanel.Children.OfType<TaskbarItem>()
                               where item.ContainPoint(pt)
                               select item).FirstOrDefault();
            if (tgt != null && tgt != p_src)
            {
                // 交换位置的最小移动控制
                pt = e.GetCurrentPoint(tgt).Position;
                double delta = tgt.ActualWidth / 2;
                if ((pt.X < delta && pt.X > 20)
                    || (pt.X > delta && pt.X < tgt.ActualWidth - 20))
                {
                    try
                    {
                        // 动画效果好
                        Items.ItemsChanged -= OnItemsChanged;
                        int srcIndex = _taskbarPanel.Children.IndexOf(p_src);
                        int tgtIndex = _taskbarPanel.Children.IndexOf(tgt);
                        Items.RemoveAt(tgtIndex);
                        _taskbarPanel.Children.RemoveAt(tgtIndex);

                        Items.Insert(srcIndex, (Win)tgt.DataContext);
                        _taskbarPanel.Children.Insert(srcIndex, tgt);
                    }
                    finally
                    {
                        Items.ItemsChanged += OnItemsChanged;
                    }
                }
            }
        }
        #endregion

        #region 加载过程
        /************************************************************************************************************************************/
        // uno在构造方法中设置Style时直接调用了OnApplyTemplate，只能在Loaded事件中加载Items
        // UWP仍在OnApplyTemplate中加载Items
        /************************************************************************************************************************************/

#if UWP
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            InitTemplate();
        }
#else
        void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;
            InitTemplate();
        }
#endif

        void InitTemplate()
        {
            _grid = (Grid)GetTemplateChild("ContentGrid");
            _taskbarPanel = (StackPanel)GetTemplateChild("TaskbarPanel");
            var home = (HomebarItem)GetTemplateChild("HomeItem");
            home.SetWin(HomeWin);

            var spLeft = (Splitter)GetTemplateChild("LeftSplitter");
            spLeft.DraggingCompleted += OnLeftSplitterDraggingCompleted;
            var spRight = (Splitter)GetTemplateChild("RightSplitter");
            spRight.DraggingCompleted += OnRightSplitterDraggingCompleted;

            // 初始化左右窗口
            if (RightWin != null)
            {
                spRight.Visibility = Visibility.Visible;
                _grid.ColumnDefinitions[4].Width = new GridLength(SysVisual.ViewWidth / 2);
            }
            if (LeftWin != null)
            {
                spLeft.Visibility = Visibility.Visible;
                _grid.ColumnDefinitions[0].Width = new GridLength(SysVisual.ViewWidth / 2);
            }

            LoadAllItems();
            Items.ItemsChanged += OnItemsChanged;
            _taskbarPanel.SizeChanged += (s, e) => ResizeAllItems();
        }

        void LoadAllItems()
        {
            foreach (var win in Items)
            {
                _taskbarPanel.Children.Add(new TaskbarItem(win));
            }
        }

        void OnItemsChanged(object sender, ItemListChangedArgs e)
        {
            if (e.CollectionChange == CollectionChange.ItemInserted)
            {
                _taskbarPanel.Children.Insert(e.Index, new TaskbarItem(Items[e.Index]));
            }
            else if (e.CollectionChange == CollectionChange.ItemRemoved)
            {
                _taskbarPanel.Children.RemoveAt(e.Index);
            }
            else
            {
                throw new Exception("Win不支持子项集合重置！");
            }
            ResizeAllItems();
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 激活指定窗口
        /// </summary>
        /// <param name="p_win"></param>
        void ActiveWinInternal(Win p_win)
        {
            if (p_win == RightWin)
                RightWin = null;
            else if (p_win == LeftWin)
                LeftWin = null;
            MainWin = p_win;
        }

        /// <summary>
        /// 重置任务栏按扭的宽度
        /// </summary>
        void ResizeAllItems()
        {
            if (Items.Count == 0)
                return;

            double width = Math.Floor((ActualWidth - 180) / Items.Count);
            if (width < _maxItemWidth && width != ((TaskbarItem)_taskbarPanel.Children[0]).Width)
            {
                foreach (var item in _taskbarPanel.Children.OfType<TaskbarItem>())
                {
                    item.Width = width;
                }
            }
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
                else if (_grid.ActualWidth - width.Value - 24 < _minSideSize)
                {
                    // 主窗口太小时左变主
                    var win = LeftWin;
                    LeftWin = null;
                    MainWin = win;
                    Windows.UI.Xaml.Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 1);
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
                else if (_grid.ActualWidth - width.Value - 24 < _minSideSize)
                {
                    var win = RightWin;
                    RightWin = null;
                    MainWin = win;
                    Windows.UI.Xaml.Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 1);
                }
            }
        }
        #endregion
    }
}
