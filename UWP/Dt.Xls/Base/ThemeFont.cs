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
    /// A class implements <see cref="T:Dt.Xls.IThemeFont" /> used for represents the theme font
    /// </summary>
    public class ThemeFont : IThemeFont
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.ThemeFont" /> class.
        /// </summary>
        /// <param name="script">The script.</param>
        /// <param name="typeface">The typeface.</param>
        public ThemeFont(string script, string typeface)
        {
            this.Script = script;
            this.Typeface = typeface;
        }

        /// <summary>
        /// Specified the script, or language, in which the typeface is supposed to be used
        /// </summary>
        /// <value></value>
        public string Script { get; internal set; }

        /// <summary>
        /// Specifies the font face to use
        /// </summary>
        /// <value></value>
        public string Typeface { get; internal set; }
    }
}

