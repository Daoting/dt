#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-27 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using System.Text.Json;
using System.Xml.Linq;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 启动管理类
    /// </summary>
    internal static class LaunchManager
    {
        const string _homeView = "主页";

        /// <summary>
        /// 启动参数
        /// </summary>
        public static string Arguments { get; set; }

        /// <summary>
        /// 加载PhoneUI模式的根Frame
        /// </summary>
        public static void LoadRootFrame()
        {
            SysVisual.RootContent = new Frame();

            // 主页作为根
            Type tp = AtApp.GetViewType(_homeView);
            if (tp != null)
            {
                Win win = Activator.CreateInstance(tp) as Win;
                if (win != null)
                {
                    if (string.IsNullOrEmpty(win.Title))
                        win.Title = _homeView;
                    win.Icon = Icons.主页;
                    win.NaviToHome();
                }
            }

            // 自启动
            AutoStartInfo autoStart;
            if (!string.IsNullOrEmpty(Arguments))
            {
                // 带启动参数的自启动
                LaunchFreely(Arguments);
                Arguments = null;
            }
            else if ((autoStart = AtLocal.GetAutoStart()) != null)
            {
                // 用户设置的自启动
                bool suc = false;
                Type type = Type.GetType(autoStart.WinType);
                if (type != null)
                {
                    try
                    {
                        Win win = null;
                        if (string.IsNullOrEmpty(autoStart.Params))
                        {
                            win = (Win)Activator.CreateInstance(type);
                        }
                        else
                        {
                            var par = JsonSerializer.Deserialize(autoStart.Params, Type.GetType(autoStart.ParamsType));
                            win = (Win)Activator.CreateInstance(type, par);
                        }

                        if (win != null)
                        {
                            win.Title = string.IsNullOrEmpty(autoStart.Title) ? "自启动" : autoStart.Title;
                            Icons icon;
                            if (Enum.TryParse(autoStart.Icon, out icon))
                                win.Icon = icon;
                            win.NaviToHome();
                            suc = true;
                        }
                    }
                    catch { }
                }
                if (!suc)
                    AtLocal.DelAutoStart();
            }
        }

        /// <summary>
        /// 加载Windows模式桌面
        /// </summary>
        public static void LoadDesktop()
        {
            Desktop desktop = new Desktop();

            // 主页
            Type tp = AtApp.GetViewType(_homeView);
            if (tp != null)
            {
                Win win = Activator.CreateInstance(tp) as Win;
                if (win != null)
                {
                    if (string.IsNullOrEmpty(win.Title))
                        win.Title = _homeView;
                    win.Icon = Icons.主页;
                    desktop.HomeWin = win;
                }
            }

            // 自启动
            AutoStartInfo autoStart;
            if (!string.IsNullOrEmpty(Arguments))
            {
                // 带启动参数的自启动
                LaunchFreely(Arguments);
                Arguments = null;
            }
            else if ((autoStart = AtLocal.GetAutoStart()) != null)
            {
                // 用户设置的自启动
                bool suc = false;
                Type type = Type.GetType(autoStart.WinType);
                if (type != null)
                {
                    try
                    {
                        Win win = null;
                        if (string.IsNullOrEmpty(autoStart.Params))
                        {
                            win = (Win)Activator.CreateInstance(type);
                        }
                        else
                        {
                            var par = JsonSerializer.Deserialize(autoStart.Params, Type.GetType(autoStart.ParamsType));
                            win = (Win)Activator.CreateInstance(type, par);
                        }

                        if (win != null)
                        {
                            win.Title = string.IsNullOrEmpty(autoStart.Title) ? "自启动" : autoStart.Title;
                            Icons icon;
                            if (Enum.TryParse(autoStart.Icon, out icon))
                                win.Icon = icon;

                            desktop.ShowNewWin(win);
                            suc = true;
                        }
                    }
                    catch { }
                }
                if (!suc)
                    AtLocal.DelAutoStart();
            }

            if (desktop.MainWin == null)
                desktop.MainWin = desktop.HomeWin;
            SysVisual.RootContent = desktop;
        }

        /// <summary>
        /// 以参数方式自启动，通常从Toast启动
        /// </summary>
        /// <param name="p_params">xml启动参数</param>
        public static void LaunchFreely(string p_params)
        {
            var root = XDocument.Parse(p_params).Root;
            var attr = root.Attribute("id");
            if (attr == null || string.IsNullOrEmpty(attr.Value))
            {
                AtKit.Msg("自启动时标识不可为空！");
                return;
            }

            // 以菜单项方式启动
            if (root.Name == "menu")
            {
                //AtUI.OpenMenu(attr.Value);
                return;
            }

            // 打开视图
            string viewName = attr.Value;
            string title = null;
            Icons icon = Icons.None;
            attr = root.Attribute("title");
            if (attr != null && !string.IsNullOrEmpty(attr.Value))
                title = attr.Value;
            attr = root.Attribute("icon");
            if (attr != null && !string.IsNullOrEmpty(attr.Value))
                Enum.TryParse(attr.Value, out icon);

            // undo
            //Dictionary<string, string> pars = new Dictionary<string, string>();
            //foreach (var elem in root.Elements("param"))
            //{
            //    var key = elem.Attribute("key");
            //    var val = elem.Attribute("val");
            //    if (key != null && !string.IsNullOrEmpty(key.Value) && val != null)
            //        pars[key.Value] = val.Value;
            //}
            AtApp.OpenView(viewName, title, icon);
        }

        /// <summary>
        /// 设置自启动
        /// </summary>
        /// <param name="p_win"></param>
        public static void SetAutoStart(Win p_win)
        {
            if (p_win == null)
                return;

            if (!AtSys.IsPhoneUI)
            {
                Tabs tabs = (Tabs)p_win.GetValue(Win.CenterTabsProperty);
                if (tabs != null
                    && tabs.Items.Count > 0
                    && ((Tab)tabs.Items[0]).Content is Win win)
                {
                    // 设置主区窗口为自启动
                    p_win = win;
                }
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
            AtLocal.SaveAutoStart(info);
            AtKit.Msg(string.Format("{0}已设置自启动！", p_win.Title));
        }

        /// <summary>
        /// 取消自启动
        /// </summary>
        public static void DelAutoStart()
        {
            AtLocal.DelAutoStart();
            AtKit.Msg("已取消自启动设置！");
        }
    }
}

