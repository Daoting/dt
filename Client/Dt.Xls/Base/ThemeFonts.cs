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
using System.Linq;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// Represents the theme fonts
    /// </summary>
    public class ThemeFonts : IThemeFonts
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.ThemeFonts" /> class.
        /// </summary>
        /// <param name="runFormats">The run formats.</param>
        /// <param name="themesFonts">The themes fonts.</param>
        public ThemeFonts(List<IRunFormatting> runFormats, List<IThemeFont> themesFonts)
        {
            if (runFormats == null)
            {
                throw new ArgumentNullException("runFormats");
            }
            HashSet<FontLanguage> set = new HashSet<FontLanguage>(Enumerable.Select<IRunFormatting, FontLanguage>((IEnumerable<IRunFormatting>) runFormats, delegate (IRunFormatting item) {
                return item.FontLanguage;
            }));
            if (set.Count != runFormats.Count)
            {
                throw new ArgumentException(ResourceHelper.GetResourceString("themesRunFormatsError"));
            }
            if (themesFonts != null)
            {
                HashSet<string> set2 = new HashSet<string>(Enumerable.Select<IThemeFont, string>((IEnumerable<IThemeFont>) themesFonts, delegate (IThemeFont item) {
                    return item.Script;
                }));
                if (set2.Count != themesFonts.Count)
                {
                    throw new ArgumentException(ResourceHelper.GetResourceString("themesFontsError"));
                }
            }
            this.RunFormattings = new ReadOnlyCollection<IRunFormatting>((IList<IRunFormatting>) runFormats);
            this.ThemesFonts = new ReadOnlyCollection<IThemeFont>((IList<IThemeFont>) themesFonts);
        }

        /// <summary>
        /// Gets the run formattings.
        /// </summary>
        /// <value>The run formattings.</value>
        public ReadOnlyCollection<IRunFormatting> RunFormattings { get; internal set; }

        /// <summary>
        /// Gets the themes fonts.
        /// </summary>
        /// <value>The themes fonts.</value>
        public ReadOnlyCollection<IThemeFont> ThemesFonts { get; internal set; }
    }
}

