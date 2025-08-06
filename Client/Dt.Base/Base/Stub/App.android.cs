#if ANDROID
#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2016-02-18
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 默认存根
    /// </summary>
    public partial class DefaultStub : Stub
    {
        static string _params;
        static ShareInfo _shareInfo;

        public override async Task OnLaunched(LaunchActivatedEventArgs p_args)
        {
            if (string.IsNullOrEmpty(_params))
                _params = p_args.Arguments;

            await Launch(_params, _shareInfo);
            _params = null;
            _shareInfo = null;
        }

        public static void ReceiveShare(ShareInfo p_shareInfo)
        {
            // 接收分享内容
            if (Inst is DefaultStub stub)
            {
                // 已启动
                _ = stub.Launch(null, p_shareInfo);
            }
            else
            {
                // 未启动，记录分享内容提供给 OnLaunched
                _shareInfo = p_shareInfo;
            }
        }

        public static void ToastStart(string p_params)
        {
            // 点击通知栏启动
            if (Inst is DefaultStub stub)
            {
                // app已启动过，不会再调用 OnLaunched
                _ = stub.Launch(p_params);
            }
            else
            {
                // 未启动，记录参数提供给 OnLaunched
                _params = p_params;
            }
        }
    }
}
#endif