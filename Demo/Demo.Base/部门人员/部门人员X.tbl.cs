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
    [Tbl("部门人员")]
    public partial class 部门人员X : EntityX<部门人员X>
    {
        #region 构造方法
        部门人员X() { }

        public 部门人员X(CellList p_cells) : base(p_cells) { }

        public 部门人员X(
            long 部门id,
            long 人员id,
            bool? 缺省 = default)
        {
            Add("部门id", 部门id);
            Add("人员id", 人员id);
            Add("缺省", 缺省);
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
        public long 人员id
        {
            get { return (long)this["人员id"]; }
            set { this["人员id"] = value; }
        }

        public Cell c人员id => _cells["人员id"];

        new public long ID { get { return -1; } }

        /// <summary>
        /// 当一个人员属于多个部门时，当前是否为缺省
        /// </summary>
        public bool? 缺省
        {
            get { return (bool?)this["缺省"]; }
            set { this["缺省"] = value; }
        }

        public Cell c缺省 => _cells["缺省"];
    }
}