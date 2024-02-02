#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-01-29 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.MgrDemo
{
    [Tbl("demo_扩展2")]
    public partial class 扩展2X : EntityX<扩展2X>
    {
        #region 构造方法
        扩展2X() { }

        public 扩展2X(CellList p_cells) : base(p_cells) { }

        public 扩展2X(
            long ID,
            string 扩展2名称 = default,
            bool 禁止删除 = default,
            string 值变事件 = default)
        {
            Add("id", ID);
            Add("扩展2名称", 扩展2名称);
            Add("禁止删除", 禁止删除);
            Add("值变事件", 值变事件);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public string 扩展2名称
        {
            get { return (string)this["扩展2名称"]; }
            set { this["扩展2名称"] = value; }
        }

        public Cell c扩展2名称 => _cells["扩展2名称"];

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
    }
}