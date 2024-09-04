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
    internal class AxisCollection : SpreadChartElementCollection<Axis>
    {
        internal AxisCollection(SpreadChart chart, ChartArea area) : base(chart, area)
        {
        }

        protected override void ClearInternal()
        {
            while (base.Count > 1)
            {
                base.RemoveAt(base.Count - 1);
            }
        }

        public override bool Remove(Axis item)
        {
            if (this.Count > 1)
            {
                base.Remove(item);
                return true;
            }
            return false;
        }
    }
}

