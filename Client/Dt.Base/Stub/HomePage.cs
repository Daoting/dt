#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2016-02-18
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
            get { return _homePageType == null ? Type.GetType("Dt.Mgr.DefaultHome,Dt.Mgr") : _homePageType; }
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
            UITree.RootContent = new Frame();

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

            // 附加左右滑动手势
            AttachFrameManipulation();
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
            UITree.RootContent = desktop;
        }
        
        /// <summary>
        /// 显示自定义根页面
        /// </summary>
        /// <param name="p_page"></param>
        public void ShowRoot(UIElement p_page)
        {
            UITree.RootContent = p_page;
        }

        #region 左右滑动手势
        static void AttachFrameManipulation()
        {
            var frame = UITree.RootFrame;
            frame.ManipulationMode = ManipulationModes.System | ManipulationModes.TranslateX | ManipulationModes.TranslateY | ManipulationModes.TranslateInertia;
            frame.ManipulationInertiaStarting += OnManipulationInertiaStarting;
        }

        static void OnManipulationInertiaStarting(object sender, ManipulationInertiaStartingRoutedEventArgs e)
        {
            PhonePage page;
            if (e.PointerDeviceType != PointerDeviceType.Touch
                || e.Handled
                || (page = UITree.RootFrame.Content as PhonePage) == null)
                return;

            var trans = e.Cumulative.Translation;
            Kit.Debug("InertiaStarting：" + trans.ToString());

            // 水平滑动距离必须大于垂直滑动的n倍
            if (Math.Abs(trans.X) < Math.Abs(trans.Y) * 4)
                return;

            ScrollViewer sv;
            var tabs = page.Content as PhoneTabs;
            if (tabs == null)
            {
                // 内容非PhoneTabs，只支持向右滑动，页面后退
                if (trans.X > 0)
                {
                    sv = page.FindChildByType<ScrollViewer>();
                    if (sv == null
                        || sv.ScrollableWidth == 0
                        || sv.HorizontalOffset == 0)
                    {
                        InputManager.GoBack();
                    }
                }
                return;
            }

            // 内容为PhoneTabs，支持左右滑动
            sv = tabs.FindChildByType<ScrollViewer>();
            if (sv != null)
            {
                // 内容正在垂直滚动
                //if (trans.Y != 0
                //    && sv.ScrollableHeight > 0
                //    && ((trans.Y < 0 && sv.VerticalOffset < sv.ScrollableHeight)
                //        || (trans.Y > 0 && sv.VerticalOffset > 0)))
                //    return;

                // 内容正在水平滚动
                if (sv.ScrollableWidth > 0
                    && ((trans.X < 0 && sv.HorizontalOffset < sv.ScrollableWidth)
                        || (trans.X > 0 && sv.HorizontalOffset > 0)))
                    return;
            }

            if (trans.X < 0)
            {
                // 选择右侧Tab
                tabs.SelectNext();
            }
            else
            {
                // 选择左侧Tab，若已是最左侧，页面后退
                var suc = tabs.SelectPrevious();
                if (!suc)
                {
                    InputManager.GoBack();
                }
            }
        }
        #endregion
    }
}