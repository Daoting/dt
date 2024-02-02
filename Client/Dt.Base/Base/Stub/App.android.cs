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
        string _params;

        public DefaultStub()
        {
            UnoKit.Init();
        }

        public override async void OnLaunched(LaunchActivatedEventArgs p_args)
        {
            if (string.IsNullOrEmpty(_params))
                _params = p_args.Arguments;

            await Launch(_params);
            _params = null;
        }

        public void ReceiveShare(ShareInfo p_shareInfo)
        {
            _ = Launch(null, p_shareInfo);
        }

        public void ToastStart(string p_params)
        {
            // 点击通知栏启动
            if (_isInited)
            {
                // 状态库打开表示app已启动过，不会再调用 OnLaunched
                _ = Launch(p_params);
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