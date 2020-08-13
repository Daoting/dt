#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using System;
using System.Collections.Generic;
using Windows.Foundation;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// 列头视口
    /// </summary>
    internal partial class ColHeaderPanel : HeaderPanel
    {
        public ColHeaderPanel(SheetView sheet)
            : base(sheet, SheetArea.ColumnHeader)
        {
        }
    }
}

