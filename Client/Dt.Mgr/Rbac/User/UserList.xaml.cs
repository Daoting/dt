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
                NaviTo(new List<Tab> { _win.MainForm, _win.GroupList, _win.RoleList });
        }

        async void OnResetPwd(Mi e)
        {
            var user = e.Data.To<UserX>();
            if (!await Kit.Confirm($"确认要重置[{user.Acc}]的密码吗？\r\n密码会重置为4个1！"))
            {
                Kit.Msg("已取消重置！");
                return;
            }

            user.IsAdded = false;
            string phone = user.Phone;
            user.Pwd = Kit.GetMD5("1111");

            if (!user.cPwd.IsChanged)
            {
                Kit.Msg("密码为4个1，无需重置！");
            }
            else if(await user.Save(false))
            {
                Kit.Msg("密码已重置为4个1！");
            }
            else
            {
                Kit.Msg("重置密码失败！");
            }
        }

        async void OnToggleExpired(Mi e)
        {
            var user = e.Data.To<UserX>();
            string act = user.Expired ? "启用" : "停用";
            if (!await Kit.Confirm($"确认要{act}[{user.Acc}]吗？"))
            {
                Kit.Msg($"已取消{act}！");
                return;
            }

            user.IsAdded = false;
            user.Expired = !user.Expired;

            if (await user.Save(false))
            {
                Kit.Msg($"账号[{e.Row.Str("acc")}]已{act}！");
                Update();
            }
            else
            {
                Kit.Msg(act + "失败！");
            }
        }

        async void OnDel(Mi e)
        {
            var user = e.Data.To<UserX>();
            if (!await Kit.Confirm($"确认要删除[{user.Acc}]吗？\r\n删除后不可恢复，请谨慎删除！"))
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

        void OnUserMenu(Mi e)
        {
            new UserMenuListDlg().Show(e.Row.ID);
        }

        void OnUserPrv(Mi e)
        {
            new UserPerListDlg().Show(e.Row.ID);
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
            var tabs = new List<Tab>();
            var fs = new FuzzySearch();
            fs.Fixed.Add("全部");
            fs.CookieID = _win.GetType().FullName;
            fs.Search += (e) =>
            {
                Clause = new QueryClause(e);
                Update();
                _dlgQuery.Close();
            };
            tabs.Add(fs);

            var qs = new UserQuery();
            qs.Query += (e) =>
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

        #region 内部
        UserWin _win => (UserWin)OwnWin;
        #endregion
    }
}