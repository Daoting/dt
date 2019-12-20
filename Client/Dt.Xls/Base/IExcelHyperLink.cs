#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// Represents Hyperlink used in excel.
    /// </summary>
    public interface IExcelHyperLink
    {
        /// <summary>
        /// Gets or sets the address of the linked location.
        /// </summary>
        /// <value>The address of the linked location.</value>
        string Address { get; set; }

        /// <summary>
        /// Gets or sets the description of the hyperlink.
        /// </summary>
        /// <value>The description of the hyperlink</value>
        string Description { get; set; }

        /// <summary>
        /// Represents the type of the  hyperlink.
        /// </summary>
        /// <value>The type of the hyperlink</value>
        HyperLinkType Type { get; }
    }
}

