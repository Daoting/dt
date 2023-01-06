#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-01-06 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dt.Mgr.Domain
{
    [Tbl("cm_pub_post_album")]
    public partial class PubPostAlbumObj : Entity
    {
        #region 构造方法
        PubPostAlbumObj() { }

        public PubPostAlbumObj(
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

        const string _svcName = "cm";
    }
}