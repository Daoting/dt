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
    /// Display the page designated by page, with its contents magnified just 
    /// enough to fit the entire page within the window both horizontally and 
    /// vertically. If the required horizontal and vertical magnification 
    /// factors are different, use the smaller of the two, centering the 
    /// page within the window in the other dimension.
    /// </summary>
    public class PdfFitDestination : PdfDestination
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfFitDestination" /> class.
        /// </summary>
        /// <param name="page">The page.</param>
        public PdfFitDestination(PdfPage page) : base(page)
        {
        }

        /// <summary>
        /// Adds the type and arguments.
        /// </summary>
        protected override void AddTypeAndArguments()
        {
            base.Add(PdfName.Fit);
        }
    }
}

