﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-09-08 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Markup;
using Windows.System;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 模糊搜索面板
    /// 固定搜索项：由外部设置，事件参数格式为 "#按钮名称"，#前缀用于区别普通搜索
    /// 历史搜索项：自动记录历史搜索，可删，事件参数内容为普通文本
    /// 搜索事件：所有固定搜索项、搜索框、历史搜索项统一触发Search事件
    /// 历史搜索项通过所在的Win路径识别，当同一Win中多个搜索时，请确保每个搜索的Title不同，以便正确识别搜索历史！！！
    /// </summary>
    [ContentProperty(Name = nameof(Fixed))]
    public partial class FuzzySearch : Tab
    {
        #region 静态内容
        public readonly static DependencyProperty CookieIDProperty = DependencyProperty.Register(
            "CookieID",
            typeof(string),
            typeof(FuzzySearch),
            null);
        #endregion

        #region 成员变量
        string _lastText;
        string _baseUri;
        TextBox _tb;
        #endregion

        #region 构造方法
        public FuzzySearch()
        {
            InitializeComponent();

            Title = "搜索";
            Icon = Icons.搜索;

            _lvFix.Data = Fixed;
            LoadTopBar();
        }
        #endregion

        /// <summary>
        /// 查询事件
        /// </summary>
        public event Action<string> Search;

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

        /// <summary>
        /// 保存搜索历史时的标识，当放在对话框中无法识别时需要设置
        /// </summary>
        public string CookieID
        {
            get { return (string)GetValue(CookieIDProperty); }
            set { SetValue(CookieIDProperty, value); }
        }

        protected override async void OnFirstLoaded()
        {
#if WIN
            // 在对话框中时为 xxx/FuzzySearch.xaml
            if (!string.IsNullOrEmpty(CookieID))
            {
                _baseUri = CookieID;
            }
            else if (BaseUri != null && !BaseUri.AbsolutePath.EndsWith("/FuzzySearch.xaml"))
            {
                _baseUri = BaseUri.AbsolutePath;
            }
            else if (OwnWin != null)
            {
                _baseUri = OwnWin.BaseUri.AbsolutePath;
            }
#else
            // 识别不同的查询面板，因uno中BaseUri为空！
            if (!string.IsNullOrEmpty(CookieID))
            {
                _baseUri = CookieID;
            }
            else if (OwnWin != null)
            {
                _baseUri = OwnWin.GetType().FullName;
            }
            else if (!BaseUri.AbsolutePath.EndsWith("/FuzzySearch.xaml"))
            {
                _baseUri = BaseUri.AbsolutePath;
            }
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
                await LoadHisItems();
            }

            if (!Kit.IsPhoneUI && OwnDlg != null)
            {
                // 防止对话框中多个查询标签时，其它标签内容高度太小造成切换时高度变化
                OwnDlg.MinHeight = 530;
            }
        }

        async void OnTextKeyUp(object sender, KeyRoutedEventArgs e)
        {
            // android只支持KeyUp，只在enter时触发！
            if (e.Key == VirtualKey.Enter)
            {
                e.Handled = true;
                string txt = ((TextBox)sender).Text.Trim();
                if (txt != "")
                    await OnSearch(txt);
            }
        }

        async void OnFixClick(ItemClickArgs e)
        {
            if (e.Data is string txt)
                await OnSearch("#" + txt);
        }

        async Task OnSearch(string p_text)
        {
            Result = p_text;
            Search?.Invoke(p_text);

            // 非Tab内首页自动后退
            if (!IsHome)
                await Backward();

            if (!string.IsNullOrEmpty(_baseUri)
                && !string.IsNullOrEmpty(p_text)
                && !p_text.StartsWith("#")
                && _lastText != p_text)
            {
                await SaveCookie(p_text);
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
                    Style = Res.浅字符按钮,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Width = 50,
                };
                btn.Click += async (s, e) => await Backward();
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

                // 设置宽高限制，否则在Dlg中水平铺满，高度太小
                _grid.MaxWidth = 455;
                _grid.MinHeight = 400;
            }
        }

        #region 搜索历史
        async Task LoadHisItems()
        {
            _lvHis.Data = await AtState.Query<SearchHistoryX>($"select * from SearchHistory where BaseUri='{_baseUri}' order by id desc limit 10");
        }

        async Task SaveCookie(string p_text)
        {
            _lastText = p_text;
            var sh = await SearchHistoryX.First($"where BaseUri='{_baseUri}' and Content=@Content", new { Content = p_text });
            if (sh == null)
            {
                SearchHistoryX his = await SearchHistoryX.New(BaseUri: _baseUri, Content: p_text);
                await his.Save(false);
                await LoadHisItems();
            }
        }

        async void OnClearHis(object sender, RoutedEventArgs e)
        {
            await AtState.Exec($"delete from SearchHistory where BaseUri='{_baseUri}'");
            _lvHis.Data = null;
        }

        async void OnHisClick(ItemClickArgs e)
        {
            await OnSearch(e.Data.To<SearchHistoryX>().Content);
        }

        async void OnDelHis(object sender, RoutedEventArgs e)
        {
            var his = ((LvItem)((Button)sender).DataContext).Data.To<SearchHistoryX>();
            await his.Delete(false);
            await LoadHisItems();
        }
        #endregion
    }
}