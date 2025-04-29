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
    public sealed partial class BgJogDemo : Win
    {
        public BgJogDemo()
        {
            InitializeComponent();

            if (Kit.GetService<IBackgroundJob>() == null)
            {
                _tbJob.Text = "无后台任务";
                _cbBgJob.IsEnabled = false;
                _btnBgJob.IsEnabled = false;
            }
            else
            {
                Kit.RunAsync(async () => _cbBgJob.IsChecked = await CookieX.IsEnableBgJob());
            }
        }
        
        void OnToggleBgJob(object sender, RoutedEventArgs e)
        {
            _ = CookieX.SetEnableBgJob((bool)_cbBgJob.IsChecked);
        }

        void OnRunBgJob(object sender, RoutedEventArgs e)
        {
#if IOS
            BgJob.OnEnterBackground();
#else
            _ = BgJob.Run();
#endif
        }
    }
}
