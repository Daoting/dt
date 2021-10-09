#if IOS
#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-06 创建
******************************************************************************/
#endregion

#region 引用命名
using Foundation;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
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

        public static void Toast(string p_title, string p_content, AutoStartInfo p_startInfo)
        {
            if (string.IsNullOrEmpty(p_title) || string.IsNullOrEmpty(p_content))
                return;

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
                    Console.WriteLine($"推送通知Error: {error.LocalizedDescription ?? ""}");
                }
                else
                {
                    Console.WriteLine("已推送本地通知");
                }
            });
        }

    }
}
#endif