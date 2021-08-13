#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-10-29 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.FormView;
using Dt.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 列表选择格
    /// 数据源优先级：
    /// 1. 外部直接设置Data
    /// 2. 基础选项
    /// 3. Sql查询
    /// 4. Sql语句键值
    /// 5. 外部设置的Enum数据
    /// 6. 格类型为枚举时，自动生成Enum数据
    /// 7. 外部(xaml中)定义的对象列表
    /// 数据源为Table时，确保存在name列；
    /// 为普通对象时，直接将对象赋值！
    /// </summary>
    [ContentProperty(Name = nameof(View))]
    public partial class CList : FvCell
    {
        #region 静态内容
        public readonly static DependencyProperty OptionProperty = DependencyProperty.Register(
            "Option",
            typeof(string),
            typeof(CList),
            new PropertyMetadata(null, OnClearData));

        public readonly static DependencyProperty SqlProperty = DependencyProperty.Register(
            "Sql",
            typeof(string),
            typeof(CList),
            new PropertyMetadata(null, OnClearData));

        public readonly static DependencyProperty SqlKeyProperty = DependencyProperty.Register(
            "SqlKey",
            typeof(string),
            typeof(CList),
            new PropertyMetadata(null, OnClearData));

        public readonly static DependencyProperty SqlKeyFilterProperty = DependencyProperty.Register(
            "SqlKeyFilter",
            typeof(string),
            typeof(CList),
            new PropertyMetadata(null, OnClearData));

        public readonly static DependencyProperty EnumProperty = DependencyProperty.Register(
            "Enum",
            typeof(string),
            typeof(CList),
            new PropertyMetadata(null, OnClearData));

        public readonly static DependencyProperty SrcIDProperty = DependencyProperty.Register(
            "SrcID",
            typeof(string),
            typeof(CList),
            new PropertyMetadata(null));

        public readonly static DependencyProperty TgtIDProperty = DependencyProperty.Register(
            "TgtID",
            typeof(string),
            typeof(CList),
            new PropertyMetadata(null));

        public readonly static DependencyProperty RefreshDataProperty = DependencyProperty.Register(
            "RefreshData",
            typeof(bool),
            typeof(CList),
            new PropertyMetadata(false));

        public readonly static DependencyProperty IsEditableProperty = DependencyProperty.Register(
            "IsEditable",
            typeof(bool),
            typeof(CList),
            new PropertyMetadata(false, OnIsEditableChanged));

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value",
            typeof(object),
            typeof(CList),
            new PropertyMetadata(null));

        static void OnClearData(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CList ls = (CList)d;
            if (ls._dlg != null)
                ls._lv.ClearValue(Lv.DataProperty);
        }

        static void OnIsEditableChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ((CList)d).LoadContent();
        }
        #endregion

        #region 成员变量
        readonly Lv _lv;
        Grid _grid;
        Nl<object> _items;
        ListDlg _dlg;
        #endregion

        #region 构造方法
        public CList()
        {
            DefaultStyleKey = typeof(CList);
            _lv = new Lv();
            ValConverter = new ListValConverter();
        }
        #endregion

        #region 事件
        /// <summary>
        /// 外部自定义数据源事件，支持异步
        /// </summary>
        public event EventHandler<AsyncEventArgs> LoadData;

        /// <summary>
        /// 选择后事件，Selected与uno中重名，xaml中附加事件报错！
        /// </summary>
        public event EventHandler<object> AfterSelect;
        #endregion

        #region Lv属性
        /// <summary>
        /// 获取设置数据源对象，Table或集合对象
        /// </summary>
        public INotifyList Data
        {
            get { return _lv.Data; }
            set { _lv.Data = value; }
        }

        /// <summary>
        /// 获取设置行视图，DataTemplate、DataTemplateSelector、Cols列定义 或 IRowView
        /// </summary>
        public object View
        {
            get { return _lv.View; }
            set { _lv.View = value; }
        }

        /// <summary>
        /// 获取设置视图类型：列表、表格、磁贴，默认List
        /// </summary>
        public ViewMode ViewMode
        {
            get { return _lv.ViewMode; }
            set { _lv.ViewMode = value; }
        }

        /// <summary>
        /// 获取设置外部自定义单元格的类型，方法名和Dot或Col的ID相同，SetStyle方法控制行样式
        /// </summary>
        public Type CellEx
        {
            get { return _lv.CellEx; }
            set { _lv.CellEx = value; }
        }

        /// <summary>
        /// 获取设置Phone模式下的视图类型，null时Win,Phone两模式统一采用ViewMode，默认null
        /// </summary>
        public ViewMode? PhoneViewMode
        {
            get { return _lv.PhoneViewMode; }
            set { _lv.PhoneViewMode = value; }
        }

        /// <summary>
        /// 获取设置选择模式，默认Single，只第一次设置有效！
        /// </summary>
        [CellParam("选择模式")]
        public SelectionMode SelectionMode
        {
            get { return _lv.SelectionMode; }
            set { _lv.SelectionMode = value; }
        }
        #endregion

        #region 属性
        /// <summary>
        /// 获取设置基础选项
        /// </summary>
        [CellParam("基础选项")]
        public string Option
        {
            get { return (string)GetValue(OptionProperty); }
            set { SetValue(OptionProperty, value); }
        }

        /// <summary>
        /// 获取设置数据查询语句，sql必须含有服务名前缀，如：
        /// Cm:select * from dt_log
        /// local:select * from letter
        /// </summary>
        [CellParam("数据查询语句")]
        public string Sql
        {
            get { return (string)GetValue(SqlProperty); }
            set { SetValue(SqlProperty, value); }
        }

        /// <summary>
        /// 获取设置数据查询语句键值，键值必须含有服务名前缀
        /// </summary>
        [CellParam("Sql语句键值")]
        public string SqlKey
        {
            get { return (string)GetValue(SqlKeyProperty); }
            set { SetValue(SqlKeyProperty, value); }
        }

        /// <summary>
        /// 获取设置使用sql键值查询时的动态过滤条件
        /// </summary>
        public string SqlKeyFilter
        {
            get { return (string)GetValue(SqlKeyFilterProperty); }
            set { SetValue(SqlKeyFilterProperty, value); }
        }

        /// <summary>
        /// 获取设置枚举格式串；
        /// 格式：枚举名(包含命名空间),程序集；
        /// 例：Dt.Base.CtType,Dt.Base
        /// </summary>
        [CellParam("枚举格式串")]
        public string Enum
        {
            get { return (string)GetValue(EnumProperty); }
            set { SetValue(EnumProperty, value); }
        }

        /// <summary>
        /// 外部(xaml中)定义的对象列表
        /// </summary>
        public Nl<object> Items
        {
            get
            {
                if (_items == null)
                    _items = new Nl<object>();
                return _items;
            }
        }

        /// <summary>
        /// 获取设置源属性列表，用'#'隔开
        /// </summary>
        [CellParam("源属性列表")]
        public string SrcID
        {
            get { return (string)GetValue(SrcIDProperty); }
            set { SetValue(SrcIDProperty, value); }
        }

        /// <summary>
        /// 获取设置目标属性列表，用'#'隔开
        /// </summary>
        [CellParam("目标属性列表")]
        public string TgtID
        {
            get { return (string)GetValue(TgtIDProperty); }
            set { SetValue(TgtIDProperty, value); }
        }

        /// <summary>
        /// 获取设置是否动态加载数据源，默认false
        /// true表示每次显示对话框时都加载数据源，false表示只第一次加载
        /// </summary>
        [CellParam("动态加载数据源")]
        public bool RefreshData
        {
            get { return (bool)GetValue(RefreshDataProperty); }
            set { SetValue(RefreshDataProperty, value); }
        }

        /// <summary>
        /// 获取设置是否可编辑
        /// </summary>
        [CellParam("是否可编辑")]
        public bool IsEditable
        {
            get { return (bool)GetValue(IsEditableProperty); }
            set { SetValue(IsEditableProperty, value); }
        }

        /// <summary>
        /// 获取设置当前值
        /// </summary>
        public object Value
        {
            get { return GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// 获取Lv对象
        /// </summary>
        public Lv Lv
        {
            get { return _lv; }
        }
        #endregion

        #region 重写方法
        protected override void OnApplyCellTemplate()
        {
            _grid = (Grid)GetTemplateChild("Grid");
#if UWP
            // TextBlock可复制
            _grid.AddHandler(TappedEvent, new TappedEventHandler(OnShowDlg), true);
#else
            _grid.Tapped += OnShowDlg;
#endif
            LoadContent();
        }

        protected override void SetValBinding()
        {
            SetBinding(ValueProperty, ValBinding);
        }

        protected override void OnReadOnlyChanged()
        {
            LoadContent();
        }

        protected override bool SetFocus()
        {
            if (_grid != null)
                OnShowDlg(null, null);
            return true;
        }
        #endregion

        #region 内部方法
        void OnShowDlg(object sender, TappedRoutedEventArgs e)
        {
            if (ReadOnlyBinding)
                return;

            if (_dlg != null && _dlg.IsOpened)
            {
                _dlg.BringToTop();
                return;
            }

            if (_dlg == null)
            {
                if (Kit.IsPhoneUI)
                {
                    _dlg = new ListDlg(this);
                }
                else
                {
                    _dlg = new ListDlg(this)
                    {
                        WinPlacement = DlgPlacement.TargetBottomLeft,
                        PlacementTarget = _grid,
                        ClipElement = _grid,
                        HideTitleBar = (_lv.SelectionMode != SelectionMode.Multiple),
                        MaxHeight = 300,
                        Width = _grid.ActualWidth,
                    };
                }
            }
            _dlg.ShowDlg();
        }

        /// <summary>
        /// 根据是否可编辑动态加载控件
        /// </summary>
        void LoadContent()
        {
            if (_grid == null)
                return;

            if (_grid.Children.Count == 2)
                _grid.Children.RemoveAt(1);

            if (IsEditable && !ReadOnlyBinding)
            {
                TextBox tb = new TextBox { Style = Res.FvTextBox };
                Binding bind = new Binding
                {
                    Path = new PropertyPath("Value"),
                    Converter = new ListTextConverter(this),
                    Mode = BindingMode.TwoWay,
                    Source = this
                };
                tb.SetBinding(TextBox.TextProperty, bind);
                _grid.Children.Add(tb);
            }
            else
            {
                TextBlock tb = new TextBlock { IsTextSelectionEnabled = true, Margin = new Thickness(10, 0, 0, 0), VerticalAlignment = VerticalAlignment.Center };
                Binding bind = new Binding
                {
                    Path = new PropertyPath("Value"),
                    Converter = new ListTextConverter(this),
                    Source = this
                };
                tb.SetBinding(TextBlock.TextProperty, bind);
                _grid.Children.Add(tb);
            }
        }

        /// <summary>
        /// 触发外部自定义数据源事件
        /// </summary>
        /// <returns></returns>
        internal async Task<bool> OnLoadData()
        {
            if (LoadData != null)
            {
                var args = new AsyncEventArgs();
                LoadData(this, args);
                await args.EnsureAllCompleted();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 触发选择后事件
        /// </summary>
        /// <param name="p_selectedItem"></param>
        internal void OnSelected(object p_selectedItem)
        {
            AfterSelect?.Invoke(this, p_selectedItem);
        }
        #endregion
    }
}