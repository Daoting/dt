#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Xls.Chart;
using System;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Specifies the display unit settings for the value axis.
    /// </summary>
    internal class DisplayUnitSettings
    {
        double _uint = double.NaN;

        /// <summary>
        /// Gets or sets the display unit.
        /// </summary>
        public double DisplayUnit
        {
            get { return  this._uint; }
            set
            {
                if (!(value - this._uint).IsZero())
                {
                    this._uint = value;
                }
            }
        }

        internal Dt.Cells.Data.Layout LabelLayout { get; set; }

        internal RichText LabelRichText { get; set; }

        internal string LabelText { get; set; }
    }
}

