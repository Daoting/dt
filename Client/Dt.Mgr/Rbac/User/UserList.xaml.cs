#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-02-04 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Mgr.Rbac
{
    public partial class UserList : List
    {
        public UserList()
        {
            InitializeComponent();
            Menu = CreateMenu();
            _lv.ItemStyle = (e) =>
            {
                if (e.Data.To<UserX>().Expired)
                {
                    e.Foreground = Res.GrayBrush;
                    e.FontStyle = Windows.UI.Text.FontStyle.Italic;
                }
            };
        }
        
        protected override async Task OnQuery()
        {
            if (_clause == null)
            {
                _lv.Data = await UserX.Query(null);
            }
            else
            {
                var par = await _clause.Build<UserX>();
                _lv.Data = await UserX.Query(par.Sql, par.Params);
            }
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
            else if (await user.Save(false))
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
                await Refresh();
            }
            else
            {
                Kit.Msg(act + "失败！");
            }
        }

        protected override async void OnDel(Mi e)
        {
            var x = e.Data as UserX;
            if (x == null)
                x = _lv.SelectedItem as UserX;
            if (x == null)
                return;
            
            if (!await Kit.Confirm($"确认要删除[{x.Acc}]吗？\r\n删除后不可恢复，请谨慎删除！"))
            {
                Kit.Msg("已取消删除！");
                return;
            }

            if (await x.Delete())
            {
                await Refresh();
            }
        }
    }
}