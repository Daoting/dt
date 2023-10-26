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

namespace Dt.MgrDemo
{
    public partial class 权限List : Tab
    {
        #region 构造方法
        public 权限List()
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
                _lv.Data = await 权限X.Query(null);
            }
            else
            {
                var par = await Clause.Build<权限X>();
                _lv.Data = await 权限X.Query(par.Sql, par.Params);
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
            NaviToChild();
            await _win.MainForm.Update(-1);
        }

        async void OnItemClick(object sender, ItemClickArgs e)
        {
            if (_lv.SelectionMode != SelectionMode.Multiple)
            {
                NaviToChild();
                if (e.IsChanged)
                    await _win.MainForm.Update(e.Row.ID);
            }
        }

        void NaviToChild()
        {
            if (Kit.IsPhoneUI)
                NaviTo(new List<Tab> { _win.MainForm, _win.角色List });
        }

        async void OnDel(object sender, Mi e)
        {
            if (!await Kit.Confirm("确认要删除选择的数据吗？"))
            {
                Kit.Msg("已取消删除！");
                return;
            }

            var d = e.Data.To<权限X>();
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

        void OnToSearch(object sender, Mi e)
        {
            if (_dlgQuery == null)
                CreateQueryDlg();
            _dlgQuery.Show();
        }

        void CreateQueryDlg()
        {
            var tabs = new List<Tab>();
            var fs = new FuzzySearch();
            fs.Fixed.Add("全部");
            fs.CookieID = _win.GetType().FullName;
            fs.Search += (s, e) =>
            {
                Clause = new QueryClause(e);
                Update();
                _dlgQuery.Close();
            };
            tabs.Add(fs);

            var qs = new 权限Query();
            qs.Query += (s, e) =>
            {
                Clause = e;
                Update();
                _dlgQuery.Close();
            };
            tabs.Add(qs);

            _dlgQuery = new Dlg
            {
                IsPinned = true,
                ShowVeil = true
            };
            _dlgQuery.LoadTabs(tabs);
        }

        Dlg _dlgQuery;
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
        权限Win _win => OwnWin as 权限Win;
        #endregion
    }
}