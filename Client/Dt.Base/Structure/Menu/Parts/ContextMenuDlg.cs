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
            Background = Res.浅灰1;
            MinWidth = 160;

            // 不向下层对话框传递Press事件
            AllowRelayPress = false;
        }
    }
}
