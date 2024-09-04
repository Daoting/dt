#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// Represents a filter dropdown dialog's separator item control.
    /// </summary>
    public partial class SeparatorDropDownItemControl : DropDownItemBaseControl
    {
        /// <summary>
        /// Creates a new instance of the <see cref="T:SeparatorDropDownItemControl" /> class.
        /// </summary>
        public SeparatorDropDownItemControl()
        {
            base.DefaultStyleKey = typeof(SeparatorDropDownItemControl);
        }

        internal override bool CanSelect
        {
            get { return  false; }
        }
    }
}

