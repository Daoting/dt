﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-06-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Rpc;
using System;
using System.IO;
using System.IO.Compression;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 启动控制
    /// </summary>
    public static class Startup
    {
        #region 启动入口
        /// <summary>
        /// 应用程序启动
        /// </summary>
        /// <typeparam name="T">存根类型</typeparam>
        /// <param name="p_launchArgs">启动参数</param>
        /// <param name="p_shareInfo">接收分享的内容描述</param>
        /// <returns></returns>
        public static async Task Launch<T>(string p_launchArgs, ShareInfo p_shareInfo)
            where T : IStub
        {
            // 非null表示app已启动过
            if (Kit.Stub != null)
            {
                // 带参数启动
                if (!string.IsNullOrEmpty(p_launchArgs))
                    Kit.RunAsync(() => LaunchFreely(p_launchArgs));
                Window.Current.Activate();

                if (p_shareInfo != null)
                    Kit.RunAsync(() => Kit.Stub.ReceiveShare(p_shareInfo));
                return;
            }

            IStub stub = Activator.CreateInstance<T>();
            Kit.Startup(stub, new DefaultCallback());

            // 加载资源
            var res = Application.Current.Resources;
            res.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("ms-appx:///Dt.Base/Themes/Generic.xaml") });
#if WASM
            // 自定义图标字体库，因Global.xaml中前缀无效无法定义
            res["IconFont"] = new FontFamily("DtIcon");
            res["Symbols"] = new FontFamily("Symbols");
#endif

            InputManager.Init();
            NotifyManager.Init();
            _launchArgs = p_launchArgs;

            // 设置WinUI模式与PhoneUI模式切换的回调方法
            // 此处调用会通过SysVisual的静态构造方法创建整个系统可视树
            SysVisual.UIModeChanged = OnUIModeChanged;

            // 从存根启动，因uno中无法在一个根UI的Loaded事件中切换到另一根UI，所以未采用启动页方式
            await stub.OnStartup();

            if (p_shareInfo != null)
                stub.ReceiveShare(p_shareInfo);
        }
        #endregion

        #region 按默认流程启动
        /// <summary>
        /// 按默认流程启动：
        /// 1. 更新打开模型库
        /// 2. 已登录过，先自动登录
        /// 3. 未登录或登录失败时，根据 p_loginFirst 显示登录页或主页
        /// </summary>
        /// <param name="p_loginFirst">是否强制先登录</param>
        /// <returns></returns>
        public static async Task Run(bool p_loginFirst)
        {
            // 更新打开模型库
            if (!await OpenModelDb())
                return;

            string phone = AtState.GetCookie("LoginPhone");
            string pwd = AtState.GetCookie("LoginPwd");
            if (!string.IsNullOrEmpty(phone) && !string.IsNullOrEmpty(pwd))
            {
                // 自动登录
                var result = await new UnaryRpc(
                    _svcName,
                    "Entry.LoginByPwd",
                    phone,
                    pwd
                ).Call<LoginResult>();

                // 登录成功
                if (result.IsSuc)
                {
                    Kit.InitUser(result);
                    // 切换到主页
                    ShowHome();
                    // 接收服务器推送
                    PushHandler.Register();
                    return;
                }
            }

            // 未登录或登录失败
            if (p_loginFirst)
            {
                // 强制先登录
                ShowLogin(false);
            }
            else
            {
                // 未登录先显示主页
                ShowHome();
            }
        }

        /// <summary>
        /// 注册登录页和主页类型，以备 登录、注销、自动登录、中途登录时用
        /// </summary>
        /// <param name="p_loginPageType">登录页类型，null时采用默认登录页 DefaultLogin</param>
        /// <param name="p_homePageType">主页类型</param>
        public static void Register(Type p_loginPageType, Type p_homePageType)
        {
            _loginPageType = p_loginPageType;
            _homePageType = p_homePageType;
        }
        #endregion

        #region 模型库
        const string _svcName = "cm";

        /// <summary>
        /// 更新打开模型文件
        /// 1. 与本地不同时下载新模型文件；
        /// 2. 打开模型库；
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> OpenModelDb()
        {
            // 获取全局参数
            Dict cfg;
            try
            {
                cfg = await new UnaryRpc(_svcName, "ModelMgr.GetConfig").Call<Dict>();
                Kit.SyncTime(cfg.Date("now"));
            }
            catch
            {
                ShowTip("服务器连接失败！");
                return false;
            }

            // 更新模型文件
            string modelVer = Path.Combine(Kit.DataPath, $"model-{cfg.Str("ver")}.ver");
            if (!File.Exists(modelVer))
            {
                string modelFile = Path.Combine(Kit.DataPath, "model.db");

                // 删除旧版的模型文件和版本号文件
                try { File.Delete(modelFile); } catch { }
                foreach (var file in new DirectoryInfo(Kit.DataPath).GetFiles($"model-*.ver"))
                {
                    try { file.Delete(); } catch { }
                }

                try
                {
                    // 下载模型文件，下载地址如 https://localhost/app/cm/.model
                    using (var response = await BaseRpc.Client.GetAsync($"{Kit.Stub.ServerUrl}/{_svcName}/.model"))
                    using (var stream = await response.Content.ReadAsStreamAsync())
                    using (var gzipStream = new GZipStream(stream, CompressionMode.Decompress))
                    using (var fs = File.Create(modelFile, 262140, FileOptions.WriteThrough))
                    {
                        gzipStream.CopyTo(fs);
                        fs.Flush();
                    }

                    // 版本号文件
                    File.Create(modelVer);
                }
                catch (Exception ex)
                {
                    try
                    {
                        File.Delete(modelFile);
                    }
                    catch { }
                    ShowTip("下载模型文件失败！" + ex.Message);
                    return false;
                }
            }

            // 打开模型库
            try
            {
                AtModel.OpenDb();
            }
            catch (Exception ex)
            {
                ShowTip("打开模型库失败！" + ex.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 显示提示信息，只在未加载任何UI时有效
        /// </summary>
        /// <param name="p_msg"></param>
        public static void ShowTip(string p_msg)
        {
            if (SysVisual.RootContent is TextBlock tb && p_msg != null)
                tb.Text = p_msg;
        }
        #endregion

        #region 注册接收推送
        /// <summary>
        /// 注册接收服务器推送
        /// </summary>
        public static void RegisterSysPush()
        {
            PushHandler.Register();
        }

        /// <summary>
        /// 主动停止接收推送
        /// </summary>
        public static void StopSysPush()
        {
            PushHandler.StopRecvPush();
        }
        #endregion

        #region 加载登录页
        /// <summary>
        /// 登录页面类型，null时采用 DefaultLogin
        /// </summary>
        static Type _loginPageType;

        /// <summary>
        /// 显示登录页面
        /// </summary>
        /// <param name="p_isPopup">是否为弹出式</param>
        public static void ShowLogin(bool p_isPopup)
        {
            Kit.RunAsync(() =>
            {
                // 外部未指定时采用默认登录页
                Type tp = _loginPageType == null ? Type.GetType("Dt.App.DefaultLogin,Dt.App") : _loginPageType;
                var page = Activator.CreateInstance(tp) as UIElement;
                if (!p_isPopup)
                {
                    SysVisual.RootContent = page;
                    return;
                }

                // 弹出式登录页面在未登录遇到需要登录的功能时
                var dlg = new Dlg
                {
                    Resizeable = false,
                    HideTitleBar = true,
                    PhonePlacement = DlgPlacement.Maximized,
                    WinPlacement = DlgPlacement.Maximized,
                    Content = page,
                };
                dlg.Show();
            });
        }
        #endregion

        #region 加载主页
        static string _launchArgs;

        /// <summary>
        /// 主页类型
        /// </summary>
        static Type _homePageType;

        /// <summary>
        /// 加载根内容 Desktop/Frame 和主页
        /// </summary>
        public static void ShowHome()
        {
            if (Kit.IsPhoneUI)
                LoadRootFrame();
            else
                LoadDesktop();
        }

        /// <summary>
        /// 加载PhoneUI模式的根Frame
        /// </summary>
        static void LoadRootFrame()
        {
            SysVisual.RootContent = new Frame();

            // 主页作为根
            if (_homePageType != null)
            {
                Win win = Activator.CreateInstance(_homePageType) as Win;
                if (win != null)
                {
                    if (string.IsNullOrEmpty(win.Title))
                        win.Title = "主页";
                    win.Icon = Icons.主页;
                    win.NaviToHome();
                }
            }

            // 自启动
            AutoStartInfo autoStart;
            if (!string.IsNullOrEmpty(_launchArgs))
            {
                // 带启动参数的自启动
                LaunchFreely(_launchArgs);
                _launchArgs = null;
            }
            else if ((autoStart = AtState.GetAutoStart()) != null)
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
                    AtState.DelAutoStart();
            }
        }

        /// <summary>
        /// 加载Windows模式桌面
        /// </summary>
        static void LoadDesktop()
        {
            Desktop desktop = new Desktop();

            // 主页
            if (_homePageType != null)
            {
                Win win = Activator.CreateInstance(_homePageType) as Win;
                if (win != null)
                {
                    if (string.IsNullOrEmpty(win.Title))
                        win.Title = "主页";
                    win.Icon = Icons.主页;
                    desktop.HomeWin = win;
                }
            }

            // 自启动
            AutoStartInfo autoStart;
            if (!string.IsNullOrEmpty(_launchArgs))
            {
                // 带启动参数的自启动
                LaunchFreely(_launchArgs);
                _launchArgs = null;
            }
            else if ((autoStart = AtState.GetAutoStart()) != null)
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
                    AtState.DelAutoStart();
            }

            if (desktop.MainWin == null)
                desktop.MainWin = desktop.HomeWin;
            SysVisual.RootContent = desktop;
        }
        #endregion

        #region 向导页面
        /// <summary>
        /// 显示向导页面
        /// </summary>
        /// <param name="p_guide">向导页面类型</param>
        /// <param name="p_showAlways">是否每次启动都显示向导</param>
        /// <returns></returns>
        public static Task ShowGuide(Type p_guide, bool p_showAlways = false)
        {
            var path = Path.Combine(Kit.CachePath, "guide.flag");
            if (File.Exists(path) && !p_showAlways)
                return Task.CompletedTask;

            var page = Activator.CreateInstance(p_guide) as GuidePage;
            if (page == null)
            {
                ShowTip($"{p_guide.Name} 向导类型需要继承 GuidePage！");
                return Task.CompletedTask;
            }

            SysVisual.RootContent = page;
            File.Create(path);
            return page.AsyncTask;
        }
        #endregion

        #region 自定义页面
        /// <summary>
        /// 显示自定义页面
        /// </summary>
        /// <param name="p_page"></param>
        public static void ShowCustom(Control p_page)
        {
            SysVisual.RootContent = p_page;
        }
        #endregion

        #region 带参数启动
        /// <summary>
        /// 以参数方式自启动，通常从Toast启动
        /// </summary>
        /// <param name="p_params">xml启动参数</param>
        static void LaunchFreely(string p_params)
        {
            var root = XDocument.Parse(p_params).Root;
            var attr = root.Attribute("id");
            if (attr == null || string.IsNullOrEmpty(attr.Value))
            {
                Kit.Msg("自启动时标识不可为空！");
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
            Kit.OpenView(viewName, title, icon);
        }
        #endregion

        #region 自启动设置
        /// <summary>
        /// 设置自启动
        /// </summary>
        /// <param name="p_win"></param>
        internal static void SetAutoStart(Win p_win)
        {
            if (p_win == null)
                return;

            if (!Kit.IsPhoneUI)
            {
                Tabs tabs = (Tabs)p_win.GetValue(Win.MainTabsProperty);
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
            AtState.SaveAutoStart(info);
            Kit.Msg(string.Format("{0}已设置自启动！", p_win.Title));
        }

        /// <summary>
        /// 取消自启动
        /// </summary>
        internal static void DelAutoStart()
        {
            AtState.DelAutoStart();
            Kit.Msg("已取消自启动设置！");
        }
        #endregion

        #region UI模式切换
        /// <summary>
        /// WinUI模式 和 PhoneUI模式切换
        /// </summary>
        static void OnUIModeChanged()
        {
#if UWP
            // 显示/隐藏后退按钮
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = Kit.IsPhoneUI ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
#endif
            if (Kit.IsPhoneUI)
            {
                // WinUI模式 -> PhoneUI模式
                Desktop.Inst = null;
            }

            // 重构根元素
            if (SysVisual.RootContent is Frame || SysVisual.RootContent is Desktop)
                ShowHome();
        }
        #endregion
    }
}