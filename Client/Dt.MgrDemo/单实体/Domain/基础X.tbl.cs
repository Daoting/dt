﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-09-07 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.MgrDemo.单实体
{
    [Tbl("DEMO_基础")]
    public partial class 基础X : EntityX<基础X>
    {
        #region 构造方法
        基础X() { }

        public 基础X(CellList p_cells) : base(p_cells) { }

        public 基础X(
            long ID,
            int 序列 = default,
            string 限长4 = default,
            string 不重复 = default,
            bool 禁止选中 = default,
            bool 禁止保存 = default,
            bool 禁止删除 = default,
            string 值变事件 = default,
            DateTime 创建时间 = default,
            DateTime 修改时间 = default)
        {
            Add("ID", ID);
            Add("序列", 序列);
            Add("限长4", 限长4);
            Add("不重复", 不重复);
            Add("禁止选中", 禁止选中);
            Add("禁止保存", 禁止保存);
            Add("禁止删除", 禁止删除);
            Add("值变事件", 值变事件);
            Add("创建时间", 创建时间);
            Add("修改时间", 修改时间);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 序列自动赋值
        /// </summary>
        public int 序列
        {
            get { return (int)this["序列"]; }
            set { this["序列"] = value; }
        }

        public Cell c序列 => _cells["序列"];

        /// <summary>
        /// 限制最大长度4
        /// </summary>
        public string 限长4
        {
            get { return (string)this["限长4"]; }
            set { this["限长4"] = value; }
        }

        public Cell c限长4 => _cells["限长4"];

        /// <summary>
        /// 列值无重复
        /// </summary>
        public string 不重复
        {
            get { return (string)this["不重复"]; }
            set { this["不重复"] = value; }
        }

        public Cell c不重复 => _cells["不重复"];

        /// <summary>
        /// 始终为false
        /// </summary>
        public bool 禁止选中
        {
            get { return (bool)this["禁止选中"]; }
            set { this["禁止选中"] = value; }
        }

        public Cell c禁止选中 => _cells["禁止选中"];

        /// <summary>
        /// true时保存前校验不通过
        /// </summary>
        public bool 禁止保存
        {
            get { return (bool)this["禁止保存"]; }
            set { this["禁止保存"] = value; }
        }

        public Cell c禁止保存 => _cells["禁止保存"];

        /// <summary>
        /// true时删除前校验不通过
        /// </summary>
        public bool 禁止删除
        {
            get { return (bool)this["禁止删除"]; }
            set { this["禁止删除"] = value; }
        }

        public Cell c禁止删除 => _cells["禁止删除"];

        /// <summary>
        /// 每次值变化时触发领域事件
        /// </summary>
        public string 值变事件
        {
            get { return (string)this["值变事件"]; }
            set { this["值变事件"] = value; }
        }

        public Cell c值变事件 => _cells["值变事件"];

        /// <summary>
        /// 初次创建时间
        /// </summary>
        public DateTime 创建时间
        {
            get { return (DateTime)this["创建时间"]; }
            set { this["创建时间"] = value; }
        }

        public Cell c创建时间 => _cells["创建时间"];

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime 修改时间
        {
            get { return (DateTime)this["修改时间"]; }
            set { this["修改时间"] = value; }
        }

        public Cell c修改时间 => _cells["修改时间"];
    }
}