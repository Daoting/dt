#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Pdf.Exceptions
{
    /// <summary>
    /// Argument Exception
    /// </summary>
    public class PdfArgumentException : PdfException
    {
        private readonly string arg;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Pdf.Exceptions.PdfArgumentException" /> class.
        /// </summary>
        /// <param name="arg">The arg.</param>
        public PdfArgumentException(string arg)
        {
            this.arg = arg;
        }

        /// <summary>
        /// Gets the arg.
        /// </summary>
        /// <value>The arg.</value>
        internal string Arg
        {
            get { return  this.arg; }
        }
    }
}

