﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-11-14 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Mgr.Rbac
{
    /// <summary>
    /// 分组，与用户和角色多对多
    /// </summary>
    [Tbl("cm_group")]
    public partial class GroupX : EntityX<GroupX>
    {
        #region 构造方法
        GroupX() { }

        public GroupX(CellList p_cells) : base(p_cells) { }

        public GroupX(
            long ID,
            string Name = default,
            string Note = default)
        {
            Add("id", ID);
            Add("name", Name);
            Add("note", Note);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 组名
        /// </summary>
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        public Cell cName => _cells["name"];

        /// <summary>
        /// 组描述
        /// </summary>
        public string Note
        {
            get { return (string)this["note"]; }
            set { this["note"] = value; }
        }

        public Cell cNote => _cells["note"];
    }
}