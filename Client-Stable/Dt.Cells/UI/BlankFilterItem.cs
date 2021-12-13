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
    internal class BlankFilterItem : AutoFilterItem
    {
        public static object Blank = new object();

        public override object Criterion
        {
            get { return  Blank; }
            set
            {
            }
        }
    }
}

