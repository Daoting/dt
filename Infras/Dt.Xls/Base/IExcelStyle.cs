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
    /// Defines cell styles used for excel
    /// </summary>
    public interface IExcelStyle
    {
        /// <summary>
        /// Gets or sets the <see cref="T:Dt.Xls.IExtendedFormat" /> used in the cell style.
        /// </summary>
        /// <value>An <see cref="T:Dt.Xls.IExtendedFormat" /> represents the cell style format setting</value>
        IExtendedFormat Format { get; set; }

        /// <summary>
        /// Gets a value indicating whether the style is built-in style.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if this style is built-in style; otherwise, <see langword="false" />.
        /// </value>
        bool IsBuiltInStyle { get; }

        /// <summary>
        /// Gets the name of the style
        /// </summary>
        /// <value>The name of the style</value>
        string Name { get; }
    }
}

