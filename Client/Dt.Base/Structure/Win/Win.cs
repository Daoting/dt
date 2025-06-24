#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-03-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.Docking;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 可停靠多区域窗口
    /// </summary>
    [ContentProperty(Name = nameof(Items))]
    public partial class Win : DtControl, IPaneList, IDestroy
    {
        #region 静态内容
        public readonly static DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title",
            typeof(string),
            typeof(Win),
            null);

        public readonly static DependencyProperty IconProperty = DependencyProperty.Register(
            "Icon",
            typeof(Icons),
            typeof(Win),
            new PropertyMetadata(Icons.文件));

        public readonly static DependencyProperty ParamsProperty = DependencyProperty.Register(
            "Params",
            typeof(object),
            typeof(Win),
            new PropertyMetadata(null));

        public static readonly DependencyProperty AutoSaveLayoutProperty = DependencyProperty.Register(
            "AutoSaveLayout",
            typeof(bool),
            typeof(Win),
            new PropertyMetadata(false));

        public static readonly DependencyProperty AutoUnpinSideProperty = DependencyProperty.Register(
            "AutoUnpinSide",
            typeof(bool),
            typeof(Win),
            new PropertyMetadata(false));

        public static readonly DependencyProperty MinWidthOfMainProperty = DependencyProperty.Register(
            "MinWidthOfMain",
            typeof(double),
            typeof(Win),
            new PropertyMetadata(480d));

        public static readonly DependencyProperty AllowResetLayoutProperty = DependencyProperty.Register(
            "AllowResetLayout",
            typeof(bool),
            typeof(Win),
            new PropertyMetadata(false));

        internal static readonly DependencyProperty MainTabsProperty = DependencyProperty.Register(
            "MainTabs",
            typeof(Tabs),
            typeof(Win),
            new PropertyMetadata(null));

        static readonly DependencyProperty PhoneMainTabProperty = DependencyProperty.Register(
            "PhoneMainTab",
            typeof(Tab),
            typeof(Win),
            new PropertyMetadata(null));

        static readonly DependencyProperty IsInnerWinInPhoneUIProperty = DependencyProperty.Register(
            "IsInnerWinInPhoneUI",
            typeof(bool),
            typeof(Win),
            new PropertyMetadata(false));

        public readonly static DependencyProperty IsActivedProperty = DependencyProperty.Register(
            "IsActived",
            typeof(bool),
            typeof(Win),
            new PropertyMetadata(false, OnIsActivedChanged));

        static readonly DependencyProperty OwnTabProperty = DependencyProperty.Register(
            "OwnTab",
            typeof(Tab),
            typeof(Win),
            new PropertyMetadata(null));

        public static readonly DependencyProperty OwnDlgProperty = DependencyProperty.Register(
            "OwnDlg",
            typeof(Dlg),
            typeof(Win),
            new PropertyMetadata(null));

        static void OnIsActivedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Win win = (Win)d;
            win.IsActivedChanged?.Invoke(win, EventArgs.Empty);
        }
        #endregion

        #region 成员变量
        const string _mainTabFlag = "MainTab";
        static Size _defaultFloatSize = new Size(300.0, 300.0);
        Canvas _popupPanel;
        Compass _compass;
        RootCompass _rootCompass;
        Border _dragCue;
        LayoutManager _layout;
        Tabs _sectWithCompass;
        bool _isDragDelta;
        #endregion

        #region 构造方法
        public Win()
        {
            // PhoneUI模式时不在可视树，省去uno在xaml自动生成代码时调用ApplyTemplate
            if (!Kit.IsPhoneUI)
                DefaultStyleKey = typeof(Win);
        }
        #endregion

        #region 事件
        /// <summary>
        /// 关闭前事件，可以取消关闭
        /// </summary>
        public event EventHandler<AsyncCancelArgs> Closing;

        /// <summary>
        /// 关闭后事件
        /// </summary>
        public event EventHandler Closed;

        /// <summary>
        /// 布局变化结束事件
        /// </summary>
        public event EventHandler LayoutChanged;

        /// <summary>
        /// 窗口激活状态变化事件
        /// </summary>
        public event EventHandler IsActivedChanged;

        /// <summary>
        /// 窗口被销毁后事件
        /// </summary>
        public event Action<Win> Destroyed;
        #endregion

        #region 属性
        /// <summary>
        /// 获取设置标题
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// 获取设置图标名称
        /// </summary>
        public Icons Icon
        {
            get { return (Icons)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        /// <summary>
        /// 获取设置初始参数
        /// </summary>
        public object Params
        {
            get { return GetValue(ParamsProperty); }
            set { SetValue(ParamsProperty, value); }
        }

        /// <summary>
        /// 获取设置是否自动保存布局状态，默认false
        /// </summary>
        public bool AutoSaveLayout
        {
            get { return (bool)GetValue(AutoSaveLayoutProperty); }
            set { SetValue(AutoSaveLayoutProperty, value); }
        }

        /// <summary>
        /// 获取设置是否自动取消窗口两侧面板的固定状态，默认false
        /// </summary>
        public bool AutoUnpinSide
        {
            get { return (bool)GetValue(AutoUnpinSideProperty); }
            set { SetValue(AutoUnpinSideProperty, value); }
        }

        /// <summary>
        /// 获取设置主区的最小宽度，AutoUnpinSide == true时有效，默认480
        /// </summary>
        public double MinWidthOfMain
        {
            get { return (double)GetValue(MinWidthOfMainProperty); }
            set { SetValue(MinWidthOfMainProperty, value); }
        }

        /// <summary>
        /// 获取内容元素集合
        /// </summary>
        public PaneList Items { get; } = new PaneList();

        /// <summary>
        /// 获取是否允许恢复默认布局
        /// </summary>
        internal bool AllowResetLayout
        {
            get { return (bool)GetValue(AllowResetLayoutProperty); }
            set { SetValue(AllowResetLayoutProperty, value); }
        }

        /// <summary>
        /// 获取设置是否为桌面的激活窗口
        /// </summary>
        public bool IsActived
        {
            get { return (bool)GetValue(IsActivedProperty); }
            internal set { SetValue(IsActivedProperty, value); }
        }

        /// <summary>
        /// 获取所属的Dlg
        /// </summary>
        public Dlg OwnDlg
        {
            get { return (Dlg)GetValue(OwnDlgProperty); }
            internal set { SetValue(OwnDlgProperty, value); }
        }

        /// <summary>
        /// 获取所有停靠在左侧的Pane
        /// </summary>
        public IEnumerable<Pane> LeftPanes
        {
            get
            {
                return from elem in Items
                       where elem is Pane pn && pn.Pos == PanePosition.Left
                       select (Pane)elem;
            }
        }

        /// <summary>
        /// 获取停靠在左侧的Pane
        /// </summary>
        public Pane LeftPane
        {
            get
            {
                return (from elem in Items
                        where elem is Pane pn && pn.Pos == PanePosition.Left
                        select (Pane)elem).FirstOrDefault();
            }
        }

        /// <summary>
        /// 获取所有停靠在右侧的Pane
        /// </summary>
        public IEnumerable<Pane> RightPanes
        {
            get
            {
                return from elem in Items
                       where elem is Pane pn && pn.Pos == PanePosition.Right
                       select (Pane)elem;
            }
        }

        /// <summary>
        /// 获取停靠在右侧的Pane
        /// </summary>
        public Pane RightPane
        {
            get
            {
                return (from elem in Items
                        where elem is Pane pn && pn.Pos == PanePosition.Right
                        select (Pane)elem).FirstOrDefault();
            }
        }

        /// <summary>
        /// 获取所有停靠在底部的Pane
        /// </summary>
        public IEnumerable<Pane> BottomPanes
        {
            get
            {
                return from elem in Items
                       where elem is Pane pn && pn.Pos == PanePosition.Bottom
                       select (Pane)elem;
            }
        }

        /// <summary>
        /// 获取停靠在底部的Pane
        /// </summary>
        public Pane BottomPane
        {
            get
            {
                return (from elem in Items
                        where elem is Pane pn && pn.Pos == PanePosition.Bottom
                        select (Pane)elem).FirstOrDefault();
            }
        }

        /// <summary>
        /// 获取所有浮动项
        /// </summary>
        public IEnumerable<Pane> FloatItems
        {
            get
            {
                if (_popupPanel != null && _popupPanel.Children.Count > 0)
                {
                    foreach (ToolWindow win in _popupPanel.Children.OfType<ToolWindow>())
                    {
                        Pane item = win.Content as Pane;
                        if (item != null)
                            yield return item;
                    }
                }
            }
        }

        /// <summary>
        /// 是否为内嵌窗口
        /// </summary>
        internal bool IsInnerWin
        {
            get { return GetValue(OwnTabProperty) != null; }
        }

        /// <summary>
        /// 获取内部停靠面板
        /// </summary>
        internal PanePanel RootPanel { get; set; }

        /// <summary>
        /// 获取左侧隐藏面板
        /// </summary>
        internal AutoHideTab LeftAutoHide { get; set; }

        /// <summary>
        /// 获取右侧隐藏面板
        /// </summary>
        internal AutoHideTab RightAutoHide { get; set; }

        /// <summary>
        /// 获取上侧隐藏面板
        /// </summary>
        internal AutoHideTab TopAutoHide { get; set; }

        /// <summary>
        /// 获取下侧隐藏面板
        /// </summary>
        internal AutoHideTab BottomAutoHide { get; set; }

        /// <summary>
        /// 获取是否正在重置布局
        /// </summary>
        internal bool IsReseting { get; set; }

        /// <summary>
        /// 获取中部停靠容器
        /// </summary>
        internal Pane CenterItem { get; } = new Pane { IsInCenter = true };

        /// <summary>
        /// 所有Tab
        /// </summary>
        internal Dictionary<string, Tab> AllTabs => _tabs ?? _layout?.AllTabs;
        #endregion

        #region 外部方法
        /// <summary>
        /// 激活旧窗口或打开新窗口
        /// </summary>
        public void Open()
        {
            if (Kit.IsPhoneUI)
                NaviToHome();
            else if (!Desktop.Inst.ActiveWin(this))
                Desktop.Inst.ShowNewWin(this);
        }

        /// <summary>
        /// 关闭窗口，三种情况：
        /// <para>PhoneUI关闭窗口所有导航页</para>
        /// <para>关闭独立的桌面窗口</para>
        /// <para>作为内嵌窗口时自动移除</para>
        /// </summary>
        public void Close()
        {
            if (Kit.IsPhoneUI)
            {
                // 关闭窗口所有导航页
                CloseAllNaviPages();
            }
            else if (GetValue(OwnTabProperty) is Tab tab)
            {
                // 移除内嵌窗口
                tab.Content = null;
            }
            else if (OwnDlg is Dlg dlg)
            {
                // 在对话框内部
                dlg.Close();
                AfterClosed();
            }
            else
            {
                // 关闭独立的桌面窗口
                _ = Desktop.Inst.CloseWin(this);
            }
        }

        /// <summary>
        /// 恢复默认布局
        /// </summary>
        public void LoadDefaultLayout()
        {
            _layout?.LoadDefaultLayout();
        }
        #endregion

        #region 加载过程
        protected override void OnLoadTemplate()
        {
            RootPanel = (PanePanel)GetTemplateChild("RootPanel");
            RootPanel.Init(CenterItem);

            _layout = new LayoutManager(this);
            Items.ItemsChanged += OnItemsChanged;
            SizeChanged += OnSizeChanged;
        }

        void OnItemsChanged(object sender, ItemListChangedArgs e)
        {
            if (IsReseting || RootPanel == null)
                return;

            // RootPanel.Children的0位置是CenterItem，比Items多一个元素！
            if (e.CollectionChange == CollectionChange.ItemInserted)
            {
                Pane item = Items[e.Index] as Pane;
                if (item != null
                    && item.Pos != PanePosition.Floating
                    && !RootPanel.Children.Contains(item))
                {
                    RootPanel.Children.Insert(e.Index + 1, item);
                }
            }
            else if (e.CollectionChange == CollectionChange.ItemRemoved)
            {
                if (RootPanel.Children.Count > e.Index + 1)
                    RootPanel.Children.RemoveAt(e.Index + 1);
            }
            else
            {
                throw new Exception("Win不支持子项集合重置！");
            }
        }
        #endregion

        #region 开始拖动
        /// <summary>
        /// 开始拖动
        /// </summary>
        /// <param name="p_target"></param>
        /// <param name="p_pointer"></param>
        internal void OnDragStarted(object p_target, PointerRoutedEventArgs p_pointer)
        {
            Tab tab;
            TabHeader header;
            Tabs tabs;

            if ((tab = p_target as Tab) != null)
            {
                tabs = tab.OwnTabs;
            }
            else if ((header = p_target as TabHeader) != null)
            {
                tabs = header.Owner as Tabs;
            }
            else
            {
                throw new Exception("拖动时异常，拖动对象只可能是Tab或TabHeader！");
            }

            // 水平位置所占比例
            Point offset = p_pointer.GetCurrentPoint(tabs).Position;
            double offsetX = offset.X / tabs.RenderSize.Width;

            ToolWindow win;
            if (tab != null)
            {
                win = OpenInWindow(tab);
            }
            else
            {
                win = OpenInWindow(tabs);
            }

            ResetCompassSize();
            Point winPos = p_pointer.GetCurrentPoint(this).Position;
            offsetX = offsetX > 0 ? Math.Round(offsetX * win.Width) : 10;
            win.HorizontalOffset = winPos.X - offsetX;
            win.VerticalOffset = winPos.Y - 20;
            win.StartDrag(p_pointer);
        }

        /// <summary>
        /// 构造ToolWindow承载Tab，结构 ToolWindow -> Pane -> Tabs -> Tab
        /// </summary>
        /// <param name="p_tab"></param>
        /// <returns></returns>
        ToolWindow OpenInWindow(Tab p_tab)
        {
            Point initPos = new Point();
            Size initSize = _defaultFloatSize;
            ToolWindow oldWin = GetParentWindow(p_tab);
            if (oldWin != null)
            {
                initPos = new Point(oldWin.HorizontalOffset, oldWin.VerticalOffset);
                initSize = new Size(oldWin.Width, oldWin.Height);
            }
            else
            {
                Pane oldContainer = null;
                if (p_tab.OwnTabs != null)
                    oldContainer = p_tab.OwnTabs.Parent as Pane;

                if (oldContainer != null)
                {
                    initPos = oldContainer.FloatLocation;
                    initSize = oldContainer.FloatSize;
                }
            }
            p_tab.RemoveFromParent();

            ToolWindow win = CreateWindow(initSize, initPos);
            Pane dockItem = new Pane();
            dockItem.Pos = PanePosition.Floating;
            Tabs sect = new Tabs();
            sect.Items.Add(p_tab);
            dockItem.Items.Add(sect);
            win.Content = dockItem;
            win.Show();
            return win;
        }

        /// <summary>
        /// 构造ToolWindow承载Tabs，直接将Tabs移动到新Pane
        /// </summary>
        /// <param name="p_tabs"></param>
        /// <returns></returns>
        ToolWindow OpenInWindow(Tabs p_tabs)
        {
            Point initPos = new Point();
            Size initSize = _defaultFloatSize;
            ToolWindow oldWin = GetParentWindow(p_tabs);
            if (oldWin != null)
            {
                initPos = new Point(oldWin.HorizontalOffset, oldWin.VerticalOffset);
                initSize = new Size(oldWin.Width, oldWin.Height);
            }

            ToolWindow win = CreateWindow(initSize, initPos);
            Pane dockItem = new Pane();
            dockItem.Pos = PanePosition.Floating;
            p_tabs.RemoveFromParent();
            dockItem.Items.Add(p_tabs);
            win.Content = dockItem;
            win.Show();
            return win;
        }

        internal ToolWindow CreateWindow(Size p_size, Point p_location)
        {
            if (_popupPanel == null)
                CreatePopupCanvas();

            ToolWindow win = new ToolWindow(this, _popupPanel);
            win.Width = p_size.Width;
            win.Height = p_size.Height;
            win.HorizontalOffset = p_location.X;
            win.VerticalOffset = p_location.Y;
            return win;
        }

        void CreatePopupCanvas()
        {
            _popupPanel = new Canvas();

            // 停靠区域提示
            _dragCue = new Border { Background = Res.暗遮罩, BorderBrush = Res.深暗遮罩, BorderThickness = new Thickness(2), Visibility = Visibility.Collapsed };
            Canvas.SetZIndex(_dragCue, 999997);
            _popupPanel.Children.Add(_dragCue);

            // 动态导航
            _compass = new Compass { Visibility = Visibility.Collapsed };
            Canvas.SetZIndex(_compass, 999998);
            _popupPanel.Children.Add(_compass);

            // 四方向导航
            _rootCompass = new RootCompass { Visibility = Visibility.Collapsed };
            Canvas.SetZIndex(_rootCompass, 999999);
            _popupPanel.Children.Add(_rootCompass);

            Grid grid = (Grid)GetTemplateChild("RootGrid");
            Grid.SetRowSpan(_popupPanel, 3);
            Grid.SetColumnSpan(_popupPanel, 3);
            grid.Children.Add(_popupPanel);
        }

        void ResetCompassSize()
        {
            // 需指定大小，不然uno中ToolWindow无法交互！
            _popupPanel.Width = ActualWidth;
            _popupPanel.Height = ActualHeight;

            _rootCompass.Width = ActualWidth;
            _rootCompass.Height = ActualHeight;
        }
        #endregion

        #region 拖动过程
        /// <summary>
        /// 拖拽中
        /// </summary>
        /// <param name="p_toolWin"></param>
        /// <param name="p_pointer"></param>
        internal void OnDragDelta(ToolWindow p_toolWin, PointerRoutedEventArgs p_pointer)
        {
            if (p_toolWin == null)
                return;

            if (CheckIsDockable(p_toolWin))
            {
                Point pos = p_pointer.GetCurrentPoint(null).Position;
                UpdateCompass(pos, p_toolWin);
                UpdateRootCompass(pos);
                AdjustCueSize(p_toolWin);
            }
            else
            {
                _rootCompass.Visibility = Visibility.Collapsed;
            }
            _isDragDelta = true;
        }

        /// <summary>
        /// 更新Tabs内部停靠导航
        /// </summary>
        /// <param name="p_pos"></param>
        /// <param name="p_win"></param>
        void UpdateCompass(Point p_pos, ToolWindow p_win)
        {
            // 获取当前位置处其他窗口中的Tabs
            Tabs sect = GetHitSect(p_pos, _popupPanel, p_win);
            if (sect == null || !CheckIsDockable(sect))
            {
                // 当前位置处的Tabs
                sect = GetHitSect(p_pos, this, p_win);
                if (sect != null && sect.IsInCenter && !p_win.CanDockInCenter)
                    sect = null;
            }

            // 有变化
            if (sect != _sectWithCompass && _compass != null)
            {
                _compass.ClearIndicators();
            }

            if (sect == null)
            {
                // 无选中区域
                _sectWithCompass = null;
                _compass.Visibility = Visibility.Collapsed;
            }
            else
            {
                // 有停靠区域
                _sectWithCompass = sect;
                _compass.Visibility = Visibility.Visible;
                double horOffset = (sect.ActualWidth - _compass.Width) / 2.0;
                double verOffset = (sect.ActualHeight - _compass.Height) / 2.0;
                Point pos = GetElementPositionRelatedToPopup(sect);
                double left = Math.Round((double)(horOffset + pos.X));
                double top = Math.Round((double)(verOffset + pos.Y));
                Canvas.SetLeft(_compass, left);
                Canvas.SetTop(_compass, top);
                _compass.ChangeDockPosition(p_pos);
            }
        }

        /// <summary>
        /// 更新最外层停靠导航
        /// </summary>
        /// <param name="p_pos"></param>
        void UpdateRootCompass(Point p_pos)
        {
            _rootCompass.Visibility = Visibility.Visible;
            _rootCompass.ChangeDockPosition(p_pos);
        }

        /// <summary>
        /// 显示可停靠区域背景
        /// </summary>
        /// <param name="p_win"></param>
        void AdjustCueSize(ToolWindow p_win)
        {
            Rect rect = new Rect();
            bool showCue = false;
            if (_compass.DockPosition != DockPosition.None && _sectWithCompass != null)
            {
                // 在Pane内部停靠
                rect = _sectWithCompass.GetRectDimenstion(_compass.DockPosition, p_win.Content as Pane);
                Point topLeft = GetElementPositionRelatedToPopup(_sectWithCompass.OwnWinItem);
                rect.X += topLeft.X;
                rect.Y += topLeft.Y;
                showCue = true;
            }
            else if (_rootCompass.DockPosition != DockPosition.None && _rootCompass.DockPosition != DockPosition.Center)
            {
                // 最外层停靠
                Pane dockItem = p_win.Content as Pane;
                Size relativeSize = new Size(dockItem.InitWidth, dockItem.InitHeight);
                switch (_rootCompass.DockPosition)
                {
                    case DockPosition.Top:
                        rect.Width = ActualWidth;
                        rect.Height = relativeSize.Height;
                        break;

                    case DockPosition.Bottom:
                        rect.Y += ActualHeight - relativeSize.Height;
                        rect.Width = ActualWidth;
                        rect.Height = relativeSize.Height;
                        break;

                    case DockPosition.Left:
                        rect.Width = relativeSize.Width;
                        rect.Height = ActualHeight;
                        break;

                    case DockPosition.Right:
                        rect.X += ActualWidth - relativeSize.Width;
                        rect.Width = relativeSize.Width;
                        rect.Height = ActualHeight;
                        break;
                }
                showCue = true;
            }

            if (showCue)
            {
                _dragCue.Visibility = Visibility.Visible;
                _dragCue.Width = rect.Width;
                _dragCue.Height = rect.Height;
                Canvas.SetLeft(_dragCue, rect.Left);
                Canvas.SetTop(_dragCue, rect.Top);
            }
            else
            {
                _dragCue.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 获取指定位置的Tabs
        /// </summary>
        /// <param name="p_pos"></param>
        /// <param name="p_subtree"></param>
        /// <param name="p_parent"></param>
        /// <returns></returns>
        static Tabs GetHitSect(Point p_pos, UIElement p_subtree, UIElement p_parent)
        {
            // uno中的 VisualTreeHelper.FindElementsInHostCoordinates 无值
            var ls = p_subtree.FindChildrenByType<Tabs>();
            foreach (var tabs in ls)
            {
                if (tabs.ContainPoint(p_pos)
                    && CheckIsDockable(tabs)
                    && !p_parent.IsAncestorOf(tabs.OwnWinItem))
                    return tabs;
            }
            return null;
        }
        #endregion

        #region 拖动结束
        /// <summary>
        /// 拖拽结束
        /// </summary>
        /// <param name="p_toolWin"></param>
        internal void OnDragCompleted(ToolWindow p_toolWin)
        {
            if (p_toolWin == null)
                return;

            Pane winItem = p_toolWin.Content as Pane;
            if (_sectWithCompass != null && _compass.DockPosition != DockPosition.None)
            {
                // 停靠在Pane内部
                p_toolWin.ClearValue(ContentControl.ContentProperty);
                _sectWithCompass.AddItem(winItem, _compass.DockPosition);
            }
            else if (_rootCompass.DockPosition != DockPosition.None && _rootCompass.DockPosition != DockPosition.Center)
            {
                // 停靠在四边
                p_toolWin.ClearValue(ContentControl.ContentProperty);
                switch (_rootCompass.DockPosition)
                {
                    case DockPosition.Top:
                        winItem.Pos = PanePosition.Top;
                        break;
                    case DockPosition.Bottom:
                        winItem.Pos = PanePosition.Bottom;
                        break;
                    case DockPosition.Left:
                        winItem.Pos = PanePosition.Left;
                        break;
                    case DockPosition.Right:
                        winItem.Pos = PanePosition.Right;
                        break;
                }
                Items.Insert(0, winItem);
            }

            _sectWithCompass = null;
            _compass.DockPosition = DockPosition.None;
            _rootCompass.DockPosition = DockPosition.None;
            _compass.Visibility = Visibility.Collapsed;
            _rootCompass.Visibility = Visibility.Collapsed;
            _dragCue.Visibility = Visibility.Collapsed;

            if (_isDragDelta)
            {
                OnLayoutChanged();
                _isDragDelta = false;
            }
        }
        #endregion

        #region 主区切换内容
        /// <summary>
        /// 动态切换主区内容
        /// </summary>
        /// <param name="p_content"></param>
        public void LoadMain(object p_content)
        {
            if (Kit.IsPhoneUI)
                LoadPhoneMain(p_content);
            else if (p_content is Tab tab)
                LoadWinMainTab(tab);
            else
                LoadWinMain(p_content);
        }

        void LoadPhoneMain(object p_content)
        {
            // 未加载win的Home页前不导航
            if (_tabs == null)
                return;

            // 内容相同也需导航
            if (p_content is Win win)
            {
                // 设置内嵌窗口标志，释放时判断
                win.SetValue(IsInnerWinInPhoneUIProperty, true);
                win.NaviToHome();
            }
            else if (p_content is Tab nt)
            {
                PhonePage.Show(nt);
            }
            else
            {
                Tab tab = (Tab)GetValue(PhoneMainTabProperty);
                if (tab == null)
                {
                    tab = new Tab();
                    SetValue(PhoneMainTabProperty, tab);
                }
                tab.Content = p_content;
                PhonePage.Show(tab);
            }
        }

        void LoadWinMainTab(Tab p_tab)
        {
            Tabs tabs = (Tabs)GetValue(MainTabsProperty);
            if (tabs != null)
            {
                if (tabs.Items.Count > 0)
                {
                    // 上次内容为相同的Tab时无需增加
                    if (tabs.Tag == _mainTabFlag && tabs.Items[0] == p_tab)
                        return;

                    if (tabs.Tag != _mainTabFlag)
                    {
                        // 清除内容，以便下次再添加
                        tabs.Items[0].Content = null;
                    }
                }
                CenterItem.Items.Remove(tabs);
                tabs.Items.Clear();
            }

            tabs = new Tabs { Tag = _mainTabFlag };
            tabs.Items.Add(p_tab);
            SetValue(MainTabsProperty, tabs);
            CenterItem.Items.Add(tabs);
        }

        void LoadWinMain(object p_content)
        {
            Tab tab;
            Tabs tabs = (Tabs)GetValue(MainTabsProperty);

            if (tabs != null && tabs.Tag == _mainTabFlag)
            {
                // 上次内容为Tab，全移除
                CenterItem.Items.Remove(tabs);
                tabs.Items.Clear();
            }

            if (tabs == null
                || tabs.Items.Count == 0
                || tabs.Tag == _mainTabFlag)
            {
                // 初次加载 或 恢复默认布局后 或 上次内容为Tab
                tabs = new Tabs { ShowHeader = false };
                tab = new Tab();
                tabs.Items.Add(tab);
                SetValue(MainTabsProperty, tabs);

                // 已应用模板
                CenterItem.Items.Add(tabs);
            }
            else
            {
                tab = (Tab)tabs.Items[0];
            }

            // 切换内容
            if (tab.Content != p_content)
            {
                if (p_content is Win win)
                {
                    // 为重合边线
                    win.Margin = new Thickness(-1, -1, 0, 0);
                    // 关闭时用
                    win.SetValue(OwnTabProperty, tab);
                }
                tab.Content = p_content;
            }
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 删除所有ToolWindow
        /// </summary>
        internal void ClearWindows()
        {
            if (_popupPanel == null || _popupPanel.Children.Count == 0)
                return;

            int index = 0;
            while (index < _popupPanel.Children.Count)
            {
                ToolWindow win = _popupPanel.Children[index] as ToolWindow;
                if (win != null)
                {
                    // 先移除当前项，再清除子项，不可颠倒！
                    _popupPanel.Children.RemoveAt(index);
                    Pane di = win.Content as Pane;
                    if (di != null)
                        LayoutManager.ClearItems(di);
                }
                else
                {
                    index++;
                }
            }
        }

        /// <summary>
        /// 窗口内容是否可停靠
        /// </summary>
        /// <param name="p_win"></param>
        /// <returns></returns>
        internal static bool CheckIsDockable(ToolWindow p_win)
        {
            return ((p_win != null) && CheckIsDockable(p_win.Content as Pane));
        }

        /// <summary>
        /// Pane内容是否可停靠
        /// </summary>
        /// <param name="p_dockItem"></param>
        /// <returns></returns>
        internal static bool CheckIsDockable(Pane p_dockItem)
        {
            if ((p_dockItem == null) || (p_dockItem.Items.Count <= 0))
                return false;

            Tabs sect;
            return (((sect = p_dockItem.Items[0] as Tabs) != null && CheckIsDockable(sect))
                || CheckIsDockable(p_dockItem.Items[0] as Pane));
        }

        /// <summary>
        /// 内容是否可停靠
        /// </summary>
        /// <param name="p_sect"></param>
        /// <returns></returns>
        internal static bool CheckIsDockable(Tabs p_sect)
        {
            return (p_sect != null
                && p_sect.Items.Count > 0
                && CheckIsDockable(p_sect.Items[0] as Tab));
        }

        /// <summary>
        /// 内容是否可停靠
        /// </summary>
        /// <param name="pane"></param>
        /// <returns></returns>
        internal static bool CheckIsDockable(Tab pane)
        {
            return ((pane != null) && pane.CanDock);
        }

        /// <summary>
        /// 内容是否可停靠
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        internal static bool CheckIsDockable(TabHeader header)
        {
            return ((header != null) && CheckIsDockable(header.Owner as Tabs));
        }

        void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (AutoUnpinSide && e.NewSize.Width != e.PreviousSize.Width)
                _layout?.OnWidthChanged(e.NewSize.Width);

            if (_rootCompass != null)
            {
                // 更新RootCompass的大小，因在Canvas中不能自动伸展
                _rootCompass.Width = e.NewSize.Width;
                _rootCompass.Height = e.NewSize.Height;
            }
        }

        static ToolWindow GetParentWindow(Tab p_sectItem)
        {
            return GetParentWindow(p_sectItem?.OwnTabs?.OwnWinItem);
        }

        static ToolWindow GetParentWindow(Tabs p_sect)
        {
            return GetParentWindow(p_sect?.OwnWinItem);
        }

        static ToolWindow GetParentWindow(Pane p_winItem)
        {
            if (p_winItem != null)
            {
                while (p_winItem != null && p_winItem.Parent is TabItemPanel pnl)
                {
                    p_winItem = pnl.Owner;
                }
                return p_winItem?.Parent as ToolWindow;
            }
            return null;
        }

        Point GetElementPositionRelatedToPopup(FrameworkElement element)
        {
            return element.TransformToVisual(this).TransformPoint(new Point());
        }
        #endregion

        #region 四周停靠
        internal void OnPinChange(Tab item)
        {
            if (item.IsPinned)
            {
                AutoHideTab autoHide = item.Owner as AutoHideTab;
                if (autoHide != null)
                {
                    autoHide.Pin(item);
                    PanePosition dockState = GetDockState(autoHide.TabStripPlacement);
                    Tabs sect = FindPinSect(dockState);
                    if (sect != null)
                    {
                        // 直接停靠
                        sect.Items.Add(item);
                    }
                    else
                    {
                        // 该区无已固定的停靠
                        Pane dockItem = new Pane();
                        dockItem.Pos = dockState;

                        // 停靠后面板尺寸按照Tab浮动时展开的尺寸
                        switch (dockItem.Pos)
                        {
                            case PanePosition.Right:
                            case PanePosition.Left:
                                if (item.ReadLocalValue(TabItem.PopWidthProperty) != DependencyProperty.UnsetValue)
                                    dockItem.InitWidth = item.PopWidth;
                                break;
                            case PanePosition.Bottom:
                            case PanePosition.Top:
                                if (item.ReadLocalValue(TabItem.PopHeightProperty) != DependencyProperty.UnsetValue)
                                    dockItem.InitHeight = item.PopHeight;
                                break;
                        }

                        sect = new Tabs();
                        dockItem.Items.Add(sect);
                        sect.Items.Add(item);
                        Items.Add(dockItem);
                    }
                }
            }
            else
            {
                Tabs sect = item.Owner as Tabs;
                Pane dockItem;
                if (sect != null && (dockItem = sect.OwnWinItem) != null)
                {
                    switch (dockItem.Pos)
                    {
                        case PanePosition.Left:
                            if (LeftAutoHide == null)
                                CreateLeftAutoHideTab();
                            LeftAutoHide.Unpin(item);
                            break;
                        case PanePosition.Bottom:
                            if (BottomAutoHide == null)
                                CreateBottomAutoHideTab();
                            BottomAutoHide.Unpin(item);
                            break;
                        case PanePosition.Right:
                            if (RightAutoHide == null)
                                CreateRightAutoHideTab();
                            RightAutoHide.Unpin(item);
                            break;
                        case PanePosition.Top:
                            if (TopAutoHide == null)
                                CreateTopAutoHideTab();
                            TopAutoHide.Unpin(item);
                            break;
                    }
                }
            }
            OnLayoutChanged();
        }

        /// <summary>
        /// 查找停靠位置的Tabs
        /// </summary>
        /// <param name="p_dockState"></param>
        /// <returns></returns>
        Tabs FindPinSect(PanePosition p_dockState)
        {
            Tabs sect = null;
            Pane item = (from dockItem in Items.OfType<Pane>()
                         where dockItem.Pos == p_dockState
                         select dockItem).FirstOrDefault();
            if (item != null)
            {
                sect = (from obj in item.GetAllTabs()
                        select obj).FirstOrDefault();
            }
            return sect;
        }

        PanePosition GetDockState(ItemPlacement p_dockState)
        {
            switch (p_dockState)
            {
                case ItemPlacement.Left:
                    return PanePosition.Left;

                case ItemPlacement.Right:
                    return PanePosition.Right;

                case ItemPlacement.Top:
                    return PanePosition.Top;

                case ItemPlacement.Bottom:
                    return PanePosition.Bottom;
            }
            return PanePosition.Left;
        }

        internal void CreateLeftAutoHideTab()
        {
            var tab = new AutoHideTab { TabStripPlacement = ItemPlacement.Left };
            Grid.SetRowSpan(tab, 3);
            Grid grid = (Grid)GetTemplateChild("RootGrid");
            grid.Children.Insert(0, tab);
            LeftAutoHide = tab;
        }

        internal void CreateRightAutoHideTab()
        {
            var tab = new AutoHideTab { TabStripPlacement = ItemPlacement.Right };
            Grid.SetRowSpan(tab, 3);
            Grid.SetColumn(tab, 2);
            Grid grid = (Grid)GetTemplateChild("RootGrid");
            grid.Children.Insert(0, tab);
            RightAutoHide = tab;
        }

        internal void CreateTopAutoHideTab()
        {
            var tab = new AutoHideTab { TabStripPlacement = ItemPlacement.Top };
            Grid.SetColumn(tab, 1);
            Grid grid = (Grid)GetTemplateChild("RootGrid");
            grid.Children.Insert(0, tab);
            TopAutoHide = tab;
        }

        internal void CreateBottomAutoHideTab()
        {
            var tab = new AutoHideTab { TabStripPlacement = ItemPlacement.Bottom };
            Grid.SetRow(tab, 2);
            Grid.SetColumn(tab, 1);
            Grid grid = (Grid)GetTemplateChild("RootGrid");
            grid.Children.Insert(0, tab);
            BottomAutoHide = tab;
        }
        #endregion

        #region 调整区域大小
        /// <summary>
        /// 调整区域大小后刷新布局
        /// </summary>
        /// <param name="p_resizer"></param>
        internal void OnLayoutChangeEnded(GridResizer p_resizer)
        {
            if (p_resizer.Preview.OffsetX != 0 || p_resizer.Preview.OffsetY != 0)
            {
                OnLayoutChanged();
            }
        }
        #endregion

        #region 触发事件
        /// <summary>
        /// 关闭或后退之前，返回false表示禁止关闭
        /// </summary>
        /// <returns>true 表允许关闭</returns>
        internal async Task<bool> AllowClose()
        {
            if (Closing != null)
            {
                var args = new AsyncCancelArgs();
                Closing(this, args);
                await args.EnsureAllCompleted();
                if (args.Cancel)
                    return false;
            }
            return await OnClosing();
        }

        /// <summary>
        /// 关闭或后退之后
        /// </summary>
        internal void AfterClosed()
        {
            Closed?.Invoke(this, EventArgs.Empty);
            OnClosed();

            // PhoneUI内嵌窗口从首页Tab返回时需要排除
            if (!Kit.IsPhoneUI || !(bool)GetValue(IsInnerWinInPhoneUIProperty))
                _cleaner.Add(this);
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

        /// <summary>
        /// 触发布局变化结束事件
        /// </summary>
        internal void OnLayoutChanged()
        {
            _layout?.SaveCurrentLayout();
            LayoutChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region IDestroy
        /// <summary>
        /// 负责所有Win内部资源的释放
        /// </summary>
        static readonly WinCleaner _cleaner = new WinCleaner();

        /// <summary>
        /// 嵌套在主区的窗口释放
        /// </summary>
        public void Destroy()
        {
            _cleaner.Add(this);
        }

        internal void OnDestroyed()
        {
            Destroyed?.Invoke(this);
        }

        internal void DetachEvent()
        {
            Items.ItemsChanged -= OnItemsChanged;
            SizeChanged -= OnSizeChanged;
        }
        #endregion
    }
}