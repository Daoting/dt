#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// An interface represents an AutoFilter definitions
    /// </summary>
    public interface IExcelAutoFilter
    {
        /// <summary>
        /// An collections used to represents AutoFilter information
        /// </summary>
        List<IExcelFilterColumn> FilterColumns { get; set; }

        /// <summary>
        /// An <see cref="T:Dt.Xls.IRange" /> instance used to define the AutoFilter scope.
        /// </summary>
        IRange Range { get; set; }
    }
}

