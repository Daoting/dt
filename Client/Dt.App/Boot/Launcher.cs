#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-21 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using Dt.Core.Rpc;
using System;
using System.Xml.Linq;
using Windows.ApplicationModel.Activation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.App
{
    /// <summary>
    /// 启动客户端
    /// </summary>
    public static class Launcher
    {
        /// <summary>
        /// 在应用程序由最终用户正常启动时进行调用。
        /// 当启动应用程序以执行打开特定的文件或显示搜索结果等操作时
        /// 将使用其他入口点。
        /// 
        /// 应用生命周期
        /// 1. NotRunning 没开启过应用，在多任务界面没有该应用时；
        /// 2. Running    正在屏幕上显示，整个系统只会有 1 个应用处于 Running 状态；
        /// 3. Suspended  不在屏幕上显示，能在多任务界面查看；
        /// 
        /// 三种状态间切换
        /// 1. NotRunning -> Running   打开应用，调用 OnLaunched
        /// 2. Running -> Suspended    Win键或返回键使应用不再占据屏幕，触发 Suspending 事件，保存状态
        /// 3. Suspended -> Running    再次占据屏幕，依次触发 Resuming 和 Launched 事件，Resuming 事件时恢复状态
        /// 4. Suspended -> NotRunning 因某些原因（比如内存不足）导致应用从挂起到终止，不触发任何事件
        /// </summary>
        /// <param name="args">
        /// args.PreviousExecutionState 记录应用之前的状态
        /// 1. CloseByUser 被用户主动在多任务界面中关闭
        /// 2. NotRunning  没有启动过
        /// 3. Running     启动中
        /// 4. Terminated  挂起状态时因内存不足等被系统终止，可以用来恢复状态
        /// 5. Suspended   挂起状态
        /// </param>
        public static void Run(LaunchActivatedEventArgs args)
        {
            // 全局注销方法
            AtSys.Logout = AuthHelper.Logout;

            // 已启动过则激活应用
            if (SysVisual.RootContent != null)
            {
                Window.Current.Activate();
                // 带参数启动
                if (!string.IsNullOrEmpty(args.Arguments))
                    AtKit.RunAsync(() => LaunchFreely(args.Arguments));
                return;
            }

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
                AtUI.LaunchCallback = () => LaunchFreely(args.Arguments);

            // 提示信息
            NotifyManager.Init();

            // 初始UI
            if (AtSys.Stub.IsLocalMode)
                AtUI.LoadLocalModeUI();
            else
                SysVisual.RootContent = new Startup();
            Window.Current.Activate();
        }

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
                AtUI.Frame = null;
            }

            // 重构根元素
            if (SysVisual.RootContent is Frame || SysVisual.RootContent is Desktop)
                AtUI.LoadRootContent();
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
                AtUI.OpenMenu(attr.Value);
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
