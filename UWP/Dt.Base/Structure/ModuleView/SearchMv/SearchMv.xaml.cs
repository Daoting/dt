#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-09-08 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.FormView;
using Dt.Base.ModuleView;
using Dt.Core;
using System;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 搜索面板
    /// 固定搜索项：由外部设置，事件参数格式为 "#按钮名称"，#前缀用于区别普通搜索
    /// 历史搜索项：自动记录历史搜索，可删，事件参数内容为普通文本
    /// 搜索事件：所有固定搜索项、搜索框、历史搜索项统一触发Search事件
    /// 历史搜索项通过所在的Win路径识别，当同一Win中多个搜索时，请确保每个搜索的Title不同，以便正确识别搜索历史！！！
    /// </summary>
    [ContentProperty(Name = nameof(Fixed))]
    public partial class SearchMv : Mv
    {
        #region 成员变量
        string _lastText;
        string _baseUri;
        TextBox _tb;
        #endregion

        #region 构造方法
        public SearchMv()
        {
            InitializeComponent();

            Title = "搜索";
            _lvFix.Data = Fixed;
            LoadTopBar();
        }
        #endregion

        /// <summary>
        /// 查询事件
        /// </summary>
        public event EventHandler<string> Search;

        /// <summary>
        /// 获取设置查询框提示内容
        /// </summary>
        public string Placeholder
        {
            get { return _tb.PlaceholderText; }
            set { _tb.PlaceholderText = value; }
        }

        /// <summary>
        /// 外部定义的固定搜索项列表
        /// </summary>
        public Nl<string> Fixed { get; } = new Nl<string>();

        protected override void OnInit(object p_params)
        {
#if UWP
            if (_tab.BaseUri != null)
                _baseUri = _tab.BaseUri.AbsolutePath;
            else if (_tab.OwnWin != null)
                _baseUri = _tab.OwnWin.BaseUri.AbsolutePath;
#else
            // 识别不同的查询面板，因uno中BaseUri为空！
            if (_tab.OwnWin != null)
                _baseUri = _tab.OwnWin.GetType().FullName;
            else if (!BaseUri.AbsolutePath.EndsWith("/SearchMv.xaml"))
                _baseUri = BaseUri.AbsolutePath;
#endif

            _tb.Text = "";
            if (string.IsNullOrEmpty(_baseUri))
            {
                // 无法识别不同的查询面板，不加载历史
                if (_sp.Children.Count == 3)
                {
                    _sp.Children.RemoveAt(2);
                    _sp.Children.RemoveAt(1);
                }
            }
            else
            {
                // 同一Win中多个搜索时，使用Title识别搜索历史
                _baseUri = $"{_baseUri}+{Title}";
                LoadHisItems();
            }
        }

        void OnTextKeyUp(object sender, KeyRoutedEventArgs e)
        {
            // android只支持KeyUp，只在enter时触发！
            if (e.Key == VirtualKey.Enter)
            {
                e.Handled = true;
                string txt = ((TextBox)sender).Text.Trim();
                if (txt != "")
                    OnSearch(txt);
            }
        }

        void OnFixClick(object sender, ItemClickArgs e)
        {
            if (e.Data is string txt)
                OnSearch("#" + txt);
        }

        void OnSearch(string p_text)
        {
            Result = p_text;
            Search?.Invoke(this, p_text);

            // 非Tab内首页自动后退
            if (!IsHome)
                Backward();

            if (!string.IsNullOrEmpty(_baseUri)
                && !string.IsNullOrEmpty(p_text)
                && !p_text.StartsWith("#")
                && _lastText != p_text)
            {
                SaveCookie(p_text);
            }
        }

        void LoadTopBar()
        {
            _tb = new TextBox { PlaceholderText = "搜索", CornerRadius = new CornerRadius(10) };
            _tb.KeyUp += OnTextKeyUp;
            _tb.Loaded += (s, e) => _tb.Focus(FocusState.Programmatic);

            if (Kit.IsPhoneUI)
            {
                // 隐藏标题栏
                HideTitleBar = true;

                Grid grid = new Grid
                {
                    Background = Res.主蓝,
                    Height = 50,
                    Margin = new Thickness(0, 0, 0, 10),
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = GridLength.Auto },
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                    },
                };
                var btn = new Button
                {
                    Content = "\uE010",
                    Style = Res.字符按钮,
                    Foreground = Res.WhiteBrush,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Width = 50,
                };
                btn.Click += (s, e) => Backward();
                grid.Children.Add(btn);

                _tb.Margin = new Thickness(0, 5, 10, 5);
                Grid.SetColumn(_tb, 1);
                grid.Children.Add(_tb);

                _grid.Children.Add(grid);
            }
            else
            {
                _tb.Margin = new Thickness(10);
                _grid.Children.Add(_tb);
            }
        }

        #region 搜索历史
        void LoadHisItems()
        {
            _lvHis.Data = AtState.Query<SearchHistory>($"select * from SearchHistory where BaseUri='{_baseUri}' order by id desc limit 10");
        }

        async void SaveCookie(string p_text)
        {
            _lastText = p_text;
            // 删除重复
            AtState.Exec($"delete from SearchHistory where BaseUri='{_baseUri}' and Content='{p_text}'");

            SearchHistory his = new SearchHistory(BaseUri: _baseUri, Content: p_text);
            await AtState.Save(his, false);
            LoadHisItems();
        }

        void OnClearHis(object sender, RoutedEventArgs e)
        {
            AtState.Exec($"delete from SearchHistory where BaseUri='{_baseUri}'");
            _lvHis.Data = null;
        }

        void OnHisClick(object sender, ItemClickArgs e)
        {
            OnSearch(e.Data.To<SearchHistory>().Content);
        }

        void OnDelHis(object sender, RoutedEventArgs e)
        {
            var his = ((LvItem)((Button)sender).DataContext).Data.To<SearchHistory>();
            AtState.Exec($"delete from SearchHistory where ID={his.ID}");
            LoadHisItems();
        }
        #endregion
    }
}