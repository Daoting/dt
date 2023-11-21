#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-11-21 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.MgrDemo
{
    public partial class 虚拟List : Tab
    {
        #region 构造方法
        public 虚拟List()
        {
            InitializeComponent();
        }
        #endregion

        #region 公开
        public async void Update()
        {
            if (Clause == null)
            {
                _lv.Data = await VirX<主表X, 扩展1X, 扩展2X>.Query(null);
            }
            else
            {
                var par = await Clause.Build<VirX<主表X, 扩展1X, 扩展2X>>();
                _lv.Data = await VirX<主表X, 扩展1X, 扩展2X>.Query(par.Sql, par.Params);
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
                var ls = _lv.SelectedItems.Cast<VirX<主表X, 扩展1X, 扩展2X>>().ToList();
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
                var d = e.Data.To<VirX<主表X, 扩展1X, 扩展2X>>();
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

        #region 内部
        虚拟Win _win => OwnWin as 虚拟Win;
        #endregion
    }
}