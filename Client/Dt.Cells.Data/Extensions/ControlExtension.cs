#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Cells.Data
{
    internal static class ControlExtension
    {
        public static bool Focus(this Control This)
        {
            return This.Focus(FocusState.Programmatic);
        }
    }
}

