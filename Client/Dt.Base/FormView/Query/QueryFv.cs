#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-01-17 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 查询面板
    /// </summary>
    public partial class QueryFv : Fv
    {
        #region 静态内容
        public readonly static DependencyProperty IsAndProperty = DependencyProperty.Register(
            "IsAnd",
            typeof(bool),
            typeof(QueryFv),
            new PropertyMetadata(true));

        public readonly static DependencyProperty AllowFuzzySearchProperty = DependencyProperty.Register(
            "AllowFuzzySearch",
            typeof(bool),
            typeof(QueryFv),
            new PropertyMetadata(false, OnAllowFuzzySearchChanged));

        public readonly static DependencyProperty EnableToggleIsAndProperty = DependencyProperty.Register(
            "EnableToggleIsAnd",
            typeof(bool),
            typeof(QueryFv),
            new PropertyMetadata(false, OnEnableToggleIsAnd));

        static void OnAllowFuzzySearchChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((QueryFv)d).OnUIChanged();
        }

        static void OnEnableToggleIsAnd(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((QueryFv)d).OnUIChanged();
        }
        #endregion

        #region 成员变量
        BaseCommand _cmdQuery;
        FuzzySearch _fuzzy;
        #endregion

        #region 构造方法
        public QueryFv()
        {
            DefaultStyleKey = typeof(QueryFv);
        }
        #endregion

        #region 事件
        /// <summary>
        /// 查询事件
        /// </summary>
        public event Action<QueryClause> Query;
        #endregion

        /// <summary>
        /// 获取设置筛选条件之间是否为【与】操作，默认true，false时【或】操作
        /// </summary>
        public bool IsAnd
        {
            get { return (bool)GetValue(IsAndProperty); }
            set { SetValue(IsAndProperty, value); }
        }

        /// <summary>
        /// 获取设置是否允许切换到模糊搜索，默认false
        /// </summary>
        public bool AllowFuzzySearch
        {
            get { return (bool)GetValue(AllowFuzzySearchProperty); }
            set { SetValue(AllowFuzzySearchProperty, value); }
        }

        /// <summary>
        /// 获取设置是否允许切换查询条件之间的与或关系，true时显示选择框，默认false
        /// </summary>
        public bool EnableToggleIsAnd
        {
            get { return (bool)GetValue(EnableToggleIsAndProperty); }
            set { SetValue(EnableToggleIsAndProperty, value); }
        }

        public void OnQuery()
        {
            Query?.Invoke(new QueryClause(this));
        }

        #region 模糊搜索
        /// <summary>
        /// 切换到模糊搜索
        /// </summary>
        public void ToggleFuzzySearch()
        {
            Tab tab = null;
            if (Kit.IsPhoneUI)
            {
                tab = this.FindParentByType<Tab>();
            }
            else if (this.FindParentByType<Tabs>() is Tabs tabs)
            {
                tab = tabs.SelectedItem as Tab;
            }
            if (tab == null)
                Throw.Msg("不在Tab内无法导航！");

            if (_fuzzy == null)
            {
                _fuzzy = new FuzzySearch { OwnWin = tab.OwnWin };
                _fuzzy.Search += OnFuzzySearch;
            }
            tab.Forward(_fuzzy);
        }

        void OnFuzzySearch(string e)
        {
            if (Query != null)
                Query(new QueryClause(e));
        }
        #endregion

        #region 命令对象
        /// <summary>
        /// 获取查询命令
        /// </summary>
        public BaseCommand CmdQuery
        {
            get
            {
                if (_cmdQuery == null)
                    _cmdQuery = new BaseCommand(p_params => OnQuery());
                return _cmdQuery;
            }
        }
        #endregion

        #region 重写方法
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var btn = (Button)GetTemplateChild("BtnQuery");
            if (btn != null)
            {
                btn.Click += (s, e) => OnQuery();
            }

            btn = (Button)GetTemplateChild("BtnReset");
            if (btn != null)
            {
                btn.Click += OnResetClick;
            }

            if (AllowFuzzySearch || EnableToggleIsAnd)
                OnUIChanged();
        }

        protected override bool OnLastCellEnter()
        {
            OnQuery();
            return true;
        }
        #endregion

        #region 重置
        Menu _resetMenu;
        Dictionary<string, CompFlag> _lastCompFlag;

        private void OnResetClick(object sender, RoutedEventArgs e)
        {
            if (_resetMenu == null)
                CreateResetMenu();
            _resetMenu.OpenContextMenu((Button)sender);
        }

        void CreateResetMenu()
        {
            _resetMenu = new Menu
            {
                IsContextMenu = true,
                Placement = MenuPosition.BottomLeft,
                Items =
                {
                    new Mi("恢复初始查询值", call:RejectQueryVal),
                    new Mi("忽略所有查询条件", call:ClearAllQueryFlag),
                    new Mi("恢复忽略前的查询条件", call:ResetAllQueryFlag)
                }
            };
        }

        /// <summary>
        /// 恢复初始查询值
        /// </summary>
        public void RejectQueryVal()
        {
            if (Data is Row row)
                row.RejectChanges();
        }

        /// <summary>
        /// 忽略所有查询条件
        /// </summary>
        public void ClearAllQueryFlag()
        {
            Dictionary<string, CompFlag> dt = new Dictionary<string, CompFlag>();
            foreach (var cell in IDCells)
            {
                // 过滤掉非查询的
                if (cell.QueryFlag == CompFlag.Ignore)
                    continue;

                dt[cell.ID] = cell.QueryFlag;
                cell.QueryFlag = CompFlag.Ignore;
            }

            if (dt.Count > 0)
                _lastCompFlag = dt;
        }

        /// <summary>
        /// 恢复忽略前的查询条件
        /// </summary>
        public void ResetAllQueryFlag()
        {
            if (_lastCompFlag == null || _lastCompFlag.Count == 0)
                return;

            foreach (var cell in IDCells)
            {
                if (_lastCompFlag.TryGetValue(cell.ID, out var flag))
                    cell.QueryFlag = flag;
            }
            _lastCompFlag = null;
        }
        #endregion

        #region 切换界面元素
        void OnUIChanged()
        {
            if (!_isLoaded)
                return;

            var bar = (ContentPresenter)GetTemplateChild("Bar");
            if (!AllowFuzzySearch && !EnableToggleIsAnd)
            {
                bar.Content = null;
            }
            else if (AllowFuzzySearch && EnableToggleIsAnd)
            {
                bar.Content = new Grid { Children = { CreateIsAndUI(), CreateFuzzyBtn() } };
            }
            else if (AllowFuzzySearch)
            {
                bar.Content = CreateFuzzyBtn();
            }
            else if (EnableToggleIsAnd)
            {
                bar.Content = CreateIsAndUI();
            }
        }

        CheckBox CreateIsAndUI()
        {
            var cb = new CheckBox { Content = "条件之间【与】关系", Margin = new Thickness(10, 4, 10, 4), VerticalAlignment = VerticalAlignment.Center };
            cb.SetBinding(CheckBox.IsCheckedProperty, new Binding { Path = new PropertyPath("IsAnd"), Source = this, Mode = BindingMode.TwoWay });
            return cb;
        }

        Button CreateFuzzyBtn()
        {
            var btn = new Button { Content = "切换搜索", Foreground = Res.BlackBrush, HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Center };
            btn.Click += (s, e) => ToggleFuzzySearch();
            return btn;
        }
        #endregion
    }
}