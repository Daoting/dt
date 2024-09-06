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
    [Tbl("物资计划明细")]
    public partial class 物资计划明细X : EntityX<物资计划明细X>
    {
        #region 构造方法
        物资计划明细X() { }

        public 物资计划明细X(CellList p_cells) : base(p_cells) { }

        public 物资计划明细X(
            long 计划id,
            long 物资id,
            float? 前期数量 = default,
            float? 上期数量 = default,
            float? 库存数量 = default,
            float? 计划数量 = default,
            float? 审批数量 = default,
            float? 单价 = default,
            float? 金额 = default,
            int? 显示顺序 = default)
        {
            Add("计划id", 计划id);
            Add("物资id", 物资id);
            Add("前期数量", 前期数量);
            Add("上期数量", 上期数量);
            Add("库存数量", 库存数量);
            Add("计划数量", 计划数量);
            Add("审批数量", 审批数量);
            Add("单价", 单价);
            Add("金额", 金额);
            Add("显示顺序", 显示顺序);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public long 计划id
        {
            get { return (long)this["计划id"]; }
            set { this["计划id"] = value; }
        }

        public Cell c计划id => _cells["计划id"];

        /// <summary>
        /// 
        /// </summary>
        public long 物资id
        {
            get { return (long)this["物资id"]; }
            set { this["物资id"] = value; }
        }

        public Cell c物资id => _cells["物资id"];

        new public long ID { get { return -1; } }

        /// <summary>
        /// 前年、上上月、前季度数量
        /// </summary>
        public float? 前期数量
        {
            get { return (float?)this["前期数量"]; }
            set { this["前期数量"] = value; }
        }

        public Cell c前期数量 => _cells["前期数量"];

        /// <summary>
        /// 去年、上个月、上季度数量
        /// </summary>
        public float? 上期数量
        {
            get { return (float?)this["上期数量"]; }
            set { this["上期数量"] = value; }
        }

        public Cell c上期数量 => _cells["上期数量"];

        /// <summary>
        /// 
        /// </summary>
        public float? 库存数量
        {
            get { return (float?)this["库存数量"]; }
            set { this["库存数量"] = value; }
        }

        public Cell c库存数量 => _cells["库存数量"];

        /// <summary>
        /// 
        /// </summary>
        public float? 计划数量
        {
            get { return (float?)this["计划数量"]; }
            set { this["计划数量"] = value; }
        }

        public Cell c计划数量 => _cells["计划数量"];

        /// <summary>
        /// 
        /// </summary>
        public float? 审批数量
        {
            get { return (float?)this["审批数量"]; }
            set { this["审批数量"] = value; }
        }

        public Cell c审批数量 => _cells["审批数量"];

        /// <summary>
        /// 
        /// </summary>
        public float? 单价
        {
            get { return (float?)this["单价"]; }
            set { this["单价"] = value; }
        }

        public Cell c单价 => _cells["单价"];

        /// <summary>
        /// 
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
        public int? 显示顺序
        {
            get { return (int?)this["显示顺序"]; }
            set { this["显示顺序"] = value; }
        }

        public Cell c显示顺序 => _cells["显示顺序"];
    }
}