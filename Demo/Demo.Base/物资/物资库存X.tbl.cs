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
    [Tbl("物资库存")]
    public partial class 物资库存X : EntityX<物资库存X>
    {
        #region 构造方法
        物资库存X() { }

        public 物资库存X(CellList p_cells) : base(p_cells) { }

        public 物资库存X(
            long ID,
            long? 部门id = default,
            long? 物资id = default,
            string 批次 = default,
            float? 可用数量 = default,
            float? 可用金额 = default,
            float? 实际数量 = default,
            float? 实际金额 = default)
        {
            Add("id", ID);
            Add("部门id", 部门id);
            Add("物资id", 物资id);
            Add("批次", 批次);
            Add("可用数量", 可用数量);
            Add("可用金额", 可用金额);
            Add("实际数量", 实际数量);
            Add("实际金额", 实际金额);
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
        public long? 物资id
        {
            get { return (long?)this["物资id"]; }
            set { this["物资id"] = value; }
        }

        public Cell c物资id => _cells["物资id"];

        /// <summary>
        /// 相同物资ID不同批次的物资独立计算库存，部门ID+物资ID+批次构成唯一索引
        /// </summary>
        public string 批次
        {
            get { return (string)this["批次"]; }
            set { this["批次"] = value; }
        }

        public Cell c批次 => _cells["批次"];

        /// <summary>
        /// 当填写申领单还未审批时只影响可用数量，确保后填写申领单时数量有效
        /// </summary>
        public float? 可用数量
        {
            get { return (float?)this["可用数量"]; }
            set { this["可用数量"] = value; }
        }

        public Cell c可用数量 => _cells["可用数量"];

        /// <summary>
        /// 
        /// </summary>
        public float? 可用金额
        {
            get { return (float?)this["可用金额"]; }
            set { this["可用金额"] = value; }
        }

        public Cell c可用金额 => _cells["可用金额"];

        /// <summary>
        /// 
        /// </summary>
        public float? 实际数量
        {
            get { return (float?)this["实际数量"]; }
            set { this["实际数量"] = value; }
        }

        public Cell c实际数量 => _cells["实际数量"];

        /// <summary>
        /// 
        /// </summary>
        public float? 实际金额
        {
            get { return (float?)this["实际金额"]; }
            set { this["实际金额"] = value; }
        }

        public Cell c实际金额 => _cells["实际金额"];
    }
}