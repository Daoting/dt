#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Pdf.BaseObject;
using System;
#endregion

namespace Dt.Pdf.Object
{
    /// <summary>
    /// A content stream is a PDF stream object whose data consists 
    /// of a sequence of instructions describing the graphical 
    /// elements to be painted on a page.
    /// </summary>
    public class PdfContent : PdfStream
    {
        private readonly PdfGraphics graphics;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfContent" /> class.
        /// </summary>
        public PdfContent() : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfContent" /> class.
        /// </summary>
        /// <param name="gStream">The g stream.</param>
        internal PdfContent(IGraphicsStream gStream)
        {
            this.graphics = new PdfGraphics(base.Psw, gStream);
        }

        /// <summary>
        /// Gets the graphics.
        /// </summary>
        /// <value>The graphics.</value>
        public PdfGraphics Graphics
        {
            get { return  this.graphics; }
        }
    }
}

