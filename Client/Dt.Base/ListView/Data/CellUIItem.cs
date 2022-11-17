#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-11-26 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.UI.Xaml;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 单元格UI的缓存项
    /// </summary>
    class CellUIItem
    {
        public CellUIItem(UIElement p_ui, bool p_isBinding)
        {
            UI = p_ui;
            IsBinding = p_isBinding;
        }

        /// <summary>
        /// 单元格UI
        /// </summary>
        public UIElement UI { get; }

        /// <summary>
        /// 单元格UI是否采用绑定方式，默认false
        /// </summary>
        public bool IsBinding { get; }
    }
}