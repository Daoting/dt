#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.ObjectModel;
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// Represents the theme fonts
    /// </summary>
    public interface IThemeFonts
    {
        /// <summary>
        /// Gets the run formattings.
        /// </summary>
        /// <value>The run formattings.</value>
        ReadOnlyCollection<IRunFormatting> RunFormattings { get; }

        /// <summary>
        /// Gets the themes fonts.
        /// </summary>
        /// <value>The themes fonts.</value>
        ReadOnlyCollection<IThemeFont> ThemesFonts { get; }
    }
}

