#if IOS
#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-06 创建
******************************************************************************/
#endregion

#region 引用命名
using BackgroundTasks;
using Foundation;
using System.Diagnostics;
using System.Text.Json;
using UIKit;
using UserNotifications;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 后台作业
    /// </summary>
    public static partial class BgJob
    {
        public const string ToastStart = "ToastStart";
        // 和 Info.plist 的 BGTaskSchedulerPermittedIdentifiers 值相同
        const string _taskID = "Dt.Core.BgJob";

        /// <summary>
        /// 注册后台任务必须在 AppDelegate 构造方法中调用
        /// </summary>
        public static void RegisterEarliest()
        {
            // iOS13 以后原 Background Fetch 方式失效，采用注册后台任务、提交后台任务请求的方式
            // 注册和提交请求通过 _taskID 识别任务，和 Info.plist 的 BGTaskSchedulerPermittedIdentifiers 值相同！

            // 只有真机才能成功提交后台任务请求！！！

            // https://dzone.com/articles/how-to-update-app-content-with-background-tasks-us
            // https://github.com/spaceotech/BackgroundTask/blob/master/SOBackgroundTask/Application/AppDelegate.swift

            try
            {
                // 必须在 AppDelegate 构造方法中调用！！！
                bool suc = BGTaskScheduler.Shared.Register(_taskID, null, BgJobHandler);
                Debug.WriteLine(suc ? "后台任务注册成功" : "后台任务注册失败");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("后台任务注册异常：" + ex.Message);
            }
        }

        /// <summary>
        /// 注册后台任务
        /// </summary>
        public static void Register()
        {
            Task.Run(() =>
            {
                // 无后台 或 未启用
                if (Kit.GetService<IBackgroundJob>() == null || !AtState.EnableBgJob)
                    return;

                // 因后台任务独立运行，记录当前的存根类型以备后台使用，秒！
                string name = Stub.Inst.GetType().AssemblyQualifiedName;
                if (name != AtState.GetCookie(_stubType))
                    AtState.SaveCookie(_stubType, name);

                // 有后台需要注册通知，因后台任务无法注册
                RegisterNotification();
            });
        }

        /// <summary>
        /// 注销后台任务
        /// </summary>
        public static void Unregister()
        {
            // 取消所有请求
            BGTaskScheduler.Shared.CancelAll();
        }

        /// <summary>
        /// 进入后台
        /// </summary>
        public static void OnEnterBackground()
        {
            if (Kit.GetService<IBackgroundJob>() != null && AtState.EnableBgJob)
            {
                Unregister();
                SendRequest(true);
            }
        }

        /// <summary>
        /// 后台任务处理
        /// </summary>
        /// <param name="p_task"></param>
        static void BgJobHandler(BGTask p_task)
        {
            // 再次提交后台请求，形成循环执行效果
            SendRequest(false);

            Run().Wait();

            p_task.ExpirationHandler = () => Unregister();
            p_task.SetTaskCompleted(true);
        }

        static void SendRequest(bool p_fromEnterBackground)
        {
            // 两种任务请求
            // BGAppRefreshTaskRequest：刷新任务请求，执行简短刷新任务的请求
            // BGProcessingTaskRequest：在后台启动应用并执行需要较长时间才能完成的进程的请求
            string msg = p_fromEnterBackground ? "进入后台，" : "再次循环，";

            var request = new BGProcessingTaskRequest(_taskID);
            // 是否需要连接网络
            request.RequiresNetworkConnectivity = true;
            // 是否需要电源
            request.RequiresExternalPower = false;
            // 在真机、未禁止后台刷新、非省电模式、锁屏的情况下，最小延时15分钟后执行，其它情况不执行！！！
            request.EarliestBeginDate = NSDate.FromTimeIntervalSinceNow(15 * 60);

            try
            {
                // 只有真机才能成功提交后台任务请求！！！
                bool suc = BGTaskScheduler.Shared.Submit(request, out var error);
                if (suc)
                {
                    msg += "已成功提交请求";
                }
                else
                {
                    // BGTaskSchedulerErrorDomain error 1
                    // error 1：不允许，只有真机可提交请求
                    // error 3：任务不存在
                    msg += $"提交请求失败，Code {error.Code}";
                }
            }
            catch (Exception ex)
            {
                Unregister();
                msg += "提交请求异常：" + ex.Message;
            }
            finally
            {
                WriteLog(msg);
            }
        }

        public static void Toast(string p_title, string p_content, AutoStartInfo p_startInfo)
        {
            if (string.IsNullOrEmpty(p_title) || string.IsNullOrEmpty(p_content))
                return;

            RegisterNotification();
            var content = new UNMutableNotificationContent();
            content.Title = p_title;
            content.Body = p_content;
            content.Sound = UNNotificationSound.Default;
            content.Badge = 1;
            if (p_startInfo != null)
            {
                string json = JsonSerializer.Serialize(p_startInfo, JsonOptions.UnsafeSerializer);
                content.UserInfo = new NSDictionary<NSString, NSString>(new[] { new NSString(ToastStart) }, new[] { new NSString(json) });
            }

            // n秒后触发一次
            var trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(6, false);
            // id不同不会覆盖旧通知
            var request = UNNotificationRequest.FromIdentifier(Guid.NewGuid().ToString().Substring(0, 8), content, trigger);

            UNUserNotificationCenter.Current.AddNotificationRequest(request, (error) =>
            {
                if (error != null)
                {
                    WriteLog($"推送通知Error: {error.LocalizedDescription ?? ""}");
                }
                else
                {
                    WriteLog("已推送本地通知");
                }
            });
        }

        /// <summary>
        /// 注册本地通知放在第一次Toast时注册，若Toast是后台任务调用，不能正确注册！！！
        /// </summary>
        static void RegisterNotification()
        {
            Kit.RunAsync(() =>
            {
                var application = UIApplication.SharedApplication;
#pragma warning disable CA1422 // 类型或成员已过时
                if (application.CurrentUserNotificationSettings.Types == UIUserNotificationType.None)
                {
                    var ns = UIUserNotificationSettings.GetSettingsForTypes(
                        UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound, null);
                    application.RegisterUserNotificationSettings(ns);
                }
#pragma warning restore CA1422
            });
        }
    }
}
#endif