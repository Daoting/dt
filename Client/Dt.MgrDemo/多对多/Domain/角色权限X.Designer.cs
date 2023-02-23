#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-02-23 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.MgrDemo.多对多
{
    /// <summary>
    /// 角色关联的权限
    /// </summary>
    [Tbl("demo_角色权限")]
    public partial class 角色权限X : EntityX<角色权限X>
    {
        #region 构造方法
        角色权限X() { }

        public 角色权限X(CellList p_cells) : base(p_cells) { }

        public 角色权限X(
            long RoleID,
            long PrvID)
        {
            AddCell("RoleID", RoleID);
            AddCell("PrvID", PrvID);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 角色标识
        /// </summary>
        public long RoleID
        {
            get { return (long)this["RoleID"]; }
            set { this["RoleID"] = value; }
        }

        /// <summary>
        /// 权限标识
        /// </summary>
        public long PrvID
        {
            get { return (long)this["PrvID"]; }
            set { this["PrvID"] = value; }
        }

        new public long ID { get { return -1; } }
    }
}