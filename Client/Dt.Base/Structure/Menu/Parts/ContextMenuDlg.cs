#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Windows.Foundation;
#endregion

namespace Dt.Base.MenuView
{
    public partial class ContextMenuDlg : Dlg
    {
        public ContextMenuDlg(Menu p_menu)
        {
            HideTitleBar = true;
            Resizeable = false;
            Content = p_menu;
            Background = AtRes.浅灰背景;
            MinWidth = 160;
        }

        /// <summary>
        /// 点击对话框外部时
        /// </summary>
        /// <param name="p_point">外部点击位置</param>
        protected override void OnOuterPressed(Point p_point)
        {
            // PhoneUI模式 或 只有当前窗口
            if (AtSys.IsPhoneUI || SysVisual.DlgCount == 1)
                base.OnOuterPressed(p_point);

            // WinUI模式不自动关闭
        }
    }
}
