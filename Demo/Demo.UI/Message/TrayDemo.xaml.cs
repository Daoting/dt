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

namespace Demo.UI
{
    public sealed partial class TrayDemo : Win
    {
        public TrayDemo()
        {
            InitializeComponent();
        }
        
        void OnShowMsg(object sender, RoutedEventArgs e)
        {
            Kit.TrayMsg("通知的详情内容");
        }

        void OnShowWarn(object sender, RoutedEventArgs e)
        {
            Kit.TrayMsg("警告通知的详情内容", true);
        }
        
        void OnMoveToTray(object sender, RoutedEventArgs e)
        {
            NotifyInfo info = new NotifyInfo();
            info.Message = "未点击【查看详情】按钮关闭后放入托盘通知";
            info.Link = "查看详情";
            info.LinkCallback = e =>
            {
                Kit.Msg("详情内容");
                Kit.CloseNotify(e);
            };
            info.Delay = 3;
            info.ToTray = e => Kit.TrayMsg(e);
            Kit.Notify(info);
        }

        void OnShowSync(object sender, RoutedEventArgs e)
        {
            var ni = Kit.Msg("提示信息同步放入托盘通知");
            Kit.TrayMsg(ni);
        }
    }
}
