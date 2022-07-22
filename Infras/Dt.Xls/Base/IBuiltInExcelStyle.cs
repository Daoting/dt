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
    /// Defines the built-in cell style
    /// </summary>
    public interface IBuiltInExcelStyle : IExcelStyle
    {
        /// <summary>
        /// Gets or sets the built in style index.
        /// </summary>
        /// <value>The built in style index</value>
        BuiltInStyleIndex BuiltInStyle { get; set; }

        /// <summary>
        /// Gets or sets the category of the built-in style
        /// </summary>
        /// <value>The category of the built-in style</value>
        int Category { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the style has been modified
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the style has been modified; otherwise, <see langword="false" />.
        /// </value>
        bool IsCustomBuiltIn { get; set; }

        /// <summary>
        /// Gets or sets the out line level of the built-in style
        /// </summary>
        /// <value>The outline level of the built-in style </value>
        byte OutLineLevel { get; set; }
    }
}

