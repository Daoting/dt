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
    [Tbl("cm_pub_post_keyword")]
    public partial class PubPostKeywordX : EntityX<PubPostKeywordX>
    {
        #region 构造方法
        PubPostKeywordX() { }

        public PubPostKeywordX(CellList p_cells) : base(p_cells) { }

        public PubPostKeywordX(
            long PostID,
            string Keyword)
        {
            AddCell("PostID", PostID);
            AddCell("Keyword", Keyword);
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
        /// 关键字
        /// </summary>
        public string Keyword
        {
            get { return (string)this["Keyword"]; }
            set { this["Keyword"] = value; }
        }

        new public long ID { get { return -1; } }
    }
}