#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-06-14 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Demo.Base
{
    [Tbl("人员")]
    public partial class 人员X : EntityX<人员X>
    {
        #region 构造方法
        人员X() { }

        public 人员X(CellList p_cells) : base(p_cells) { }

        public 人员X(
            long ID,
            string 姓名 = default,
            DateTime? 出生日期 = default,
            Gender? 性别 = default,
            DateTime? 工作日期 = default,
            string 办公室电话 = default,
            string 电子邮件 = default,
            DateTime? 建档时间 = default,
            DateTime? 撤档时间 = default,
            string 撤档原因 = default,
            long? UserID = default)
        {
            Add("id", ID);
            Add("姓名", 姓名);
            Add("出生日期", 出生日期);
            Add("性别", 性别);
            Add("工作日期", 工作日期);
            Add("办公室电话", 办公室电话);
            Add("电子邮件", 电子邮件);
            Add("建档时间", 建档时间);
            Add("撤档时间", 撤档时间);
            Add("撤档原因", 撤档原因);
            Add("user_id", UserID);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public string 姓名
        {
            get { return (string)this["姓名"]; }
            set { this["姓名"] = value; }
        }

        public Cell c姓名 => _cells["姓名"];

        /// <summary>
        /// 
        /// </summary>
        public DateTime? 出生日期
        {
            get { return (DateTime?)this["出生日期"]; }
            set { this["出生日期"] = value; }
        }

        public Cell c出生日期 => _cells["出生日期"];

        /// <summary>
        /// 
        /// </summary>
        public Gender? 性别
        {
            get { return (Gender?)this["性别"]; }
            set { this["性别"] = value; }
        }

        public Cell c性别 => _cells["性别"];

        /// <summary>
        /// 
        /// </summary>
        public DateTime? 工作日期
        {
            get { return (DateTime?)this["工作日期"]; }
            set { this["工作日期"] = value; }
        }

        public Cell c工作日期 => _cells["工作日期"];

        /// <summary>
        /// 
        /// </summary>
        public string 办公室电话
        {
            get { return (string)this["办公室电话"]; }
            set { this["办公室电话"] = value; }
        }

        public Cell c办公室电话 => _cells["办公室电话"];

        /// <summary>
        /// 
        /// </summary>
        public string 电子邮件
        {
            get { return (string)this["电子邮件"]; }
            set { this["电子邮件"] = value; }
        }

        public Cell c电子邮件 => _cells["电子邮件"];

        /// <summary>
        /// 
        /// </summary>
        public DateTime? 建档时间
        {
            get { return (DateTime?)this["建档时间"]; }
            set { this["建档时间"] = value; }
        }

        public Cell c建档时间 => _cells["建档时间"];

        /// <summary>
        /// 
        /// </summary>
        public DateTime? 撤档时间
        {
            get { return (DateTime?)this["撤档时间"]; }
            set { this["撤档时间"] = value; }
        }

        public Cell c撤档时间 => _cells["撤档时间"];

        /// <summary>
        /// 
        /// </summary>
        public string 撤档原因
        {
            get { return (string)this["撤档原因"]; }
            set { this["撤档原因"] = value; }
        }

        public Cell c撤档原因 => _cells["撤档原因"];

        /// <summary>
        /// 账号ID
        /// </summary>
        public long? UserID
        {
            get { return (long?)this["user_id"]; }
            set { this["user_id"] = value; }
        }

        public Cell cUserID => _cells["user_id"];
    }
}