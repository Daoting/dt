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

        public static readonly DependencyProperty ReadOnlyBindingProperty = DependencyProperty.Register(
            "ReadOnlyBinding",
            typeof(bool),
            typeof(FvCell),
            new PropertyMetadata(false, OnReadOnlyChanged));

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
        public event Action<FvCell, object> Changed;

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
        /// 获取设置当前列最终是否只读，综合Fv.IsReadOnly、FvCell.IsReadOnly，内部绑定用
        /// </summary>
        public bool ReadOnlyBinding
        {
            get { return (bool)GetValue(ReadOnlyBindingProperty); }
            internal set { SetValue(ReadOnlyBindingProperty, value); }
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

                    // 初始修改状态背景色
                    if (_panel != null && ValBinding.Source is ICell cell && cell.IsChanged)
                        _panel.ToggleIsChanged(true);
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