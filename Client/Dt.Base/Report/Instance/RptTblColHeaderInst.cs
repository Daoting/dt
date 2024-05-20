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
    /// 表格列头实例
    /// </summary>
    public class RptTblColHeaderInst : RptTblPartInst
    {
        public RptTblColHeaderInst(RptItemBase p_item)
            : base(p_item)
        {
        }

        /// <summary>
        /// 输出报表项内容
        /// </summary>
        protected override void DoOutput()
        {
            OutputChildren();
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <returns></returns>
        public RptTblColHeaderInst Clone()
        {
            RptTblColHeaderInst inst = new RptTblColHeaderInst(_item);
            foreach (RptTextInst item in _children)
            {
                inst.AddChild(item.Clone());
            }
            return inst;
        }
    }
}
