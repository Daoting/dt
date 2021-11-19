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
    /// Specifies the icon set and particular icon within that set to filter by.
    /// </summary>
    /// <remarks>
    /// For any cells whose icon does
    /// not match the specified criteria, the corresponding rows shall be hidden from view when the filter is applied.  
    /// </remarks>
    public interface IExcelIconFilter : IExcelFilter
    {
        /// <summary>
        /// Zero-based index of an icon in an icon set
        /// </summary>
        uint IconId { get; set; }

        /// <summary>
        /// Specifies which icon set is used in the filter criteria.
        /// </summary>
        ExcelIconSetType IconSet { get; set; }

        /// <summary>
        /// Flag indicate whether show icon or not
        /// </summary>
        bool NoIcon { get; set; }
    }
}

