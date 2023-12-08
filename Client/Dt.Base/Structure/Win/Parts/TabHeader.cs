#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-03-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Linq;
using Windows.Foundation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
#endregion

namespace Dt.Base.Docking
{
    /// <summary>
    /// 面板标题
    /// </summary>
    public partial class TabHeader : Control
    {
        #region 静态内容
        public static readonly DependencyProperty OwnerProperty = DependencyProperty.Register(
            "Owner",
            typeof(object),
            typeof(TabHeader),
            null);
        #endregion

        double _minOffset = 12;
        bool _dragging;
        Point _start;

        /// <summary>
        /// 构造函数
        /// </summary>
        public TabHeader()
        {
            DefaultStyleKey = typeof(TabHeader);
        }

        /// <summary>
        /// 获取所属Tabs
        /// </summary>
        public object Owner
        {
            get { return GetValue(OwnerProperty); }
            internal set { SetValue(OwnerProperty, value); }
        }

        #region 重写方法
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Button btn = GetTemplateChild("BackButton") as Button;
            if (btn != null)
                btn.Click += OnBackClick;

            if (Owner is Tabs tabs
                && tabs.TabStripPlacement == ItemPlacement.TopLeft
                && tabs.Items.Count > 1)
            {
                ToggleTabListButton(true);
            }
        }

        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            base.OnPointerPressed(e);
            if (CanFloat() && e.IsLeftButton())
            {
                Focus(FocusState.Programmatic);
                _dragging = CapturePointer(e.Pointer);
                if (_dragging)
                    _start = e.GetCurrentPoint(null).Position;
            }
        }

        protected override void OnPointerMoved(PointerRoutedEventArgs e)
        {
            base.OnPointerMoved(e);
            if (_dragging)
            {
                Point position = e.GetCurrentPoint(null).Position;
                double offsetX = position.X - _start.X;
                double offsetY = position.Y - _start.Y;
                if (Math.Abs(offsetX) > _minOffset || Math.Abs(offsetY) > _minOffset)
                {
                    _dragging = false;
                    ReleasePointerCapture(e.Pointer);
                    if (Owner is Tabs tabs && tabs.SelectedItem is Tab tab)
                        tab.OwnWin?.OnDragStarted(this, e);
                }
            }
        }

        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            base.OnPointerReleased(e);
            if (_dragging)
            {
                _dragging = false;
                ReleasePointerCapture(e.Pointer);
            }
        }
        #endregion

        #region 右键菜单
        static Menu _menu;

        protected override async void OnRightTapped(RightTappedRoutedEventArgs e)
        {
            base.OnRightTapped(e);

            if (_menu == null)
            {
                _menu = new Menu { IsContextMenu = true };

                Mi mi = new Mi { ID = "自动隐藏" };
                mi.Click += OnAutoHide;
                _menu.Items.Add(mi);

                mi = new Mi { ID = "复制容器类名" };
                mi.Click += OnCopyWin;
                _menu.Items.Add(mi);

                mi = new Mi { ID = "标签内容类名" };
                mi.Click += OnCopyTab;
                _menu.Items.Add(mi);

#if WIN
                mi = new Mi { ID = "预览容器Xaml" };
                mi.Click += OnWinXaml;
                _menu.Items.Add(mi);

                mi = new Mi { ID = "预览标签Xaml" };
                mi.Click += OnTabXaml;
                _menu.Items.Add(mi);
#endif
            }

            if (Owner is TabControl tc
                && tc.SelectedItem is Tab t)
            {
                _menu.DataContext = t;

                if (Owner is Tabs tabs
                    && tabs.OwnWin != null
                    && tabs.SelectedItem is Tab tab
                    && tab.CanUserPin
                    && !tab.IsInCenter
                    && !tab.IsFloating)
                {
                    _menu[0].Visibility = Visibility.Visible;
                }
                else
                {
                    _menu[0].Visibility = Visibility.Collapsed;
                }
                await _menu.OpenContextMenu(e.GetPosition(null));
            }
        }

        protected override void OnDoubleTapped(DoubleTappedRoutedEventArgs e)
        {
            base.OnDoubleTapped(e);
            if (Owner is TabControl tc
                && tc.SelectedItem is Tab t)
            {
                t.OnHeaderDoubleClick(e);
            }
        }

        static void OnAutoHide(Mi e)
        {
            if (_menu.DataContext is Tab tab)
                tab.IsPinned = false;
        }

        static void OnCopyWin(Mi e)
        {
            var tab = _menu.DataContext as Tab;
            Type tp = tab.OwnWin != null ? tab.OwnWin.GetType() : (tab.OwnDlg != null ? tab.OwnDlg.GetType() : null);
            if (tp == null)
            {
                Kit.Warn($"未找到[{tab.GetType().FullName}]的容器！");
            }
            else
            {
                Kit.CopyToClipboard(tp.FullName, true);
            }
        }

        static void OnCopyTab(Mi e)
        {
            var tab = _menu.DataContext as Tab;
            Type tp = tab.GetType();
            // fifo的Fab
            if ((tp == typeof(Tab) || tp.Name == "Fab")
                && tab.Content != null)
            {
                tp = tab.Content.GetType();
            }
            Kit.CopyToClipboard(tp.FullName, true);
        }

#if WIN
        static void OnWinXaml(Mi e)
        {
            var tab = _menu.DataContext as Tab;
            Type tp = tab.OwnWin != null ? tab.OwnWin.GetType() : (tab.OwnDlg != null ? tab.OwnDlg.GetType() : null);
            if (tp == null)
            {
                Kit.Warn($"未找到 [{tab.GetType().FullName}] 的容器！");
                return;
            }

            string res = GetSourcePath(tp);
            if (res == null)
            {
                Kit.Warn($"未找到 [{tp.FullName}] 的Xaml内容！");
            }
            else
            {
                ShowXamlDlg(res, tp);
            }
        }

        static void OnTabXaml(Mi e)
        {
            var tab = _menu.DataContext as Tab;
            Type tp = tab.GetType();
            if ((tp == typeof(Tab) || tp.Name == "Fab")
                && tab.Content != null)
            {
                tp = tab.Content.GetType();
            }

            string res = GetSourcePath(tp);
            if (res == null)
            {
                Kit.Msg($"[{tp.Name}] 无Xaml内容，\r\n显示容器Xaml");
                OnWinXaml(e);
            }
            else
            {
                ShowXamlDlg(res, tp);
            }
        }

        internal static string GetSourcePath(Type p_type)
        {
            var asmName = p_type.Assembly.GetName().Name;
            if (asmName.StartsWith("Microsoft.")
                || asmName.StartsWith("Windows.")
                || asmName.StartsWith("System."))
                return null;

            string res = null;
            var xbf = p_type.Name + ".xbf";
            var xbfPath = $"/{p_type.Name}.xbf";

            try
            {
                var rm = new Microsoft.Windows.ApplicationModel.Resources.ResourceManager();
                var sub = rm.MainResourceMap.GetSubtree("Files/" + asmName);
                for (uint i = 0; i < sub.ResourceCount; i++)
                {
                    var key = sub.GetValueByIndex(i).Key;
                    if (key.EndsWith(xbfPath, StringComparison.OrdinalIgnoreCase)
                        || key.Equals(xbf, StringComparison.OrdinalIgnoreCase))
                    {
                        res = asmName + "." + key.Substring(0, key.Length - 3).Replace('/', '.') + "xaml";
                        break;
                    }
                }
            }
            catch { }
            return res;
        }

        internal static void ShowXamlDlg(string p_res, Type p_tp)
        {
            try
            {
                string xaml;
                using (Stream stream = p_tp.Assembly.GetManifestResourceStream(p_res))
                using (var sr = new StreamReader(stream))
                {
                    xaml = sr.ReadToEnd();
                }

                Dlg dlg = new Dlg
                {
                    Title = p_tp.FullName,
                    IsPinned = true,
                    Height = Kit.ViewHeight - 150,
                    Width = Math.Ceiling(Kit.ViewWidth / 2),
                };
                var sc = new ScrollViewer
                {
                    HorizontalScrollMode = ScrollMode.Auto,
                    HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                    VerticalScrollMode = ScrollMode.Auto,
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                    Margin = new Thickness(0, 0, 10, 10),
                };
                sc.Content = new TextBlock
                {
                    Text = xaml,
                    TextTrimming = TextTrimming.None,
                    TextWrapping = TextWrapping.NoWrap,
                    IsTextSelectionEnabled = true,
                    Padding = new Thickness(20),
                };
                dlg.Content = sc;
                dlg.Show();
            }
            catch { }
        }
#endif
        #endregion

        #region 内部方法
        async void OnBackClick(object sender, RoutedEventArgs e)
        {
            if (Owner is Tabs tabs && tabs.SelectedItem is Tab tab)
                await tab.Backward();
            else if (Owner is AutoHideTab autoTabs && autoTabs.SelectedItem is Tab autoTab)
                await autoTab.Backward();
        }

        bool CanFloat()
        {
            Tabs tabs;
            if (Owner == null || (tabs = Owner as Tabs) == null)
                return false;

            return (from pane in tabs.Items
                    where pane is Tab
                    select pane).All((item) => (item as Tab).CanFloat);
        }

        internal void ToggleTabListButton(bool p_show)
        {
            if (Owner is not Tabs tabs
                || GetTemplateChild("Content") is not Grid grid)
                return;

            if (p_show && tabs.Items.Count > 1)
            {
                var btn = new Button
                {
                    Content = "\uE05D",
                    Style = Res.字符按钮,
                    Foreground = Res.深灰1,
                };
                btn.Click += tabs.OpenTabListDlg;
                Grid.SetColumn(btn, 1);
                grid.Children.Add(btn);
            }
            else
            {
                foreach (var elem in grid.Children)
                {
                    if (elem is Button btn && btn.Content == "\uE05D")
                    {
                        grid.Children.Remove(elem);
                        break;
                    }
                }
            }
        }
        #endregion
    }
}