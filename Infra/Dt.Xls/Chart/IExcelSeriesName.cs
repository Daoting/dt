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
    /// Specifies text for a series name.
    /// </summary>
    public interface IExcelSeriesName
    {
        /// <summary>
        /// Specifies whethere the text is reference formula or text value.
        /// </summary>
        bool IsReferenceFormula { get; set; }

        /// <summary>
        /// Specifies a reference to data for a single datable or title.
        /// </summary>
        string ReferenceFormula { get; set; }

        /// <summary>
        /// Specifies a text value for a category axis lable or a series name.
        /// </summary>
        string TextValue { get; set; }
    }
}

