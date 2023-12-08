#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-03-14 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Mgr.Module
{
    public partial class ParamsList : Tab
    {
        #region 构造方法
        public ParamsList()
        {
            InitializeComponent();
        }
        #endregion

        #region 公开
        public async void Update()
        {
            if (Clause == null)
            {
                _lv.Data = await ParamsX.Query(null);
            }
            else
            {
                var par = await Clause.Build<ParamsX>();
                _lv.Data = await ParamsX.Query(par.Sql, par.Params);
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
        async void OnAdd(Mi e)
        {
            NaviTo(_win.Form);
            await _win.Form.Update(-1);
        }

        async void OnItemClick(ItemClickArgs e)
        {
            if (_lv.SelectionMode != Base.SelectionMode.Multiple)
            {
                NaviTo(_win.Form);
                if (e.IsChanged)
                    await _win.Form.Update(e.Row.ID);
            }
        }

        async void OnDel(Mi e)
        {
            if (!await Kit.Confirm("确认要删除选择的数据吗？"))
            {
                Kit.Msg("已取消删除！");
                return;
            }

            if (_lv.SelectionMode == Base.SelectionMode.Multiple)
            {
                var ls = _lv.SelectedItems.Cast<ParamsX>().ToList();
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
                var d = e.Data.To<ParamsX>();
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

        void OnToSearch(Mi e)
        {
            NaviTo(_win.Search);
        }
        #endregion

        #region 选择
        void OnSelectAll(Mi e)
        {
            _lv.SelectAll();
        }

        void OnMultiMode(Mi e)
        {
            _lv.SelectionMode = Base.SelectionMode.Multiple;
            Menu.HideExcept("删除", "全选", "取消");
        }

        void OnCancelMulti(Mi e)
        {
            _lv.SelectionMode = Base.SelectionMode.Single;
            Menu.ShowExcept("删除", "全选", "取消");
        }
        #endregion

        #region 内部
        ParamsWin _win => (ParamsWin)OwnWin;
        #endregion
    }
}