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
    [Tbl("物资目录")]
    public partial class 物资目录X : EntityX<物资目录X>
    {
        #region 构造方法
        物资目录X() { }

        public 物资目录X(CellList p_cells) : base(p_cells) { }

        public 物资目录X(
            long ID,
            long? 分类id = default,
            string 名称 = default,
            string 规格 = default,
            string 产地 = default,
            float? 成本价 = default,
            物资核算方式? 核算方式 = default,
            int? 摊销月数 = default,
            DateTime? 建档时间 = default,
            DateTime? 撤档时间 = default)
        {
            Add("id", ID);
            Add("分类id", 分类id);
            Add("名称", 名称);
            Add("规格", 规格);
            Add("产地", 产地);
            Add("成本价", 成本价);
            Add("核算方式", 核算方式);
            Add("摊销月数", 摊销月数);
            Add("建档时间", 建档时间);
            Add("撤档时间", 撤档时间);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public long? 分类id
        {
            get { return (long?)this["分类id"]; }
            set { this["分类id"] = value; }
        }

        public Cell c分类id => _cells["分类id"];

        /// <summary>
        /// 
        /// </summary>
        public string 名称
        {
            get { return (string)this["名称"]; }
            set { this["名称"] = value; }
        }

        public Cell c名称 => _cells["名称"];

        /// <summary>
        /// 计量单位，如 盒、10个/包、20个/箱、支
        /// </summary>
        public string 规格
        {
            get { return (string)this["规格"]; }
            set { this["规格"] = value; }
        }

        public Cell c规格 => _cells["规格"];

        /// <summary>
        /// 名称,规格,产地构成唯一索引
        /// </summary>
        public string 产地
        {
            get { return (string)this["产地"]; }
            set { this["产地"] = value; }
        }

        public Cell c产地 => _cells["产地"];

        /// <summary>
        /// 预估价格，库存计算金额用
        /// </summary>
        public float? 成本价
        {
            get { return (float?)this["成本价"]; }
            set { this["成本价"] = value; }
        }

        public Cell c成本价 => _cells["成本价"];

        /// <summary>
        /// 一次性、分期摊销(折旧)
        /// </summary>
        public 物资核算方式? 核算方式
        {
            get { return (物资核算方式?)this["核算方式"]; }
            set { this["核算方式"] = value; }
        }

        public Cell c核算方式 => _cells["核算方式"];

        /// <summary>
        /// 当核算方式为分期摊销时的总月数
        /// </summary>
        public int? 摊销月数
        {
            get { return (int?)this["摊销月数"]; }
            set { this["摊销月数"] = value; }
        }

        public Cell c摊销月数 => _cells["摊销月数"];

        /// <summary>
        /// 
        /// </summary>
        public DateTime? 建档时间
        {
            get { return (DateTime?)this["建档时间"]; }
            set { this["建档时间"] = value; }
        }

        public Cell c建档时间 => _cells["建档时间"];

        /// <summary>
        /// 
        /// </summary>
        public DateTime? 撤档时间
        {
            get { return (DateTime?)this["撤档时间"]; }
            set { this["撤档时间"] = value; }
        }

        public Cell c撤档时间 => _cells["撤档时间"];
    }
}