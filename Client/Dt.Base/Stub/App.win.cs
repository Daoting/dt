#if WIN
#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2016-02-18
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.UI.Xaml;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 默认存根
    /// </summary>
    public abstract partial class DefaultStub : Stub
    {
        TaskCompletionSource<bool> _tcs;

        public DefaultStub()
        {
            ToastNotificationManagerCompat.OnActivated += OnToastActivated;
        }

        public override void OnLaunched(LaunchActivatedEventArgs p_args)
        {
            // 当用户单击Toast通知（或通知上的按钮）时：
            // 1. 如果应用当前正在运行，将在后台线程上调用 OnActivated 事件
            // 2. 如果应用当前已关闭，EXE 正常启动，且 WasCurrentProcessToastActivated() 返回 true，同样在后台线程上调用OnActivated 事件
            //    因启动参数在 OnActivated 事件获得，需要在该事件中异步等待正常启动完成后再应用启动参数
            if (ToastNotificationManagerCompat.WasCurrentProcessToastActivated())
                _tcs = new TaskCompletionSource<bool>();

            // 参数始终null，只有Toast启动带参数
            Launch().Wait();

            // 通知正常启动完成
            _tcs?.SetResult(true);
        }

        async void OnToastActivated(ToastNotificationActivatedEventArgsCompat e)
        {
            // 异步等待正常启动完成后再应用启动参数
            if (_tcs != null && !_tcs.Task.IsCompleted)
            {
                await _tcs.Task;
                _tcs = null;
            }

            // 从后台线程切换到UI线程，传递启动参数
            Kit.RunAsync(() => _ = Launch(e.Argument));
        }
    }
}
#endif