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
    /// A class implements <see cref="T:Dt.Xls.IFontScheme" /> represents the font scheme used for excel theme
    /// </summary>
    public class FontScheme : IFontScheme
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.FontScheme" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="majorFont">The major font.</param>
        /// <param name="minorFont">The minor font.</param>
        public FontScheme(string name, IThemeFonts majorFont, IThemeFonts minorFont)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("name");
            }
            if (majorFont == null)
            {
                throw new ArgumentNullException("majorFont");
            }
            if (minorFont == null)
            {
                throw new ArgumentNullException("minorFont");
            }
            this.Name = name;
            this.MajorFont = majorFont;
            this.MinorFont = minorFont;
        }

        /// <summary>
        /// Gets the major font of the font scheme
        /// </summary>
        /// <value>The major font scheme</value>
        public IThemeFonts MajorFont { get; internal set; }

        /// <summary>
        /// Gets the minor font of the font scheme
        /// </summary>
        /// <value>The minor font scheme</value>
        public IThemeFonts MinorFont { get; internal set; }

        /// <summary>
        /// The name of the font scheme shown in the user interface
        /// </summary>
        /// <value></value>
        public string Name { get; internal set; }
    }
}

