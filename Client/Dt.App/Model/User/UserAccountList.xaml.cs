#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dt.App.Model
{
    public partial class UserAccountList : Mv
    {
        string _query;

        public UserAccountList()
        {
            InitializeComponent();
            _lv.View = Resources[Kit.IsPhoneUI ? "TileView" : "TableView"];
            _lv.CellEx = typeof(UserViewEx);
        }

        public async void Update()
        {
            if (string.IsNullOrEmpty(_query) || _query == "#全部")
            {
                _lv.Data = await AtCm.Query<UserObj>("用户-所有");
            }
            else if (_query == "#最近修改")
            {
                _lv.Data = await AtCm.Query<UserObj>("用户-最近修改");
            }
            else
            {
                _lv.Data = await AtCm.Query<UserObj>("用户-模糊查询", new { input = $"%{_query}%" });
            }
        }

        protected override void OnInit(object p_params)
        {
            Update();
        }

        async void OnToSearch(object sender, Mi e)
        {
            var txt = await Forward<string>(_lzSm.Value);
            if (!string.IsNullOrEmpty(txt))
            {
                _query = txt;
                Title = "用户列表 - " + txt;
                Update();
            }
        }

        Lazy<SearchMv> _lzSm = new Lazy<SearchMv>(() => new SearchMv
        {
            Placeholder = "姓名或手机号",
            Fixed = { "全部", "最近修改", },
        });

        void OnAdd(object sender, Mi e)
        {
            _win.Form.Update(-1);
            NaviTo(new List<Mv> { _win.Form, _win.RoleList });
        }

        void OnItemClick(object sender, ItemClickArgs e)
        {
            if (e.IsChanged)
                _win.Form.Update(e.Row.ID);
            NaviTo(new List<Mv> { _win.Form, _win.RoleList });
        }

        async void OnResetPwd(object sender, Mi e)
        {
            var usr = new UserObj(ID: e.Row.ID);
            usr.IsAdded = false;
            string phone = e.Row.Str("phone");
            usr.Pwd = Kit.GetMD5(phone.Substring(phone.Length - 4));

            if (await AtCm.Save(usr, false))
                Kit.Msg("密码已重置为手机号后4位！");
            else
                Kit.Msg("重置密码失败！");
        }

        async void OnToggleExpired(object sender, Mi e)
        {
            bool expired = e.Row.Bool("expired");
            var usr = new UserObj(ID: e.Row.ID, Expired: expired);
            usr.IsAdded = false;
            usr.Expired = !expired;

            string act = expired ? "启用" : "停用";
            if (await AtCm.Save(usr, false))
            {
                Kit.Msg($"账号[{e.Row.Str("name")}]已{act}！");
                Update();
            }
            else
            {
                Kit.Msg(act + "失败！");
            }
        }

        async void OnDelUser(object sender, Mi e)
        {
            var isSelected = _lv.SelectedItem == e.Data;
            UserObj user = e.Data.To<UserObj>();
            if (!await Kit.Confirm($"确认要删除[{user.Name}]吗？"))
            {
                Kit.Msg("已取消删除！");
                return;
            }

            if (await AtCm.DeleteBySvc(user))
            {
                // 删除的为选择行时，清空关联Mv
                if (isSelected)
                    _win.Form.Clear();
                Update();
            }
        }

        void OnUserMenu(object sender, Mi e)
        {
            Forward(new UserMenuList(), e.Row.ID);
        }

        void OnUserPrv(object sender, Mi e)
        {
            Forward(new UserPrvList(), e.Row.ID);
        }

        UserAccountWin _win => (UserAccountWin)_tab.OwnWin;

        class UserViewEx
        {
            public static void SetStyle(ViewItem p_item)
            {
                if (p_item.Row.Bool("expired"))
                {
                    p_item.Foreground = Res.GrayBrush;
                    p_item.FontStyle = Windows.UI.Text.FontStyle.Italic;
                }
            }
        }
    }
}