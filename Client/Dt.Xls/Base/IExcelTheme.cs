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
    /// Represents theme used for excel.
    /// </summary>
    public interface IExcelTheme
    {
        /// <summary>
        /// Represents a set of colors which are referenced to as a color scheme
        /// </summary>
        /// <value>An <see cref="T:Dt.Xls.IColorScheme" /> instance represents the colors settings of the theme</value>
        IColorScheme ColorScheme { get; }

        /// <summary>
        /// Represents a font scheme within the theme
        /// </summary>
        /// <value>An <see cref="T:Dt.Xls.IFontScheme" /> instance represents the fonts settings of the theme</value>
        IFontScheme FontScheme { get; }

        /// <summary>
        /// The theme name
        /// </summary>
        /// <value>The name of the theme</value>
        string Name { get; }
    }
}

