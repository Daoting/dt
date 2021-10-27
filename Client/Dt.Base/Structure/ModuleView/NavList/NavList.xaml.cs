#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-10-15 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Windows.UI.Xaml;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 功能列表
    /// </summary>
    public partial class NavList : Mv
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
                nav._lv.View = nav._lv.ViewMode == Base.ViewMode.List ? nav.Resources["ListView"] : nav.Resources["TileView"];
            }
        }
        #endregion

        public NavList()
        {
            InitializeComponent();

            // 默认采用自动行高，因数据行数较少！
            _lv.ItemHeight = double.NaN;
            _lv.View = Resources["ListView"];
        }

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
            get { return _lv.ViewMode == Base.ViewMode.List ? NavViewMode.List : NavViewMode.Tile; }
            set
            {
                var mode = value == NavViewMode.List ? Base.ViewMode.List : Base.ViewMode.Tile;
                if (mode != _lv.ViewMode)
                {
                    if (ItemTemplate != null)
                    {
                        _lv.ViewMode = mode;
                    }
                    else
                    {
                        _lv.ChangeView(mode == Base.ViewMode.List ? Resources["ListView"] : Resources["TileView"], mode);
                    }
                }
            }
        }

        /// <summary>
        /// 获取设置数据源对象，Table或集合对象
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

        void OnItemClick(object sender, ItemClickArgs e)
        {
            var nav = e.Data as Nav;
            if (nav == null)
            {
                Kit.Warn("NavList仅支持Nav类型的数据源！");
                return;
            }

            if (nav.Callback != null)
            {
                nav.Callback.Invoke(_tab.OwnWin, nav);
            }
            else if (nav.Type != null)
            {
                var to = nav.To == null ? To : nav.To.Value;
                if (to == NavTarget.WinMain)
                {
                    var center = nav.GetCenter(nav.Params);
                    if (center is Win win)
                    {
                        win.Title = nav.Title;
                        if (nav.Icon != Icons.None)
                            win.Icon = nav.Icon;
                    }
                    _tab.OwnWin?.LoadMain(center);
                }
                else
                {
                    Kit.OpenWin(nav.Type, nav.Title, nav.Icon, nav.Params);
                }
            }
        }
    }
}