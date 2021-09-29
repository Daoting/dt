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
using System.Threading;
using System.Threading.Tasks;
#endregion

namespace Dt.Core
{
    public static partial class BgJob
    {
        public const string ToastStart = "ToastStart";
        const string _channelID = "default";
        static NotificationManager _manager;
        static int _id = 1;
        static Type _mainActivity;

        public static void Init(Type p_mainActivity)
        {
            _mainActivity = p_mainActivity;
        }

        /// <summary>
        /// 后台作业运行入口
        /// 此方法不可使用任何UI和外部变量，保证可独立运行！！！
        /// </summary>
        /// <param name="p_stub"></param>
        /// <returns></returns>
        public static async Task Run(IStub p_stub)
        {
            await Task.Delay(5000);
            Toast("abc", DateTime.Now.ToString(), "123");
        }

        public static void Toast(string p_title, string p_msg, string p_params)
        {
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

            // 设置点击通知栏的动作为启动另外一个广播
            Intent intent = new Intent(context, _mainActivity)
                .SetAction(ToastStart)
                .AddCategory(Intent.CategoryLauncher)
                .PutExtra(ToastStart, "{\"WinType\":\"Dt.Sample.SamplesMain, Dt.Sample, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\",\"Params\":null,\"ParamsType\":null,\"Title\":\"控件样例\",\"Icon\":\"词典\"}");
            var pending = PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.UpdateCurrent);

            // 一定要设置 channel 和 icon，否则不通知！！！
            Notification notify = new Notification.Builder(context, _channelID)
                .SetSmallIcon(Resource.Drawable.abc_ic_star_black_48dp)
                .SetContentTitle(p_title)
                .SetContentText(p_msg)
                .SetContentIntent(pending)
                .SetAutoCancel(true)
                .Build();
            _manager.Notify(_id++, notify);
        }
    }
}
#endif