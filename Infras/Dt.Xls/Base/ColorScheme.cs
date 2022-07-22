#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// A class implements <see cref="T:Dt.Xls.IColorScheme" /> represents color scheme used for excel theme
    /// </summary>
    public class ColorScheme : IColorScheme
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.ColorScheme" /> class.
        /// </summary>
        /// <param name="name">The name of the color scheme</param>
        /// <param name="schemeColors">The scheme colors.</param>
        public ColorScheme(string name, List<IExcelColor> schemeColors)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("name");
            }
            if (schemeColors == null)
            {
                throw new ArgumentNullException("schemeColors");
            }
            if (schemeColors.Count != 12)
            {
                throw new ArgumentException(ResourceHelper.GetResourceString("schemeColorsCountError"));
            }
            this.Name = name;
            this.SchemeColors = new ReadOnlyCollection<IExcelColor>((IList<IExcelColor>) schemeColors);
        }

        /// <summary>
        /// Get the color of the specific color scheme.
        /// </summary>
        /// <value></value>
        /// <returns>The theme color</returns>
        public IExcelColor this[ColorSchemeIndex index]
        {
            get { return  this.SchemeColors[(int) index]; }
        }

        /// <summary>
        /// The common name for this color scheme.
        /// </summary>
        /// <value>The name of the color scheme</value>
        public string Name { get; internal set; }

        /// <summary>
        /// The twelve <seealso cref="T:Dt.Xls.IExcelColor" /> collections used to define the colors.
        /// The twelve colors consist of six accent colors, two dark colors, two light colors
        /// and a color for each of a hyperlink and followed hyperlink.
        /// </summary>
        /// <value></value>
        public ReadOnlyCollection<IExcelColor> SchemeColors { get; internal set; }
    }
}

