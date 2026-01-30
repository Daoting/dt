#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-01-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.ListView;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System.Reflection;
using Windows.System;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 为Lv Tv提供筛选功能
    /// </summary>
#if WIN
    [WinRT.GeneratedBindableCustomProperty]
#else
    [Microsoft.UI.Xaml.Data.Bindable]
#endif
    public partial class FilterCfg : DependencyObject
    {
        #region 静态内容
        public static readonly DependencyProperty PlaceholderProperty = DependencyProperty.Register(
            "Placeholder",
            typeof(string),
            typeof(FilterCfg),
            new PropertyMetadata(null, OnPlaceholderChanged));

        public static readonly DependencyProperty IsRealtimeProperty = DependencyProperty.Register(
            "IsRealtime",
            typeof(bool),
            typeof(FilterCfg),
            new PropertyMetadata(false));

        public static readonly DependencyProperty EnablePinYinProperty = DependencyProperty.Register(
            "EnablePinYin",
            typeof(bool),
            typeof(FilterCfg),
            new PropertyMetadata(false, OnPlaceholderChanged));

        public static readonly DependencyProperty FilterColsProperty = DependencyProperty.Register(
            "FilterCols",
            typeof(string),
            typeof(FilterCfg),
            new PropertyMetadata(null));

        static void OnPlaceholderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((FilterCfg)d).UpdatePlaceholder();
        }
        #endregion

        #region 变量
        TextBox _tb;
        Grid _root;
        bool _pinyin;
        IFilterHost _host;
        DispatcherTimer _filterTimer;
        Menu _menu;
        string _curTxt;
        // UI显示是所有列
        Table _allCols;
        // 最终参与筛选的列，Row为id，object为PropertyInfo
        readonly List<object> _filterCols = new List<object>();
        // 第一行数据
        object _firstRow;
        #endregion

        #region 属性
        /// <summary>
        /// 获取设置筛选框水印提示
        /// </summary>
        public string Placeholder
        {
            get { return (string)GetValue(PlaceholderProperty); }
            set { SetValue(PlaceholderProperty, value); }
        }

        /// <summary>
        /// 获取设置文本内容变化时是否执行查询，默认false
        /// </summary>
        public bool IsRealtime
        {
            get { return (bool)GetValue(IsRealtimeProperty); }
            set { SetValue(IsRealtimeProperty, value); }
        }

        /// <summary>
        /// 是否启用拼音简码过滤，默认false
        /// </summary>
        public bool EnablePinYin
        {
            get { return (bool)GetValue(EnablePinYinProperty); }
            set { SetValue(EnablePinYinProperty, value); }
        }

        /// <summary>
        /// 过滤的列名，多列用逗号隔开，null或空时过滤所用列
        /// </summary>
        public string FilterCols
        {
            get { return (string)GetValue(FilterColsProperty); }
            set { SetValue(FilterColsProperty, value); }
        }

        /// <summary>
        /// 外部自定义筛选
        /// </summary>
        public Func<object, string, bool> MyFilter { get; set; }

        internal Table AllCols => _allCols;

        internal IFilterHost Host => _host;

        internal object FirstRow => _firstRow ?? _host.GetFirstRowData();

        internal List<object> LastCols => _filterCols;

        internal bool NeedFiltering =>
            _root != null
            && VisualTreeHelper.GetParent(_root) != null
            && (_tb?.Text.Trim() != "" || MyFilter != null);
        #endregion

        #region 过滤
        /// <summary>
        /// 过滤前确定要过滤的列
        /// </summary>
        /// <param name="p_obj"></param>
        internal void PrepareFilter(object p_obj)
        {
            // _allCols在加载 UI 后有效
            if (_root != null)
            {
                // 节省多余的确定列
                if (_firstRow == p_obj)
                    return;

                _firstRow = p_obj;
            }

            _filterCols.Clear();
            List<string> uiCols = null;
            if (_allCols != null)
            {
                uiCols = (from r in _allCols
                          where r.Bool("ischecked")
                          select r.Str("id")).ToList();
            }
            if (uiCols == null || uiCols.Count == 0)
                return;

            if (p_obj is Row row)
            {
                foreach (var c in uiCols)
                {
                    if (row.Contains(c))
                        _filterCols.Add(c);
                }
            }
            else if (p_obj is string str)
            {

            }
            else if (p_obj.GetType().IsValueType)
            {

            }
            else
            {
                var tp = p_obj.GetType();
                foreach (var c in uiCols)
                {
                    var prop = tp.GetProperty(c, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.IgnoreCase);
                    if (prop != null)
                        _filterCols.Add(prop);
                }
            }
        }

        /// <summary>
        /// 默认过滤方法
        /// </summary>
        /// <param name="p_obj"></param>
        /// <returns></returns>
        internal bool DoFilter(object p_obj)
        {
            if (_tb == null)
                return true;

            if (MyFilter != null)
                return MyFilter(p_obj, _curTxt);

            if (_curTxt == "")
                return true;
            return DoDefaultFilter(p_obj);
        }

        /// <summary>
        /// 默认过滤方法
        /// </summary>
        /// <param name="p_obj"></param>
        /// <returns></returns>
        bool DoDefaultFilter(object p_obj)
        {
            // 启用拼音简码
            _pinyin = EnablePinYin;
            if (_pinyin)
            {
                foreach (char vChar in _curTxt)
                {
                    if ((vChar >= 'a' && vChar <= 'z')
                        || (vChar >= 'A' && vChar <= 'Z')
                        || (vChar >= '0' && vChar <= '9'))
                    {
                        continue;
                    }
                    _pinyin = false;
                    break;
                }
            }

            if (p_obj is Row row)
            {
                if (_filterCols.Count > 0)
                {
                    foreach (var id in _filterCols)
                    {
                        if (IsContains(row.Cells[(string)id].Val))
                            return true;
                    }
                }
            }
            else if (p_obj is string str)
            {
                if (IsContains(str))
                    return true;
            }
            else if (p_obj.GetType().IsValueType)
            {
                if (IsContains(p_obj.ToString()))
                    return true;
            }
            else if (_filterCols.Count > 0)
            {
                foreach (var pi in _filterCols.OfType<PropertyInfo>())
                {
                    var val = pi.GetValue(p_obj, null);
                    if (IsContains(val))
                        return true;
                }
            }
            
            return false;
        }

        bool IsContains(object p_val)
        {
            if (p_val == null || string.IsNullOrEmpty(p_val.ToString()))
                return false;

            if (p_val.ToString().Contains(_curTxt, StringComparison.OrdinalIgnoreCase))
                return true;

            if (_pinyin && p_val.GetType() == typeof(string))
            {
                var py = Kit.GetPinYin(p_val.ToString());
                return py.Contains(_curTxt.ToLower());
            }
            return false;
        }
        #endregion

        #region UI
        internal FrameworkElement LoadUI(IFilterHost p_host)
        {
            _host = p_host;

            if (_root == null)
            {
                CreateUI();
                _allCols = _host.GetAllCols(FilterCols);
            }

            UpdatePlaceholder();
            if (_host is Panel pnl)
                pnl.Children.Add(_root);
            return _root;
        }

        internal void RemoveUI()
        {
            if (_filterTimer != null)
            {
                _filterTimer.Stop();
                _filterTimer.Tick -= OnTimerTick;
                _filterTimer = null;
            }
        }

        void CreateUI()
        {
            _root = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = GridLength.Auto },
                    new ColumnDefinition { Width = GridLength.Auto },
                },
                BorderThickness = new Thickness(1),
                BorderBrush = Res.浅灰2,
                Margin = new Thickness(-1, -1, -1, 0),
                Background = Res.浅灰1,
            };

            CreateTextBox();
            _root.Children.Add(_tb);

            var btn = new Button { Content = "\uE004", Style = Res.字符按钮 };
            btn.Click += OnSetting;
            Grid.SetColumn(btn, 1);
            _root.Children.Add(btn);

            btn = new Button { Content = "\uE009", Style = Res.字符按钮 };
            btn.Click += OnClose;
            Grid.SetColumn(btn, 2);
            _root.Children.Add(btn);
        }

        void CreateTextBox()
        {
            _tb = new TextBox
            {
                BorderThickness = new Thickness(0),
                Background = Res.WhiteBrush,
                Padding = new Thickness(10, 9, 10, 8),
            };
            _tb.Loaded += (s, e) =>
            {
                // 确保每次移除又添加后能继续输入，非虚拟行或有分组时数据变化会清除所以元素
                _tb.Focus(FocusState.Programmatic);
                _tb.Select(_tb.Text.Length, 0);
            };

            _tb.TextChanged += OnTextChanged;

            // 使用KeyUp原因：
            // 1. android只支持KeyUp，只在enter时触发！
            // 2. 避免和Lv回车触发ItemClick事件冲突
#if ANDROID || IOS
                _tb.KeyUp += OnTextKeyUp;
#else
            _tb.KeyUp += OnTextKeyUp;
            // 直接处理Lv快捷键，否则被ScrollView处理！如：上下移动键
            _tb.KeyDown += OnTextKeyDown;
#endif
        }

        void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            _curTxt = _tb.Text.Trim();
            if (IsRealtime)
                StartTimer();
        }

        void StartTimer()
        {
            if (_filterTimer == null)
            {
                _filterTimer = new DispatcherTimer();
                _filterTimer.Interval = TimeSpan.FromMilliseconds(300);
                _filterTimer.Tick += OnTimerTick;
            }
            _filterTimer.Start();
        }

        void OnTimerTick(object sender, object e)
        {
            _filterTimer.Stop();
            _host.Refresh();
        }

        void OnTextKeyUp(object sender, KeyRoutedEventArgs e)
        {
            // 使用KeyUp原因：
            // 1. android只支持KeyUp，只在enter时触发！
            // 2. 避免和Lv回车触发ItemClick事件冲突
            if (e.Key == VirtualKey.Enter)
            {
                e.Handled = true;
                _host.Refresh();
            }
        }

        void OnTextKeyDown(object sender, KeyRoutedEventArgs e)
        {
            // 直接处理快捷键，否则被ScrollView处理！
            _host.OnKeyDown(sender, e);
        }

        void UpdatePlaceholder()
        {
            if (_tb == null)
                return;

            if (!string.IsNullOrEmpty(Placeholder))
                _tb.PlaceholderText = Placeholder;
            else if (EnablePinYin)
                _tb.PlaceholderText = "请输入筛选内容或拼音简码";
            else
                _tb.PlaceholderText = "请输入筛选内容";
        }

        void OnClose(object sender, RoutedEventArgs e)
        {
            _host.CloseFilterUI();
            _tb.Text = "";
            _curTxt = "";
        }

        void OnSetting(object sender, RoutedEventArgs e)
        {
            if (_menu == null)
            {
                _menu = new Menu { IsContextMenu = true, Placement = MenuPosition.BottomLeft };
                var mi = new Mi { ID = "启用拼音简码", IsCheckable = true, IsChecked = EnablePinYin };
                mi.Click += a => EnablePinYin = a.IsChecked;
                _menu.Items.Add(mi);

                mi = new Mi { ID = "启用实时筛选", IsCheckable = true, IsChecked = IsRealtime };
                mi.Click += a => IsRealtime = a.IsChecked;
                _menu.Items.Add(mi);

                mi = new Mi { ID = "设置筛选列", Icon = Icons.大图标 };
                mi.Click += a => OnSelectCols();
                _menu.Items.Add(mi);

                mi = new Mi { ID = "组合筛选", Icon = Icons.表格 };
                mi.Click += a => OnCombinedFilter();
                _menu.Items.Add(mi);
            }

            _ = _menu.OpenContextMenu((Button)sender);
        }

        void OnCombinedFilter()
        {
            _tb.Text = "";
            _curTxt = "";
            new CombinedFilterDlg().ShowDlg(this);
        }

        void OnSelectCols()
        {
            new FilterColsDlg().ShowDlg(_allCols);
            _firstRow = null;
        }
        #endregion
    }
}
