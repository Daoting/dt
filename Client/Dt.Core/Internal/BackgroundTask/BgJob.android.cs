#if ANDROID
#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-06 创建
******************************************************************************/
#endregion

#region 引用命名
using Android.App;
using Android.Content;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
#endregion

namespace Dt.Core
{
    public static partial class BgJob
    {
        public const string ActionToast = "Toast";
        const string _channelID = "default";
        static NotificationManager _manager;
        static int _id = 1;
        static Type _mainActivity;

        public static Task DoWork(IStub p_stub, Type p_mainActivity)
        {
            _mainActivity = p_mainActivity;
            return Run(p_stub);
        }

        public static void Toast(string p_title, string p_content, AutoStartInfo p_startInfo)
        {
            if (string.IsNullOrEmpty(p_title) || string.IsNullOrEmpty(p_content))
                return;

            var context = Application.Context;
            if (_manager == null)
            {
                _manager = (NotificationManager)context.GetSystemService(Context.NotificationService);
                var channel = _manager.GetNotificationChannel(_channelID);
                if (channel == null)
                {
                    channel = new NotificationChannel(_channelID, "主通道", NotificationImportance.High);
                    channel.LockscreenVisibility = NotificationVisibility.Private;
                    _manager.CreateNotificationChannel(channel);
                }
            }

            // 点击通知自定义启动
            Intent intent = new Intent(context, _mainActivity)
                .SetAction(ActionToast)
                .AddCategory(Intent.CategoryLauncher);
            if (p_startInfo != null)
            {
                string json = JsonSerializer.Serialize(p_startInfo, JsonOptions.UnsafeSerializer);
                intent.PutExtra(ActionToast, json);
            }
            var pending = PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.UpdateCurrent);

            // 一定要设置 channel 和 icon，否则不通知！！！
            Notification notify = new Notification.Builder(context, _channelID)
                .SetSmallIcon(Resource.Drawable.abc_ic_star_black_48dp)
                .SetContentTitle(p_title)
                .SetContentText(p_content)
                .SetContentIntent(pending)
                .SetAutoCancel(true)
                .Build();
            _manager.Notify(_id++, notify);
        }
    }
}
#endif