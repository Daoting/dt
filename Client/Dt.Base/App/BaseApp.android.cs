#if ANDROID
#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-12-14 创建
******************************************************************************/
#endregion

#region 引用命名
using Android.Content;
using Android.OS;
using Dt.Core;
using Microsoft.UI.Xaml;
using System;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 默认的Application行为
    /// </summary>
    public abstract class BaseApp : Application
    {
        string _params;

        /// <summary>
        /// 存根
        /// </summary>
        protected Stub _stub;

        protected override async void OnLaunched(LaunchActivatedEventArgs p_args)
        {
            if (string.IsNullOrEmpty(_params))
                _params = p_args.Arguments;

            await Startup.Launch(_stub, _params);
            _params = null;
        }

        public async void ReceiveShare(ShareInfo p_shareInfo)
        {
            await Startup.Launch(_stub, null, p_shareInfo);
        }

        public void ToastStart(string p_params)
        {
            // 点击通知栏启动
            if (Kit.Stub != null)
            {
                // 非null表示app已启动过，不会再调用 OnLaunched
                _ = Startup.Launch(_stub, p_params);
            }
            else
            {
                // 未启动，记录参数提供给 OnLaunched
                _params = p_params;
            }
        }
    }

    public class BaseAppActivity : ApplicationActivity
    {
        // 启动调用顺序：Application.OnCreate -> MainActivity.OnCreate -> App.OnLaunched

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            BgJob.MainActivity = GetType();

            // 确保 Permissions.RequestAsync 调用时正常
            Microsoft.Maui.Essentials.Platform.Init(this, bundle);

            var it = Intent;
            switch (it.Action)
            {
                case Intent.ActionMain:
                    // 普通启动，不传递参数
                    // 注册后台服务放在 App.OnLaunched，初始化结束后再注册！
                    break;

                case Intent.ActionSend:
                    // 接收分享内容
                    if (it.Type != null)
                        ReceiveShare();
                    break;

                case BgJob.ActionToast:
                    // 点击通知栏后，接收传递参数
                    var startInfo = it.GetStringExtra(BgJob.ActionToast);
                    if (!string.IsNullOrEmpty(startInfo))
                        ((BaseApp)Microsoft.UI.Xaml.Application.Current).ToastStart(startInfo);
                    break;
            }
        }

        void ReceiveShare()
        {
            var it = Intent;
            ShareInfo info = new ShareInfo();
            string tp = it.Type;

            if (tp.StartsWith("text/"))
            {
                info.DataType = ShareDataType.Text;
                info.Content = it.GetStringExtra(Intent.ExtraText);
            }
            else
            {
                if (tp.StartsWith("image/"))
                {
                    info.DataType = ShareDataType.Image;
                }
                else if (tp.StartsWith("video/"))
                {
                    info.DataType = ShareDataType.Video;
                }
                else if (tp.StartsWith("audio/"))
                {
                    info.DataType = ShareDataType.Audio;
                }
                else
                {
                    info.DataType = ShareDataType.File;
                }

                Android.Net.Uri uri = (Android.Net.Uri)it.GetParcelableExtra(Intent.ExtraStream);
                var path = IOUtil.GetPath(Android.App.Application.Context, uri);
                // 通过FileProvider分享时无法获取路径但可读取文件内容，参见ShareInfo.GetStream
                if (string.IsNullOrEmpty(path))
                    path = uri.ToString();
                info.FilePath = path;
            }
            ((BaseApp)Microsoft.UI.Xaml.Application.Current).ReceiveShare(info);
        }
    }
}
#endif