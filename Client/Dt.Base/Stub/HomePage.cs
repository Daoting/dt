#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2016-02-18
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.Rpc;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.IO.Compression;
using System.Text.Json;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 主页
    /// </summary>
    public abstract partial class DefaultStub : Stub
    {
        Type _homePageType;

        // 自启动信息，加载主页前设置有效
        AutoStartInfo _autoStartOnce;

        /// <summary>
        /// 主页类型
        /// </summary>
        public override Type HomePageType
        {
            get { return _homePageType == null ? Type.GetType("Dt.App.DefaultHome,Dt.App") : _homePageType; }
        }

        /// <summary>
        /// 加载根内容 Desktop/Frame 和主页
        /// </summary>
        public override void ShowHome()
        {
            if (Kit.IsPhoneUI)
                LoadRootFrame();
            else
                LoadDesktop();
        }

        /// <summary>
        /// 加载PhoneUI模式的根Frame
        /// </summary>
        void LoadRootFrame()
        {
            SysVisual.RootContent = new Frame();

            // 主页作为根
            if (HomePageType != null)
            {
                Win win = Activator.CreateInstance(HomePageType) as Win;
                if (win != null)
                {
                    if (string.IsNullOrEmpty(win.Title))
                        win.Title = "主页";
                    win.Icon = Icons.主页;
                    win.NaviToHome();
                }
            }

            // 自启动
            AutoStartInfo autoStart = _autoStartOnce != null ? _autoStartOnce : AtState.GetAutoStart();
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
                    AtState.DelAutoStart();
                }
                // 只自启动一次
                _autoStartOnce = null;
            }
        }

        /// <summary>
        /// 加载Windows模式桌面
        /// </summary>
        void LoadDesktop()
        {
            Desktop desktop = new Desktop();

            // 主页
            if (HomePageType != null)
            {
                Win win = Activator.CreateInstance(HomePageType) as Win;
                if (win != null)
                {
                    if (string.IsNullOrEmpty(win.Title))
                        win.Title = "主页";
                    win.Icon = Icons.主页;
                    desktop.HomeWin = win;
                }
            }

            // 自启动
            AutoStartInfo autoStart = _autoStartOnce != null ? _autoStartOnce : AtState.GetAutoStart();
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
                    AtState.DelAutoStart();
                }
                // 只自启动一次
                _autoStartOnce = null;
            }

            if (desktop.MainWin == null)
                desktop.MainWin = desktop.HomeWin;
            SysVisual.RootContent = desktop;
        }
        
        /// <summary>
        /// 显示自定义根页面
        /// </summary>
        /// <param name="p_page"></param>
        public void ShowRoot(UIElement p_page)
        {
            SysVisual.RootContent = p_page;
        }
    }
}