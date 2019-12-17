#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-12-17 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Base
{
#if UWP
    /// <summary>
    /// 响应式UI管理类
    /// </summary>
    internal static class UIModeManager
    {
        const string _deskStylePath = "ms-appx:///Dt.Base/Themes/Styles/Desk.xaml";
        static ResourceDictionary _deskDict;

        public static void Init()
        {
            SysVisual.UIModeChanged = OnUIModeChanged;
            if (!AtSys.IsPhoneUI)
            {
                _deskDict = new ResourceDictionary() { Source = new Uri(_deskStylePath) };
                Application.Current.Resources.MergedDictionaries.Add(_deskDict);
            }
        }

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
            }

            // 重构根元素
            if (SysVisual.RootContent is Frame || SysVisual.RootContent is Desktop)
                AtApp.LoadRootUI();
        }
    }
#endif
}

