﻿#if WIN
#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-06 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Dispatching;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
using System.Diagnostics;
using System.Text.Json;
using Windows.ApplicationModel.Background;
using Windows.UI.Notifications;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 后台作业
    /// </summary>
    public static partial class BgJob
    {
        const string _bgTaskName = "TimeTriggeredTask";

        /// <summary>
        /// 默认为最小时间间隔15分钟
        /// </summary>
        const uint _interval = 15;

        /// <summary>
        /// 注册后台任务
        /// </summary>
        public static void Register()
        {
            Task.Run(async () =>
            {
                // 无后台 或 未启用
                if (Kit.GetService<IBackgroundJob>() == null || !await CookieX.IsEnableBgJob())
                    return;

                // 因后台任务独立运行，记录当前的存根类型以备后台使用，秒！
                string name = Stub.Inst.GetType().AssemblyQualifiedName;
                if (name != await CookieX.Get(_stubType))
                    await CookieX.Save(_stubType, name);

                var res = await BackgroundExecutionManager.RequestAccessAsync();
                if (res == BackgroundAccessStatus.Unspecified
                    || res == BackgroundAccessStatus.DeniedBySystemPolicy
                    || res == BackgroundAccessStatus.DeniedByUser)
                    return;

                try
                {
                    var task = (from item in BackgroundTaskRegistration.AllTasks.Values
                                where item.Name == _bgTaskName
                                select item).FirstOrDefault();
                    if (task != null)
                        return;

                    // 注册后台任务
                    BackgroundTaskBuilder bd = new BackgroundTaskBuilder();
                    // 任务名称
                    bd.Name = _bgTaskName;
                    // 入口点
                    bd.TaskEntryPoint = "Dt.Tasks.TimeTriggeredTask";
                    // 设置触发器，周期运行
                    bd.SetTrigger(new TimeTrigger(_interval, false));
                    bd.Register();
                }
                catch { }
            });
        }

        /// <summary>
        /// 注销后台任务
        /// </summary>
        public static void Unregister()
        {
            foreach (var cur in BackgroundTaskRegistration.AllTasks)
            {
                if (cur.Value.Name == _bgTaskName)
                {
                    cur.Value.Unregister(true);
                    break;
                }
            }
            Debug.WriteLine("注销后台任务");
        }

        public static void Toast(string p_title, string p_content, AutoStartInfo p_startInfo)
        {
            if (string.IsNullOrEmpty(p_title) && string.IsNullOrEmpty(p_content))
                return;

            Windows.Data.Xml.Dom.XmlDocument xml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
            if (p_startInfo != null)
            {
                string json = JsonSerializer.Serialize(p_startInfo, JsonOptions.UnsafeSerializer);
                ((Windows.Data.Xml.Dom.XmlElement)xml.FirstChild).SetAttribute("launch", json);
            }
            
            xml.GetElementsByTagName("text").Item(0).InnerText = !string.IsNullOrEmpty(p_title) ? p_title : p_content;
            if (!string.IsNullOrEmpty(p_title) && !string.IsNullOrEmpty(p_content))
                xml.GetElementsByTagName("text").Item(1).InnerText = p_content;
            ToastNotificationManager.CreateToastNotifier().Show(new ToastNotification(xml));

            // 以下无法在后台任务发出Toast通知！！！
            //var builder = new AppNotificationBuilder();
            //if (!string.IsNullOrEmpty(p_title))
            //    builder.AddText(p_title);
            //if (!string.IsNullOrEmpty(p_content))
            //    builder.AddText(p_content);

            //if (p_startInfo != null)
            //{
            //    string json = JsonSerializer.Serialize(p_startInfo, JsonOptions.UnsafeSerializer);
            //    builder.AddArgument("launch", json);
            //}
            //AppNotificationManager.Default.Show(builder.BuildNotification());
        }
    }
}
#endif