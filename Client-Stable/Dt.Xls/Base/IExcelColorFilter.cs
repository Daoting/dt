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

namespace Dt.Xls
{
    /// <summary>
    /// Specifies the color to filter by and whether to use the cell's fill or font color in the filter criteria. 
    /// </summary>
    /// <remarks>
    /// If the cell's font or fill color does not match the color specified in the criteria, the rows corresponding to those cells
    /// are hidden from view.
    /// </remarks>
    public interface IExcelColorFilter : IExcelFilter
    {
        /// <summary>
        /// Flag indicating whether or not to filter by the cell's fill color
        /// </summary>
        bool CellColor { get; set; }

        /// <summary>
        /// Id of differential format record (dxf) in the Styles Part which expresses the color value to
        /// filter by.
        /// </summary>
        uint DxfId { get; set; }
    }
}

