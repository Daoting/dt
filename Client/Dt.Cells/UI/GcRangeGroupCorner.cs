#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using System;
using Windows.Foundation;
#endregion

namespace Dt.Cells.UI
{
    internal partial class GcRangeGroupCorner : GcGroupBase
    {
        public GcRangeGroupCorner(Excel p_excel) : base(p_excel)
        {
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            MeasureBorderLines(availableSize);
            return base.MeasureOverride(availableSize);
        }
    }
}

