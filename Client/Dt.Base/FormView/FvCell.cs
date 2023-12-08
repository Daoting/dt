﻿#region 文件描述
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
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using System.ComponentModel;
using System.Reflection;
using Windows.Foundation;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 单元格
    /// </summary>
    public partial class FvCell : DtControl, IFvCell
    {
        #region 静态内容
        // 8个中文字，原140，fifo调，现6个中文字
        internal const double DefaultTitleWidth = 120;
        internal const int DefaultColumnSpan = 8;

        public readonly static DependencyProperty IDProperty = DependencyProperty.Register(
            "ID",
            typeof(string),
            typeof(FvCell),
            new PropertyMetadata(null));

        public readonly static DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title",
            typeof(string),
            typeof(FvCell),
            new PropertyMetadata(null));

        public static readonly DependencyProperty TitleWidthProperty = DependencyProperty.Register(
            "TitleWidth",
            typeof(double),
            typeof(FvCell),
            new PropertyMetadata(DefaultTitleWidth, OnInvalidatePanel));

        public static readonly DependencyProperty ShowTitleProperty = DependencyProperty.Register(
            "ShowTitle",
            typeof(bool),
            typeof(FvCell),
            new PropertyMetadata(true, OnShowTitleChanged));

        public static readonly DependencyProperty CallProperty = DependencyProperty.Register(
            "Call",
            typeof(string),
            typeof(FvCell),
            new PropertyMetadata(null));

        public static readonly DependencyProperty IsVerticalTitleProperty = DependencyProperty.Register(
            "IsVerticalTitle",
            typeof(bool),
            typeof(FvCell),
            new PropertyMetadata(false, OnIsVerticalTitleChanged));

        public static readonly DependencyProperty ColSpanProperty = DependencyProperty.Register(
            "ColSpan",
            typeof(double),
            typeof(FvCell),
            new PropertyMetadata(1.0d, OnColSpanChanged));

        public static readonly DependencyProperty RowSpanProperty = DependencyProperty.Register(
            "RowSpan",
            typeof(int),
            typeof(FvCell),
            new PropertyMetadata(1, OnUpdateLayout));

        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(
            "IsReadOnly",
            typeof(bool),
            typeof(FvCell),
            new PropertyMetadata(false, OnIsReadOnlyChanged));

        public static readonly DependencyProperty AutoCookieProperty = DependencyProperty.Register(
            "AutoCookie",
            typeof(bool),
            typeof(FvCell),
            new PropertyMetadata(false));

        public static readonly DependencyProperty PlaceholderProperty = DependencyProperty.Register(
            "Placeholder",
            typeof(string),
            typeof(FvCell),
            new PropertyMetadata(null));

        public static readonly DependencyProperty QueryProperty = DependencyProperty.Register(
            "Query",
            typeof(QueryType),
            typeof(FvCell),
            new PropertyMetadata(QueryType.Disable, OnQueryTypeChanged));

        public static readonly DependencyProperty QueryFlagProperty = DependencyProperty.Register(
            "QueryFlag",
            typeof(CompFlag),
            typeof(FvCell),
            new PropertyMetadata(CompFlag.Ignore, OnQueryFlagChanged));

        public static readonly DependencyProperty ReadOnlyBindingProperty = DependencyProperty.Register(
            "ReadOnlyBinding",
            typeof(bool),
            typeof(FvCell),
            new PropertyMetadata(false, OnReadOnlyChanged));

        public static readonly DependencyProperty ValBindingProperty = DependencyProperty.Register(
            "ValBinding",
            typeof(Binding),
            typeof(FvCell),
            new PropertyMetadata(new Binding(), OnValBindingChanged));

        static void OnUpdateLayout(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pnl = ((FvCell)d).GetParent();
            if (pnl != null)
                pnl.InvalidateMeasure();
        }

        static void OnColSpanChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FvCell cell = (FvCell)d;
            double span = (double)e.NewValue;
            if (span > 1)
            {
                cell.ColSpan = 1;
            }
            else if (span < 0)
            {
                cell.ColSpan = 0;
            }
            else if (Math.Round(span, 2) != span)
            {
                // xaml中的double有17位小数，精度有误差
                cell.ColSpan = Math.Round(span, 2);
            }
            else
            {
                OnUpdateLayout(d, e);
            }
        }

        static void OnInvalidatePanel(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FvCell cell = (FvCell)d;
            if (cell._panel != null)
                cell._panel.InvalidateMeasure();
        }

        static void OnShowTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FvCell cell = (FvCell)d;
            if (cell._panel != null)
                cell._panel.UpdateChildren();
        }

        static void OnIsVerticalTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FvCell cell = (FvCell)d;
            if (cell._panel != null)
                cell._panel.OnIsVerticalTitleChanged();
        }

        static void OnIsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FvCell cell = (FvCell)d;
            if (cell._isLoaded)
                cell.ApplyIsReadOnly();
        }

        static void OnValBindingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FvCell cell = (FvCell)d;
            if (cell._isLoaded)
            {
                Binding bind = (Binding)e.NewValue;
                if (bind != null && bind.Source != null)
                {
                    if (cell._panel != null && cell._panel.Child != null)
                        cell._panel.Child.ClearValue(VisibilityProperty);
                    cell.SetValBinding();
                }
                else if (cell._panel != null && cell._panel.Child != null)
                {
                    // 未设置数据源时隐藏编辑器
                    cell._panel.Child.Visibility = Visibility.Collapsed;
                }
            }
        }

        static void OnQueryTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FvCell cell = (FvCell)d;
            if (cell._panel != null)
                cell._panel.UpdateChildren();
        }

        static void OnQueryFlagChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FvCell cell = (FvCell)d;
            if (cell._panel != null)
                cell._panel.OnQueryFlagChanged();
        }

        static void OnReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ((FvCell)d).OnReadOnlyChanged();
        }
        #endregion

        #region 成员变量
        protected CellPanel _panel;
        protected bool _isLoaded;
        #endregion

        #region 构造方法
        public FvCell()
        {
            DefaultStyleKey = typeof(FvCell);
        }
        #endregion

        #region 事件
        /// <summary>
        /// 单元格值修改后事件，参数为新值
        /// </summary>
        public event Action<object> Changed;

        /// <summary>
        /// 编辑器的按键事件
        /// </summary>
        new public event KeyEventHandler KeyUp;
        #endregion

        #region 属性
        /// <summary>
        /// 获取设置列名(字段名或属性名)
        /// </summary>
        public string ID
        {
            get { return (string)GetValue(IDProperty); }
            set { SetValue(IDProperty, value); }
        }

        /// <summary>
        /// 获取设置列标题
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// 获取设置列名的宽度
        /// </summary>
        public double TitleWidth
        {
            get { return (double)GetValue(TitleWidthProperty); }
            set { SetValue(TitleWidthProperty, value); }
        }

        /// <summary>
        /// 获取设置是否显示标题列
        /// </summary>
        public bool ShowTitle
        {
            get { return (bool)GetValue(ShowTitleProperty); }
            set { SetValue(ShowTitleProperty, value); }
        }

        /// <summary>
        /// 获取设置自定义取值赋值过程的类名
        /// </summary>
        public string Call
        {
            get { return (string)GetValue(CallProperty); }
            set { SetValue(CallProperty, value); }
        }

        /// <summary>
        /// 获取设置是否垂直显示标题
        /// </summary>
        public bool IsVerticalTitle
        {
            get { return (bool)GetValue(IsVerticalTitleProperty); }
            set { SetValue(IsVerticalTitleProperty, value); }
        }

        /// <summary>
        /// 获取设置占用的行数，默认1行，-1时自动行高
        /// </summary>
        public int RowSpan
        {
            get { return (int)GetValue(RowSpanProperty); }
            set { SetValue(RowSpanProperty, value); }
        }

        /// <summary>
        /// 获取设置单元格占用列的比例，取值范围 0~1，0表示水平填充，1表示占满整列，默认1
        /// </summary>
        public double ColSpan
        {
            get { return (double)GetValue(ColSpanProperty); }
            set { SetValue(ColSpanProperty, value); }
        }

        /// <summary>
        /// 获取设置列是否只读，最终是否只读由ReadOnlyBinding确定！
        /// </summary>
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        /// <summary>
        /// 获取设置是否自动保存单元格最后一次编辑值，默认False
        /// </summary>
        public bool AutoCookie
        {
            get { return (bool)GetValue(AutoCookieProperty); }
            set { SetValue(AutoCookieProperty, value); }
        }

        /// <summary>
        /// 获取设置占位符文本
        /// </summary>
        public string Placeholder
        {
            get { return (string)GetValue(PlaceholderProperty); }
            set { SetValue(PlaceholderProperty, value); }
        }

        /// <summary>
        /// 获取设置查询时对条件比较符控制：无比较符、可修改比较符、比较符只读
        /// </summary>
        public QueryType Query
        {
            get { return (QueryType)GetValue(QueryProperty); }
            set { SetValue(QueryProperty, value); }
        }

        /// <summary>
        /// 获取设置查询时的条件比较符
        /// </summary>
        public CompFlag QueryFlag
        {
            get { return (CompFlag)GetValue(QueryFlagProperty); }
            set { SetValue(QueryFlagProperty, value); }
        }

        /// <summary>
        /// 获取设置当前列最终是否只读，综合Fv.IsReadOnly、FvCell.IsReadOnly，内部绑定用
        /// </summary>
        public bool ReadOnlyBinding
        {
            get { return (bool)GetValue(ReadOnlyBindingProperty); }
            internal set { SetValue(ReadOnlyBindingProperty, value); }
        }

        /// <summary>
        /// 获取设置格的值绑定，内部绑定用，始终非null，ConverterParameter保存了绑定的数据源属性类型
        /// </summary>
        internal Binding ValBinding
        {
            get { return (Binding)GetValue(ValBindingProperty); }
            set { SetValue(ValBindingProperty, value); }
        }

        /// <summary>
        /// 获取所属的Fv
        /// </summary>
        internal Fv Owner { get; set; }

        /// <summary>
        /// 在面板上的布局区域
        /// </summary>
        Rect IFvCell.Bounds { get; set; }

        /// <summary>
        /// 默认的取值赋值处理对象
        /// </summary>
        protected virtual IFvCall DefaultMiddle { get; }
        #endregion

        #region 外部方法
        /// <summary>
        /// 回滚数据
        /// </summary>
        public void RejectChanges()
        {
            Cell dc = ValBinding.Source as Cell;
            if (dc != null)
                dc.RejectChanges();
        }
        #endregion

        #region 提示信息
        /// <summary>
        /// 显示警告提示信息
        /// </summary>
        /// <param name="p_msg"></param>
        public void Warn(string p_msg)
        {
            DlgEx.ShowMessage((_panel == null ? this : _panel.Child), p_msg, Res.RedBrush);
        }

        /// <summary>
        /// 显示提示消息
        /// </summary>
        /// <param name="p_msg"></param>
        public void Msg(string p_msg)
        {
            DlgEx.ShowMessage((_panel == null ? this : _panel.Child), p_msg, Res.BlackBrush);
        }
        #endregion

        #region 重写方法
        protected override void OnLoadTemplate()
        {
            _panel = (CellPanel)GetTemplateChild("Panel");
            _panel?.SetOwner(this);

            // 后面SetValBinding时需要内部元素
            OnApplyCellTemplate();

            if (IsReadOnly || Owner.IsReadOnly)
                ReadOnlyBinding = true;

            if (!string.IsNullOrEmpty(ID))
            {
                if (ReadLocalValue(ValBindingProperty) != DependencyProperty.UnsetValue)
                {
                    SetValBinding();
                }
                else if (_panel != null && _panel.Child != null)
                {
                    // 未设置数据源时隐藏编辑器
                    _panel.Child.Visibility = Visibility.Collapsed;
                }

                // ID作为缺省标题
                if (ReadLocalValue(TitleProperty) == DependencyProperty.UnsetValue)
                    SetValue(TitleProperty, ID);
            }
            _isLoaded = true;
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 综合Fv.IsReadOnly、FvCell.IsReadOnly，确定是否只读
        /// </summary>
        internal void ApplyIsReadOnly()
        {
            if (IsReadOnly || Owner.IsReadOnly)
                ReadOnlyBinding = true;
            else
                ClearValue(ReadOnlyBindingProperty);
        }

        /// <summary>
        /// 内部编辑器获得焦点
        /// </summary>
        /// <returns></returns>
        internal bool ReceiveFocus()
        {
            if (IsEnabled
                && Visibility == Visibility.Visible
                && !ReadOnlyBinding
                && _panel != null
                && _panel.Child != null)
            {
                Owner.ScrollInto(this);
                return SetFocus();
            }
            return false;
        }

        /// <summary>
        /// 获取格的取值赋值处理对象
        /// </summary>
        /// <returns></returns>
        internal IFvCall GetMiddle()
        {
            if (!string.IsNullOrEmpty(Call))
            {
                var tp = Kit.GetTypeByAlias(typeof(FvCallAttribute), Call);
                if (tp != null && tp.GetInterface("IFvCall") == typeof(IFvCall))
                    return Activator.CreateInstance(tp) as IFvCall;
            }
            return DefaultMiddle;
        }

        internal void PostKeyUp(KeyRoutedEventArgs e)
        {
            KeyUp?.Invoke(this, e);
        }
        #endregion

        #region 获取/设置值
        /// <summary>
        /// 获取格的当前值
        /// </summary>
        /// <returns></returns>
        internal object GetVal()
        {
            if (ValBinding.Source is ICell cell)
                return cell.Val;
            return null;
        }

        /// <summary>
        /// 设置格的值
        /// </summary>
        /// <param name="p_val"></param>
        internal void SetVal(object p_val)
        {
            if (ValBinding.Source is ICell cell)
                cell.Val = p_val;
        }
        #endregion

        #region 数据源
        /// <summary>
        /// 切换数据源，有ID
        /// </summary>
        /// <param name="p_data"></param>
        internal void OnDataChanged(object p_data)
        {
            Binding oldBind = ValBinding;
            if (oldBind.Source is ICell cell)
            {
                // 不需回滚，有时需要编辑结果！
                //cell.RejectChanges();
                cell.PropertyChanged -= OnDataPropertyChanged;
                cell.Message -= OnCellMessage;
            }

            // 空数据源
            if (p_data == null)
            {
                ClearValue(ValBindingProperty);
                return;
            }

            // Row数据源
            if (p_data is Row row)
            {
                // 包含ID列
                if (row.Contains(ID))
                {
                    var c = row.Cells[ID];
                    c.PropertyChanged += OnDataPropertyChanged;
                    c.Message += OnCellMessage;

                    // 设置新绑定，只设置Source引起immutable异常！
                    ValBinding = new FvCellBind(this, p_data)
                    {
                        Path = new PropertyPath("Val"),
                        Mode = BindingMode.TwoWay,
                        Source = c,
                        ConverterParameter = c.Type
                    };
                    return;
                }

                var pi = p_data.GetType().GetProperty(ID, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (pi != null)
                {
                    // ID为Row的属性，OneTime且只读，只为同步显示用，无法保存
                    ValBinding = new FvCellBind(this, p_data)
                    {
                        Path = new PropertyPath(pi.Name),
                        Mode = BindingMode.OneTime,
                        Source = p_data,
                        ConverterParameter = pi.PropertyType
                    };
                    IsReadOnly = true;
                }
                else
                {
                    // ID既不是列，也不是属性，不绑定
                    ClearValue(ValBindingProperty);
                    Kit.Debug($"【{ID}】在数据源中既不是列，也不是属性，无法绑定！");
                }
                return;
            }

            // ID为普通对象的属性 或 以.分隔的多级子属性(如 Prop1.ChildProp.XXX)，解析绑定路径
            Binding bind = ParseBinding(p_data);
            if (bind == null)
            {
                // 绑定属性不存在
                ClearValue(ValBindingProperty);
                return;
            }

            // 控制只读状态
            if (bind.Mode == BindingMode.TwoWay)
                ClearValue(IsReadOnlyProperty);
            else
                IsReadOnly = true;

            ((PropertyView)bind.Source).PropertyChanged += OnDataPropertyChanged;
            ValBinding = bind;
        }

        /// <summary>
        /// 附加值改变时的处理方法，提供外部自定义显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnDataPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ICell cell = (ICell)sender;
            if (e.PropertyName == "Val")
            {
                OnValChanged();
                if (!AutoCookie || string.IsNullOrEmpty(Owner.Name))
                    return;

                // 记录单元格最近一次编辑值
                Kit.RunAsync(async () =>
                {
                    string id = string.Format("{0}+{1}+{2}", BaseUri.AbsolutePath, Owner.Name, ID);
                    object val = cell.Val;
                    if (val != null && !string.IsNullOrEmpty(val.ToString()))
                    {
                        var clv = await CellLastValX.GetByID(id);
                        if (clv != null)
                        {
                            clv.Val = val.ToString();
                            await clv.Save(false);
                        }
                        else
                        {
                            await new CellLastValX(id, val.ToString()).Save(false);
                        }
                    }
                    else
                    {
                        // 删除旧记录
                        await CellLastValX.DelByID(id);
                    }
                });
            }
            else if (e.PropertyName == "IsChanged")
            {
                if (_panel != null)
                    _panel.ToggleIsChanged(cell.IsChanged);
            }
        }

        void OnCellMessage(object sender, CellMessageArgs e)
        {
            if (e.IsWarning)
            {
                Warn(e.Message);
            }
            else
            {
                Msg(e.Message);
            }
        }

        /// <summary>
        /// 解析目标路径
        /// </summary>
        /// <param name="p_data"></param>
        /// <returns></returns>
        Binding ParseBinding(object p_data)
        {
            object tgt = null;
            PropertyInfo pi = null;
            string[] arr = ID.Split('.');
            if (arr.Length > 1)
            {
                var type = p_data.GetType();
                for (int i = 0; i < arr.Length; i++)
                {
                    pi = type.GetProperty(arr[i], BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (pi != null)
                    {
                        if (i == arr.Length - 1)
                            break;

                        type = pi.PropertyType;
                        if (i == 0)
                            tgt = pi.GetValue(p_data);
                        else if (tgt != null)
                            tgt = pi.GetValue(tgt);
                        else
                            return null;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            else
            {
                pi = p_data.GetType().GetProperty(ID, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (pi != null)
                    tgt = p_data;
            }

            if (tgt != null)
            {
                return new FvCellBind(this, p_data)
                {
                    Path = new PropertyPath("Val"),
                    Mode = pi.CanWrite ? BindingMode.TwoWay : BindingMode.OneWay,
                    Source = new PropertyView(Owner.DataView, pi, tgt),
                    ConverterParameter = pi.PropertyType
                };
            }
            return null;
        }

        /// <summary>
        /// 触发列值修改后事件
        /// </summary>
        void OnValChanged()
        {
            if (Changed != null)
            {
                object obj = GetVal();
                Changed.Invoke(obj);
            }
        }
        #endregion

        #region 虚方法
        /// <summary>
        /// 应用单元格模板
        /// </summary>
        protected virtual void OnApplyCellTemplate()
        {
        }

        /// <summary>
        /// 数据源绑定编辑器
        /// </summary>
        protected virtual void SetValBinding()
        {
        }

        protected virtual bool SetFocus()
        {
            if (_panel != null && _panel.Child is Control con)
                return con.Focus(FocusState.Programmatic);
            return true;
        }

        protected virtual void OnReadOnlyChanged()
        {
        }
        #endregion
    }
}