#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-06-14 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Demo.Base
{
    [Tbl("部门")]
    public partial class 部门X : EntityX<部门X>
    {
        #region 构造方法
        部门X() { }

        public 部门X(CellList p_cells) : base(p_cells) { }

        public 部门X(
            long ID,
            long? 上级id = default,
            string 编码 = default,
            string 名称 = default,
            string 说明 = default,
            DateTime? 建档时间 = default,
            DateTime? 撤档时间 = default)
        {
            Add("id", ID);
            Add("上级id", 上级id);
            Add("编码", 编码);
            Add("名称", 名称);
            Add("说明", 说明);
            Add("建档时间", 建档时间);
            Add("撤档时间", 撤档时间);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public long? 上级id
        {
            get { return (long?)this["上级id"]; }
            set { this["上级id"] = value; }
        }

        public Cell c上级id => _cells["上级id"];

        /// <summary>
        /// 
        /// </summary>
        public string 编码
        {
            get { return (string)this["编码"]; }
            set { this["编码"] = value; }
        }

        public Cell c编码 => _cells["编码"];

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
        /// 位置、环境、备注等
        /// </summary>
        public string 说明
        {
            get { return (string)this["说明"]; }
            set { this["说明"] = value; }
        }

        public Cell c说明 => _cells["说明"];

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