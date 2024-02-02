#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Xls.Chart
{
    /// <summary>
    /// Specifies a line style that can be applied to shapes and texts.
    /// </summary>
    public interface ILineFormat
    {
        /// <summary>
        /// Specifies the compound line type that is to be used for lines with text such as underlines.
        /// </summary>
        Dt.Xls.Chart.CompoundLineType CompoundLineType { get; set; }

        /// <summary>
        /// Specifies the line color fill format.
        /// </summary>
        IFillFormat FillFormat { get; set; }

        /// <summary>
        /// Specifies decorations which can be added to the head of a line
        /// </summary>
        LineEndStyle HeadLineEndStyle { get; set; }

        /// <summary>
        /// 
        /// </summary>
        EndLineJoinType JoinType { get; set; }

        /// <summary>
        /// Represents the line dash type
        /// </summary>
        Dt.Xls.Chart.LineDashType LineDashType { get; set; }

        /// <summary>
        /// Specifies the ending caps that should be used for this line.
        /// </summary>
        /// <remarks>
        /// The default value is Square.
        /// </remarks>
        EndLineCap LineEndingCap { get; set; }

        /// <summary>
        /// Specifies the Pen Alignment type.
        /// </summary>
        Dt.Xls.Chart.PenAlignment PenAlignment { get; set; }

        /// <summary>
        /// Specifies decorations which can be added to the tail of a line
        /// </summary>
        LineEndStyle TailLineEndStyle { get; set; }

        /// <summary>
        /// Specifies the width to be used for the underline stroke. Default value is 0.
        /// </summary>
        double Width { get; set; }
    }
}

