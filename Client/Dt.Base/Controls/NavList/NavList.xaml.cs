#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-10-15 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 功能列表
    /// </summary>
    public partial class NavList : Tab
    {
        #region 静态内容
        public readonly static DependencyProperty ToProperty = DependencyProperty.Register(
            "To",
            typeof(NavTarget),
            typeof(NavList),
            new PropertyMetadata(NavTarget.WinMain));

        public readonly static DependencyProperty ItemTemplateProperty = DependencyProperty.Register(
            "ItemTemplate",
            typeof(DataTemplate),
            typeof(NavList),
            new PropertyMetadata(null, OnItemTemplateChanged));

        public readonly static DependencyProperty AutoCloseDlgProperty = DependencyProperty.Register(
            "AutoCloseDlg",
            typeof(bool),
            typeof(NavList),
            new PropertyMetadata(true));

        static void OnItemTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var nav = (NavList)d;
            var temp = (DataTemplate)e.NewValue;
            if (temp != null)
            {
                nav._lv.View = temp;
            }
            else
            {
                nav._lv.View = new NavRowView(nav);
            }
        }
        #endregion

        #region 成员变量
        Nav _initNav;
        #endregion

        #region 构造方法
        public NavList()
        {
            InitializeComponent();

            // 默认采用自动行高，因数据行数较少！
            _lv.ItemHeight = double.NaN;
            _lv.View = new NavRowView(this);
        }
        #endregion

        #region 属性
        /// <summary>
        /// 加载内容的目标位置，默认WinMain(当前Win的主区)，Nav.To的优先级更高
        /// </summary>
        public NavTarget To
        {
            get { return (NavTarget)GetValue(ToProperty); }
            set { SetValue(ToProperty, value); }
        }

        /// <summary>
        /// 获取设置视图类型：列表List、磁贴Tile，默认List
        /// </summary>
        public NavViewMode ViewMode
        {
            get { return _lv.ViewMode == Base.ViewMode.Tile ? NavViewMode.Tile : NavViewMode.List; }
            set
            {
                var mode = value == NavViewMode.Tile ? Base.ViewMode.Tile : Base.ViewMode.List;
                if (mode != _lv.ViewMode)
                {
                    if (ItemTemplate != null)
                    {
                        _lv.ViewMode = mode;
                    }
                    else
                    {
                        _lv.ChangeView(new NavRowView(this), mode);
                    }
                }
            }
        }

        /// <summary>
        /// 获取设置 Nav 类型的数据源列表，如 Nl&lt;Nav&gt; 和 Nl&lt;GroupData&lt;Nav&gt;&gt;，其它类型不支持
        /// </summary>
        public INotifyList Data
        {
            get { return _lv.Data; }
            set { _lv.Data = value; }
        }

        /// <summary>
        /// 获取设置行/项目高度，0时以第一项高度为准，NaN时自动高度
        /// <para>默认NaN：因数据行数较少，每项的内容多少不同！</para>
        /// </summary>
        public double ItemHeight
        {
            get { return _lv.ItemHeight; }
            set { _lv.ItemHeight = value; }
        }

        /// <summary>
        /// 获取设置项的DataTemplate，null时采用默认模板
        /// </summary>
        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        /// <summary>
        /// 获取设置选择模式，默认None
        /// </summary>
        public SelectionMode SelectionMode
        {
            get { return _lv.SelectionMode; }
            set { _lv.SelectionMode = value; }
        }

        /// <summary>
        /// 当放在Dlg中时，点击项后是否自动关闭对话框，默认true
        /// </summary>
        public bool AutoCloseDlg
        {
            get { return (bool)GetValue(AutoCloseDlgProperty); }
            set { SetValue(AutoCloseDlgProperty, value); }
        }

        /// <summary>
        /// 获取内部的Lv控件
        /// </summary>
        public Lv ListView => _lv;
        #endregion

        #region 外部方法
        /// <summary>
        /// 选择某项
        /// </summary>
        /// <param name="p_index">索引</param>
        public void Select(int p_index)
        {
            if (p_index < 0)
                return;

            Nav nav = null;
            int index = -1;
            foreach (var item in Data)
            {
                if (item is Nav n)
                {
                    if (++index == p_index)
                    {
                        nav = n;
                        break;
                    }
                }
                else if (item is GroupData<Nav> group)
                {
                    foreach (var gi in group)
                    {
                        if (++index == p_index)
                        {
                            nav = gi;
                            break;
                        }
                    }
                    if (nav != null)
                        break;
                }
            }
            Select(nav);
        }

        /// <summary>
        /// 选择某项
        /// </summary>
        /// <param name="p_nav">项对象</param>
        public void Select(Nav p_nav)
        {
            if (p_nav == null)
                return;
            
            if (OwnWin == null && !IsLoaded)
            {
                // OnInit中重新选择
                _initNav = p_nav;
                return;
            }

            if (p_nav.Callback != null)
            {
                p_nav.Callback.Invoke(OwnWin == null ? OwnDlg : OwnWin, p_nav);
            }
            else if (p_nav.Type != null)
            {
                var to = p_nav.To == null ? To : p_nav.To.Value;
                if (to == NavTarget.WinMain)
                {
                    var center = p_nav.GetCenter();
                    if (center is Win win)
                    {
                        win.Title = p_nav.Title;
                        if (p_nav.Icon != Icons.None)
                            win.Icon = p_nav.Icon;
                    }
                    OwnWin?.LoadMain(center);
                }
                else
                {
                    Kit.OpenWin(p_nav.Type, p_nav.Title, p_nav.Icon, p_nav.Params);
                    if (OwnDlg != null && AutoCloseDlg)
                        OwnDlg.Close();
                }
            }
        }
        #endregion

        #region 内部方法
        void OnItemClick(ItemClickArgs e)
        {
            Select(e.Data as Nav);
        }

        protected override void OnFirstLoaded()
        {
            // 递归触发嵌套子窗口Closing事件，PhoneUI模式页面返回时已处理
            if (OwnWin != null)
            {
                if (_initNav != null)
                {
                    Select(_initNav);
                    _initNav = null;
                }

                if (!Kit.IsPhoneUI)
                {
                    OwnWin.Closing += OwnWin_Closing;
                }
            }
        }

        async void OwnWin_Closing(object sender, AsyncCancelArgs e)
        {
            using (e.Wait())
            {
                foreach (var row in _lv.Rows)
                {
                    if (row.Data is Nav nav)
                    {
                        var to = nav.To == null ? To : nav.To.Value;
                        if (to == NavTarget.WinMain)
                        {
                            if (!await nav.AllowClose())
                            {
                                e.Cancel = true;
                                return;
                            }
                        }
                    }
                }
            }
        }
        #endregion
    }
}