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
    [Tbl("供应商")]
    public partial class 供应商X : EntityX<供应商X>
    {
        #region 构造方法
        供应商X() { }

        public 供应商X(CellList p_cells) : base(p_cells) { }

        public 供应商X(
            long ID,
            string 名称 = default,
            string 执照号 = default,
            DateTime? 执照效期 = default,
            string 税务登记号 = default,
            string 地址 = default,
            string 电话 = default,
            string 开户银行 = default,
            string 帐号 = default,
            string 联系人 = default,
            DateTime? 建档时间 = default,
            DateTime? 撤档时间 = default,
            string 备注 = default)
        {
            Add("id", ID);
            Add("名称", 名称);
            Add("执照号", 执照号);
            Add("执照效期", 执照效期);
            Add("税务登记号", 税务登记号);
            Add("地址", 地址);
            Add("电话", 电话);
            Add("开户银行", 开户银行);
            Add("帐号", 帐号);
            Add("联系人", 联系人);
            Add("建档时间", 建档时间);
            Add("撤档时间", 撤档时间);
            Add("备注", 备注);
            IsAdded = true;
        }
        #endregion

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
        /// 
        /// </summary>
        public string 执照号
        {
            get { return (string)this["执照号"]; }
            set { this["执照号"] = value; }
        }

        public Cell c执照号 => _cells["执照号"];

        /// <summary>
        /// 
        /// </summary>
        public DateTime? 执照效期
        {
            get { return (DateTime?)this["执照效期"]; }
            set { this["执照效期"] = value; }
        }

        public Cell c执照效期 => _cells["执照效期"];

        /// <summary>
        /// 
        /// </summary>
        public string 税务登记号
        {
            get { return (string)this["税务登记号"]; }
            set { this["税务登记号"] = value; }
        }

        public Cell c税务登记号 => _cells["税务登记号"];

        /// <summary>
        /// 
        /// </summary>
        public string 地址
        {
            get { return (string)this["地址"]; }
            set { this["地址"] = value; }
        }

        public Cell c地址 => _cells["地址"];

        /// <summary>
        /// 
        /// </summary>
        public string 电话
        {
            get { return (string)this["电话"]; }
            set { this["电话"] = value; }
        }

        public Cell c电话 => _cells["电话"];

        /// <summary>
        /// 
        /// </summary>
        public string 开户银行
        {
            get { return (string)this["开户银行"]; }
            set { this["开户银行"] = value; }
        }

        public Cell c开户银行 => _cells["开户银行"];

        /// <summary>
        /// 
        /// </summary>
        public string 帐号
        {
            get { return (string)this["帐号"]; }
            set { this["帐号"] = value; }
        }

        public Cell c帐号 => _cells["帐号"];

        /// <summary>
        /// 
        /// </summary>
        public string 联系人
        {
            get { return (string)this["联系人"]; }
            set { this["联系人"] = value; }
        }

        public Cell c联系人 => _cells["联系人"];

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

        /// <summary>
        /// 
        /// </summary>
        public string 备注
        {
            get { return (string)this["备注"]; }
            set { this["备注"] = value; }
        }

        public Cell c备注 => _cells["备注"];
    }
}