#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-03-06 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Mgr.Rbac
{
    public partial class UserList : Tab
    {
        #region 构造方法
        public UserList()
        {
            InitializeComponent();
            ToggleView(Kit.IsPhoneUI ? ViewMode.List : ViewMode.Table);
			_lv.ItemStyle = (e) =>
            {
                if (e.Data.To<UserX>().Expired)
                {
                    e.Foreground = Res.GrayBrush;
                    e.FontStyle = Windows.UI.Text.FontStyle.Italic;
                }
            };
        }
        #endregion

        #region 公开
        public async void Update()
        {
            if (Clause == null)
            {
                _lv.Data = await UserX.Query(null);
            }
            else
            {
                var par = await Clause.Build<UserX>();
                _lv.Data = await UserX.Query(par.Sql, par.Params);
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
                NaviTo(new List<Tab> { _win.MainForm, _win.GroupList, _win.RoleList });
        }

        async void OnResetPwd(object sender, Mi e)
        {
            var user = e.Data.To<UserX>();
            if (!await Kit.Confirm($"确认要重置[{user.Name}]的密码吗？\r\n密码会重置为手机号后4位！"))
            {
                Kit.Msg("已取消重置！");
                return;
            }

            var usr = new UserX(ID: user.ID);
            usr.IsAdded = false;
            string phone = user.Phone;
            usr.Pwd = Kit.GetMD5(phone.Substring(phone.Length - 4));

            if (await usr.Save(false))
                Kit.Msg("密码已重置为手机号后4位！");
            else
                Kit.Msg("重置密码失败！");
        }

        async void OnToggleExpired(object sender, Mi e)
        {
            var user = e.Data.To<UserX>();
            string act = user.Expired ? "启用" : "停用";
            if (!await Kit.Confirm($"确认要{act}[{user.Name}]吗？"))
            {
                Kit.Msg($"已取消{act}！");
                return;
            }

            var usr = new UserX(ID: e.Row.ID, Expired: user.Expired);
            usr.IsAdded = false;
            usr.Expired = !user.Expired;

            if (await usr.Save(false))
            {
                Kit.Msg($"账号[{e.Row.Str("name")}]已{act}！");
                Update();
            }
            else
            {
                Kit.Msg(act + "失败！");
            }
        }

        async void OnDel(object sender, Mi e)
        {
            var user = e.Data.To<UserX>();
            if (!await Kit.Confirm($"确认要删除[{user.Name}]吗？\r\n删除后不可恢复，请谨慎删除！"))
            {
                Kit.Msg("已取消删除！");
                return;
            }

            if (await user.Delete())
            {
                Update();
                if (user == _win.MainForm.Data)
                    _win.MainForm.Clear();
            }
        }

        void OnUserMenu(object sender, Mi e)
        {
            new UserMenuListDlg().Show(e.Row.ID);
        }

        void OnUserPrv(object sender, Mi e)
        {
            new UserPerListDlg().Show(e.Row.ID);
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

            var qs = new UserQuery();
            qs.Query += (s, e) =>
            {
                Clause = e;
                Update();
                _dlgQuery.Close();
            };
            tabs.Add(qs);

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
        UserWin _win => (UserWin)OwnWin;
        #endregion
    }
}