#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-11-20 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
#endregion

namespace Dt.Mgr.Publish
{
    #region 自动生成
    [Tbl("cm_pub_postalbum")]
    public partial class PubPostalbumObj : Entity
    {
        #region 构造方法
        PubPostalbumObj() { }

        public PubPostalbumObj(
            long PostID,
            long AlbumID)
        {
            AddCell("PostID", PostID);
            AddCell("AlbumID", AlbumID);
            IsAdded = true;
            AttachHook();
        }
        #endregion

        #region 属性
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
        #endregion
    }
    #endregion
}
