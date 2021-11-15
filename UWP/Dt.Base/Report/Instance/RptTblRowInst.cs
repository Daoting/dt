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
    /// 表格数据行实例
    /// </summary>
    internal class RptTblRowInst : RptTblPartInst
    {
        public RptTblRowInst(RptItemBase p_item)
            : base(p_item)
        {
        }

        /// <summary>
        /// 输出报表项内容
        /// </summary>
        protected override void DoOutput()
        {
            Table.Data.Current = Index;
            OutputChildren();
        }
    }
}
