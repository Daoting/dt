#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-10-10 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Threading.Tasks;
using Windows.UI.Xaml;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 对话框扩展方法
    /// </summary>
    public static class DlgEx
    {
        /// <summary>
        /// 显示对话框
        /// </summary>
        /// <param name="p_dlg">对话框</param>
        /// <param name="p_winPlacement">windows模式的显示位置</param>
        /// <param name="p_phonePlacement">phone模式的显示位置</param>
        /// <param name="p_target">采用相对位置显示时的目标元素</param>
        /// <param name="p_hideTitleBar">否隐藏标题栏</param>
        /// <param name="p_isPinned">是否固定对话框</param>
        public static void ShowAt(
            this Dlg p_dlg,
            DlgPlacement p_winPlacement,
            DlgPlacement p_phonePlacement,
            FrameworkElement p_target = null,
            bool p_hideTitleBar = false,
            bool p_isPinned = false)
        {
            if (p_dlg != null)
            {
                p_dlg.WinPlacement = p_winPlacement;
                p_dlg.PhonePlacement = p_phonePlacement;
                if (p_target != null)
                    p_dlg.PlacementTarget = p_target;
                if (p_hideTitleBar)
                    p_dlg.HideTitleBar = true;
                if (p_isPinned)
                    p_dlg.IsPinned = true;
                p_dlg.Show();
            }
        }

        /// <summary>
        /// 显示对话框
        /// </summary>
        /// <param name="p_dlg">对话框</param>
        /// <param name="p_winPlacement">windows模式的显示位置</param>
        /// <param name="p_phonePlacement">phone模式的显示位置</param>
        /// <param name="p_target">采用相对位置显示时的目标元素</param>
        /// <param name="p_hideTitleBar">否隐藏标题栏</param>
        /// <param name="p_isPinned">是否固定对话框</param>
        /// <returns></returns>
        public static Task ShowAtAsync(
            this Dlg p_dlg,
            DlgPlacement p_winPlacement,
            DlgPlacement p_phonePlacement,
            FrameworkElement p_target = null,
            bool p_hideTitleBar = false,
            bool p_isPinned = false)
        {
            if (p_dlg != null)
            {
                p_dlg.WinPlacement = p_winPlacement;
                p_dlg.PhonePlacement = p_phonePlacement;
                if (p_target != null)
                    p_dlg.PlacementTarget = p_target;
                if (p_hideTitleBar)
                    p_dlg.HideTitleBar = true;
                if (p_isPinned)
                    p_dlg.IsPinned = true;
                return p_dlg.ShowAsync();
            }
            return Task.CompletedTask;
        }
    }
}
