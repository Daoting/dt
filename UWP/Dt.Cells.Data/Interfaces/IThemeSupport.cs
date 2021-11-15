#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using Windows.UI;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents an interface for supporting a theme.
    /// </summary>
    public interface IThemeSupport
    {
        /// <summary>
        /// Gets the color with a specified theme name.
        /// </summary>
        /// <param name="themeName">Name of the theme.</param>
        /// <returns>The color.</returns>
        Windows.UI.Color GetThemeColor(string themeName);
        /// <summary>
        /// Gets the font family with a specified theme name.
        /// </summary>
        /// <param name="themeName">Name of the theme.</param>
        /// <returns>The Font family.</returns>
        FontFamily GetThemeFont(string themeName);
    }
}

