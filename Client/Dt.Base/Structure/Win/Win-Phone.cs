#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-03-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.Docking;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Base
{
    public partial class Win
    {
        Dictionary<string, Tab> _tabs;
        // 主页在Frame中的索引
        int _frameStartIndex;
        // 多页PhoneTabs缓存
        List<PhoneTabs> _cacheMultiTabs;
        string _homeID;

        /// <summary>
        /// 导航到窗口主页
        /// </summary>
        public void NaviToHome()
        {
            if (!Kit.IsPhoneUI)
                return;

            if (_tabs == null)
            {
                _tabs = new Dictionary<string, Tab>(StringComparer.OrdinalIgnoreCase);
                ExtractItems(this);
            }
            // 记录起始索引
            _frameStartIndex = UITree.RootFrame.BackStackDepth;

            var ls = (from tab in _tabs.Values
                      where tab.Order > 0
                      orderby tab.Order
                      select tab).ToList();
            if (ls.Count == 0)
            {
                var tab = _tabs.Values.FirstOrDefault();
                if (tab != null)
                {
                    // 置首页标志
                    tab.Order = 1;
                    NaviToSingleTab(tab);
                }
            }
            else if (ls.Count == 1)
            {
                NaviToSingleTab(ls[0]);
            }
            else if (ls.Count > 1)
            {
                _homeID = ls[0].Title;
                for (int i = 1; i < ls.Count; i++)
                {
                    _homeID = $"{_homeID},{ls[i].Title}";
                }
                NaviToMultiTabs(_homeID);
            }
        }

        /// <summary>
        /// 导航到指定页，支持多页Tab形式
        /// </summary>
        /// <param name="p_tabTitle">多个页面时用逗号隔开(自动以Tab形式显示)，null时自动导航到第一个Tab</param>
        public void NaviTo(string p_tabTitle)
        {
            if (!Kit.IsPhoneUI)
                return;

            // 导航到单页或多页Tab
            if (string.IsNullOrEmpty(p_tabTitle))
            {
                Tab tab = _tabs.Values.FirstOrDefault();
                if (tab != null)
                    NaviToSingleTab(tab);
            }
            else if (!p_tabTitle.Contains(','))
            {
                if (_tabs.TryGetValue(p_tabTitle, out var tab))
                    NaviToSingleTab(tab);
                else
                    Kit.Warn($"导航出错，缺少{p_tabTitle}Tab页！");
            }
            else
            {
                NaviToMultiTabs(p_tabTitle);
            }
        }

        /// <summary>
        /// 在多页Tab中选择指定Tab
        /// </summary>
        /// <param name="p_tabTitle">Tab标题</param>
        public void SelectTab(string p_tabTitle)
        {
            if (!Kit.IsPhoneUI)
                return;

            if (((PhonePage)UITree.RootFrame.Content).Content is PhoneTabs tabs)
            {
                tabs.Select(p_tabTitle);
            }
        }

        /// <summary>
        /// 在多页Tab中选择指定Tab
        /// </summary>
        /// <param name="p_index">Tab索引</param>
        public void SelectTab(int p_index)
        {
            if (!Kit.IsPhoneUI)
                return;

            if (((PhonePage)UITree.RootFrame.Content).Content is PhoneTabs tabs)
            {
                tabs.Select(p_index);
            }
        }

        /// <summary>
        /// 导航到单页Tab
        /// </summary>
        /// <param name="p_tab"></param>
        internal void NaviToSingleTab(Tab p_tab)
        {
            // 判断是否为向后导航
            var frame = UITree.RootFrame;
            if (frame.BackStackDepth > _frameStartIndex)
            {
                // 向后查询
                int index = -1;
                for (int i = frame.BackStackDepth - 1; i >= _frameStartIndex; i--)
                {
                    var pageEntry = frame.BackStack[i];
                    if (pageEntry.Parameter is PhonePageParameter param
                        && param.Content as Control == p_tab)
                    {
                        // 后退位置
                        index = i;
                        break;
                    }
                }

                if (index > 0)
                {
                    // 向后导航
                    for (int i = frame.BackStackDepth - 1; i >= index; i--)
                    {
                        if (frame.CanGoBack)
                            frame.GoBack();
                    }
                    return;
                }
            }

            // 起始页隐藏返回按钮
            if (frame.Content == null)
                p_tab.BackButtonVisibility = Visibility.Collapsed;

            // 向前导航
            PhonePage.Show(p_tab);
        }

        /// <summary>
        /// 导航到多页Tab
        /// </summary>
        /// <param name="p_tabTitle"></param>
        internal void NaviToMultiTabs(string p_tabTitle)
        {
            // 判断是否为向后导航
            var frame = UITree.RootFrame;
            if (frame.BackStackDepth > _frameStartIndex)
            {
                // 向后查询
                int index = -1;
                for (int i = frame.BackStackDepth - 1; i >= _frameStartIndex; i--)
                {
                    var pageEntry = frame.BackStack[i];
                    if (pageEntry.Parameter is PhonePageParameter param
                        && param.Content is PhoneTabs pts
                        && pts.NaviID == p_tabTitle)
                    {
                        // 后退位置
                        index = i;
                        break;
                    }
                }

                if (index > 0)
                {
                    // 向后导航
                    for (int i = frame.BackStackDepth - 1; i >= index; i--)
                    {
                        if (frame.CanGoBack)
                            frame.GoBack();
                    }
                    return;
                }
            }

            // 向前导航
            PhoneTabs tabs;
            if (_cacheMultiTabs != null)
            {
                tabs = (from t in _cacheMultiTabs
                        where t.NaviID == p_tabTitle
                        select t).FirstOrDefault();
                if (tabs != null)
                {
                    // 缓存中存在
                    PhonePage.Show(tabs);
                    return;
                }
            }

            tabs = new PhoneTabs();
            tabs.NaviID = p_tabTitle;
            if (p_tabTitle == _homeID)
                tabs.IsHome = true;

            Tab tab;
            string[] names = p_tabTitle.Split(',');
            foreach (var name in names)
            {
                if (_tabs.TryGetValue(name, out tab))
                    tabs.AddItem(tab);
                else
                    Kit.Warn($"导航出错，缺少{name}Tab页！");
            }

            // 起始页隐藏返回按钮
            if (frame.Content == null)
                tabs.HideBackButton();
            tabs.Select(0);

            if (_cacheMultiTabs == null)
                _cacheMultiTabs = new List<PhoneTabs>();
            _cacheMultiTabs.Add(tabs);

            OnInitPhoneTabs(tabs);
            PhonePage.Show(tabs);
        }

        /// <summary>
        /// 关闭窗口所属的所有导航页
        /// </summary>
        void CloseAllNaviPages()
        {
            var frame = UITree.RootFrame;
            if (frame.BackStackDepth > _frameStartIndex)
            {
                // 向后导航
                for (int i = frame.BackStackDepth; i > _frameStartIndex; i--)
                {
                    if (frame.CanGoBack)
                        frame.GoBack();
                }
            }
        }

        /// <summary>
        /// 初次加载多页Tab，可用于动态选择Tab
        /// </summary>
        /// <param name="p_tabs"></param>
        protected virtual void OnInitPhoneTabs(PhoneTabs p_tabs)
        {
        }

        /// <summary>
        /// 深度查找所有Tab项，构造以Tab.Title为键名以Tab为值的字典
        /// </summary>
        /// <param name="p_items"></param>
        void ExtractItems(IPaneList p_items)
        {
            foreach (var obj in p_items.Items)
            {
                if (obj is Tabs tabs)
                {
                    foreach (var tab in tabs.Items.OfType<Tab>())
                    {
                        string title = tab.Title;
                        if (string.IsNullOrEmpty(title))
                        {
                            // Tab无标题时内部生成id代替，无法使用标题名进行导航！
                            title = Kit.NewGuid.Substring(0, 6);
                            tab.Title = title;
                        }
                        tab.OwnWin = this;
                        _tabs[title] = tab;
                    }
                    tabs.Items.Clear();
                }
                else if (obj is Tab tab)
                {
                    string title = tab.Title;
                    if (string.IsNullOrEmpty(title))
                    {
                        // Tab无标题时内部生成id代替，无法使用标题名进行导航！
                        title = Kit.NewGuid.Substring(0, 6);
                        tab.Title = title;
                    }
                    tab.OwnWin = this;
                    _tabs[title] = tab;
                }
                else if (obj is IPaneList ic)
                {
                    ExtractItems(ic);
                }
                else
                {
                    // 普通界面元素，一般为单视图窗口
                    tab = new Tab
                    {
                        Content = obj,
                        Title = Title,
                        OwnWin = this
                    };
                    _tabs[Kit.NewGuid] = tab;
                }
            }
        }
    }
}