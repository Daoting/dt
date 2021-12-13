#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.ObjectModel;
using System.Reflection;
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// An interface used to defines a set of colors which are referenced to as a color scheme. The
    /// color scheme is responsible for defining a list a twelve colors. The twelve colors consist of
    /// six accent colors, two dark colors, two light colors and a color for each of a hyperlink and
    /// followed hyperlink.
    /// </summary>
    public interface IColorScheme
    {
        /// <summary>
        /// Get the color of the specific color scheme.
        /// </summary>
        /// <param name="index">The color scheme index used to locate the theme color</param>
        /// <returns>The theme color</returns>
        IExcelColor this[ColorSchemeIndex index] { get; }

        /// <summary>
        /// The common name for this color scheme.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The twelve <seealso cref="T:Dt.Xls.IExcelColor" /> collections used to define the colors.
        /// The twelve colors consist of six accent colors, two dark colors, two light colors 
        /// and a color for each of a hyperlink and followed hyperlink.
        /// </summary>
        ReadOnlyCollection<IExcelColor> SchemeColors { get; }
    }
}

