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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Sample
{
    public sealed partial class NotifyDemo : PageWin
    {
        public NotifyDemo()
        {
            InitializeComponent();
        }

        void OnShowNotify(object sender, RoutedEventArgs e)
        {
            if ((bool)_cbWarning.IsChecked)
                AtKit.Warn(_tbMessage.Text, (bool)_cbAutoClose.IsChecked);
            else
                AtKit.Msg(_tbMessage.Text, (bool)_cbAutoClose.IsChecked);
        }

        void OnCustomNotify(object sender, RoutedEventArgs e)
        {
            AtKit.Notify(GetInfo());
        }

        NotifyInfo GetInfo()
        {
            NotifyInfo info = new NotifyInfo();
            info.NotifyType = (bool)_cbWarning.IsChecked ? NotifyType.Warning : NotifyType.Information;
            info.Message = _tbMessage.Text;
            info.Link = "查看详情";
            info.LinkCallback = OnLink;
            info.AutoClose = (bool)_cbAutoClose.IsChecked;
            return info;
        }

        void OnLink(NotifyInfo p_info)
        {
            AtKit.Msg(string.Format("点击链接 [{0}]", p_info.Link));
        }
    }
}
