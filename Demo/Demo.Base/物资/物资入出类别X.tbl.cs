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
    [Tbl("物资入出类别")]
    public partial class 物资入出类别X : EntityX<物资入出类别X>
    {
        #region 构造方法
        物资入出类别X() { }

        public 物资入出类别X(CellList p_cells) : base(p_cells) { }

        public 物资入出类别X(
            long ID,
            string 名称 = default,
            short 系数 = default,
            string 单号前缀 = default)
        {
            Add("id", ID);
            Add("名称", 名称);
            Add("系数", 系数);
            Add("单号前缀", 单号前缀);
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
        /// 1-入库；-1-出库
        /// </summary>
        public short 系数
        {
            get { return (short)this["系数"]; }
            set { this["系数"] = value; }
        }

        public Cell c系数 => _cells["系数"];

        /// <summary>
        /// 
        /// </summary>
        public string 单号前缀
        {
            get { return (string)this["单号前缀"]; }
            set { this["单号前缀"] = value; }
        }

        public Cell c单号前缀 => _cells["单号前缀"];
    }
}