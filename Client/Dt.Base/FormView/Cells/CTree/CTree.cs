#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-10-29 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.FormView;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Markup;
using System.Xml;
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

        public static readonly DependencyProperty PlaceholderProperty = DependencyProperty.Register(
            "Placeholder",
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

        #region Design
        /// <summary>
        /// 设计时用，行视图的xaml
        /// </summary>
        public string ViewXaml
        {
            get { return _tv.ViewXaml; }
            set { _tv.ViewXaml = value; }
        }
        
        protected override void ExportCustomXaml(XmlWriter p_xw)
        {
            if (!string.IsNullOrEmpty(ViewXaml))
            {
                FvDesignKit.CopyXml(p_xw, ViewXaml);
            }
            
            if (Sql == null || string.IsNullOrEmpty(Sql.SqlStr))
                return;

            p_xw.WriteStartElement("a", "CTree.Sql", null);
            p_xw.WriteStartElement("a", "Sql", null);

            if (!string.IsNullOrEmpty(Sql.LocalDb))
                p_xw.WriteAttributeString("LocalDb", Sql.LocalDb);
            if (!string.IsNullOrEmpty(Sql.Svc))
                p_xw.WriteAttributeString("Svc", Sql.Svc);
            p_xw.WriteCData(Sql.SqlStr);

            p_xw.WriteEndElement();
            p_xw.WriteEndElement();
        }
        
        public override void AddCustomDesignCells(FvItems p_items)
        {
            AddViewDesignCells(p_items);
            
            // 空时无法绑定
            if (Sql == null)
                Sql = new Sql();

            CList.AddSqlDesignCells(p_items);
        }

        public override void LoadXamlString(XmlNode p_node)
        {
            for (int i = 0; i < p_node.ChildNodes.Count; i++)
            {
                var node = p_node.ChildNodes[i];
                if (!node.LocalName.StartsWith("CTree."))
                {
                    ViewXaml = FvDesignKit.GetNodeXml(node, false);
                }
            }
        }
        
        void AddViewDesignCells(FvItems p_items)
        {
            CBar bar = new CBar { Title = "行视图" };
            p_items.Add(bar);

            CText text = new CText
            {
                ID = "ViewXaml",
                ShowTitle = false,
                AcceptsReturn = true,
                RowSpan = 6,
            };
            p_items.Add(text);

            var btn = new Button { Content = "+模板", HorizontalAlignment = HorizontalAlignment.Right };
            Menu m = new Menu { Placement = MenuPosition.BottomLeft };
            Mi mi = new Mi { ID = "单列模板" };
            mi.Call += () => FvDesignKit.AddXamlToCText(text, "<DataTemplate>\n  <a:Dot ID=\"name\" />\n</DataTemplate>");
            m.Add(mi);
            
            mi = new Mi { ID = "多列模板" };
            mi.Call += () => FvDesignKit.AddXamlToCText(text,
@"<DataTemplate>
    <Grid Padding=""6"">
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width=""50"" />
            <ColumnDefinition Width=""*"" />
        </Grid.ColumnDefinitions>
        <a:Dot ID=""icon"" />
        <StackPanel Margin=""10,0,0,0"" Grid.Column=""1"">
            <a:Dot ID=""xm"" />
            <a:Dot ID=""xb"" />
        </StackPanel>
    </Grid>
</DataTemplate>");
            m.Add(mi);

            Dt.Base.Ex.SetMenu(btn, m);
            bar.Content = btn;
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