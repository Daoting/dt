#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-03-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// PhoneUI时的TabControl，内部用，为uno节省一级ContentPresenter！
    /// </summary>
    public partial class PhoneTabs : Control, IPhonePage
    {
        #region 成员变量
        Grid _root;
        readonly Grid _grid;
        Button _selected;
        #endregion

        public PhoneTabs()
        {
            DefaultStyleKey = typeof(PhoneTabs);

            _grid = new Grid { BorderThickness = new Thickness(0, 1, 0, 0), BorderBrush = AtRes.浅灰边框, Background = AtRes.浅灰背景 };
            Grid.SetRow(_grid, 1);
        }

        /// <summary>
        /// 只有是首页时才有值
        /// </summary>
        internal Win OwnWin { get; set; }

        /// <summary>
        /// 导航时的标识，所有Tab标题逗号隔开
        /// </summary>
        internal string NaviID { get; set; }

        /// <summary>
        /// 添加标签
        /// </summary>
        /// <param name="p_tab"></param>
        internal void AddItem(Tab p_tab)
        {
            _grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            Button btn = new Button();
            btn.Style = (Style)Application.Current.Resources["PhoneTabsBtn"];
            btn.DataContext = p_tab;
            btn.Click += OnBtnClick;
            btn.HorizontalAlignment = HorizontalAlignment.Stretch;
            Grid.SetColumn(btn, _grid.ColumnDefinitions.Count - 1);
            _grid.Children.Add(btn);
        }

        /// <summary>
        /// 隐藏所有标签的返回按钮
        /// </summary>
        internal void HideBackButton()
        {
            foreach (var btn in _grid.Children.OfType<Button>())
            {
                ((Tab)btn.DataContext).PinButtonVisibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 选择第一个标签
        /// </summary>
        internal void SelectFirstItem()
        {
            if (_grid.Children.Count > 0)
                SelectItem((Button)_grid.Children[0]);
        }

        #region 实现接口
        /// <summary>
        /// 关闭或后退之前，返回false表示禁止关闭
        /// </summary>
        /// <returns>true 表允许关闭</returns>
        Task<bool> IPhonePage.OnClosing()
        {
            if (OwnWin != null)
                return OwnWin.OnClosing();
            return Task.FromResult(true);
        }

        /// <summary>
        /// 关闭或后退之后
        /// </summary>
        void IPhonePage.OnClosed()
        {
            // 只在首页时处理
            if (OwnWin != null)
                OwnWin.OnClosed();
        }
        #endregion

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // 为uno节省一级ContentPresenter！
            _root = (Grid)GetTemplateChild("RootGrid");
            _root.Children.Add(_grid);
            if (_selected != null)
                SelectItem(_selected);
        }

        void OnBtnClick(object sender, RoutedEventArgs e)
        {
            SelectItem((Button)sender);
        }

        void SelectItem(Button p_btn)
        {
            _selected = p_btn;
            if (_root != null)
            {
                if (_root.Children.Count > 1)
                    _root.Children.RemoveAt(1);
                _root.Children.Add((UIElement)p_btn.DataContext);
                foreach (var btn in _grid.Children.OfType<Button>())
                {
                    btn.IsEnabled = (btn != p_btn);
                }
            }
        }
    }
}