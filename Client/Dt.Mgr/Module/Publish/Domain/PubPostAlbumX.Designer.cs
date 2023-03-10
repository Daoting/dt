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
    [Tbl("cm_pub_post_album")]
    public partial class PubPostAlbumX : EntityX<PubPostAlbumX>
    {
        #region 构造方法
        PubPostAlbumX() { }

        public PubPostAlbumX(CellList p_cells) : base(p_cells) { }

        public PubPostAlbumX(
            long PostID,
            long AlbumID)
        {
            AddCell("PostID", PostID);
            AddCell("AlbumID", AlbumID);
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
        /// 专辑标识
        /// </summary>
        public long AlbumID
        {
            get { return (long)this["AlbumID"]; }
            set { this["AlbumID"] = value; }
        }

        new public long ID { get { return -1; } }
    }
}