#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-10-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.FormView;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
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
        public readonly static DependencyProperty DataProperty = DependencyProperty.Register(
            "Data",
            typeof(object),
            typeof(Fv),
            new PropertyMetadata(null, OnDataChanged));

        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(
            "IsReadOnly",
            typeof(bool),
            typeof(Fv),
            new PropertyMetadata(false, OnIsReadOnlyChanged));

        public static readonly DependencyProperty IsDirtyProperty = DependencyProperty.Register(
            "IsDirty",
            typeof(bool),
            typeof(Fv),
            new PropertyMetadata(false, OnIsDirtyChanged));

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

        public readonly static DependencyProperty DataViewProperty = DependencyProperty.Register(
            "DataView",
            typeof(ObjectView),
            typeof(Fv),
            new PropertyMetadata(null));

        static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Fv fv = (Fv)d;
            // 未显示前不处理
            if (fv._isLoaded)
            {
                if (e.OldValue != null)
                {
                    // 移除旧数据事件
                    if (e.OldValue is Row row)
                        row.Changed -= fv.OnCellValueChanged;
                    else if (fv.DataView != null)
                        fv.DataView.Changed -= fv.OnPropertyValueChanged;
                }
                fv.OnDataChanged();
            }
        }

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

        static void OnIsDirtyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Fv)d).OnDirty();
        }

        static void OnMaxColCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Fv fv = (Fv)d;
            if (fv._isLoaded)
                fv._panel.InvalidateMeasure();
        }
        #endregion

        #region 成员变量
        protected readonly FormPanel _panel;
        ScrollViewer _scroll;
        bool _isLoaded;
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
        public event EventHandler<object> DataChanged;

        /// <summary>
        /// 数据源的列值/属性值修改后事件
        /// </summary>
        public event EventHandler<ICell> Changed;

        /// <summary>
        /// 数据源修改状态变化事件
        /// </summary>
        public event EventHandler<bool> Dirty;

        /// <summary>
        /// 单元格点击事件
        /// </summary>
        public event EventHandler<FvCell> CellClick;
        #endregion

        #region 属性
        /// <summary>
        /// 获取设置数据源，Row或普通对象
        /// </summary>
        public object Data
        {
            get { return GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        /// <summary>
        /// 获取Row数据源
        /// </summary>
        public Row Row
        {
            get { return GetValue(DataProperty) as Row; }
        }

        /// <summary>
        /// 获取设置表单是否只读
        /// </summary>
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        /// <summary>
        /// 获取表单数据是否已修改
        /// </summary>
#if ANDROID
        new
#endif
        public bool IsDirty
        {
            get { return (bool)GetValue(IsDirtyProperty); }
            set { SetValue(IsDirtyProperty, value); }
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
        /// 获取单元格集合
        /// </summary>
        public FvItems Items { get; } = new FvItems();

        /// <summary>
        /// 获取具有指定id的单元格
        /// </summary>
        /// <param name="p_id"></param>
        /// <returns></returns>
        public FvCell this[string p_id]
        {
            get { return Items[p_id]; }
        }

        /// <summary>
        /// 获取指定索引的单元格
        /// </summary>
        /// <param name="p_index"></param>
        /// <returns></returns>
        public FvCell this[int p_index]
        {
            get { return Items[p_index] as FvCell; }
        }

        /// <summary>
        /// 获取所有含ID的格
        /// </summary>
        public IEnumerable<FvCell> IDCells
        {
            get
            {
                return from obj in Items
                       let cell = obj as FvCell
                       where cell != null && !string.IsNullOrEmpty(cell.ID)
                       select cell;
            }
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
            foreach (var item in Items)
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
            foreach (var item in Items)
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
        /// 名称列表中的格不允许为空，空时给出警告并返回true
        /// </summary>
        /// <param name="p_names"></param>
        /// <returns></returns>
        public bool ExistNull(params string[] p_names)
        {
            bool existNull = false;
            foreach (string name in p_names)
            {
                if (string.IsNullOrEmpty(name))
                    continue;

                var item = Items[name];
                if (item != null)
                {
                    object val = item.GetVal();
                    if (val == null || val.ToString() == "")
                    {
                        existNull = true;
                        item.Warn("不可为空！");
                    }
                }
            }
            return existNull;
        }

        /// <summary>
        /// 提交自上次调用以来对该行进行的所有更改
        /// </summary>
        public void AcceptChanges()
        {
            if (Data is Row row)
                row.AcceptChanges();
            else if (DataView != null)
                DataView.AcceptChanges();
        }

        /// <summary>
        /// 回滚自该表加载以来或上次调用 AcceptChanges 以来对该行进行的所有更改
        /// </summary>
        public void RejectChanges()
        {
            if (Data is Row row)
                row.RejectChanges();
            else if (DataView != null)
                DataView.RejectChanges();
        }

        /// <summary>
        /// 是否丢弃所有的修改
        /// </summary>
        /// <returns>true 未修改或丢弃修改</returns>
        public async Task<bool> DiscardChanges()
        {
            if (Data is Row row)
            {
                if (row.IsChanged)
                {
                    if (!await Kit.Confirm("数据未保存，确认要丢弃所有修改吗？"))
                        return false;
                    row.RejectChanges();
                }
            }
            else if (DataView != null)
            {
                var dv = DataView;
                if (dv.IsChanged)
                {
                    if (!await Kit.Confirm("数据未保存，确认要丢弃所有修改吗？"))
                        return false;
                    dv.RejectChanges();
                }
            }
            return true;
        }

        /// <summary>
        /// 获取单元格cookie值，FvCell.AutoCookie为true时有效
        /// </summary>
        /// <param name="p_cellID"></param>
        /// <returns></returns>
        public string GetCookie(string p_cellID)
        {
            FvCell cell;
            if (string.IsNullOrEmpty(Name)
                || (cell = this[p_cellID]) == null
                || !cell.AutoCookie)
                return null;

            string path = $"{BaseUri.AbsolutePath}+{Name}+{cell.ID}";
            return AtState.GetScalar<string>($"select val from CellLastVal where id=\"{path}\"");
        }

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
        #endregion

        #region 焦点处理
        /// <summary>
        /// 自动跳入第一个可接收焦点的列
        /// </summary>
        public void GotoFirstCell()
        {
            foreach (var item in _panel.Children)
            {
                FvCell cell = item as FvCell;
                if (cell != null && cell.ReceiveFocus())
                    return;
            }
        }

        /// <summary>
        /// 最后一个非只读单元格获得焦点
        /// </summary>
        public void GotoLastCell()
        {
            for (int i = _panel.Children.Count - 1; i >= 0; i--)
            {
                FvCell cell = _panel.Children[i] as FvCell;
                if (cell != null && cell.ReceiveFocus())
                    return;
            }
        }

        /// <summary>
        /// 跳到指定单元格
        /// </summary>
        /// <param name="p_cellName">要跳到的格名称</param>
        /// <returns>true 跳成功</returns>
        public void GotoCell(string p_cellName)
        {
            FvCell cell = this[p_cellName];
            if (cell != null)
            {
                if (!cell.ReceiveFocus())
                    Kit.Msg(string.Format("要跳入的单元格({0})无法获得焦点！", p_cellName));
            }
            else
            {
                Kit.Msg(string.Format("未找到要跳入的单元格({0})！", p_cellName));
            }
        }

        /// <summary>
        /// 移向下一编辑器
        /// </summary>
        /// <param name="p_cell">当前格</param>
        internal void GotoNextCell(FvCell p_cell)
        {
            int index = _panel.Children.IndexOf(p_cell);
            if (index == -1)
                return;

            int preIndex = index;
            while (true)
            {
                index++;
                if (index >= _panel.Children.Count)
                    index = 0;

                // 避免只一个可编辑格
                if (index == preIndex)
                    break;

                FvCell cell = _panel.Children[index] as FvCell;
                if (cell != null && cell.ReceiveFocus())
                    break;
            }
        }

        /// <summary>
        /// 移向上一编辑器
        /// </summary>
        /// <param name="p_cell">当前格</param>
        internal void GotoPreviousCell(FvCell p_cell)
        {
            int index = _panel.Children.IndexOf(p_cell);
            if (index == -1)
                return;

            while (true)
            {
                index--;
                if (index < 0)
                    break;

                FvCell cell = _panel.Children[index] as FvCell;
                if (cell != null && cell.ReceiveFocus())
                    break;
            }
        }
        #endregion

        #region 滚动到
        /// <summary>
        /// 滚动到最顶端
        /// </summary>
        public void ScrollTop()
        {
            if (_panel.Children.Count > 0)
                ScrollInto((FrameworkElement)_panel.Children[0]);
        }

        /// <summary>
        /// 滚动到最底端
        /// </summary>
        public void ScrollBottom()
        {
            // 末尾为边框
            if (_panel.Children.Count > 1)
                ScrollInto((FrameworkElement)_panel.Children[_panel.Children.Count - 2]);
        }

        /// <summary>
        /// 将指定格滚动到可视区域
        /// </summary>
        /// <param name="p_index">格索引</param>
        public void ScrollInto(int p_index)
        {
            if (p_index >= 0 && p_index < _panel.Children.Count)
                ScrollInto((FrameworkElement)_panel.Children[p_index]);
        }

        /// <summary>
        /// 将指定单元格滚动到可视范围
        /// </summary>
        /// <param name="p_elem"></param>
        public void ScrollInto(FrameworkElement p_elem)
        {
            if (_scroll == null || p_elem == null)
                return;

            // 单元格相对面板位置
            Point pt = p_elem.TransformToVisual(_panel).TransformPoint(new Point());
            if (_scroll.Content == _panel)
            {
                // 内部滚动栏
                if (pt.Y < _scroll.VerticalOffset)
                {
                    // 超出上部
                    _scroll.ChangeView(null, pt.Y, null);
                }
                else if ((pt.Y + p_elem.ActualHeight) > (_scroll.VerticalOffset + _scroll.ViewportHeight))
                {
                    // 超出下部
                    _scroll.ChangeView(null, pt.Y + p_elem.ActualHeight - _scroll.ViewportHeight, null);
                }
            }
            else
            {
                // 外部滚动栏
                // 面板相对滚动栏位置
                Point ptScroll = _panel.TransformToVisual(_scroll).TransformPoint(new Point());
                if (pt.Y + ptScroll.Y < 0)
                {
                    // 超出上部
                    _scroll.ChangeView(null, pt.Y + ptScroll.Y + _scroll.VerticalOffset, null);
                }
                else if (pt.Y + ptScroll.Y + p_elem.ActualHeight > _scroll.ViewportHeight)
                {
                    // 超出下部
                    _scroll.ChangeView(null, pt.Y + ptScroll.Y + p_elem.ActualHeight + _scroll.VerticalOffset - _scroll.ViewportHeight, null);
                }
            }
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

#if UWP
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
        #endregion

        #region 数据源
        /// <summary>
        /// 切换数据源，Row或普通对象
        /// </summary>
        void OnDataChanged()
        {
            object data = Data;
            Row row = data as Row;

            if (data == null)
            {
                ClearValue(IsDirtyProperty);
                ClearValue(DataViewProperty);
            }
            else
            {
                // Data.IsChanged <=> Fv.IsChanged
                Binding bind = new Binding { Path = new PropertyPath("IsChanged") };
                if (row != null)
                {
                    bind.Source = row;
                    row.Changed += OnCellValueChanged;
                }
                else
                {
                    DataView = new ObjectView();
                    bind.Source = DataView;
                    DataView.Changed += OnPropertyValueChanged;
                }
                SetBinding(IsDirtyProperty, bind);
            }

            if (AutoCreateCell)
            {
                // 根据数据源自动生成格
                using (Items.Defer())
                {
                    Items.Clear();
                    _panel.Children.Clear();
                    if (row != null)
                    {
                        foreach (var dc in row.Cells)
                        {
                            FvCell cell = CreateCell(dc.Type, dc.ID);
                            cell.Owner = this;
                            cell.OnDataChanged(data);
                            Items.Add(cell);
                        }
                    }
                    else if (data != null)
                    {
                        PropertyInfo[] pis = data.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
                        foreach (var pi in pis)
                        {
                            FvCell cell = CreateCell(pi.PropertyType, pi.Name);
                            cell.Owner = this;
                            cell.Title = pi.Name;
                            cell.OnDataChanged(data);
                            Items.Add(cell);
                        }
                    }
                }
            }
            else
            {
                // 只处理含ID的格，其它格怕有影响未设置DataContext！
                foreach (var cell in IDCells)
                {
                    cell.OnDataChanged(data);
                }
            }

            // 切换数据源事件
            OnFvDataChanged();
        }

        void OnCellValueChanged(object sender, Cell e)
        {
            OnValueChanged(e);
        }

        void OnPropertyValueChanged(object sender, PropertyView e)
        {
            OnValueChanged(e);
        }
        #endregion

        #region Items管理
        void OnItemsChanged(object sender, ItemListChangedArgs e)
        {
            if (e.CollectionChange == CollectionChange.ItemRemoved || e.CollectionChange == CollectionChange.ItemChanged)
                _panel.Children.RemoveAt(e.Index);

            if (e.CollectionChange == CollectionChange.ItemInserted || e.CollectionChange == CollectionChange.ItemChanged)
            {
                AddItem(Items[e.Index], e.Index);
            }
            else if (e.CollectionChange == CollectionChange.Reset)
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    var item = Items[i];
                    if (_panel.Children.Count > i)
                    {
                        var elem = _panel.Children[i];

                        // 内容没变
                        if (item == elem
                            || (elem is CFree f && f.Content == item))
                            continue;

                        // 变了移除旧元素
                        _panel.Children.RemoveAt(i);
                    }
                    AddItem(item, i);
                }

                // 移除多余的元素
                while (_panel.Children.Count > Items.Count)
                {
                    _panel.Children.RemoveAt(_panel.Children.Count - 1);
                }
            }
        }

        protected virtual void LoadAllItems()
        {
            for (int i = 0; i < Items.Count; i++)
            {
                AddItem(Items[i], i);
            }
        }

        protected void AddItem(FrameworkElement p_item, int p_index)
        {
            FrameworkElement elem = p_item;
            if (p_item is FvCell cell)
            {
                cell.Owner = this;
            }
            else if (!(p_item is CBar))
            {
                // 自定义内容
                CFree c = new CFree();
                c.Owner = this;
                c.ShowTitle = false;
                c.Content = p_item;
                elem = c;
            }
            _panel.Children.Insert(p_index, elem);
        }
        #endregion

        #region 触发事件
        /// <summary>
        /// 触发数据源切换事件
        /// </summary>
        void OnFvDataChanged()
        {
            DataChanged?.Invoke(this, Data);
        }

        /// <summary>
        /// 触发单元格数据修改事件
        /// </summary>
        /// <param name="e"></param>
        void OnValueChanged(ICell e)
        {
            Changed?.Invoke(this, e);
        }

        /// <summary>
        /// 触发数据源修改状态变化事件
        /// </summary>
        void OnDirty()
        {
            Dirty?.Invoke(this, IsDirty);
        }

        /// <summary>
        /// 触发内部单元格点击事件
        /// </summary>
        /// <param name="p_cell"></param>
        internal void OnCellClick(FvCell p_cell)
        {
            CellClick?.Invoke(this, p_cell);
        }
        #endregion
    }
}