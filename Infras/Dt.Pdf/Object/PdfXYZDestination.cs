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
    /// Display the page designated by page, with the coordinates (left, top)
    /// positioned at the upper-left corner of the window and the contents 
    /// of the page magnified by the factor zoom. A null value for any of the 
    /// parameters left, top, or zoom specifies that the current value of that 
    /// parameter is to be retained unchanged. A zoom value of 0 has the same 
    /// meaning as a null value.
    /// </summary>
    public class PdfXYZDestination : PdfDestination
    {
        private float left;
        private float top;
        private float zoom;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfXYZDestination" /> class.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="left">The left.</param>
        /// <param name="top">The top.</param>
        /// <param name="zoom">The zoom.</param>
        public PdfXYZDestination(PdfPage page, float left, float top, float zoom) : base(page)
        {
            this.left = left;
            this.top = top;
            this.zoom = zoom;
        }

        /// <summary>
        /// Adds the type and arguments.
        /// </summary>
        protected override void AddTypeAndArguments()
        {
            base.Add(PdfName.XYZ);
            base.Add(new PdfNumber((double) this.left));
            base.Add(new PdfNumber((double) this.top));
            base.Add(new PdfNumber((double) this.zoom));
        }

        /// <summary>
        /// Gets or sets the left.
        /// </summary>
        /// <value>The left.</value>
        public float Left
        {
            get { return  this.left; }
            set { this.left = value; }
        }

        /// <summary>
        /// Gets or sets the top.
        /// </summary>
        /// <value>The top.</value>
        public float Top
        {
            get { return  this.top; }
            set { this.top = value; }
        }

        /// <summary>
        /// Gets or sets the zoom.
        /// </summary>
        /// <value>The zoom.</value>
        public float Zoom
        {
            get { return  this.zoom; }
            set { this.zoom = value; }
        }
    }
}

