#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Cells.UI
{
    internal class NavigationButtonClickEventArgs : EventArgs
    {
        ButtonType _type;

        public NavigationButtonClickEventArgs(ButtonType type)
        {
            _type = type;
        }

        public ButtonType TabButton
        {
            get { return  _type; }
        }
    }
}

