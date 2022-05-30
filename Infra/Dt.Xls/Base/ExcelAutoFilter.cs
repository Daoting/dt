#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// A class implements interface <see cref="T:Dt.Xls.IExcelAutoFilter" />
    /// </summary>
    public class ExcelAutoFilter : IExcelAutoFilter
    {
        private List<IExcelFilterColumn> _filterColumns;

        /// <summary>
        /// An collections used to represents AutoFilter information
        /// </summary>
        /// <value></value>
        public List<IExcelFilterColumn> FilterColumns
        {
            get
            {
                if (this._filterColumns == null)
                {
                    this._filterColumns = new List<IExcelFilterColumn>();
                }
                return this._filterColumns;
            }
            set { this._filterColumns = value; }
        }

        /// <summary>
        /// An <see cref="T:Dt.Xls.IRange" /> instance used to define the AutoFilter scope.
        /// </summary>
        /// <value>
        /// The AutoFilter Range
        /// </value>
        public IRange Range { get; set; }
    }
}

