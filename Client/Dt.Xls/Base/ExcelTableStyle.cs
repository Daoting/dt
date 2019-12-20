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
    /// An implementation of <see cref="T:Dt.Xls.IExcelTableStyle" />
    /// </summary>
    public class ExcelTableStyle : IExcelTableStyle
    {
        private List<IExcelTableStyleElement> _tableStyleElement;

        /// <summary>
        /// A flag indicate whether this table style should be shown as an available pivot table style.
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// Not mutually exclusive with table - both can be true.
        /// </remarks>
        public bool IsPivotStyle { get; set; }

        /// <summary>
        /// A flag indicate whether this table style should be shown as an available table style.
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// Not mutually exclusive with pivot - both can be true.
        /// </remarks>
        public bool IsTableStyle { get; set; }

        /// <summary>
        /// Name of this table style.
        /// </summary>
        /// <value></value>
        public string Name { get; set; }

        /// <summary>
        /// Table style elements defined for this table style.
        /// </summary>
        /// <value></value>
        public List<IExcelTableStyleElement> TableStyleElements
        {
            get
            {
                if (this._tableStyleElement == null)
                {
                    this._tableStyleElement = new List<IExcelTableStyleElement>();
                }
                return this._tableStyleElement;
            }
            set { this._tableStyleElement = value; }
        }
    }
}

