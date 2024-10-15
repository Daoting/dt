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
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 树形选择格
    /// </summary>
    [ContentProperty(Name = nameof(View))]
    public partial class CTree : FvCell
    {
        #region 静态内容
        public readonly static DependencyProperty SqlProperty = DependencyProperty.Register(
            "Sql",
            typeof(Sql),
            typeof(CTree),
            new PropertyMetadata(null, OnClearData));
        
        public readonly static DependencyProperty RefreshDataProperty = DependencyProperty.Register(
            "RefreshData",
            typeof(bool),
            typeof(CTree),
            new PropertyMetadata(false));

        public readonly static DependencyProperty SrcIDProperty = DependencyProperty.Register(
            "SrcID",
            typeof(string),
            typeof(CTree),
            new PropertyMetadata(null));

        public readonly static DependencyProperty TgtIDProperty = DependencyProperty.Register(
            "TgtID",
            typeof(string),
            typeof(CTree),
            new PropertyMetadata(null));
        
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text",
            typeof(string),
            typeof(CTree),
            new PropertyMetadata(null));

        static void OnClearData(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ls = (CTree)d;
            if (ls._dlg != null)
                ls._tv.ClearValue(Tv.DataProperty);
        }
        #endregion

        #region 成员变量
        readonly Tv _tv;
        Grid _grid;
        TreeDlg _dlg;
        #endregion

        #region 构造方法
        public CTree()
        {
            DefaultStyleKey = typeof(CTree);
            _tv = new Tv();
        }
        #endregion

        #region 事件
        /// <summary>
        /// 外部自定义数据源事件，支持异步
        /// </summary>
        public event Action<CTree, AsyncArgs> LoadData;

        /// <summary>
        /// 选择后事件，Selected与uno中重名，xaml中附加事件报错！
        /// </summary>
        public event Action<CTree, object> AfterSelect;
        #endregion

        #region Tv属性
        /// <summary>
        /// 获取设置数据源对象，Table已实现ITreeData
        /// </summary>
        public ITreeData Data
        {
            get { return _tv.Data; }
            set { _tv.Data = value; }
        }

        /// <summary>
        /// 获取设置节点模板或模板选择器
        /// </summary>
        public object View
        {
            get { return _tv.View; }
            set { _tv.View = value; }
        }

        /// <summary>
        /// 获取设置自定义行/项目样式的回调方法
        /// </summary>
        public Action<TvItemStyleArgs> ItemStyle
        {
            get { return _tv.ItemStyle; }
            set { _tv.ItemStyle = value; }
        }

        /// <summary>
        /// 获取设置选择模式，默认Single，只第一次设置有效！
        /// </summary>
        [CellParam("选择模式")]
        public SelectionMode SelectionMode
        {
            get { return _tv.SelectionMode; }
            set { _tv.SelectionMode = value; }
        }

        /// <summary>
        /// 获取设置固定根节点，切换数据源时不变
        /// </summary>
        public object FixedRoot
        {
            get { return _tv.FixedRoot; }
            set { _tv.FixedRoot = value; }
        }
        #endregion

        #region 属性
        /// <summary>
        /// 获取设置数据源Sql属性，方便在xaml中设置，select语句可包含变量或占位符，变量以@开头，占位符首尾添加#，它们内容格式相同，分两类：
        /// <para>1. 内部表达式取值：</para>
        /// <para>   userid：当前登录ID</para>
        /// <para>   username：当前登录名</para>
        /// <para>   input：CPick中输入的过滤串</para>
        /// <para>   [列名]：当前Fv数据源的列值</para>
        /// 
        /// <para>2. 调用外部方法取值： 外部类名.方法(参数)，如RptValueCall.GetMaxID(demo_父表)</para>
        /// <para>
        /// SELECT
        /// 	大儿名
        /// FROM
        /// 	demo_大儿
        /// WHERE
        /// 	parent_id = @[parentid]
        ///     AND name LIKE '#input#%'
        ///     AND id = @RptValueCall.GetMaxID(demo_大儿)
        ///     AND owner = @userid
        /// </para>
        /// </summary>
        public Sql Sql
        {
            get { return (Sql)GetValue(SqlProperty); }
            set { SetValue(SqlProperty, value); }
        }
        
        /// <summary>
        /// 获取设置是否动态加载树数据源，默认false
        /// true表示每次显示对话框时都加载数据源，false表示只第一次加载
        /// </summary>
        [CellParam("动态加载数据源")]
        public bool RefreshData
        {
            get { return (bool)GetValue(RefreshDataProperty); }
            set { SetValue(RefreshDataProperty, value); }
        }

        /// <summary>
        /// 获取设置源属性列表，用'#'隔开
        /// <para>1. 当为目标列填充null时，用'-'标志，如：SrcID="id#-#-"</para>
        /// <para>2. SrcID空时默认取name列 或 数据源第一列的列名</para>
        /// </summary>
        [CellParam("源属性列表")]
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
        [CellParam("目标属性列表")]
        public string TgtID
        {
            get { return (string)GetValue(TgtIDProperty); }
            set { SetValue(TgtIDProperty, value); }
        }
        
        /// <summary>
        /// 获取设置当前值
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// 获取Tv对象
        /// </summary>
        public Tv Tv
        {
            get { return _tv; }
        }
        #endregion

        #region 重写方法
        protected override void OnApplyCellTemplate()
        {
            _grid = (Grid)GetTemplateChild("Grid");
#if WIN
            // TextBlock可复制
            _grid.AddHandler(TappedEvent, new TappedEventHandler(OnShowDlg), true);
#else
            _grid.Tapped += OnShowDlg;
#endif
        }

        protected override void SetValBinding()
        {
            SetBinding(TextProperty, ValBinding);
        }

        protected override bool SetFocus()
        {
            if (_grid != null)
                OnShowDlg(null, null);
            return true;
        }

        protected override void Unload()
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
                _dlg.Dispose();
                _dlg = null;
            }
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
                    _dlg = new TreeDlg(this);
                }
                else
                {
                    _dlg = new TreeDlg(this)
                    {
                        WinPlacement = DlgPlacement.TargetBottomLeft,
                        PlacementTarget = _grid,
                        ClipElement = _grid,
                        MinHeight = 200,
                        MaxHeight = 400,
                        MaxWidth = 400,
                    };
                }
            }
            _dlg.ShowDlg();
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