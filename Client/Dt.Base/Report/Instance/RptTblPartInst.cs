#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-06-11 创建
**************************************************************************/
#endregion

#region 命名空间
#endregion

namespace Dt.Base.Report
{
    /// <summary>
    /// 表格部分
    /// </summary>
    internal class RptTblPartInst : RptItemPartInst
    {
        public RptTblPartInst(RptItemBase p_item)
            : base(p_item)
        {
        }

        /// <summary>
        /// 获取所属表格
        /// </summary>
        public RptTableInst Table
        {
            get { return Parent as RptTableInst; }
        }

        public int Index { get; set; }

        /// <summary>
        /// 输出子元素
        /// </summary>
        protected void OutputChildren()
        {
            if (_children.Count == 0)
                return;

            // 统一输出后才可统计占的行数，因输出过程位置在变
            foreach (RptTextInst inst in _children)
            {
                inst.Output();
            }

            // 统计行跨度
            int maxSpan = 0;
            foreach (RptTextInst inst in _children)
            {
                RptRegion region = inst.Region;
                int span = region.Row + region.RowSpan - _region.Row;
                if (span > maxSpan)
                    maxSpan = span;
            }
            _region.RowSpan = maxSpan;
        }
    }
}
