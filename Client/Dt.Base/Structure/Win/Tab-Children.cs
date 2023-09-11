#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-09-12 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 内含子Tab
    /// </summary>
    public partial class Tab : TabItem, IPhonePage
    {
        #region 静态内容
        public static readonly DependencyProperty ChildrenProperty = DependencyProperty.Register(
            "Children",
            typeof(Menu),
            typeof(Tab),
            new PropertyMetadata(null, OnChildrenChanged));

        public static readonly DependencyProperty ChildrenButtonVisibilityProperty = DependencyProperty.Register(
            "ChildrenButtonVisibility",
            typeof(Visibility),
            typeof(Tab),
            new PropertyMetadata(Visibility.Collapsed));

        public static readonly DependencyProperty ParentTabProperty = DependencyProperty.Register(
            "ParentTab",
            typeof(Tab),
            typeof(Tab),
            new PropertyMetadata(null));

        public static readonly DependencyProperty CurrentTabProperty = DependencyProperty.Register(
            "CurrentTab",
            typeof(Tab),
            typeof(Tab),
            new PropertyMetadata(null));

        static void OnChildrenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Tab tab = (Tab)d;
            if (e.OldValue is Menu menu)
            {
                menu.ItemClick -= tab.OnChildItemClick;
            }
            tab.OnChildrenChanged();
        }
        #endregion

        #region 属性
        /// <summary>
        /// 获取设置可切换的所有Tab列表
        /// </summary>
        public Menu Children
        {
            get { return (Menu)GetValue(ChildrenProperty); }
            set { SetValue(ChildrenProperty, value); }
        }

        /// <summary>
        /// 获取是否显示所有Tab列表按钮，默认Collapsed，内部绑定用
        /// </summary>
        public Visibility ChildrenButtonVisibility
        {
            get { return (Visibility)GetValue(ChildrenButtonVisibilityProperty); }
            internal set { SetValue(ChildrenButtonVisibilityProperty, value); }
        }

        /// <summary>
        /// 当前Tab的父Tab
        /// </summary>
        internal Tab ParentTab
        {
            get { return (Tab)GetValue(ParentTabProperty); }
            private set { SetValue(ParentTabProperty, value); }
        }

        /// <summary>
        /// 当前显示的Tab，此属性只在 ParentTab 有值
        /// </summary>
        Tab CurrentTab
        {
            get { return (Tab)GetValue(CurrentTabProperty); }
            set { SetValue(CurrentTabProperty, value); }
        }
        #endregion

        #region 内部方法
        internal void ShowChildrenMenu(object sender, RoutedEventArgs e)
        {
            if (ParentTab != null)
            {
                // 父Tab负责显示列表
                ParentTab.ShowChildrenMenu(sender, e);
                return;
            }

            var btn = sender as Button;
            var menu = Children;
            if (btn != null && menu != null)
            {
                // 初次显示
                if (menu.Items[menu.Items.Count - 1].Tag != this)
                {
                    // 自动添加父Tab，切换时添加可能无Title
                    menu.Items.Add(new Mi { ID = Title, Tag = this, IsCheckable = true, IsChecked = true, GroupName = "Toggle" });
                }

                menu.OpenContextMenu(btn);
            }
        }

        void OnChildrenChanged()
        {
            var menu = Children;
            if (menu != null && menu.Items.Count > 0)
            {
                foreach (var mi in menu.Items)
                {
                    mi.IsCheckable = true;
                    mi.IsChecked = false;
                    mi.GroupName = "Toggle";
                }

                menu.IsContextMenu = true;
                menu.Placement = MenuPosition.BottomLeft;
                menu.ItemClick -= OnChildItemClick;
                menu.ItemClick += OnChildItemClick;
                ChildrenButtonVisibility = Visibility.Visible;
                CurrentTab = this;
            }
            else
            {
                ChildrenButtonVisibility = Visibility.Collapsed;
            }
        }

        void OnChildItemClick(object sender, Mi e)
        {
            // 此方法只在 ParentTab 中调用！！！
            if (e.Tag == null || !e.IsCheckable)
                return;

            if (e.Tag is Tab tab)
            {
                CurrentTab.Switch(tab);
                CurrentTab = tab;
                return;
            }

            Tab tgt = null;
            if (e.Tag is Type type)
            {
                var t = Activator.CreateInstance(type) as Tab;
                if (t != null)
                {
                    tgt = t;
                }
                else
                {
                    Throw.Msg($"类型 [{type.FullName}] 不是Tab！");
                }
            }
            else if (e.Tag is string s)
            {
                var tp = Type.GetType(s);
                if (tp != null && Activator.CreateInstance(tp) is Tab t)
                {
                    tgt = t;
                }
                else
                {
                    Throw.Msg($"类型 [{s}] 不存在或不是Tab！");
                }
            }

            if (tgt != null)
            {
                tgt.ParentTab = this;
                tgt.ChildrenButtonVisibility = Visibility.Visible;
                e.Tag = tgt;
                CurrentTab.Switch(tgt);
                CurrentTab = tgt;
            }
        }

        /// <summary>
        /// 切换到指定Tab
        /// </summary>
        /// <param name="p_tab"></param>
        void Switch(Tab p_tab)
        {
            // 正在显示
            if (p_tab == null || p_tab == this)
                return;

            // PhoneUI模式
            if (Kit.IsPhoneUI)
            {
                // 向后导航到上页
                if (OwnDlg != null)
                {
                    OwnDlg.Close();
                }
                else if (UITree.RootFrame.CanGoBack)
                {
                    UITree.RootFrame.GoBack();
                }

                // 若上页Tab在Dlg中，切换的Tab也在新Dlg中
                if (OwnDlg != null)
                {
                    ShowDlg(p_tab);
                }
                else
                {
                    PhonePage.Show(p_tab);
                }
            }
            else
            {
                p_tab.OwnWin = OwnWin;
                p_tab.OwnDlg = OwnDlg;
                Owner.ReplaceItem(this, p_tab);
            }
        }
        #endregion
    }
}
