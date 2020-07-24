#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using Windows.Foundation;
#endregion

namespace Dt.Cells.UI
{
    internal partial class GcRangeGroupCorner : GcGroupBase
    {
        public GcRangeGroupCorner(SheetView sheetView) : base(sheetView)
        {
        }

        protected override Windows.Foundation.Size MeasureOverride(Windows.Foundation.Size availableSize)
        {
            MeasureBorderLines(availableSize);
            return base.MeasureOverride(availableSize);
        }
    }
}

