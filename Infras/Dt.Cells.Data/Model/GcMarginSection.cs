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
    /// Defines an abstract class for margin sections.
    /// </summary>
    internal abstract class GcMarginSection : GcAppendixSection, IGcSpecialSection
    {
        GcReport report;

        protected GcMarginSection()
        {
        }

        /// <summary>
        /// Overrides the <see cref="T:Dt.Cells.Data.GcSection" /> property.
        /// </summary>
        /// <value>
        /// This property is always <c>false</c>.
        /// </value>
        /// <remarks>This property is read-only.</remarks>
        public override bool CanGrow
        {
            get { return  false; }
            set
            {
            }
        }

        /// <summary>
        /// Overrides the <see cref="T:Dt.Cells.Data.GcSection" /> property.
        /// </summary>
        /// <value>
        /// This property is always <c>false</c>.
        /// </value>
        /// <remarks>This property is read-only.</remarks>
        public override bool CanShrink
        {
            get { return  false; }
            set
            {
            }
        }

        /// <summary>
        /// Internal only.
        /// Gets or sets the page setting.
        /// </summary>
        /// <value>The page setting</value>
        internal GcReport Report
        {
            get { return  this.report; }
            set { this.report = value; }
        }
    }
}

