#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2015-08-04
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
#endregion

namespace Dt.Shell
{
    sealed partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs p_args)
        {
            _ = Startup.Launch<Stub>(p_args.Arguments, null);
        }
    }
}