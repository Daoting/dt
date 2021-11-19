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
    /// Defines the run formatting
    /// </summary>
    public interface IRunFormatting
    {
        /// <summary>
        /// Specified font category
        /// </summary>
        Dt.Xls.FontLanguage FontLanguage { get; }

        /// <summary>
        /// Gets the typeface.
        /// </summary>
        /// <value>The typeface.</value>
        string Typeface { get; }
    }
}

