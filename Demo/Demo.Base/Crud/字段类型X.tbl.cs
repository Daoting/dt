#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-06-20 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Demo.Base
{
    [Tbl("crud_字段类型")]
    public partial class 字段类型X : EntityX<字段类型X>
    {
        #region 构造方法
        字段类型X() { }

        public 字段类型X(CellList p_cells) : base(p_cells) { }

        public 字段类型X(
            long ID,
            string 字符串 = default,
            int 整型 = default,
            int? 可空整型 = default,
            long? 长整型 = default,
            bool 布尔 = default,
            bool? 可空布尔 = default,
            DateTime 日期时间 = default,
            DateTime? 可空时间 = default,
            Gender 枚举 = default,
            Gender? 可空枚举 = default,
            float 单精度 = default,
            float? 可空单精度 = default)
        {
            Add("id", ID);
            Add("字符串", 字符串);
            Add("整型", 整型);
            Add("可空整型", 可空整型);
            Add("长整型", 长整型);
            Add("布尔", 布尔);
            Add("可空布尔", 可空布尔);
            Add("日期时间", 日期时间);
            Add("可空时间", 可空时间);
            Add("枚举", 枚举);
            Add("可空枚举", 可空枚举);
            Add("单精度", 单精度);
            Add("可空单精度", 可空单精度);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public string 字符串
        {
            get { return (string)this["字符串"]; }
            set { this["字符串"] = value; }
        }

        public Cell c字符串 => _cells["字符串"];

        /// <summary>
        /// 
        /// </summary>
        public int 整型
        {
            get { return (int)this["整型"]; }
            set { this["整型"] = value; }
        }

        public Cell c整型 => _cells["整型"];

        /// <summary>
        /// 
        /// </summary>
        public int? 可空整型
        {
            get { return (int?)this["可空整型"]; }
            set { this["可空整型"] = value; }
        }

        public Cell c可空整型 => _cells["可空整型"];

        /// <summary>
        /// 
        /// </summary>
        public long? 长整型
        {
            get { return (long?)this["长整型"]; }
            set { this["长整型"] = value; }
        }

        public Cell c长整型 => _cells["长整型"];

        /// <summary>
        /// 
        /// </summary>
        public bool 布尔
        {
            get { return (bool)this["布尔"]; }
            set { this["布尔"] = value; }
        }

        public Cell c布尔 => _cells["布尔"];

        /// <summary>
        /// 
        /// </summary>
        public bool? 可空布尔
        {
            get { return (bool?)this["可空布尔"]; }
            set { this["可空布尔"] = value; }
        }

        public Cell c可空布尔 => _cells["可空布尔"];

        /// <summary>
        /// 
        /// </summary>
        public DateTime 日期时间
        {
            get { return (DateTime)this["日期时间"]; }
            set { this["日期时间"] = value; }
        }

        public Cell c日期时间 => _cells["日期时间"];

        /// <summary>
        /// 
        /// </summary>
        public DateTime? 可空时间
        {
            get { return (DateTime?)this["可空时间"]; }
            set { this["可空时间"] = value; }
        }

        public Cell c可空时间 => _cells["可空时间"];

        /// <summary>
        /// 性别
        /// </summary>
        public Gender 枚举
        {
            get { return (Gender)this["枚举"]; }
            set { this["枚举"] = value; }
        }

        public Cell c枚举 => _cells["枚举"];

        /// <summary>
        /// 性别
        /// </summary>
        public Gender? 可空枚举
        {
            get { return (Gender?)this["可空枚举"]; }
            set { this["可空枚举"] = value; }
        }

        public Cell c可空枚举 => _cells["可空枚举"];

        /// <summary>
        /// 
        /// </summary>
        public float 单精度
        {
            get { return (float)this["单精度"]; }
            set { this["单精度"] = value; }
        }

        public Cell c单精度 => _cells["单精度"];

        /// <summary>
        /// 
        /// </summary>
        public float? 可空单精度
        {
            get { return (float?)this["可空单精度"]; }
            set { this["可空单精度"] = value; }
        }

        public Cell c可空单精度 => _cells["可空单精度"];
    }
}