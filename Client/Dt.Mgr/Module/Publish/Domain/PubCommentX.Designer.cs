#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-03-10 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Mgr.Module
{
    /// <summary>
    /// 评论信息
    /// </summary>
    [Tbl("cm_pub_comment")]
    public partial class PubCommentX : EntityX<PubCommentX>
    {
        #region 构造方法
        PubCommentX() { }

        public PubCommentX(CellList p_cells) : base(p_cells) { }

        public PubCommentX(
            long ID,
            long PostID = default,
            string Content = default,
            long UserID = default,
            string UserName = default,
            DateTime Ctime = default,
            bool IsSpam = default,
            long? ParentID = default,
            int Support = default,
            int Oppose = default)
        {
            AddCell("ID", ID);
            AddCell("PostID", PostID);
            AddCell("Content", Content);
            AddCell("UserID", UserID);
            AddCell("UserName", UserName);
            AddCell("Ctime", Ctime);
            AddCell("IsSpam", IsSpam);
            AddCell("ParentID", ParentID);
            AddCell("Support", Support);
            AddCell("Oppose", Oppose);
            IsAdded = true;
        }
        #endregion

        /// <summary>
        /// 文章标识
        /// </summary>
        public long PostID
        {
            get { return (long)this["PostID"]; }
            set { this["PostID"] = value; }
        }

        /// <summary>
        /// 评论内容
        /// </summary>
        public string Content
        {
            get { return (string)this["Content"]; }
            set { this["Content"] = value; }
        }

        /// <summary>
        /// 评论人标识
        /// </summary>
        public long UserID
        {
            get { return (long)this["UserID"]; }
            set { this["UserID"] = value; }
        }

        /// <summary>
        /// 评论人
        /// </summary>
        public string UserName
        {
            get { return (string)this["UserName"]; }
            set { this["UserName"] = value; }
        }

        /// <summary>
        /// 评论时间
        /// </summary>
        public DateTime Ctime
        {
            get { return (DateTime)this["Ctime"]; }
            set { this["Ctime"] = value; }
        }

        /// <summary>
        /// 是否为垃圾评论
        /// </summary>
        public bool IsSpam
        {
            get { return (bool)this["IsSpam"]; }
            set { this["IsSpam"] = value; }
        }

        /// <summary>
        /// 上级评论标识
        /// </summary>
        public long? ParentID
        {
            get { return (long?)this["ParentID"]; }
            set { this["ParentID"] = value; }
        }

        /// <summary>
        /// 对该评论的支持数
        /// </summary>
        public int Support
        {
            get { return (int)this["Support"]; }
            set { this["Support"] = value; }
        }

        /// <summary>
        /// 对该评论的反对数
        /// </summary>
        public int Oppose
        {
            get { return (int)this["Oppose"]; }
            set { this["Oppose"] = value; }
        }
    }
}