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
        string _params;

        public App()
        {
            InitializeComponent();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs p_args)
        {
            string args = string.IsNullOrEmpty(_params) ? p_args.Arguments : _params;
            _ = Startup.Launch<Stub>(args, null);
            _params = null;
        }

#if UWP
        protected override async void OnShareTargetActivated(ShareTargetActivatedEventArgs p_args)
        {
            var info = new ShareInfo();
            await info.Init(p_args.ShareOperation);
            await Startup.Launch<Stub>(null, info);
        }
#elif __IOS__
        public override bool OpenUrl(UIKit.UIApplication p_app, Foundation.NSUrl p_url, Foundation.NSDictionary p_options)
        {
            var doc = new UIKit.UIDocument(p_url);
            string path = doc.FileUrl?.Path;
            if (!string.IsNullOrEmpty(path))
                _ = Startup.Launch<Stub>(null, new ShareInfo(path));

            return true;
        }
#elif ANDROID
        public async void ReceiveShare(ShareInfo p_shareInfo)
        {
            await Startup.Launch<Stub>(null, p_shareInfo);
        }

        public void ToastStart(string p_params)
        {
            if (Kit.Stub != null)
            {
                // 非null表示app已启动过，不会再调用 OnLaunched
                _ = Startup.Launch<Stub>(p_params, null);
            }
            else
            {
                // 未启动，记录参数提供给 OnLaunched
                _params = p_params;
            }
        }
#endif
    }
}