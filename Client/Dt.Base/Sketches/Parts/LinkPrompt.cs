#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-09-09 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using Windows.Foundation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;

#endregion

namespace Dt.Base.Sketches
{
    /// <summary>
    /// 
    /// </summary>
    public sealed partial class LinkPrompt : Control
    {
        /// <summary>
        /// 
        /// </summary>
        public LinkPrompt()
        {
            DefaultStyleKey = typeof(LinkPrompt);
        }

        /// <summary>
        /// 设置有效点
        /// </summary>
        /// <param name="p_pos"></param>
        public void SetValidPos(LinkPortPosition p_pos)
        {
            VisualStateManager.GoToState(this, p_pos.ToString(), true);
        }
    }
}
