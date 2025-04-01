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
using System.Reflection;
using System.Xml;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 简单选择器
    /// </summary>
    [ContentProperty(Name = nameof(View))]
    public partial class CPick : FvCell
    {
        #region 静态内容
        public readonly static DependencyProperty SqlProperty = DependencyProperty.Register(
            "Sql",
            typeof(Sql),
            typeof(CPick),
            new PropertyMetadata(null, OnClearData));

        public readonly static DependencyProperty SrcIDProperty = DependencyProperty.Register(
            "SrcID",
            typeof(string),
            typeof(CPick),
            new PropertyMetadata(null));

        public readonly static DependencyProperty TgtIDProperty = DependencyProperty.Register(
            "TgtID",
            typeof(string),
            typeof(CPick),
            new PropertyMetadata(null));

        public static readonly DependencyProperty ShowButtonProperty = DependencyProperty.Register(
            "ShowButton",
            typeof(bool),
            typeof(CPick),
            new PropertyMetadata(false));

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text",
            typeof(string),
            typeof(CPick),
            new PropertyMetadata(null));

        public static readonly DependencyProperty PlaceholderProperty = DependencyProperty.Register(
            "Placeholder",
            typeof(string),
            typeof(CPick),
            new PropertyMetadata(null));
        
        static void OnClearData(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ls = (CPick)d;
            if (ls._dlg != null)
                ls._lv.ClearValue(Lv.DataProperty);
        }
        #endregion

        #region 成员变量
        readonly Lv _lv;
        Grid _grid;
        PickDlg _dlg;
        string[] _srcIDs;
        string[] _tgtIDs;
        #endregion

        #region 构造方法
        public CPick()
        {
            DefaultStyleKey = typeof(CPick);
            _lv = new Lv();
            _lv.Toolbar = new Menu();
        }
        #endregion

        #region 事件
        /// <summary>
        /// 查询事件
        /// </summary>
        public event Action<CPick, string> Search;

        /// <summary>
        /// 选择前事件，支持异步，可取消选择
        /// </summary>
        public event Action<CPick, AsyncCancelArgs> Picking;

        /// <summary>
        /// 选择后事件
        /// </summary>
        public event Action<CPick> Picked;

        /// <summary>
        /// 筛选按钮事件
        /// </summary>
        public event Action<CPick> BtnClick;
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
        /// 获取设置是否显示筛选按钮
        /// </summary>
        [CellParam("显示筛选按钮")]
        public bool ShowButton
        {
            get { return (bool)GetValue(ShowButtonProperty); }
            set { SetValue(ShowButtonProperty, value); }
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
        /// 下拉框
        /// </summary>
        public PickDlg Dlg => _dlg;

        internal double CurrentWidth => _grid.ActualWidth > 0 ? _grid.ActualWidth : 300;
        
        protected override IFvCall DefaultMiddle => new ListValConverter();
        #endregion

        #region 重写方法
        protected override void OnApplyCellTemplate()
        {
            _grid = (Grid)GetTemplateChild("Grid");
            
            var border = (Border)GetTemplateChild("TextBorder");
#if WIN
            // TextBlock可复制
            border.AddHandler(TappedEvent, new TappedEventHandler(OnShowDlg), true);
#else
            border.Tapped += OnShowDlg;
#endif

            var btn = (Button)GetTemplateChild("BtnFilter");
            if (btn != null)
            {
                btn.Click -= OnButtonClick;
                btn.Click += OnButtonClick;
            }

            // 拆分填充列
            if (!string.IsNullOrEmpty(SrcID) && !string.IsNullOrEmpty(TgtID))
            {
                _srcIDs = SrcID.Split(new string[] { "#" }, StringSplitOptions.RemoveEmptyEntries);
                _tgtIDs = TgtID.Split(new string[] { "#" }, StringSplitOptions.RemoveEmptyEntries);
                if (_srcIDs.Length != _tgtIDs.Length)
                {
                    _srcIDs = null;
                    _tgtIDs = null;
                    Kit.Error("数据填充：源列表、目标列表列个数不一致！");
                }
            }
        }

        protected override void SetValBinding()
        {
            SetBinding(TextProperty, ValBinding);
        }

        protected override bool SetFocus()
        {
            if (_grid != null && !ReadOnlyBinding)
            {
                OnShowDlg(null, null);
                return true;
            }
            return false;
        }

        public override void Destroy()
        {
            var border = (Border)GetTemplateChild("TextBorder");
            if (border != null)
            {
#if WIN
                // TextBlock可复制
                border.RemoveHandler(TappedEvent, new TappedEventHandler(OnShowDlg));
#else
                border.Tapped -= OnShowDlg;
#endif
            }

            var btn = (Button)GetTemplateChild("BtnFilter");
            if (btn != null)
            {
                btn.Click -= OnButtonClick;
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

            if (_dlg != null && _dlg.IsOpened)
            {
                _dlg.BringToTop();
                return;
            }

            if (_dlg == null)
            {
                if (Kit.IsPhoneUI)
                {
                    _dlg = new PickDlg(this);
                }
                else
                {
                    _dlg = new PickDlg(this)
                    {
                        WinPlacement = DlgPlacement.TargetTopLeft,
                        PlacementTarget = _grid,
                        MaxHeight = 400,
                    };
                }
            }
            _dlg.Show();
        }

        void OnButtonClick(object sender, RoutedEventArgs e)
        {
            BtnClick?.Invoke(this);
        }

        internal void OnSearch(string p_txt)
        {
            Search?.Invoke(this, p_txt);
        }
        #endregion

        #region 选择
        internal async void OnPicking()
        {
            if (_lv.SelectedItem == null)
                return;

            // 外部判断选择项是否允许
            if (Picking != null)
            {
                var args = new AsyncCancelArgs();
                Picking(this, args);
                await args.EnsureAllCompleted();
                if (args.Cancel)
                    return;
            }

            if (_lv.SelectionMode == SelectionMode.Single)
            {
                // 同步填充目标Cell的值
                FillCells(_lv.SelectedItem);
            }

            Picked?.Invoke(this);
            _dlg.Close();
            Owner.GotoNextCell(this);
        }

        /// <summary>
        /// 同步填充目标Cell的值
        /// </summary>
        /// <param name="p_selectedItem"></param>
        public void FillCells(object p_selectedItem)
        {
            // 拆分填充列
            if (_srcIDs == null)
                SplitIDs();

            if (_srcIDs == null)
                return;
            
            // 同步填充
            if (p_selectedItem is Row srcRow)
            {
                object tgtObj = Owner.Data;
                Row tgtRow = tgtObj as Row;
                for (int i = 0; i < _srcIDs.Length; i++)
                {
                    string srcID = _srcIDs[i];
                    // - 代表当前列值
                    string tgtID = _tgtIDs[i] == "-" ? ID : _tgtIDs[i];

                    if (srcRow.Contains(srcID) || srcID == "-")
                    {
                        object tgtVal = null;
                        if (srcRow.Contains(srcID))
                            tgtVal = srcRow[srcID];
                        
                        if (tgtRow != null)
                        {
                            if (tgtRow.Contains(tgtID))
                                tgtRow[tgtID] = tgtVal;
                        }
                        else
                        {
                            var pi = tgtObj.GetType().GetProperty(tgtID, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                            if (pi != null)
                                pi.SetValue(tgtObj, tgtVal);
                        }

                        // 对于联动的CList CPick CTree，清空其下拉数据源
                        if (srcID == "-")
                        {
                            var cell = Owner[tgtID];
                            if (cell is CList cl)
                                cl.Data = null;
                            else if (cell is CPick cp)
                                cp.Data = null;
                            else if (cell is CTree ct)
                                ct.Data = null;
                        }
                    }
                }
            }
            else
            {
                object tgtObj = Owner.Data;
                Row tgtRow = tgtObj as Row;
                for (int i = 0; i < _srcIDs.Length; i++)
                {
                    string srcID = _srcIDs[i];
                    PropertyInfo srcPi = null;
                    if (srcID == "-"
                        || (srcPi = p_selectedItem.GetType().GetProperty(srcID, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)) != null)
                    {
                        string tgtID = _tgtIDs[i] == "-" ? ID : _tgtIDs[i];
                        object tgtVal = srcPi == null ? null : srcPi.GetValue(p_selectedItem);

                        if (tgtRow != null)
                        {
                            if (tgtRow.Contains(tgtID))
                                tgtRow[tgtID] = tgtVal;
                        }
                        else
                        {
                            var tgtPi = tgtObj.GetType().GetProperty(tgtID, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                            if (tgtPi != null)
                                tgtPi.SetValue(tgtObj, tgtVal);
                        }

                        // 对于联动的CList CPick CTree，清空其下拉数据源
                        if (srcID == "-")
                        {
                            var cell = Owner[tgtID];
                            if (cell is CList cl)
                                cl.Data = null;
                            else if (cell is CPick cp)
                                cp.Data = null;
                            else if (cell is CTree ct)
                                ct.Data = null;
                        }
                    }
                }
            }
        }
        
        void SplitIDs()
        {
            if (!string.IsNullOrEmpty(SrcID))
            {
                _srcIDs = SrcID.Split(new string[] { "#" }, StringSplitOptions.RemoveEmptyEntries);

                if (!string.IsNullOrEmpty(TgtID))
                {
                    _tgtIDs = TgtID.Split(new string[] { "#" }, StringSplitOptions.RemoveEmptyEntries);
                }
                else if (_srcIDs.Length == 1)
                {
                    // 默认填充当前列
                    _tgtIDs = new string[] { ID };
                }
                else
                {
                    _srcIDs = null;
                    Throw.Msg(ID + _error);
                }

                if (_srcIDs.Length != _tgtIDs.Length)
                {
                    _srcIDs = null;
                    _tgtIDs = null;
                    Throw.Msg(ID + _error);
                }
            }
            else
            {
                if (_lv.Data is Table tbl)
                {
                    // 取name列 或 第一列作为源列名
                    _srcIDs = new string[] { tbl.Columns.Contains("name") ? "name" : tbl.Columns[0].ID };
                    _tgtIDs = new string[] { ID };
                }
                else if (_lv.Data.Count > 0
                    && _lv.Data[0].GetType().GetProperty("name", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance) is PropertyInfo pi)
                {
                    _srcIDs = new string[] { "name" };
                    _tgtIDs = new string[] { ID };
                }
                else
                {
                    Throw.Msg(ID + _error);
                }
            }
        }

        const string _error = " 单元格填充时源列表、目标列表列个数不一致！";
        #endregion
    }
}