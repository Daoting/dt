#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-07-17 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Demo.Base
{
    [Tbl("物资主单")]
    public partial class 物资主单X : EntityX<物资主单X>
    {
        #region 构造方法
        物资主单X() { }

        public 物资主单X(CellList p_cells) : base(p_cells) { }

        public 物资主单X(
            long ID,
            long 部门id = default,
            long 入出类别id = default,
            单据状态 状态 = default,
            string 单号 = default,
            string 摘要 = default,
            string 填制人 = default,
            DateTime? 填制日期 = default,
            string 审核人 = default,
            DateTime? 审核日期 = default,
            short? 入出系数 = default,
            long? 供应商id = default,
            string 发料人 = default,
            DateTime? 发料日期 = default,
            float? 金额 = default,
            float? 发票金额 = default)
        {
            Add("id", ID);
            Add("部门id", 部门id);
            Add("入出类别id", 入出类别id);
            Add("状态", 状态);
            Add("单号", 单号);
            Add("摘要", 摘要);
            Add("填制人", 填制人);
            Add("填制日期", 填制日期);
            Add("审核人", 审核人);
            Add("审核日期", 审核日期);
            Add("入出系数", 入出系数);
            Add("供应商id", 供应商id);
            Add("发料人", 发料人);
            Add("发料日期", 发料日期);
            Add("金额", 金额);
            Add("发票金额", 发票金额);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public long 部门id
        {
            get { return (long)this["部门id"]; }
            set { this["部门id"] = value; }
        }

        public Cell c部门id => _cells["部门id"];

        /// <summary>
        /// 
        /// </summary>
        public long 入出类别id
        {
            get { return (long)this["入出类别id"]; }
            set { this["入出类别id"] = value; }
        }

        public Cell c入出类别id => _cells["入出类别id"];

        /// <summary>
        /// 0-填写;1-待审核;2-已审核;3-被冲销;4-冲销
        /// </summary>
        public 单据状态 状态
        {
            get { return (单据状态)this["状态"]; }
            set { this["状态"] = value; }
        }

        public Cell c状态 => _cells["状态"];

        /// <summary>
        /// 相同单号可以不同的冲销状态，命名：前缀+连续序号
        /// </summary>
        public string 单号
        {
            get { return (string)this["单号"]; }
            set { this["单号"] = value; }
        }

        public Cell c单号 => _cells["单号"];

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
        /// 如果是申领单，表示申领人
        /// </summary>
        public string 填制人
        {
            get { return (string)this["填制人"]; }
            set { this["填制人"] = value; }
        }

        public Cell c填制人 => _cells["填制人"];

        /// <summary>
        /// 
        /// </summary>
        public DateTime? 填制日期
        {
            get { return (DateTime?)this["填制日期"]; }
            set { this["填制日期"] = value; }
        }

        public Cell c填制日期 => _cells["填制日期"];

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

        /// <summary>
        /// 1:物资入,-1:物资出;0-盘点记录单
        /// </summary>
        public short? 入出系数
        {
            get { return (short?)this["入出系数"]; }
            set { this["入出系数"] = value; }
        }

        public Cell c入出系数 => _cells["入出系数"];

        /// <summary>
        /// 外购入库时填写
        /// </summary>
        public long? 供应商id
        {
            get { return (long?)this["供应商id"]; }
            set { this["供应商id"] = value; }
        }

        public Cell c供应商id => _cells["供应商id"];

        /// <summary>
        /// 申请单时用效,主要反应该张单据什么人发的料
        /// </summary>
        public string 发料人
        {
            get { return (string)this["发料人"]; }
            set { this["发料人"] = value; }
        }

        public Cell c发料人 => _cells["发料人"];

        /// <summary>
        /// 申请单时用效
        /// </summary>
        public DateTime? 发料日期
        {
            get { return (DateTime?)this["发料日期"]; }
            set { this["发料日期"] = value; }
        }

        public Cell c发料日期 => _cells["发料日期"];

        /// <summary>
        /// 单据内所有详单的金额和
        /// </summary>
        public float? 金额
        {
            get { return (float?)this["金额"]; }
            set { this["金额"] = value; }
        }

        public Cell c金额 => _cells["金额"];

        /// <summary>
        /// 
        /// </summary>
        public float? 发票金额
        {
            get { return (float?)this["发票金额"]; }
            set { this["发票金额"] = value; }
        }

        public Cell c发票金额 => _cells["发票金额"];
    }
}