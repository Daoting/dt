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
using AndroidX.Work;
using System;
using System.Text.Json;
using System.Threading.Tasks;
#endregion

namespace Dt.Core
{
    public static partial class BgJob
    {
        public const string ActionToast = "Toast";
        const string _channelID = "default";
        const string _mainActivityName = "MainActivity";
        static NotificationManager _manager;
        static int _id = 1;
        static Type _mainActivity;

        /// <summary>
        /// 主Activity类型，点击Toast启动时需要
        /// </summary>
        public static Type MainActivity
        {
            get
            {
                if (_mainActivity == null)
                {
                    string tpName = AtState.GetCookie(_mainActivityName);
                    if (!string.IsNullOrEmpty(tpName))
                        _mainActivity = Type.GetType(tpName);
                }
                return _mainActivity;
            }
            set { _mainActivity = value; }
        }

        /// <summary>
        /// 注册后台任务
        /// </summary>
        public static void Register()
        {
            Task.Run(() =>
            {
                // 因后台任务独立运行，记录当前的存根类型以备后台使用，秒！
                string name = Kit.Stub.GetType().AssemblyQualifiedName;
                if (name != AtState.GetCookie(_stubType))
                    AtState.SaveCookie(_stubType, name);

                if (_mainActivity != null)
                {
                    name = _mainActivity.AssemblyQualifiedName;
                    if (name != AtState.GetCookie(_mainActivityName))
                        AtState.SaveCookie(_mainActivityName, name);
                }

                // 注册后台服务，后台Worker每15分钟运行一次，系统要求最短间隔15分钟！
                var workRequest = PeriodicWorkRequest.Builder.From<PluginWorker>(TimeSpan.FromMinutes(15)).Build();
                // 设为Replace时每次启动都运行后台服务，方便调试！
                WorkManager.GetInstance(Android.App.Application.Context).EnqueueUniquePeriodicWork("PluginWorker", ExistingPeriodicWorkPolicy.Replace, workRequest);
            });
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
            Intent intent = new Intent(context, MainActivity)
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

    public class PluginWorker : Worker
    {
        public PluginWorker(Context context, WorkerParameters workerParameters)
            : base(context, workerParameters)
        {
        }

        public override Result DoWork()
        {
            try
            {
                BgJob.Run().Wait();
            }
            catch { }
            return Result.InvokeSuccess();
        }
    }
}
#endif