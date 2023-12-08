#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-03-09 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Mgr.Rbac
{
    public partial class RoleList : Tab
    {
        #region 构造方法
        public RoleList()
        {
            InitializeComponent();
        }
        #endregion

        #region 公开
        public async void Update()
        {
            if (Clause == null)
            {
                _lv.Data = await RoleX.Query(null);
            }
            else
            {
                var par = await Clause.Build<RoleX>();
                _lv.Data = await RoleX.Query(par.Sql, par.Params);
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
            NaviToChild();
            await _win.MainForm.Update(-1);
        }

        async void OnItemClick(ItemClickArgs e)
        {
            if (_lv.SelectionMode != Base.SelectionMode.Multiple)
            {
                NaviToChild();
                if (e.IsChanged)
                    await _win.MainForm.Update(e.Row.ID);
            }
        }

        void NaviToChild()
        {
            if (Kit.IsPhoneUI)
                NaviTo(new List<Tab> { _win.MainForm, _win.UserList, _win.MenuList, _win.PerList, _win.GroupList });
        }

        async void OnDel(Mi e)
        {
            if (!await Kit.Confirm("确认要删除选择的数据吗？"))
            {
                Kit.Msg("已取消删除！");
                return;
            }

            var d = e.Data.To<RoleX>();
            if (await d.Delete())
            {
                Update();
                if (d == _win.MainForm.Data)
                    _win.MainForm.Clear();
            }
        }
        #endregion

        #region 搜索
        /// <summary>
        /// 查询参数
        /// </summary>
        public QueryClause Clause { get; set; }

        void OnToSearch(Mi e)
        {
            if (_dlgQuery == null)
                CreateQueryDlg();
            _dlgQuery.Show();
        }

        void CreateQueryDlg()
        {
            var fs = new FuzzySearch();
            fs.Fixed.Add("全部");
            fs.CookieID = _win.GetType().FullName;
            fs.Search += (e) =>
            {
                Clause = new QueryClause(e);
                Update();
                _dlgQuery.Close();
            };

            _dlgQuery = new Dlg
            {
                Title = "搜索",
                IsPinned = true
            };

            if (!Kit.IsPhoneUI)
            {
                _dlgQuery.WinPlacement = DlgPlacement.CenterScreen;
                _dlgQuery.Width = Kit.ViewWidth / 4;
                _dlgQuery.Height = Kit.ViewHeight - 100;
                _dlgQuery.ShowVeil = true;
            }
            _dlgQuery.LoadTab(fs);
        }

        Dlg _dlgQuery;
        #endregion

        #region 内部
        RoleWin _win => (RoleWin)OwnWin;
        #endregion
    }
}