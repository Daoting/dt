﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-05-13 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 桌面容器
    /// </summary>
    public partial class Desktop : DtControl
    {
        #region 静态成员
        // 和edge标签宽度相同
        const double _maxItemWidth = 240;

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
            ((Desktop)d).ChangeMainWin((Win)e.OldValue, (Win)e.NewValue);
        }

        static void OnLeftWinChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Desktop)d).ChangeLeftWin((Win)e.OldValue, (Win)e.NewValue);
        }

        static void OnRightWinChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Desktop)d).ChangeRightWin((Win)e.OldValue, (Win)e.NewValue);
        }

        /// <summary>
        /// 获取桌面实例
        /// </summary>
        public static Desktop Inst => UITree.RootContent as Desktop;
        #endregion

        #region 成员变量
        Grid _gridContent;
        Grid _gridTaskbar;
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
        public bool ActiveWin(Win p_win)
        {
            if (Items.Contains(p_win))
            {
                MainWin = p_win;
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
        public Win ActiveWin(Type p_type, object p_params)
        {
            foreach (var win in Items)
            {
                if (win.GetType() == p_type)
                {
                    if ((win.Params == null && p_params == null)
                        || (p_params != null && win.Params != null && win.Params.Equals(p_params)))
                    {
                        MainWin = win;
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
            if (!await p_win.AllowClose())
                return false;

            Items.RemoveAt(index);

            // 若待关闭的窗口为激活状态
            if (MainWin == p_win)
            {
                if (Items.Count > 0)
                {
                    // 激活下一窗口
                    MainWin = index < Items.Count ? Items[index] : Items[Items.Count - 1];
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
            p_win.AfterClosed();
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

            var ls = (from win in Items
                      where win != p_win
                      select win).ToList();
            if (ls != null && ls.Count > 0)
            {
                foreach (var win in ls)
                {
                    // 触发关闭前事件，外部判断是否允许关闭
                    if (await win.AllowClose())
                    {
                        Items.Remove(win);
                        // 关闭后
                        win.AfterClosed();
                    }
                }
            }
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
        protected override void OnLoadTemplate()
        {
            _gridContent = (Grid)GetTemplateChild("ContentGrid");
            _gridTaskbar = (Grid)GetTemplateChild("TaskbarGrid");
            _taskbarPanel = (StackPanel)GetTemplateChild("TaskbarPanel");
            var home = (HomebarItem)GetTemplateChild("HomeItem");
            home.SetWin(HomeWin);
            LoadTaskbar();

            if (MainWin != null)
                ChangeMainWin(null, MainWin);
            if (LeftWin != null)
                ChangeLeftWin(null, LeftWin);
            if (RightWin != null)
                ChangeRightWin(null, RightWin);
        }

        #endregion

        #region 切换 MainWin
        void ChangeMainWin(Win p_oldWin, Win p_newWin)
        {
            if (_gridContent == null)
                return;

            if (p_oldWin != null)
            {
                p_oldWin.IsActived = false;
                _gridContent.Children.Remove(p_oldWin);
            }

            if (p_newWin != null)
            {
                // 若主窗口在两侧停靠，先移除停靠
                if (p_newWin == LeftWin)
                    LeftWin = null;
                else if (p_newWin == RightWin)
                    RightWin = null;

                p_newWin.IsActived = true;
                Grid.SetColumn(p_newWin, 2);
                _gridContent.Children.Add(p_newWin);
            }
        }
        #endregion

        #region 两侧停靠
        void ChangeLeftWin(Win p_oldWin, Win p_newWin)
        {
            if (_gridContent == null)
                return;

            Splitter splitter;
            if (p_oldWin != null)
            {
                _gridContent.Children.Remove(p_oldWin);
                splitter = (from item in _gridContent.Children.OfType<Splitter>()
                            where Grid.GetColumn(item) == 1
                            select item).FirstOrDefault();
                if (splitter != null)
                {
                    splitter.CloseLeft -= OnLeftCloseLeftWin;
                    splitter.CloseRight -= OnLeftCloseRightWin;
                    _gridContent.Children.Remove(splitter);

                }
                _gridContent.ColumnDefinitions[0].Width = new GridLength(0d);
                _gridContent.ColumnDefinitions[1].Width = new GridLength(0d);
            }

            if (p_newWin != null)
            {
                // 若窗口已加载，先移除
                if (p_newWin == RightWin)
                    RightWin = null;
                else if (p_newWin == MainWin)
                    MainWin = null;

                Grid.SetColumn(p_newWin, 0);
                _gridContent.Children.Add(p_newWin);

                splitter = new Splitter();
                splitter.CloseLeft += OnLeftCloseLeftWin;
                splitter.CloseRight += OnLeftCloseRightWin;
                Grid.SetColumn(splitter, 1);
                _gridContent.Children.Add(splitter);

                double width = p_newWin.GetSplitWidth();
                if (width > 0)
                {
                    // 可通过SplitWidth附加属性自定义宽度
                    _gridContent.ColumnDefinitions[0].Width = new GridLength(width);
                }
                else
                {
                    if (_gridContent.ColumnDefinitions[4].Width.Value > 0)
                    {
                        // 右侧已停靠
                        _gridContent.ColumnDefinitions[0].Width = new GridLength(Kit.ViewWidth / 3);
                        _gridContent.ColumnDefinitions[4].Width = new GridLength(Kit.ViewWidth / 3);
                    }
                    else
                    {
                        _gridContent.ColumnDefinitions[0].Width = new GridLength(Kit.ViewWidth / 2);
                    }
                }
                _gridContent.ColumnDefinitions[1].Width = GridLength.Auto;
            }
        }

        void OnLeftCloseLeftWin(object sender, EventArgs e)
        {
            LeftWin = null;
        }

        void OnLeftCloseRightWin(object sender, EventArgs e)
        {
            var win = LeftWin;
            LeftWin = null;
            MainWin = win;
        }

        void ChangeRightWin(Win p_oldWin, Win p_newWin)
        {
            if (_gridContent == null)
                return;

            Splitter splitter;
            if (p_oldWin != null)
            {
                _gridContent.Children.Remove(p_oldWin);
                splitter = (from item in _gridContent.Children.OfType<Splitter>()
                            where Grid.GetColumn(item) == 3
                            select item).FirstOrDefault();
                if (splitter != null)
                {
                    splitter.CloseLeft -= OnRightCloseLeftWin;
                    splitter.CloseRight -= OnRightCloseRightWin;
                    _gridContent.Children.Remove(splitter);

                }
                _gridContent.ColumnDefinitions[3].Width = new GridLength(0d);
                _gridContent.ColumnDefinitions[4].Width = new GridLength(0d);
            }

            if (p_newWin != null)
            {
                // 若窗口已加载，先移除
                if (p_newWin == LeftWin)
                    LeftWin = null;
                else if (p_newWin == MainWin)
                    MainWin = null;

                Grid.SetColumn(p_newWin, 4);
                _gridContent.Children.Add(p_newWin);

                splitter = new Splitter();
                splitter.CloseLeft += OnRightCloseLeftWin;
                splitter.CloseRight += OnRightCloseRightWin;
                Grid.SetColumn(splitter, 3);
                _gridContent.Children.Add(splitter);

                _gridContent.ColumnDefinitions[3].Width = GridLength.Auto;
                double width = p_newWin.GetSplitWidth();
                if (width > 0)
                {
                    // 可通过SplitWidth附加属性自定义宽度
                    _gridContent.ColumnDefinitions[4].Width = new GridLength(width);
                }
                else
                {
                    if (_gridContent.ColumnDefinitions[0].Width.Value > 0)
                    {
                        // 左侧已停靠
                        _gridContent.ColumnDefinitions[0].Width = new GridLength(Kit.ViewWidth / 3);
                        _gridContent.ColumnDefinitions[4].Width = new GridLength(Kit.ViewWidth / 3);
                    }
                    else
                    {
                        _gridContent.ColumnDefinitions[4].Width = new GridLength(Kit.ViewWidth / 2);
                    }
                }
            }
        }

        void OnRightCloseLeftWin(object sender, EventArgs e)
        {
            var win = RightWin;
            RightWin = null;
            MainWin = win;
        }

        void OnRightCloseRightWin(object sender, EventArgs e)
        {
            RightWin = null;
        }
        #endregion

        #region 任务栏

        void LoadTaskbar()
        {
            FrameworkElement start = null;
            FrameworkElement tray = null;
            var svc = Kit.GetService<ITaskbar>();
            if (svc != null)
            {
                start = svc.GetStartUI();
                tray = svc.GetTrayUI();
            }
            if (start != null)
            {
                start.Margin = new Thickness(20, 0, 0, 0);
                _gridTaskbar.Children.Add(start);
            }
            if (tray != null)
            {
                Grid.SetColumn(tray, 3);
                _gridTaskbar.Children.Add(tray);
            }

            LoadAllItems();
            Items.ItemsChanged += OnItemsChanged;
            _gridTaskbar.SizeChanged += (s, e) => UpdateTaskbar();

#if WIN
            var bar = Kit.MainWin.AppWindow.TitleBar;
            var margin = new Thickness(0, 0, Math.Ceiling(((double)bar.RightInset / XamlRoot.RasterizationScale) + 10), 0);
            if (tray != null)
                tray.Margin = margin;
            else
                _taskbarPanel.Margin = margin;

            // 最小留出标题栏拖拽宽度
            _taskbarPanel.Padding = new Thickness(0, 0, 40, 0);
#else
            var margin = new Thickness(0, 0, 20, 0);
            if (tray != null)
                tray.Margin = margin;
            else
                _taskbarPanel.Margin = margin;
#endif
        }

        /// <summary>
        /// 重置任务栏按扭的宽度
        /// </summary>
        void UpdateTaskbar()
        {
            // _taskbarPanel.ActualWidth不准！
            double pnlWidth = _gridTaskbar.ColumnDefinitions[2].ActualWidth - _taskbarPanel.Margin.Right;
            if (Items.Count > 0)
            {
                double width = Math.Floor((pnlWidth - _taskbarPanel.Padding.Right) / Items.Count);
                if (width < _maxItemWidth && width != ((TaskbarItem)_taskbarPanel.Children[0]).Width)
                {
                    foreach (var item in _taskbarPanel.Children.OfType<TaskbarItem>())
                    {
                        item.Width = width;
                    }
                }
                else if (width >= _maxItemWidth && _maxItemWidth != ((TaskbarItem)_taskbarPanel.Children[0]).Width)
                {
                    foreach (var item in _taskbarPanel.Children.OfType<TaskbarItem>())
                    {
                        item.Width = _maxItemWidth;
                    }
                }
            }

#if WIN
            // 左边空出开始、主页按钮
            double left = _gridTaskbar.ColumnDefinitions[0].ActualWidth + _gridTaskbar.ColumnDefinitions[1].ActualWidth;
            double dragWidth = pnlWidth;
            if (Items.Count > 0)
            {
                double barWidth = ((TaskbarItem)_taskbarPanel.Children[0]).Width * Items.Count;
                left += barWidth;
                dragWidth -= barWidth;
            }

            Windows.Graphics.RectInt32 rect = new(
                (int)Math.Ceiling(left * XamlRoot.RasterizationScale),
                0,
                (int)Math.Ceiling(dragWidth * XamlRoot.RasterizationScale),
                (int)Math.Ceiling(_taskbarPanel.ActualHeight * XamlRoot.RasterizationScale));
            Kit.MainWin.AppWindow.TitleBar.SetDragRectangles(new Windows.Graphics.RectInt32[] { rect });
#endif
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
            UpdateTaskbar();
        }
        #endregion
    }
}
