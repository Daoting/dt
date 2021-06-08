#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-01-21 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.App.Model;
using Dt.Base;
using Dt.Core;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.App.Home
{
    /// <summary>
    /// 我的
    /// </summary>
    public sealed partial class MyMain : UserControl
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
                btn.Click += (s, e) => Kit.Login(false);
                Content = btn;
                Kit.LoginSuc += () => { Content = _fv; LoadInfo(); };
            }
        }

        async void LoadInfo()
        {
            _tbName.Text = Kit.UserName;
            _tbPhone.Text = Kit.UserPhone.Substring(0, 3) + "****" + Kit.UserPhone.Substring(7, 4);
            string photo = string.IsNullOrEmpty(Kit.UserPhoto) ? Kit.DefaultUserPhoto : Kit.UserPhoto;
            await ImgKit.LoadImage(photo, _img);
        }

        void OnExit(object sender, RoutedEventArgs e)
        {
            Kit.Logout();
        }

        async void OnClearLocalFile(object sender, RoutedEventArgs e)
        {
            if (await Kit.Confirm("清除缓存后再次用到时需要重新下载，建议存储空间充足时不必清除。\r\n确认要清除吗？"))
            {
                Kit.ClearCacheFiles();
                Kit.Msg("清除完毕！");
            }
        }

        async void OnEditInfo(object sender, EventArgs e)
        {
            var dlg = new EditUserDlg();
            if (await dlg.Show(Kit.UserID, false))
            {
                Row row = dlg.Info;
                Kit.UserName = row.Str("name");
                Kit.UserPhone = row.Str("phone");
                Kit.UserPhoto = row.Str("photo");
                LoadInfo();
            }
        }

        void OnSetting(object sender, EventArgs e)
        {
            Type tp = Kit.GetViewType("我的设置");
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

        async void OnAbout(object sender, EventArgs e)
        {
            var b = await Kit.GetParam<string>("接收新任务");
            var c = await Kit.GetParam<bool>("接收新发布通知");
        }
    }
}
