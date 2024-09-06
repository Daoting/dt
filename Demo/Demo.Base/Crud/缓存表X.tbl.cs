#region 文件描述
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
    [Tbl("crud_缓存表")]
    public partial class 缓存表X : EntityX<缓存表X>
    {
        #region 构造方法
        缓存表X() { }

        public 缓存表X(CellList p_cells) : base(p_cells) { }

        public 缓存表X(
            long ID,
            string 手机号 = default,
            string 姓名 = default)
        {
            Add("id", ID);
            Add("手机号", 手机号);
            Add("姓名", 姓名);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public string 手机号
        {
            get { return (string)this["手机号"]; }
            set { this["手机号"] = value; }
        }

        public Cell c手机号 => _cells["手机号"];

        /// <summary>
        /// 
        /// </summary>
        public string 姓名
        {
            get { return (string)this["姓名"]; }
            set { this["姓名"] = value; }
        }

        public Cell c姓名 => _cells["姓名"];
    }
}