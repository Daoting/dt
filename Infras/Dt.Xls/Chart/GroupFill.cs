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
    /// Represents a group fill.it indicates that the chart is part of a group and should inherit the fill properties of the group.
    /// </summary>
    public class GroupFill : IFillFormat
    {
        /// <summary>
        /// specifies the fill format type.
        /// </summary>
        /// <exception cref="T:System.NotImplementedException"></exception>
        public Dt.Xls.Chart.FillFormatType FillFormatType
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}

