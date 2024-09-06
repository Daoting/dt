#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-07-30 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Demo.Base
{
    [Tbl("物资详单")]
    public partial class 物资详单X : EntityX<物资详单X>
    {
        #region 构造方法
        物资详单X() { }

        public 物资详单X(CellList p_cells) : base(p_cells) { }

        public 物资详单X(
            long ID,
            long 单据id = default,
            long? 物资id = default,
            short? 序号 = default,
            string 批次 = default,
            float? 数量 = default,
            float? 单价 = default,
            float? 金额 = default,
            string 随货单号 = default,
            string 发票号 = default,
            DateTime? 发票日期 = default,
            float? 发票金额 = default,
            DateTime? 盘点时间 = default,
            float? 盘点金额 = default)
        {
            Add("id", ID);
            Add("单据id", 单据id);
            Add("物资id", 物资id);
            Add("序号", 序号);
            Add("批次", 批次);
            Add("数量", 数量);
            Add("单价", 单价);
            Add("金额", 金额);
            Add("随货单号", 随货单号);
            Add("发票号", 发票号);
            Add("发票日期", 发票日期);
            Add("发票金额", 发票金额);
            Add("盘点时间", 盘点时间);
            Add("盘点金额", 盘点金额);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public long 单据id
        {
            get { return (long)this["单据id"]; }
            set { this["单据id"] = value; }
        }

        public Cell c单据id => _cells["单据id"];

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
        /// 在一张单据内部从1连续编号，入出类别+冲销状态+单号+序号共同构成唯一索引
        /// </summary>
        public short? 序号
        {
            get { return (short?)this["序号"]; }
            set { this["序号"] = value; }
        }

        public Cell c序号 => _cells["序号"];

        /// <summary>
        /// 
        /// </summary>
        public string 批次
        {
            get { return (string)this["批次"]; }
            set { this["批次"] = value; }
        }

        public Cell c批次 => _cells["批次"];

        /// <summary>
        /// 按散装单位填写
        /// </summary>
        public float? 数量
        {
            get { return (float?)this["数量"]; }
            set { this["数量"] = value; }
        }

        public Cell c数量 => _cells["数量"];

        /// <summary>
        /// 售价
        /// </summary>
        public float? 单价
        {
            get { return (float?)this["单价"]; }
            set { this["单价"] = value; }
        }

        public Cell c单价 => _cells["单价"];

        /// <summary>
        /// 实际数量与单价的乘积。
        /// </summary>
        public float? 金额
        {
            get { return (float?)this["金额"]; }
            set { this["金额"] = value; }
        }

        public Cell c金额 => _cells["金额"];

        /// <summary>
        /// 外购入库时填写
        /// </summary>
        public string 随货单号
        {
            get { return (string)this["随货单号"]; }
            set { this["随货单号"] = value; }
        }

        public Cell c随货单号 => _cells["随货单号"];

        /// <summary>
        /// 外购入库时填写
        /// </summary>
        public string 发票号
        {
            get { return (string)this["发票号"]; }
            set { this["发票号"] = value; }
        }

        public Cell c发票号 => _cells["发票号"];

        /// <summary>
        /// 外购入库时填写
        /// </summary>
        public DateTime? 发票日期
        {
            get { return (DateTime?)this["发票日期"]; }
            set { this["发票日期"] = value; }
        }

        public Cell c发票日期 => _cells["发票日期"];

        /// <summary>
        /// 外购入库时填写
        /// </summary>
        public float? 发票金额
        {
            get { return (float?)this["发票金额"]; }
            set { this["发票金额"] = value; }
        }

        public Cell c发票金额 => _cells["发票金额"];

        /// <summary>
        /// 盘点有效
        /// </summary>
        public DateTime? 盘点时间
        {
            get { return (DateTime?)this["盘点时间"]; }
            set { this["盘点时间"] = value; }
        }

        public Cell c盘点时间 => _cells["盘点时间"];

        /// <summary>
        /// 盘点有效
        /// </summary>
        public float? 盘点金额
        {
            get { return (float?)this["盘点金额"]; }
            set { this["盘点金额"] = value; }
        }

        public Cell c盘点金额 => _cells["盘点金额"];
    }
}