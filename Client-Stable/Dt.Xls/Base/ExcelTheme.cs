#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// A class implements <see cref="T:Dt.Xls.IExcelTheme" /> used for represents theme used for excel.
    /// </summary>
    public class ExcelTheme : IExcelTheme
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.ExcelTheme" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="colorScheme">The color scheme.</param>
        /// <param name="fontScheme">The font scheme.</param>
        public ExcelTheme(string name, IColorScheme colorScheme, IFontScheme fontScheme)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("name");
            }
            if (colorScheme == null)
            {
                throw new ArgumentNullException("colorScheme");
            }
            if (fontScheme == null)
            {
                throw new ArgumentNullException("fontScheme");
            }
            this.Name = name;
            this.ColorScheme = colorScheme;
            this.FontScheme = fontScheme;
        }

        /// <summary>
        /// Represents a set of colors which are referenced to as a color scheme
        /// </summary>
        /// <value>
        /// An <see cref="T:Dt.Xls.IColorScheme" /> instance represents the colors settings of the theme
        /// </value>
        public IColorScheme ColorScheme { get; internal set; }

        /// <summary>
        /// Represents a font scheme within the theme
        /// </summary>
        /// <value>
        /// An <see cref="T:Dt.Xls.IFontScheme" /> instance represents the fonts settings of the theme
        /// </value>
        public IFontScheme FontScheme { get; internal set; }

        /// <summary>
        /// The theme name
        /// </summary>
        /// <value>The name of the theme</value>
        public string Name { get; internal set; }
    }
}

