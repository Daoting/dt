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
using System.Diagnostics;
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
                    string tpName = CookieX.Get(_mainActivityName).Result;
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
            Task.Run(async () =>
            {
                // 无后台 或 未启用
                var job = Kit.GetService<IBackgroundJob>();
                if (job == null || !await CookieX.IsEnableBgJob())
                    return;

                // 因后台任务独立运行，记录当前的后台任务类型以备后台使用，秒！
                string name = job.GetType().AssemblyQualifiedName;
                if (name != await CookieX.Get(_bgJobType))
                    await CookieX.Save(_bgJobType, name);

                if (_mainActivity != null)
                {
                    name = _mainActivity.AssemblyQualifiedName;
                    if (name != await CookieX.Get(_mainActivityName))
                        await CookieX.Save(_mainActivityName, name);
                }

                // 注册后台任务，后台Worker每15分钟运行一次，系统要求最短间隔15分钟！
                var workRequest = PeriodicWorkRequest.Builder.From<PluginWorker>(TimeSpan.FromMinutes(15)).Build();
                // 若已注册，保持原有，设为Replace时每次启动都运行后台服务！
                WorkManager.GetInstance(Android.App.Application.Context).EnqueueUniquePeriodicWork("PluginWorker", ExistingPeriodicWorkPolicy.Keep, workRequest);
                Debug.WriteLine("后台任务注册成功");
            });
        }

        /// <summary>
        /// 注销后台任务
        /// </summary>
        public static void Unregister()
        {
            WorkManager.GetInstance(Android.App.Application.Context).CancelAllWork();
            Debug.WriteLine("注销后台任务");
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
            var pending = PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);

            // 一定要设置 channel 和 icon，否则不通知！！！
            Notification notify = new Notification.Builder(context, _channelID)
                .SetSmallIcon(Resource.Drawable.abc_star_black_48dp)
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