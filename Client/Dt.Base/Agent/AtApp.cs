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
using System.Linq;
using System.IO.Compression;
using System.Reflection;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
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
                    AtKit.RunAsync(() => LaunchManager.LaunchFreely(args.Arguments));
                return;
            }

            AtSys.Login = Login;
            AtSys.Logout = Logout;
            InputManager.Init();
            NotifyManager.Init();
            LaunchManager.Arguments = args.Arguments;

#if UWP
            // 支持响应式UI模式切换
            UIModeManager.Init();
#endif

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
        /// <param name="p_dlg"></param>
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

            // 接收服务器推送
            _ = PushHandler.Register();
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
        #endregion

        #region 窗口
        /// <summary>
        /// 根据视图名称激活旧窗口或打开新窗口
        /// </summary>
        /// <param name="p_viewName">窗口视图名称</param>
        /// <param name="p_title">标题</param>
        /// <param name="p_icon">图标</param>
        /// <param name="p_params">启动参数</param>
        /// <returns>返回打开的窗口或视图，null表示打开失败</returns>
        public static object OpenView(
            string p_viewName,
            string p_title = null,
            Icons p_icon = Icons.None,
            object p_params = null)
        {
            Type tp = GetViewType(p_viewName);
            if (tp == null)
            {
                AtKit.Msg(string.Format("【{0}】视图未找到！", p_viewName));
                return null;
            }
            return OpenWin(tp, p_title, p_icon, p_params);
        }

        /// <summary>
        /// 根据窗口类型和参数激活旧窗口或打开新窗口
        /// </summary>
        /// <param name="p_type">窗口类型</param>
        /// <param name="p_title">标题</param>
        /// <param name="p_icon">图标</param>
        /// <param name="p_params">初始参数</param>
        /// <returns>返回打开的窗口或视图，null表示打开失败</returns>
        public static object OpenWin(
            Type p_type,
            string p_title = null,
            Icons p_icon = Icons.None,
            object p_params = null)
        {
            if (p_type == null)
                AtKit.Throw("待显示的窗口类型不可为空！");

            // 激活旧窗口，比较窗口类型和初始参数
            Win win;
            if (!AtSys.IsPhoneUI && (win = Desktop.Inst.ActiveWin(p_type, p_params)) != null)
            {
                Taskbar.Inst.ActiveTaskItem(win);
                return win;
            }

            // 打开新窗口
            if (typeof(Win).IsAssignableFrom(p_type))
            {
                if (p_params == null)
                    win = (Win)Activator.CreateInstance(p_type);
                else
                    win = (Win)Activator.CreateInstance(p_type, p_params);

                if (string.IsNullOrEmpty(win.Title) && string.IsNullOrEmpty(p_title))
                    win.Title = "无标题";
                else if (!string.IsNullOrEmpty(p_title))
                    win.Title = p_title;

                if (p_icon != Icons.None)
                    win.Icon = p_icon;

                // 记录初始参数，设置自启动和win模式下识别窗口时使用
                if (p_params != null)
                    win.Params = p_params;

                if (AtSys.IsPhoneUI)
                {
                    win.NaviToHome();
                }
                else
                {
                    Taskbar.LoadTaskItem(win);
                    Desktop.Inst.ShowNewWin(win);
                }
                return win;
            }

            // 处理自定义启动情况
            if (p_type.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IView)))
            {
                IView viewer = Activator.CreateInstance(p_type) as IView;
                viewer.Run(p_params);
                return viewer;
            }

            AtKit.Msg("打开窗口失败，窗口类型继承自Win或实现IView接口！");
            return null;
        }

        /// <summary>
        /// 获取视图类型
        /// </summary>
        /// <param name="p_typeName">类型名称</param>
        /// <returns>返回类型</returns>
        public static Type GetViewType(string p_typeName)
        {
            Type tp;
            if (!string.IsNullOrEmpty(p_typeName) && AtSys.Stub.ViewTypes.TryGetValue(p_typeName, out tp))
                return tp;
            return null;
        }
        #endregion

        #region 根元素
        /// <summary>
        /// 加载根内容 Desktop 或 Frame
        /// </summary>
        public static void LoadRootUI()
        {
            if (AtSys.IsPhoneUI)
                LaunchManager.LoadRootFrame();
            else
                LaunchManager.LoadDesktop();
        }

        public static void GoBackToHome()
        {
            var frame = SysVisual.RootFrame;
            while (frame.BackStackDepth > 0)
            {
                frame.GoBack();
            }
        }

        public static void GoBack()
        {
            InputManager.GoBack();
        }

        /// <summary>
        /// PhoneUI模式的根Frame
        /// </summary>
        public static Frame RootFrame
        {
            get { return SysVisual.RootFrame; }
        }
        #endregion
    }
}
