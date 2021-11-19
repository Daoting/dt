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
    /// Argument out of range Exception
    /// </summary>
    public class PdfArgumentOutOfRangeException : PdfArgumentException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Pdf.Exceptions.PdfArgumentOutOfRangeException" /> class.
        /// </summary>
        /// <param name="arg">The arg.</param>
        public PdfArgumentOutOfRangeException(string arg) : base(arg)
        {
        }
    }
}

