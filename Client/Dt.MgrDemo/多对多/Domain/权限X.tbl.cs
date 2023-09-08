#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-09-07 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.MgrDemo.多对多
{
    /// <summary>
    /// 权限
    /// </summary>
    [Tbl("DEMO_权限")]
    public partial class 权限X : EntityX<权限X>
    {
        #region 构造方法
        权限X() { }

        public 权限X(CellList p_cells) : base(p_cells) { }

        public 权限X(
            long ID,
            string 权限名称 = default)
        {
            Add("ID", ID);
            Add("权限名称", 权限名称);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public string 权限名称
        {
            get { return (string)this["权限名称"]; }
            set { this["权限名称"] = value; }
        }

        public Cell c权限名称 => _cells["权限名称"];
    }
}