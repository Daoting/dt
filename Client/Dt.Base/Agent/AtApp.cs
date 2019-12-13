#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-08-22 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Rpc;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.ApplicationModel.Activation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 客户端整个生命周期管理类
    /// </summary>
    public static class AtApp
    {
        #region 启动
        /// <summary>
        /// 应用程序启动
        /// </param>
        public static void Run(LaunchActivatedEventArgs args)
        {
            // 初始根元素用来提示信息
            TextBlock info = SysVisual.RootContent as TextBlock;
            Window.Current.Activate();

            // 已启动过
            if (info == null)
            {
                // 带参数启动
                if (!string.IsNullOrEmpty(args.Arguments))
                    AtKit.RunAsync(() => LaunchFreely(args.Arguments));
                return;
            }

            // 登录注销回调
            AtSys.Login = Login;
            AtSys.Logout = Logout;

            // uwp和wasm 支持UI模式切换
#if UWP
            SysVisual.UIModeChanged = OnUIModeChanged;
            if (!AtSys.IsPhoneUI)
            {
                _deskDict = new ResourceDictionary() { Source = new Uri(_deskStylePath) };
                Application.Current.Resources.MergedDictionaries.Add(_deskDict);
            }
#endif

            // 后退键
            var view = SystemNavigationManager.GetForCurrentView();
            view.BackRequested += InputManager.OnBackRequested;
            if (AtSys.System == TargetSystem.Windows)
            {
                if (AtSys.IsPhoneUI)
                    view.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                // 全局快捷键
                Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated += InputManager.AcceleratorKeyActivated;
            }

            // 带参数启动
            if (!string.IsNullOrEmpty(args.Arguments))
                _launchCallback = () => LaunchFreely(args.Arguments);

            // 提示信息
            NotifyManager.Init();

            // 从存根启动，因uno中无法在一个根UI的Loaded事件中切换到另一根UI，所以未采用启动页方式
            AtSys.Stub.OnStartup(new StartupInfo());
        }

        /// <summary>
        /// 更新打开模型文件
        /// 1. 与本地不同时下载新模型文件；
        /// 2. 打开模型库；
        /// </summary>
        /// <param name="p_svcName">服务名，</param>
        /// <returns></returns>
        public static async Task<string> OpenModelDb(string p_svcName)
        {
            // 获取全局参数
            Dict cfg;
            try
            {
                cfg = await new UnaryRpc(p_svcName, "Entry.GetConfig").Call<Dict>();
                AtSys.SyncTime(cfg.Date("now"));
            }
            catch (Exception ex)
            {
                return "服务器连接失败！" + ex.Message;
            }

            // 更新模型文件
            string modelFile = cfg.Str("ver") + ".db";
            bool existFile = File.Exists(Path.Combine(AtSys.LocalDbPath, modelFile));
            if (!existFile)
            {
                // 关闭模型库，打开时无法删除文件
                AtLocal.CloseModelDb();

                // 删除旧版的模型文件
                foreach (var file in new DirectoryInfo(AtSys.LocalDbPath).GetFiles())
                {
                    if (file.Extension == ".db" && file.Name != "State.db")
                        try { file.Delete(); } catch { }
                }

                try
                {
                    // 下载模型文件，下载地址如 https://localhost/app/cm/.model
                    using (var response = await BaseRpc.Client.GetAsync($"{AtSys.Stub.ServerUrl.TrimEnd('/')}/{p_svcName}/.model"))
                    using (var stream = await response.Content.ReadAsStreamAsync())
                    using (var gzipStream = new GZipStream(stream, CompressionMode.Decompress))
                    using (var fs = File.Create(Path.Combine(AtSys.LocalDbPath, modelFile), 262140, FileOptions.WriteThrough))
                    {
                        gzipStream.CopyTo(fs);
                        fs.Flush();
                    }
                }
                catch (Exception ex)
                {
                    try
                    {
                        File.Delete(Path.Combine(AtSys.LocalDbPath, modelFile));
                    }
                    catch { }
                    return "下载模型文件失败！" + ex.Message;
                }
            }

            // 打开模型库
            try
            {
                AtLocal.OpenModelDb(modelFile);
            }
            catch (Exception ex)
            {
                return "打开模型库失败！" + ex.Message;
            }
            return null;
        }
        #endregion

        #region 登录注销
        /// <summary>
        /// 显示登录页面
        /// </summary>
        /// <param name="p_isPopup">是否为弹出式</param>
        static void Login(bool p_isPopup)
        {
            if (!p_isPopup)
            {
                SysVisual.RootContent = AtSys.Stub.LoginPage;
                return;
            }

            // 弹出式登录页面在未登录遇到需要登录的功能时
            AtKit.RunAsync(() =>
            {
                var dlg = new Dlg
                {
                    Resizeable = false,
                    HideTitleBar = true,
                    PhonePlacement = DlgPlacement.Maximized,
                    WinPlacement = DlgPlacement.Maximized,
                    Content = AtSys.Stub.LoginPage,
                };
                dlg.Show();
            });
        }

        /// <summary>
        /// 登录成功后的处理
        /// </summary>
        /// <param name="p_id"></param>
        /// <param name="p_phone"></param>
        /// <param name="p_name"></param>
        /// <param name="p_pwd"></param>
        public static void LoginSuccess(long p_id, string p_phone, string p_name, string p_pwd = null, Dlg p_dlg = null)
        {
            // 登录后初始化用户信息
            AtUser.ID = p_id;
            AtUser.Phone = p_phone;
            AtUser.Name = p_name;

            // 初次登录
            if (!string.IsNullOrEmpty(p_pwd))
            {
                AtLocal.SaveCookie("LoginPhone", p_phone);
                AtLocal.SaveCookie("LoginPwd", p_pwd);
            }
            BaseRpc.RefreshHeader();

            // 正常登录后切换到主页，中途登录后关闭对话框
            if (p_dlg == null)
                LoadRootUI();
            else
                p_dlg.Close();

            _ = HandlePushMsg();
        }

        /// <summary>
        /// 注销后重新登录
        /// </summary>
        /// <returns></returns>
        static async void Logout()
        {
            // 注销时清空用户信息
            AtUser.ID = -1;
            AtUser.Name = null;
            AtUser.Phone = null;

            AtLocal.DeleteCookie("LoginPhone");
            AtLocal.DeleteCookie("LoginPwd");
            BaseRpc.RefreshHeader();

            await AtSys.Stub.OnLogout();
            SysVisual.RootContent = AtSys.Stub.LoginPage;
        }

        /// <summary>
        /// 处理服务器推送
        /// </summary>
        /// <returns></returns>
        static async Task HandlePushMsg()
        {
            try
            {
                var reader = await AtMsg.Register((int)AtSys.System);
                PushHandler.RetryTimes = 0;
                while (await reader.MoveNext())
                {
                    new PushHandler().Call(reader.Val<string>());
                }
            }
            catch { }

            // 未停止接收推送时重连
            if (!PushHandler.StopPush && PushHandler.RetryTimes < 5)
            {
                PushHandler.RetryTimes++;
                _ = Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, PushHandler.RetryTimes))).ContinueWith((t) => HandlePushMsg());
            }
        }
        #endregion

        #region 根元素
        /// <summary>
        /// PhoneUI模式的根Frame
        /// </summary>
        public static Frame Frame { get; internal set; }

        /// <summary>
        /// 加载根内容 Desktop 或 Frame
        /// </summary>
        public static void LoadRootUI()
        {
            if (AtSys.IsPhoneUI)
                LoadRootFrame();
            else
                LoadDesktop();
        }

        /// <summary>
        /// 带参数启动的回调方法
        /// </summary>
        static Action _launchCallback;

        /// <summary>
        /// 加载根Frame
        /// </summary>
        static void LoadRootFrame()
        {
            // uno中默认字体大小11
            Frame = new Frame { FontSize = 15 };
            SysVisual.RootContent = Frame;

            // 主页作为根
            Type tp = AtUI.GetViewType(AtUI.HomeView);
            if (tp != null)
            {
                IWin win = Activator.CreateInstance(tp) as IWin;
                if (win != null)
                {
                    win.Title = "主页";
                    win.Icon = Icons.主页;
                    win.NaviToHome();
                }
            }

            // 自启动
            AutoStartInfo autoStart;
            if (_launchCallback != null)
            {
                // 带启动参数的自启动
                _launchCallback();
                _launchCallback = null;
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
                        IWin win = null;
                        if (string.IsNullOrEmpty(autoStart.Params))
                            win = (IWin)Activator.CreateInstance(type);
                        else
                            win = (IWin)Activator.CreateInstance(type, autoStart.Params);

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
        /// 加载桌面
        /// </summary>
        static void LoadDesktop()
        {
            Desktop desktop = new Desktop();

            // 主页
            Type tp = AtUI.GetViewType(AtUI.HomeView);
            if (tp != null)
            {
                IWin win = Activator.CreateInstance(tp) as IWin;
                if (win != null)
                {
                    win.Title = "主页";
                    win.Icon = Icons.主页;
                    desktop.HomeWin = win;
                }
            }

            // 自启动
            AutoStartInfo autoStart;
            if (_launchCallback != null)
            {
                // 带启动参数的自启动
                _launchCallback();
                _launchCallback = null;
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
                        IWin win = null;
                        if (string.IsNullOrEmpty(autoStart.Params))
                            win = (IWin)Activator.CreateInstance(type);
                        else
                            win = (IWin)Activator.CreateInstance(type, autoStart.Params);

                        if (win != null)
                        {
                            win.Title = string.IsNullOrEmpty(autoStart.Title) ? "自启动" : autoStart.Title;
                            Icons icon;
                            if (Enum.TryParse(autoStart.Icon, out icon))
                                win.Icon = icon;

                            Taskbar.LoadTaskItem(win);
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
        #endregion

        #region 响应式UI
#if UWP
        const string _deskStylePath = "ms-appx:///Dt.Base/Themes/Styles/Desk.xaml";
        static ResourceDictionary _deskDict;

        /// <summary>
        /// 系统区域大小变化时UI自适应
        /// </summary>
        static void OnUIModeChanged()
        {
            // 刷新样式
            var dict = Application.Current.Resources.MergedDictionaries;
            if (AtSys.IsPhoneUI)
            {
                // 卸载桌面版样式
                if (_deskDict != null)
                    dict.Remove(_deskDict);
                // 桌面Mini版显示后退按钮
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                Desktop.Inst = null;
                Taskbar.Inst = null;
            }
            else
            {
                // 加载桌面版样式
                if (_deskDict == null)
                    _deskDict = new ResourceDictionary() { Source = new Uri(_deskStylePath) };
                dict.Add(_deskDict);
                // 隐藏后退按钮
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
                Frame = null;
            }

            // 重构根元素
            if (SysVisual.RootContent is Frame || SysVisual.RootContent is Desktop)
                LoadRootUI();
        }
#endif
        #endregion

        #region 参数启动
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
            AtUI.OpenView(viewName, title, icon);
        }
        #endregion
    }
}
