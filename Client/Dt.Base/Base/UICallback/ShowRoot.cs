#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-10-20
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Input;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 根内容
    /// </summary>
    partial class DefUICallback : IUICallback
    {
        // 自启动信息，加载主页前设置有效
        internal static AutoStartInfo _autoStartOnce;
        Type _rootElementType;

        /// <summary>
        /// 加载根内容，支持任意类型的UIElement，特殊类型有：
        /// <para>Win：PhoneUI模式加载Frame、导航到窗口主页、再导航到自启动窗口主页；Win模式加载桌面、打开窗口、再打开自启动窗口</para>
        /// <para>Page：加载Frame，导航到页面</para>
        /// <para>其余可视元素直接加载</para>
        /// </summary>
        /// <param name="p_elementType">类型：Win Page 或 任意可视元素UIElement</param>
        public void ShowRoot(Type p_elementType)
        {
            if (p_elementType == null)
                throw new ArgumentNullException(nameof(p_elementType));

            _rootElementType = p_elementType;
            if (p_elementType.IsSubclassOf(typeof(Win)))
            {
                Win win = CreateRootWin();
                if (Kit.IsPhoneUI)
                    LoadRootFrame(win);
                else
                    LoadRootDesktop(win);
            }
            else if (p_elementType.IsSubclassOf(typeof(Page)))
            {
                LoadRootPage(p_elementType);
            }
            else if (p_elementType.IsSubclassOf(typeof(UIElement)))
            {
                UITree.RootContent = Activator.CreateInstance(p_elementType) as UIElement;
            }
            else
            {
                throw new Exception($"{p_elementType.Name} 不是可视元素类型！");
            }
        }

        /// <summary>
        /// 加载PhoneUI模式的根Frame
        /// </summary>
        /// <param name="p_win"></param>
        async void LoadRootFrame(Win p_win)
        {
            UITree.RootContent = new Frame();

            // 主页
            p_win.NaviToHome();

            // 自启动
            AutoStartInfo autoStart = _autoStartOnce != null ? _autoStartOnce : await CookieX.GetAutoStart();
            if (autoStart != null)
            {
                // 用户设置的自启动
                Win win = AutoStartKit.CreateAutoStartWin(autoStart);
                if (win != null)
                {
                    win.NaviToHome();
                }
                else if (_autoStartOnce == null)
                {
                    // 用户设置的自启动，启动失败删除cookie
                    await CookieX.DelAutoStart();
                }
                // 只自启动一次
                _autoStartOnce = null;
            }
        }

        /// <summary>
        /// 加载Windows模式桌面
        /// </summary>
        /// <param name="p_win"></param>
        async void LoadRootDesktop(Win p_win)
        {
            Desktop desktop = new Desktop();

            // 主页
            desktop.HomeWin = p_win;

            // 自启动
            AutoStartInfo autoStart = _autoStartOnce != null ? _autoStartOnce : await CookieX.GetAutoStart();
            if (autoStart != null)
            {
                // 用户设置的自启动
                Win win = AutoStartKit.CreateAutoStartWin(autoStart);
                if (win != null)
                {
                    desktop.ShowNewWin(win);
                }
                else if (_autoStartOnce == null)
                {
                    // 用户设置的自启动，启动失败删除cookie
                    await CookieX.DelAutoStart();
                }
                // 只自启动一次
                _autoStartOnce = null;
            }

            if (desktop.MainWin == null)
                desktop.MainWin = desktop.HomeWin;
            UITree.RootContent = desktop;
        }

        /// <summary>
        /// 加载根页面
        /// </summary>
        /// <param name="p_pageType"></param>
        void LoadRootPage(Type p_pageType)
        {
            // 使用Frame确保PhoneUI模式下正常导航！如 系统->本地库
            Frame fm = new Frame();
            UITree.RootContent = fm;
            fm.Navigate(p_pageType);
        }

        Win CreateRootWin()
        {
            Win win = Activator.CreateInstance(_rootElementType) as Win;
            if (string.IsNullOrEmpty(win.Title))
                win.Title = "主页";
            win.Icon = Icons.主页;
            return win;
        }
    }
}