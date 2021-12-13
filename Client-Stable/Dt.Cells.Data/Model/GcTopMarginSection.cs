#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a section located on the top margin of every page. 
    /// </summary>
    internal class GcTopMarginSection : GcMarginSection
    {
        /// <summary>
        /// Overrides the <see cref="T:Dt.Cells.Data.GcSection" /> property.
        /// </summary>
        /// <value>Equals the top margin of the report, in hundredths of an inch.</value>
        public override int Height
        {
            get
            {
                if (base.Report != null)
                {
                    return Math.Max(0, base.Report.Margin.Top - base.Report.Margin.Header);
                }
                return base.Height;
            }
            set
            {
                if (base.Report == null)
                {
                    base.Height = value;
                }
                else
                {
                    base.Report.Margin.Top = value + base.Report.Margin.Header;
                }
            }
        }
    }
}

