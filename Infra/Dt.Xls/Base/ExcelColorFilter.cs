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
    /// A class implements interface <see cref="T:Dt.Xls.IExcelColorFilter" />
    /// </summary>
    public class ExcelColorFilter : IExcelColorFilter, IExcelFilter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.ExcelColorFilter" /> class.
        /// </summary>
        public ExcelColorFilter()
        {
            this.CellColor = true;
        }

        /// <summary>
        /// Flag indicating whether or not to filter by the cell's fill color
        /// </summary>
        /// <value></value>
        public bool CellColor { get; set; }

        /// <summary>
        /// Id of differential format record (dxf) in the Styles Part which expresses the color value to
        /// filter by.
        /// </summary>
        /// <value></value>
        public uint DxfId { get; set; }
    }
}

