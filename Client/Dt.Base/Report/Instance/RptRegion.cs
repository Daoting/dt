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
    /// 报表元素区域描述
    /// </summary>
    internal class RptRegion
    {
        #region 构造方法
        public RptRegion()
        {
        }

        public RptRegion(int p_row, int p_column, int p_rowSpan, int p_colSpan)
        {
            Row = p_row;
            Col = p_column;
            RowSpan = p_rowSpan;
            ColSpan = p_colSpan;
        }

        public RptRegion(RptRegion p_region)
        {
            Row = p_region.Row;
            Col = p_region.Col;
            RowSpan = p_region.RowSpan;
            ColSpan = p_region.ColSpan;
        }
        #endregion

        /// <summary>
        /// 获取设置起始行索引
        /// </summary>
        public int Row { get; set; }

        /// <summary>
        /// 获取设置起始列索引
        /// </summary>
        public int Col { get; set; }

        /// <summary>
        /// 获取设置占用行数
        /// </summary>
        public int RowSpan { get; set; }

        /// <summary>
        /// 获取设置占用列数
        /// </summary>
        public int ColSpan { get; set; }

        public RptRegion Clone()
        {
            return new RptRegion(this);
        }
    }
}
