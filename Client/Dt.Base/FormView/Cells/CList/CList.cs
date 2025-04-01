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
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Markup;
using Dt.Toolkit.Sql;
using System.Xml;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 列表选择格
    /// 数据源优先级：
    /// 1. 外部直接设置Data
    /// 2. 通过LoadData事件加载数据
    /// 3. 设置`Sql`属性，该属性定义了select语句(sql可包含参数、占位符)、服务名或本地库名，主要方便在xaml中设置
    /// 3. 设置`Ex`属性，该属性是`类名#参数`的字符串，类需要继承自`CListEx`，重写方法可控制下拉对话框和数据源。
    /// 4. 外部(xaml中)定义的对象列表
    /// 5. 格的数据源类型为枚举时，自动生成Enum数据
    /// 为普通对象时，直接将对象赋值！
    /// </summary>
    [ContentProperty(Name = nameof(View))]
    public partial class CList : FvCell
    {
        #region 静态内容
        public readonly static DependencyProperty ExProperty = DependencyProperty.Register(
            "Ex",
            typeof(string),
            typeof(CList),
            new PropertyMetadata(null, OnClearData));

        public readonly static DependencyProperty SqlProperty = DependencyProperty.Register(
            "Sql",
            typeof(Sql),
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

        public static readonly DependencyProperty PlaceholderProperty = DependencyProperty.Register(
            "Placeholder",
            typeof(string),
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
        NlItems _items;
        ListDlg _dlg;
        #endregion

        #region 构造方法
        public CList()
        {
            DefaultStyleKey = typeof(CList);
            _lv = new Lv();

            // 全面屏底部易误点
            if (Kit.IsPhoneUI)
                _lv.Margin = new Thickness(0, 0, 0, 40);
        }
        #endregion

        #region 事件
        /// <summary>
        /// 外部自定义数据源事件，支持异步
        /// </summary>
        public event Action<CList, AsyncArgs> LoadData;

        /// <summary>
        /// 选择后事件，Selected与uno中重名，xaml中附加事件报错！
        /// </summary>
        public event Action<CList, object> AfterSelect;
        #endregion

        #region 属性
        /// <summary>
        /// 获取设置数据源的select语句，sql可包含 变量 或 占位符
        /// <para>1. 变量以@开头：</para>
        /// <para>    @属性名，取Fv数据源的属性值，以Sql参数方式查询</para>
        /// <para>    @类名.方法(串参数或无)，调用有ValueCall标签的类的静态方法取值，以Sql参数方式查询</para>
        /// <para>    @{键}，支持键：userid username input，当前用户ID、用户名、CPick中输入的过滤串</para>
        /// <para>    如：@parentid  @RptValueCall.GetMaxID(crud_父表)  @{input}</para>
        /// <para></para>
        /// <para>2. 占位符首尾添加#：</para>
        /// <para>    #属性名#，取Fv数据源的属性值，查询前替换占位符的值</para>
        /// <para>    #类名.方法(串参数或无)#，调用有ValueCall标签的类的静态方法取值替换占位符</para>
        /// <para>    #{键}#，和变量相同，用键值替换占位符</para>
        /// <para>    如：#parentid#  #RptValueCall.GetMaxID(crud_父表)# #{input}#</para>
        /// <para>
        /// SELECT
        /// 	大儿名
        /// FROM
        /// 	crud_大儿
        /// WHERE
        /// 	parent_id = @parentid
        ///     AND name LIKE '#{input}#%'
        ///     AND id = @RptValueCall.GetMaxID(crud_大儿)
        ///     AND owner = @{userid}
        /// </para>
        /// </summary>
        public Sql Sql
        {
            get { return (Sql)GetValue(SqlProperty); }
            set { SetValue(SqlProperty, value); }
        }

        /// <summary>
        /// 获取设置扩展CList功能的类名和参数，用于控制下拉对话框和数据源，类名和参数之间用#隔开，如：
        /// <para>EnumData#Dt.Base.DlgPlacement,Dt.Base</para>
        /// <para>Option#民族</para>
        /// </summary>
        [CellParam("数据源扩展：类别名#参数，类标签[CListEx]")]
        public string Ex
        {
            get { return (string)GetValue(ExProperty); }
            set { SetValue(ExProperty, value); }
        }

        /// <summary>
        /// 外部(xaml中)定义的对象列表
        /// </summary>
        public NlItems Items
        {
            get
            {
                if (_items == null)
                    _items = new NlItems();
                return _items;
            }
        }

        /// <summary>
        /// 获取设置源属性列表，用'#'隔开
        /// <para>1. 当为目标列填充null时，用'-'标志，如：SrcID="id#-#-"</para>
        /// <para>2. SrcID空时默认取name列 或 数据源第一列的列名</para>
        /// </summary>
        [CellParam("源属性：#隔开，-目标清空，空时name或第一列")]
        public string SrcID
        {
            get { return (string)GetValue(SrcIDProperty); }
            set { SetValue(SrcIDProperty, value); }
        }

        /// <summary>
        /// 获取设置目标属性列表，用'#'隔开
        /// <para>1. TgtID空时默认取当前列名</para>
        /// <para>2. '#'隔开多列时可用'-'代表当前列名，如：TgtID="-#child1#child2"，也可以直接写当前列名</para>
        /// </summary>
        [CellParam("目标属性：#隔开，-代表当前列名，空时当前列名")]
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
        /// 获取设置占位符文本
        /// </summary>
        [CellParam("占位符文本")]
        public string Placeholder
        {
            get { return (string)GetValue(PlaceholderProperty); }
            set { SetValue(PlaceholderProperty, value); }
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
        /// 下拉框
        /// </summary>
        public ListDlg Dlg => _dlg;

        protected override IFvCall DefaultMiddle => new ListValConverter();
        #endregion

        #region 重写方法
        protected override void OnApplyCellTemplate()
        {
            _grid = (Grid)GetTemplateChild("Grid");
#if WIN
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
            {
                if (IsEditable && !ReadOnlyBinding)
                {
                    if (_grid.Children[_grid.Children.Count - 1] is TextBox tb)
                    {
                        tb.SelectAll();
                        tb.Focus(FocusState.Programmatic);
                    }
                }
                else
                {
                    OnShowDlg(null, null);
                }
            }
            return true;
        }

        public override void Destroy()
        {
            if (_grid != null)
            {
#if WIN
                _grid.RemoveHandler(TappedEvent, new TappedEventHandler(OnShowDlg));
#else
                _grid.Tapped -= OnShowDlg;
#endif
            }

            if (_dlg != null)
            {
                _dlg.Destroy();
                _dlg = null;
            }
        }
        #endregion

        #region 内部方法
        void OnShowDlg(object sender, TappedRoutedEventArgs e)
        {
            if (ReadOnlyBinding)
                return;

            // 可编辑时只有点击右侧的下拉才显示选择框
            if (IsEditable && e != null && e.OriginalSource is not TextBlock)
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
                        MaxHeight = 300,
                    };
                    if (_lv.CurrentViewMode != ViewMode.Table)
                        _dlg.Width = _grid.ActualWidth > 0 ? _grid.ActualWidth : 300;
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

            TextBox tb = new TextBox { Style = Res.FvTextBox };
            Binding bind = new Binding
            {
                Path = new PropertyPath("Value"),
                Converter = new ListTextConverter(this),
                Source = this
            };
            if (IsEditable && !ReadOnlyBinding)
            {
                bind.Mode = BindingMode.TwoWay;
            }
            else
            {
                tb.IsReadOnly = true;
            }
            tb.SetBinding(TextBox.TextProperty, bind);

            bind = new Binding
            {
                Path = new PropertyPath("Placeholder"),
                Source = this
            };
            tb.SetBinding(TextBox.PlaceholderTextProperty, bind);
            _grid.Children.Add(tb);
        }

        /// <summary>
        /// 触发外部自定义数据源事件
        /// </summary>
        /// <returns></returns>
        internal async Task<bool> OnLoadData()
        {
            if (LoadData != null)
            {
                var args = new AsyncArgs();
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