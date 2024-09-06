#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-06-19 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Demo.Base
{
    [Tbl("物资计划")]
    public partial class 物资计划X : EntityX<物资计划X>
    {
        #region 构造方法
        物资计划X() { }

        public 物资计划X(CellList p_cells) : base(p_cells) { }

        public 物资计划X(
            long ID,
            long? 部门id = default,
            string No = default,
            计划类型? 计划类型 = default,
            计划编制方法? 编制方法 = default,
            string 摘要 = default,
            string 编制人 = default,
            DateTime? 编制日期 = default,
            string 审核人 = default,
            DateTime? 审核日期 = default)
        {
            Add("id", ID);
            Add("部门id", 部门id);
            Add("no", No);
            Add("计划类型", 计划类型);
            Add("编制方法", 编制方法);
            Add("摘要", 摘要);
            Add("编制人", 编制人);
            Add("编制日期", 编制日期);
            Add("审核人", 审核人);
            Add("审核日期", 审核日期);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public long? 部门id
        {
            get { return (long?)this["部门id"]; }
            set { this["部门id"] = value; }
        }

        public Cell c部门id => _cells["部门id"];

        /// <summary>
        /// 
        /// </summary>
        public string No
        {
            get { return (string)this["no"]; }
            set { this["no"] = value; }
        }

        public Cell cNo => _cells["no"];

        /// <summary>
        /// 月;季;年
        /// </summary>
        public 计划类型? 计划类型
        {
            get { return (计划类型?)this["计划类型"]; }
            set { this["计划类型"] = value; }
        }

        public Cell c计划类型 => _cells["计划类型"];

        /// <summary>
        /// 1-往年同期线性参照法,2-临近期间平均参照法,3-物资储备定额参照法,4-由部门申购计划产生
        /// </summary>
        public 计划编制方法? 编制方法
        {
            get { return (计划编制方法?)this["编制方法"]; }
            set { this["编制方法"] = value; }
        }

        public Cell c编制方法 => _cells["编制方法"];

        /// <summary>
        /// 
        /// </summary>
        public string 摘要
        {
            get { return (string)this["摘要"]; }
            set { this["摘要"] = value; }
        }

        public Cell c摘要 => _cells["摘要"];

        /// <summary>
        /// 
        /// </summary>
        public string 编制人
        {
            get { return (string)this["编制人"]; }
            set { this["编制人"] = value; }
        }

        public Cell c编制人 => _cells["编制人"];

        /// <summary>
        /// 
        /// </summary>
        public DateTime? 编制日期
        {
            get { return (DateTime?)this["编制日期"]; }
            set { this["编制日期"] = value; }
        }

        public Cell c编制日期 => _cells["编制日期"];

        /// <summary>
        /// 
        /// </summary>
        public string 审核人
        {
            get { return (string)this["审核人"]; }
            set { this["审核人"] = value; }
        }

        public Cell c审核人 => _cells["审核人"];

        /// <summary>
        /// 
        /// </summary>
        public DateTime? 审核日期
        {
            get { return (DateTime?)this["审核日期"]; }
            set { this["审核日期"] = value; }
        }

        public Cell c审核日期 => _cells["审核日期"];
    }
}