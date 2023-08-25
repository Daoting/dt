#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-06-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.MgrDemo.单实体
{
    public partial class 实体List : Tab
    {
        #region 构造方法
        public 实体List()
        {
            InitializeComponent();
            ToggleView(Kit.IsPhoneUI ? ViewMode.List : ViewMode.Table);
        }
        #endregion

        #region 公开
        public async void Update()
        {
            if (Clause == null)
            {
                _lv.Data = await 基础X.Query(null);
            }
            else
            {
                var par = await Clause.Build<基础X>();
                _lv.Data = await 基础X.Query(par.Sql, par.Params);
            }
        }
        #endregion

        #region 初始化 
        protected override void OnFirstLoaded()
        {
            Update();
        }
        #endregion

        #region 交互
        async void OnAdd(object sender, Mi e)
        {
            NaviTo(_win.Form);
            await _win.Form.Update(-1);
        }

        async void OnItemClick(object sender, ItemClickArgs e)
        {
            if (_lv.SelectionMode != SelectionMode.Multiple)
            {
                NaviTo(_win.Form);
                if (e.IsChanged)
                    await _win.Form.Update(e.Row.ID);
            }
        }

        async void OnDel(object sender, Mi e)
        {
            if (!await Kit.Confirm("确认要删除选择的数据吗？"))
            {
                Kit.Msg("已取消删除！");
                return;
            }

            if (_lv.SelectionMode == SelectionMode.Multiple)
            {
                var ls = _lv.SelectedItems.Cast<基础X>().ToList();
                if (await ls.Delete())
                {
                    Update();
                    var d = _win.Form.Data;
                    if (d != null)
                    {
                        foreach (var item in ls)
                        {
                            if (item == d)
                            {
                                _win.Form.Clear();
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                var d = e.Data.To<基础X>();
                if (await d.Delete())
                {
                    Update();
                    if (d == _win.Form.Data)
                        _win.Form.Clear();
                }
            }
        }
        #endregion

        #region 搜索
        /// <summary>
        /// 获取设置查询内容
        /// </summary>
        public QueryClause Clause { get; set; }

        public void OnSearch(QueryClause p_clause)
        {
            Clause = p_clause;
            Update();
            NaviTo(this);
        }

        void OnToSearch(object sender, Mi e)
        {
            NaviTo(new List<Tab> { _win.Search, _win.Query });
        }
        #endregion

        #region 选择
        void OnSelectAll(object sender, Mi e)
        {
            _lv.SelectAll();
        }

        void OnMultiMode(object sender, Mi e)
        {
            _lv.SelectionMode = SelectionMode.Multiple;
            Menu.HideExcept("删除", "全选", "取消");
        }

        void OnCancelMulti(object sender, Mi e)
        {
            _lv.SelectionMode = SelectionMode.Single;
            Menu.ShowExcept("删除", "全选", "取消");
        }
        #endregion

        #region 视图
        void OnToggleView(object sender, Mi e)
        {
            ToggleView(_lv.ViewMode == ViewMode.List ? ViewMode.Table : ViewMode.List);
        }

        void ToggleView(ViewMode p_mode)
        {
            if (p_mode == ViewMode.List)
            {
                _lv.ChangeView(Resources["ListView"], ViewMode.List);
                _mi.Icon = Icons.表格;
                _mi.ID = "表格";
            }
            else
            {
                _lv.ChangeView(Resources["TableView"], ViewMode.Table);
                _mi.Icon = Icons.列表;
                _mi.ID = "列表";
            }
        }
        #endregion

        #region 内部
        实体Win _win => OwnWin as 实体Win;
        #endregion
    }
}