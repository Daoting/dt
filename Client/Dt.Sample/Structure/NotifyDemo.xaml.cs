#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Sample
{
    public sealed partial class NotifyDemo : Win
    {
        public NotifyDemo()
        {
            InitializeComponent();
        }

        void OnShowNotify(object sender, RoutedEventArgs e)
        {
            if ((bool)_cbWarning.IsChecked)
                Kit.Warn(_tbMessage.Text, (bool)_cbAutoClose.IsChecked ? 5 : 0);
            else
                Kit.Msg(_tbMessage.Text, (bool)_cbAutoClose.IsChecked ? 3 : 0);
        }

        void OnCustomNotify(object sender, RoutedEventArgs e)
        {
            Kit.Notify(GetInfo());
        }

        async void OnEditNotify(object sender, RoutedEventArgs e)
        {
            NotifyInfo info = Kit.Msg(_tbMessage.Text, -1);

            await Task.Delay(2000);
            info.Message = "3";
            info.NotifyType = NotifyType.Warning;
            await Task.Delay(2000);
            info.Message = "2";
            info.NotifyType = NotifyType.Information;
            info.Link = "查看";
            await Task.Delay(2000);
            info.Message = "1";
            info.NotifyType = NotifyType.Warning;
            info.Link = null;
            await Task.Delay(2000);
            Kit.CloseNotify(info);
        }

        NotifyInfo GetInfo()
        {
            NotifyInfo info = new NotifyInfo();
            info.NotifyType = (bool)_cbWarning.IsChecked ? NotifyType.Warning : NotifyType.Information;
            info.Message = _tbMessage.Text;
            info.Link = "查看详情";
            info.LinkCallback = OnLink;
            info.DelaySeconds = (bool)_cbAutoClose.IsChecked ? 3 : 0;
            return info;
        }

        void OnLink(NotifyInfo p_info)
        {
            Kit.Msg(string.Format("点击链接 [{0}]", p_info.Link));
        }

        void OnCommonToast(object sender, RoutedEventArgs e)
        {
            Kit.Toast("普通通知", "无启动参数\r\n" + DateTime.Now.ToString());
        }

        void OnParamsToast(object sender, RoutedEventArgs e)
        {
            Kit.Toast("带自启动参数的通知", "点击打开LvHome\r\n" + DateTime.Now.ToString(), new AutoStartInfo { WinType = typeof(LvHome).AssemblyQualifiedName, Title = "列表" });
        }
    }
}
