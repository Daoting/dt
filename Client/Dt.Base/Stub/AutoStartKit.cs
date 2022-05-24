#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2016-02-18
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using System.Text.Json;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 自启动工具
    /// </summary>
    public static class AutoStartKit
    {
        /// <summary>
        /// 设置自启动
        /// </summary>
        /// <param name="p_win"></param>
        public static void SetAutoStart(Win p_win)
        {
            AutoStartInfo info = GetAutoStartInfo(p_win);
            AtState.SaveAutoStart(info);
            Kit.Msg(string.Format("{0}已设置自启动！", p_win.Title));
        }

        /// <summary>
        /// 取消自启动
        /// </summary>
        public static void DelAutoStart()
        {
            AtState.DelAutoStart();
            Kit.Msg("已取消自启动设置！");
        }

        /// <summary>
        /// 创建自启动Win
        /// </summary>
        /// <param name="p_autoStart"></param>
        /// <returns></returns>
        public static Win CreateAutoStartWin(AutoStartInfo p_autoStart)
        {
            Win win = null;
            Type type = Type.GetType(p_autoStart.WinType);
            if (type != null)
            {
                try
                {
                    if (string.IsNullOrEmpty(p_autoStart.Params))
                    {
                        win = (Win)Activator.CreateInstance(type);
                    }
                    else
                    {
                        var par = JsonSerializer.Deserialize(p_autoStart.Params, Type.GetType(p_autoStart.ParamsType));
                        win = (Win)Activator.CreateInstance(type, par);
                    }

                    if (win != null)
                    {
                        win.Title = string.IsNullOrEmpty(p_autoStart.Title) ? "自启动" : p_autoStart.Title;
                        Icons icon;
                        if (Enum.TryParse(p_autoStart.Icon, out icon))
                            win.Icon = icon;
                    }
                }
                catch { }
            }
            return win;
        }

        /// <summary>
        /// 获取Win的自启动信息
        /// </summary>
        /// <param name="p_win"></param>
        /// <returns></returns>
        public static AutoStartInfo GetAutoStartInfo(Win p_win)
        {
            if (p_win == null)
                return null;

            Tabs tabs = (Tabs)p_win.GetValue(Win.MainTabsProperty);
            if (tabs != null
                && tabs.Items.Count > 0
                && ((Tab)tabs.Items[0]).Content is Win win)
            {
                // 设置主区窗口为自启动
                p_win = win;
            }

            AutoStartInfo info = new AutoStartInfo();
            info.WinType = p_win.GetType().AssemblyQualifiedName;
            info.Title = p_win.Title;
            info.Icon = p_win.Icon.ToString();
            if (p_win.Params != null)
            {
                info.Params = JsonSerializer.Serialize(p_win.Params, JsonOptions.UnsafeSerializer);
                info.ParamsType = p_win.Params.GetType().AssemblyQualifiedName;
            }
            return info;
        }
    }
}