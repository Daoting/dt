﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-07-20 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.MgrDemo.单实体
{
    [Tbl("demo_扩展1")]
    public partial class 扩展1X : EntityX<扩展1X>
    {
        #region 构造方法
        扩展1X() { }

        public 扩展1X(CellList p_cells) : base(p_cells) { }

        public 扩展1X(
            long ID,
            string 扩展1名称 = default,
            bool 禁止选中 = default,
            bool 禁止保存 = default)
        {
            Add("id", ID);
            Add("扩展1名称", 扩展1名称);
            Add("禁止选中", 禁止选中);
            Add("禁止保存", 禁止保存);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public string 扩展1名称
        {
            get { return (string)this["扩展1名称"]; }
            set { this["扩展1名称"] = value; }
        }

        /// <summary>
        /// 始终为false
        /// </summary>
        public bool 禁止选中
        {
            get { return (bool)this["禁止选中"]; }
            set { this["禁止选中"] = value; }
        }

        /// <summary>
        /// true时保存前校验不通过
        /// </summary>
        public bool 禁止保存
        {
            get { return (bool)this["禁止保存"]; }
            set { this["禁止保存"] = value; }
        }
    }
}