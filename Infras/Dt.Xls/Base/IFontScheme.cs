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
    /// Represents the font scheme within the theme. The font scheme consists of a pair of major and
    /// minor fonts for which to used in a document.
    /// </summary>
    public interface IFontScheme
    {
        /// <summary>
        /// Gets the major font of the font scheme
        /// </summary>
        /// <value>The major font scheme</value>
        IThemeFonts MajorFont { get; }

        /// <summary>
        /// Gets the minor font of the font scheme
        /// </summary>
        /// <value>The minor font scheme</value>
        IThemeFonts MinorFont { get; }

        /// <summary>
        /// The name of the font scheme shown in the user interface
        /// </summary>
        string Name { get; }
    }
}

