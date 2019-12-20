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
    /// Defines the theme font
    /// </summary>
    public interface IThemeFont
    {
        /// <summary>
        /// Specified the script, or language, in which the typeface is supposed to be used
        /// </summary>
        string Script { get; }

        /// <summary>
        /// Specifies the font face to use
        /// </summary>
        string Typeface { get; }
    }
}

