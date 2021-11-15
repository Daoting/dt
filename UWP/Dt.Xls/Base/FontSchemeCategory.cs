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
    /// Defines the font scheme,  if any, to which this font belongs
    /// </summary>
    /// <remarks>
    /// When a font definition is part of a theme definition,
    /// then the font is categorized as either a major or minor font scheme component. When a new theme is chosen,
    /// every font that is part of a theme definition is updated to use the new major or minor font definition for that
    /// theme. Usually major fonts are used for styles like headings, and minor fonts are used for body and paragraph
    /// text.
    /// </remarks>
    public enum FontSchemeCategory
    {
        /// <summary>
        /// This font is the major font for this theme.
        /// </summary>
        Major = 1,
        /// <summary>
        /// This font is the minor font for this theme.
        /// </summary>
        Minor = 2,
        /// <summary>
        /// This font is in Ninched state.
        /// </summary>
        Ninched = 0xff,
        /// <summary>
        /// This font is not a theme font.
        /// </summary>
        None = 0
    }
}

