#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-06-14 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Mgr.Rbac;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Demo.Lob
{
    using A = 人员X;
    
    public partial class 人员List : List
    {
        public 人员List()
        {
            InitializeComponent();
            Menu = CreateMenu();

            var cm = CreateContextMenu();
            cm.Add(new Mi("绑定账号", Icons.多人, OnBindingUser));
            cm.Add(Mi.停用(click: OnStop));
            cm.Opening += OnMenuOpening;
            _lv.SetMenu(cm);

            _lv.ItemStyle = e =>
            {
                var x = e.Data as A;
                e.Foreground = x.撤档时间 != null ? Res.RedBrush : Res.BlackBrush;
            };
        }

        protected override async Task OnQuery()
        {
            if (_clause == null)
            {
                _lv.Data = await A.View1.Query(null);
            }
            else
            {
                var row = _clause.Fv.Row;
                string where = "where 1=1";
                if (row.Str("姓名") != "")
                    where += $" and 姓名 like %{row.Str("姓名")}%";
                if (row["部门id"] != null)
                    where += $" and id in (select 人员id from 部门人员 where 部门id={row.Long("部门id")})";
                _lv.Data = await A.View1.Query(where);
            }
        }

        async void OnStop(Mi mi)
        {
            var x = mi.Data as A;
            if (x.撤档时间 == null)
            {
                Dlg dlg = new Dlg { IsPinned = true, Title = "填写", Width = 300 };
                var sp = new StackPanel();
                TextBox tb = new TextBox { Height = 100, PlaceholderText = "撤档原因" };
                sp.Children.Add(tb);
                var btn = new Button { Content = "确认", HorizontalAlignment = HorizontalAlignment.Stretch };
                btn.Click += (s, e) => dlg.Close(true);
                sp.Children.Add(btn);
                dlg.Content = sp;

                if (await dlg.ShowAsync())
                {
                    x.撤档时间 = Kit.Now;
                    x.撤档原因 = tb.Text.Trim();
                }
            }
            else if (await Kit.Confirm("确认要启用吗？"))
            {
                x.撤档时间 = null;
                x.撤档原因 = null;
            }

            if (x.IsChanged && await x.Save(false))
            {
                await Refresh();
            }
        }

        void OnBindingUser(Mi mi)
        {
            _ = DoBindUser(mi.Data as A);
        }

        public static async Task<bool> DoBindUser(人员X x)
        {
            if (x.UserID != null)
            {
                if (await Kit.Confirm("确认要解除绑定的账号吗？"))
                {
                    // 解除绑定
                    x.UserID = null;
                    x["账号"] = "";

                    if (await x.Save(false))
                    {
                        Kit.Msg("已解除绑定的账号！");
                        return true;
                    }
                    Kit.Warn("解除绑定账号失败！");
                }
                return false;
            }

            var dlg = new 绑定账号Dlg();
            if (await dlg.Show(x.ID) && dlg.SelectedRow is UserX usr)
            {
                x.UserID = usr.ID;
                x["账号"] = usr.Acc == "" ? usr.Phone : usr.Acc;

                if (await x.Save(false))
                {
                    Kit.Msg("绑定账号成功！");
                    return true;
                }
                Kit.Warn("绑定账号失败！");
            }
            return false;
        }

        void OnMenuOpening(Menu menu, AsyncCancelArgs args)
        {
            var x = menu.TargetData as A;
            menu[2].ID = x.UserID == null ? "绑定账号" : "解除账号绑定";
            menu[3].ID = x.撤档时间 == null ? "停用" : "启用";
        }
    }
}