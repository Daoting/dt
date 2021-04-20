#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-11-22 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.FormView;
using Dt.Core;
using System;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 查询面板，暂时只支持用在Tab内！
    /// 面板内的所有固定按钮、搜索框、搜索历史统一触发Search事件
    /// 固定按钮Click事件传递格式为 "#按钮名称"，#前缀用于区别普通搜索
    /// 搜索历史管理
    /// </summary>
    public partial class SearchFv : Fv
    {
        public static readonly DependencyProperty PlaceholderProperty = DependencyProperty.Register(
            "Placeholder",
            typeof(string),
            typeof(SearchFv),
            new PropertyMetadata("搜索"));

        int _hisStart;
        string _lastText;
        string _baseUri;

        #region 构造方法
        public SearchFv()
        {
            DefaultStyleKey = typeof(SearchFv);
            Loaded += OnLoaded;
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
            get { return (string)GetValue(PlaceholderProperty); }
            set { SetValue(PlaceholderProperty, value); }
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var btn = (Button)GetTemplateChild("CloseButton");
            if (AtSys.IsPhoneUI)
                btn.Click += OnCloseClick;
            else
                btn.Visibility = Visibility.Collapsed;

            var tb = (TextBox)GetTemplateChild("SearchBox");
            // android只支持KeyUp，只在enter时触发！
            tb.KeyUp += OnTextKeyUp;

            if (!AtSys.IsPhoneUI)
            {
                // 显示上边框
                _panel.Margin = new Thickness(-1, 0, 0, 0);
            }
        }

        void OnFixedBtnClick(object sender, RoutedEventArgs e)
        {
            string txt = ((Button)sender).Content as string;
            if (!string.IsNullOrEmpty(txt))
                OnSearch("#" + txt);
        }

        void OnTextKeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                e.Handled = true;
                string txt = ((TextBox)sender).Text.Trim();
                if (txt != "")
                    OnSearch(txt);
            }
        }

        void OnCloseClick(object sender, RoutedEventArgs e)
        {
            OnSearch(null);
        }

        void OnSearch(string p_text)
        {
            Search?.Invoke(this, p_text);

            if (!string.IsNullOrEmpty(p_text)
                && !p_text.StartsWith("#")
                && _lastText != p_text)
            {
                SaveCookie(p_text);
            }
        }

        async void SaveCookie(string p_text)
        {
            _lastText = p_text;
            // 删除重复
            AtState.Exec($"delete from SearchFvHis where BaseUri='{_baseUri}' and Content='{p_text}'");

            SearchFvHis his = new SearchFvHis(BaseUri: _baseUri, Content: p_text);
            await AtState.Save(his, false);

            using (Items.Defer())
            {
                RemoveAllHis();
                LoadHisItems();
            }
        }

        void OnClearHis(object sender, RoutedEventArgs e)
        {
            using (Items.Defer())
            {
                RemoveAllHis();
            }
            AtState.Exec($"delete from SearchFvHis where BaseUri='{_baseUri}'");
        }

        void RemoveAllHis()
        {
            while (Items.Count > _hisStart)
            {
                Items.RemoveAt(_hisStart);
            }
        }

        void OnClickHis(object sender, RoutedEventArgs e)
        {
            SearchFvHis his = (SearchFvHis)((Button)sender).DataContext;
            OnSearch(his.Content);
        }

        void OnDelHis(object sender, RoutedEventArgs e)
        {
            SearchFvHis his = (SearchFvHis)((Button)sender).DataContext;
            if (AtState.Exec($"delete from SearchFvHis where BaseUri='{_baseUri}' and Content='{his.Content}'") == 1)
            {
                for (int i = _hisStart; i < Items.Count; i++)
                {
                    if (((Button)sender).DataContext == Items[i].DataContext)
                    {
                        Items.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;
            if (AtSys.IsPhoneUI)
            {
                var tab = this.FindParentByType<Tab>();
                if (tab != null)
                {
                    // 隐藏标题栏
                    tab.HideTitleBar = true;
                    // 识别不同的查询面板，因uno中BaseUri为空！
                    if (tab.OwnWin != null)
                        _baseUri = tab.OwnWin.GetType().FullName;
                }
            }
            else
            {
                // uno中BaseUri为空！
                _baseUri = BaseUri.AbsolutePath;
            }

            // 附加固定按钮事件
            for (int i = 0; i < Items.Count; i++)
            {
                var item = Items[i];
                if (item is Button b)
                {
                    b.HorizontalAlignment = HorizontalAlignment.Stretch;
                    b.HorizontalContentAlignment = HorizontalAlignment.Left;
                    b.Click += OnFixedBtnClick;
                }
                else
                {
                    foreach (var btn in item.FindChildrenByType<Button>())
                    {
                        btn.Click += OnFixedBtnClick;
                    }
                }
            }

            // 历史搜索
            LoadHisBar();
            LoadHisItems();
        }

        void LoadHisItems()
        {
            var his = AtState.Each<SearchFvHis>($"select * from SearchFvHis where BaseUri='{_baseUri}' order by id desc");
            foreach (var item in his)
            {
                Grid grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                grid.DataContext = item;

                Button btn = new Button { Content = item.Content, HorizontalAlignment = HorizontalAlignment.Stretch, HorizontalContentAlignment = HorizontalAlignment.Left };
                btn.Click += OnClickHis;
                grid.Children.Add(btn);

                btn = new Button { Content = "\uE009", Style = AtRes.字符按钮 };
                btn.Click += OnDelHis;
                Grid.SetColumn(btn, 1);
                grid.Children.Add(btn);

                Items.Add(grid);
            }
        }

        void LoadHisBar()
        {
            CBar bar = new CBar();

            Grid grid = new Grid { Background = AtRes.浅灰背景 };
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            StackPanel sp = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(10, 0, 10, 0) };
            TextBlock tb = new TextBlock { FontFamily = AtRes.IconFont, Text = "\uE02D", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, 4, 0) };
            sp.Children.Add(tb);
            tb = new TextBlock { Text = "搜索历史", TextWrapping = TextWrapping.NoWrap, VerticalAlignment = VerticalAlignment.Center };
            sp.Children.Add(tb);
            grid.Children.Add(sp);

            Button btn = new Button { Content = "\uE00A", Style = AtRes.字符按钮 };
            btn.Click += OnClearHis;
            Grid.SetColumn(btn, 1);
            grid.Children.Add(btn);

            bar.Content = grid;
            Items.Add(bar);
            _hisStart = Items.Count;
        }
    }
}