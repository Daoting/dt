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
    /// A class implements <see cref="T:Dt.Xls.IRunFormatting" /> represents the run formatting
    /// </summary>
    public class RunFormating : IRunFormatting
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.RunFormating" /> class.
        /// </summary>
        /// <param name="language">The font language of the run formatting</param>
        /// <param name="typeface">The typeface of the run formatting</param>
        public RunFormating(Dt.Xls.FontLanguage language, string typeface)
        {
            this.FontLanguage = language;
            this.Typeface = typeface;
        }

        /// <summary>
        /// Specified font category
        /// </summary>
        /// <value></value>
        public Dt.Xls.FontLanguage FontLanguage { get; internal set; }

        /// <summary>
        /// The typeface is a string name of the specific font that should be used in rendering the presentation
        /// </summary>
        public string Typeface { get; internal set; }
    }
}

