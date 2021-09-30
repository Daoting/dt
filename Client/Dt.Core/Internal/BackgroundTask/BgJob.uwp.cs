#if UWP
#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-06 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Notifications;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 后台作业
    /// </summary>
    public static partial class BgJob
    {
        public static async Task Run(IStub p_stub)
        {
            await Task.Delay(5000);

        }

        public static void Toast(string p_title, string p_content, AutoStartInfo p_startInfo)
        {
            if (string.IsNullOrEmpty(p_title) || string.IsNullOrEmpty(p_content))
                return;

            Windows.Data.Xml.Dom.XmlDocument xml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
            if (p_startInfo != null)
            {
                string json = JsonSerializer.Serialize(p_startInfo, JsonOptions.UnsafeSerializer);
                ((Windows.Data.Xml.Dom.XmlElement)xml.FirstChild).SetAttribute("launch", json);
            }
            xml.GetElementsByTagName("text").Item(0).InnerText = p_title;
            xml.GetElementsByTagName("text").Item(1).InnerText = p_content;
            ToastNotificationManager.CreateToastNotifier().Show(new ToastNotification(xml));
        }
    }
}
#endif