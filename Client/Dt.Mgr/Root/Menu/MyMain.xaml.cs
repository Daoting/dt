#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-01-21 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Dt.Mgr.Rbac;
#endregion

namespace Dt.Mgr.Home
{
    /// <summary>
    /// 我的
    /// </summary>
    public sealed partial class MyMain : Tab
    {
        public MyMain()
        {
            InitializeComponent();

            if (Kit.IsLogon)
            {
                LoadInfo();
            }
            else
            {
                var btn = new Button { Content = "点击登录", HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
                btn.Click += (s, e) => Kit.ShowRoot(LobViews.登录页);
                Content = btn;

                LoginDs.LoginSuc += () =>
                {
                    Content = _fv;
                    LoadInfo();
                };
            }
        }

        async void LoadInfo()
        {
            _tbName.Text = Kit.UserName;
            string photo = string.IsNullOrEmpty(Kit.UserPhoto) ? Kit.DefaultUserPhoto : Kit.UserPhoto;
            await Kit.LoadImage(photo, _img);
        }

        void OnExit(object sender, RoutedEventArgs e)
        {
            LoginDs.Logout();
        }

        async void OnClearLocalFile(object sender, RoutedEventArgs e)
        {
            if (await Kit.Confirm("清除缓存后再次用到时需要重新下载，建议存储空间充足时不必清除。\r\n确认要清除吗？"))
            {
                Kit.ClearCacheFiles();
                Kit.Msg("清除完毕！");
            }
        }

        async void OnEditInfo()
        {
            var edit = new UserForm();
            if (await edit.Open(Kit.UserID, false))
            {
                LoginDs.UpdateUserInfo(edit.Data);
                LoadInfo();
            }
        }

        void OnSetting()
        {
            Type tp = Kit.GetViewTypeByAlias("我的设置");
            if (tp == null)
            {
                Kit.Msg("未找到设置视图！");
                return;
            }

            var dlg = new Dlg { Title = "设置" };
            if (!Kit.IsPhoneUI)
            {
                dlg.Width = 400;
                dlg.Height = 500;
            }
            dlg.Content = Activator.CreateInstance(tp);
            dlg.Show();
        }

        void OnChangePwd()
        {

        }
    }
}
