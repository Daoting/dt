﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-05-22 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Demo.Base
{
    /// <summary>
    /// 角色
    /// </summary>
    [Tbl("crud_角色")]
    public partial class 角色X : EntityX<角色X>
    {
        #region 构造方法
        角色X() { }

        public 角色X(CellList p_cells) : base(p_cells) { }

        public 角色X(
            long ID,
            string 角色名称 = default,
            string 角色描述 = default)
        {
            Add("id", ID);
            Add("角色名称", 角色名称);
            Add("角色描述", 角色描述);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 角色名称
        /// </summary>
        public string 角色名称
        {
            get { return (string)this["角色名称"]; }
            set { this["角色名称"] = value; }
        }

        public Cell c角色名称 => _cells["角色名称"];

        /// <summary>
        /// 角色描述
        /// </summary>
        public string 角色描述
        {
            get { return (string)this["角色描述"]; }
            set { this["角色描述"] = value; }
        }

        public Cell c角色描述 => _cells["角色描述"];
    }
}