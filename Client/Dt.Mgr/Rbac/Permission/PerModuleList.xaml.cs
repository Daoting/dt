#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-10-26 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Mgr.Rbac
{
    public partial class PerModuleList : Tab
    {
        public PerModuleList()
        {
            InitializeComponent();
        }

        public async Task Update()
        {
            if (Clause == null)
            {
                _lv.Data = await PermissionModuleX.Query(null);
            }
            else
            {
                var par = await Clause.Build<PermissionModuleX>();
                _lv.Data = await PermissionModuleX.Query(par.Sql, par.Params);
            }
            UpdateRelated(-1);
        }

        protected override void OnFirstLoaded()
        {
            _ = Update();
        }

        void OnAdd()
        {
            ShowForm(-1);
        }

        void OnEdit(Mi e)
        {
            ShowForm(e.Row.ID);
        }

        void OnItemDoubleClick(object e)
        {
            ShowForm(e.To<Row>().ID);
        }

        async void ShowForm(long p_id)
        {
            var fm = new PerModuleForm();
            fm.Update(p_id);
            var dlg = new Dlg
            {
                IsPinned = true,
                ShowVeil = true,

            };
            dlg.LoadTab(fm);
            await dlg.ShowAsync();

            if (fm.IsChanged)
                await Update();
        }

        async void OnDel(Mi e)
        {
            if (!await Kit.Confirm("确认要删除吗？"))
            {
                Kit.Msg("已取消删除！");
            }
            else if (await e.Data.To<PermissionModuleX>().Delete())
            {
                await Update();
            }
        }

        void OnItemClick(ItemClickArgs e)
        {
            if (e.IsChanged)
                UpdateRelated(e.Row.ID);
            NaviTo(_win.FuncList);
        }

        void UpdateRelated(long p_id)
        {
            _win.FuncList.Update(p_id);
        }

        #region 搜索
        /// <summary>
        /// 获取设置查询内容
        /// </summary>
        public QueryClause Clause { get; set; }

        void OnQuery()
        {
            if (_dlgQuery == null)
            {
                var fs = new FuzzySearch();
                fs.Fixed.Add("全部");
                fs.CookieID = _win.GetType().FullName;
                fs.Search += (e) =>
                {
                    Clause = new QueryClause(e);
                    _ = Update();
                    _dlgQuery.Close();
                };

                _dlgQuery = new Dlg
                {
                    IsPinned = true,
                    ShowVeil = true
                };
                _dlgQuery.LoadTab(fs);
            }
            _dlgQuery.Show();
        }

        Dlg _dlgQuery;
        #endregion

        PerWin _win => (PerWin)OwnWin;
    }
}