#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-08-28 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Base.Tools
{
    public partial class ToggleSvcUrlDlg : Dlg
    {
        public ToggleSvcUrlDlg()
        {
            InitializeComponent();
            Init();
        }

        void Init()
        {
            _fv.Data = new Row { { "url", Kit.GetSvcUrl("cm") } };

            var rpc = Kit.GetRequiredService<IRpcConfig>();
            if (rpc != null && rpc.GetSvcUrlOptions() is List<string> ls)
            {
                ((CList)_fv["url"]).Data = new Nl<string>(ls);
            }
        }

        public static void ShowDlg()
        {
            var dlg = new ToggleSvcUrlDlg
            {
                IsPinned = true,
            };

            dlg.Show();
        }

        async void OnRestart(object sender, RoutedEventArgs e)
        {
            var url = _fv.Row.Str(0);
            if (url == "")
            {
                Kit.Warn("服务地址不可为空！");
                return;
            }
            if (url.Equals(Kit.GetSvcUrl("cm"), StringComparison.OrdinalIgnoreCase))
            {
                Kit.Warn("服务地址未修改，无需重启！");
                return;
            }

            await CookieX.Save("CustomSvcUrl", url);
#if WIN
            Microsoft.Windows.AppLifecycle.AppInstance.Restart(null);
#else
            Kit.Warn("关闭应用后重启有效！");
#endif

        }
    }
}