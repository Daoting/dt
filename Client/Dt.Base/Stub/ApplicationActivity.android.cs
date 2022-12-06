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
using Microsoft.UI.Xaml;
using System.Reflection;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 启动调用顺序：
    /// NativeApplication.OnCreate -> ApplicationActivity.OnCreate -> App.OnLaunched -> DefaultStub.OnLaunched
    /// </summary>
    public class BaseAppActivity : ApplicationActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            BgJob.MainActivity = GetType();

            // 确保 Permissions.RequestAsync 调用时正常
            Microsoft.Maui.ApplicationModel.Platform.Init(this, bundle);

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
                        ((DefaultStub)Stub.Inst).ToastStart(startInfo);
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

#pragma warning disable CA1422 // 类型或成员已过时
                Android.Net.Uri uri = (Android.Net.Uri)it.GetParcelableExtra(Intent.ExtraStream);
#pragma warning restore CA1422 // 类型或成员已过时
                var path = IOUtil.GetPath(Android.App.Application.Context, uri);
                // 通过FileProvider分享时无法获取路径但可读取文件内容，参见ShareInfo.GetStream
                if (string.IsNullOrEmpty(path))
                    path = uri.ToString();
                info.FilePath = path;
            }
            ((DefaultStub)Stub.Inst).ReceiveShare(info);
        }
    }
}
#endif