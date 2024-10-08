﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-05-22 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Demo.Crud
{
    [Tbl("crud_大儿")]
    public partial class 大儿X : EntityX<大儿X>
    {
        #region 构造方法
        大儿X() { }

        public 大儿X(CellList p_cells) : base(p_cells) { }

        public 大儿X(
            long ID,
            long ParentID = default,
            string 大儿名 = default)
        {
            Add("id", ID);
            Add("parent_id", ParentID);
            Add("大儿名", 大儿名);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public long ParentID
        {
            get { return (long)this["parent_id"]; }
            set { this["parent_id"] = value; }
        }

        public Cell cParentID => _cells["parent_id"];

        /// <summary>
        /// 
        /// </summary>
        public string 大儿名
        {
            get { return (string)this["大儿名"]; }
            set { this["大儿名"] = value; }
        }

        public Cell c大儿名 => _cells["大儿名"];
    }
}