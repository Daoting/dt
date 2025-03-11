#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-10-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.FormView;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using Windows.Foundation;
using Windows.System;
using Windows.UI;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 表单控件
    /// </summary>
    [ContentProperty(Name = nameof(Items))]
    public partial class Fv : DtControl
    {
        #region 静态内容
        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(
            "IsReadOnly",
            typeof(bool),
            typeof(Fv),
            new PropertyMetadata(false, OnIsReadOnlyChanged));

        public readonly static DependencyProperty AutoCreateCellProperty = DependencyProperty.Register(
            "AutoCreateCell",
            typeof(bool),
            typeof(Fv),
            new PropertyMetadata(false));

        public readonly static DependencyProperty MaxColCountProperty = DependencyProperty.Register(
            "MaxColCount",
            typeof(int),
            typeof(Fv),
            new PropertyMetadata(4, OnMaxColCountChanged));

        public readonly static DependencyProperty IsDesignModeProperty = DependencyProperty.Register(
            "IsDesignMode",
            typeof(bool),
            typeof(Fv),
            new PropertyMetadata(false, OnIsDesignModeChanged));
        
        public readonly static DependencyProperty DataViewProperty = DependencyProperty.Register(
            "DataView",
            typeof(ObjectView),
            typeof(Fv),
            new PropertyMetadata(null));

        static void OnIsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Fv fv = (Fv)d;
            if (fv._isLoaded)
            {
                foreach (FvCell cell in fv.IDCells)
                {
                    cell.ApplyIsReadOnly();
                }
            }
        }

        static void OnMaxColCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Fv fv = (Fv)d;
            if (fv._isLoaded)
                fv._panel.InvalidateMeasure();
        }

        static void OnIsDesignModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Fv fv = (Fv)d;
            if (fv._isLoaded && !(bool)e.NewValue)
            {

            }
        }
        #endregion

        #region 成员变量
        protected readonly FormPanel _panel;
        ScrollViewer _scroll;
        protected bool _isLoaded;
        FvUndoCmd _cmdUndo;
        #endregion

        #region 构造方法
        public Fv()
        {
            DefaultStyleKey = typeof(Fv);
            _panel = new FormPanel(this);
        }
        #endregion

        #region 事件
        /// <summary>
        /// 切换数据源事件
        /// </summary>
        public event Action<object> DataChanged;

        /// <summary>
        /// 数据源的列值/属性值修改后事件
        /// </summary>
        public event Action<ICell> Changed;

        /// <summary>
        /// 数据源修改状态变化事件
        /// </summary>
        public event Action<bool> Dirty;

        /// <summary>
        /// 单元格点击事件，参数为单元格
        /// </summary>
        public event Action<object> CellClick;

        /// <summary>
        /// 保存事件
        /// </summary>
        public event Action Save;

        /// <summary>
        /// 新增事件
        /// </summary>
        public event Action Create;

        /// <summary>
        /// 删除事件
        /// </summary>
#if IOS
        new
#endif
        public event Action Delete;

        /// <summary>
        /// 在末尾单元格按回车事件
        /// </summary>
        public event Action LastCellEnter;
        #endregion

        #region 属性
        /// <summary>
        /// 获取设置表单是否只读
        /// </summary>
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        /// <summary>
        /// 获取设置是否根据数据源自动生成格，默认false
        /// </summary>
        public bool AutoCreateCell
        {
            get { return (bool)GetValue(AutoCreateCellProperty); }
            set { SetValue(AutoCreateCellProperty, value); }
        }

        /// <summary>
        /// 获取设置布局时的最大列数，默认最多4列
        /// </summary>
        public int MaxColCount
        {
            get { return (int)GetValue(MaxColCountProperty); }
            set { SetValue(MaxColCountProperty, value); }
        }

        /// <summary>
        /// 获取设置是否为设计模式，默认false，设计模式时点击格显示选中状态、可拖拽格调序
        /// </summary>
        public bool IsDesignMode
        {
            get { return (bool)GetValue(IsDesignModeProperty); }
            set { SetValue(IsDesignModeProperty, value); }
        }
        
        /// <summary>
        /// 获取撤消命令
        /// </summary>
        public FvUndoCmd CmdUndo
        {
            get
            {
                if (_cmdUndo == null)
                    _cmdUndo = new FvUndoCmd(this);
                return _cmdUndo;
            }
        }

        /// <summary>
        /// 普通数据源对象的视图包装对象
        /// </summary>
        internal ObjectView DataView
        {
            get { return (ObjectView)GetValue(DataViewProperty); }
            set { SetValue(DataViewProperty, value); }
        }
        #endregion

        #region 显示/隐藏格
        /// <summary>
        /// 隐藏名称列表中的格
        /// </summary>
        /// <param name="p_names"></param>
        public void Hide(params string[] p_names)
        {
            foreach (string name in p_names)
            {
                if (string.IsNullOrEmpty(name))
                    continue;

                var item = Items[name];
                if (item != null)
                    item.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 除显示名称列表中的格外，其它都隐藏，列表空时隐藏所有
        /// </summary>
        /// <param name="p_names">无值时隐藏所有</param>
        public void HideExcept(params string[] p_names)
        {
            foreach (var item in Items.OfType<UIElement>())
            {
                if (item is FvCell cell && p_names.Contains(cell.ID))
                    item.Visibility = Visibility.Visible;
                else if (item.Visibility == Visibility.Visible)
                    item.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 显示名称列表中的格
        /// </summary>
        /// <param name="p_names"></param>
        public void Show(params string[] p_names)
        {
            foreach (string name in p_names)
            {
                if (string.IsNullOrEmpty(name))
                    continue;

                var item = Items[name];
                if (item != null)
                    item.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// 除隐藏名称列表中的格外，其它都显示，列表空时显示所有
        /// </summary>
        /// <param name="p_names">无值时显示所有</param>
        public void ShowExcept(params string[] p_names)
        {
            foreach (var item in Items.OfType<UIElement>())
            {
                if (item is FvCell cell && p_names.Contains(cell.ID))
                    item.Visibility = Visibility.Collapsed;
                else if (item.Visibility == Visibility.Collapsed)
                    item.Visibility = Visibility.Visible;
            }
        }
        #endregion

        #region 格可用/不可用
        /// <summary>
        /// 设置名称列表中的格为可用
        /// </summary>
        /// <param name="p_names"></param>
        public void Enable(params string[] p_names)
        {
            foreach (string name in p_names)
            {
                if (string.IsNullOrEmpty(name))
                    continue;

                var item = Items[name];
                if (item != null && !item.IsEnabled)
                    item.IsEnabled = true;
            }
        }

        /// <summary>
        /// 除名称列表中的格外，其它都可用，列表空时所有可用
        /// </summary>
        public void EnableExcept(params string[] p_names)
        {
            foreach (var item in Items)
            {
                if (item is FvCell cell && p_names.Contains(cell.ID))
                    cell.IsEnabled = false;
                else if (item is Control con)
                    con.IsEnabled = true;
            }
        }

        /// <summary>
        /// 设置名称列表中的格为不可用
        /// </summary>
        /// <param name="p_names"></param>
        public void Disable(params string[] p_names)
        {
            foreach (string name in p_names)
            {
                if (string.IsNullOrEmpty(name))
                    continue;

                var item = Items[name];
                if (item != null && item.IsEnabled)
                    item.IsEnabled = false;
            }
        }

        /// <summary>
        /// 除名称列表中的格外，其它都不可用，列表空时所有不可用
        /// </summary>
        public void DisableExcept(params string[] p_names)
        {
            foreach (var item in Items)
            {
                if (item is FvCell cell && p_names.Contains(cell.ID))
                    cell.IsEnabled = true;
                else if (item is Control con)
                    con.IsEnabled = false;
            }
        }
        #endregion

        #region 外部方法
        /// <summary>
        /// 根据类型生成格
        /// </summary>
        /// <param name="p_type"></param>
        /// <param name="p_id"></param>
        /// <returns></returns>
        public static FvCell CreateCell(Type p_type, string p_id)
        {
            if (p_type == typeof(string))
                return new CText { ID = p_id };

            if (p_type == typeof(int) || p_type == typeof(long) || p_type == typeof(short))
                return new CNum { ID = p_id, IsInteger = true };

            if (p_type == typeof(double) || p_type == typeof(float))
                return new CNum { ID = p_id };

            if (p_type == typeof(bool))
                return new CBool { ID = p_id };

            if (p_type == typeof(DateTime))
                return new CDate { ID = p_id };

            if (p_type == typeof(Icons))
                return new CIcon { ID = p_id };

            if (p_type.IsEnum)
                return new CList { ID = p_id };

            if (p_type == typeof(SolidColorBrush) || p_type == typeof(Color))
                return new CColor { ID = p_id };

            return new CText { ID = p_id };
        }

        /// <summary>
        /// 增删单元格时延迟刷新视图，等同调用_fv.Items.Defer()
        /// using (_fv.Defer())
        /// {
        ///     _fv.Items.Clear();
        ///     _fv.Items.Add(new CBool());
        /// }
        /// </summary>
        /// <returns></returns>
        public IDisposable Defer()
        {
            return Items.Defer();
        }

        /// <summary>
        /// 获取按指定列数布局时占用的总高度
        /// </summary>
        /// <param name="p_colCount">列数，范围 1~4</param>
        /// <returns></returns>
        public double GetTotalHeight(int p_colCount)
        {
            return _panel.GetTotalHeight(p_colCount);
        }
        #endregion

        #region 重写方法
        protected override void OnLoadTemplate()
        {
            var root = (Border)GetTemplateChild("Border");

            // win模式查询范围限制在Tabs内，phone模式限制在Tab内
            _scroll = this.FindParentInWin<ScrollViewer>();
            if (_scroll == null)
            {
                // 内部滚动栏
                _scroll = new ScrollViewer();
                _scroll.Content = _panel;
                root.Child = _scroll;
            }
            else
            {
                // 外部滚动栏
                root.Child = _panel;
            }
            _scroll.HorizontalScrollMode = ScrollMode.Disabled;
            _scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            _scroll.VerticalScrollMode = ScrollMode.Auto;
            _scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;

#if WIN
            // 在 StackPanel 内时无法画出底线
            if (VisualTreeHelper.GetParent(this) is StackPanel sp
                && ReadLocalValue(BorderThicknessProperty) == DependencyProperty.UnsetValue)
            {
                BorderThickness = new Thickness(0, 0, 0, 1);
                BorderBrush = Res.浅灰2;
            }
#endif

            LoadAllItems();
            Items.ItemsChanged += OnItemsChanged;

            _isLoaded = true;

            // 初次加载时首次执行切换数据源操作，避免在不可见Tab页内时异常
            if (Data != null)
                OnDataChanged();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            // 准确获取高度
            if (!double.IsInfinity(availableSize.Width) && !double.IsInfinity(availableSize.Height))
            {
                // 外部无ScrollViewer StackPanel的情况
                _panel.SetMaxSize(availableSize);
            }
            else
            {
                // 和Lv相似，参见win.xaml：win模式在Tabs定义，phone模式在Tab定义
                var pre = _scroll.FindParentInWin<SizedPresenter>();
                if (pre != null)
                {
                    _panel.SetMaxSize(pre.AvailableSize);
                }
                else
                {
                    // 无有效大小时以窗口大小为准
                    double width = double.IsInfinity(availableSize.Width) ? Kit.ViewWidth : availableSize.Width;
                    double height = double.IsInfinity(availableSize.Height) ? Kit.ViewHeight : availableSize.Height;
                    _panel.SetMaxSize(new Size(width, height));
                }
            }
            return base.MeasureOverride(availableSize);
        }

        protected override void OnKeyUp(KeyRoutedEventArgs e)
        {
            base.OnKeyUp(e);

            if (InputKit.IsCtrlPressed && !e.Handled)
            {
                if (e.Key == VirtualKey.S)
                {
                    // Ctrl + S 保存
                    OnSave();
                }
                else if (e.Key == VirtualKey.N)
                {
                    // Ctrl + N 新建
                    OnCreate();
                }
                else if (e.Key == VirtualKey.Delete)
                {
                    // Ctrl + Delete 删除
                    OnDelete();
                }
            }
        }
        #endregion

        #region 触发事件
        /// <summary>
        /// 触发数据源切换事件
        /// </summary>
        void OnFvDataChanged()
        {
            DataChanged?.Invoke(Data);
        }

        /// <summary>
        /// 触发单元格数据修改事件
        /// </summary>
        /// <param name="e"></param>
        void OnValueChanged(ICell e)
        {
            Changed?.Invoke(e);
        }

        /// <summary>
        /// 触发数据源修改状态变化事件
        /// </summary>
        void OnDirty()
        {
            Dirty?.Invoke(IsDirty);
        }

        /// <summary>
        /// 触发内部单元格点击事件
        /// </summary>
        /// <param name="p_cell"></param>
        internal void OnCellClick(object p_cell)
        {
            CellClick?.Invoke(p_cell);
        }

        internal void OnSave()
        {
            if (IsDirty && Save != null)
                Save.Invoke();
        }

        void OnCreate()
        {
            Create?.Invoke();
        }

        void OnDelete()
        {
            Delete?.Invoke();
        }
        #endregion
    }
}