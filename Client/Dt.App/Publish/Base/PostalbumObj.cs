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

namespace Dt.App.Publish
{
    #region 自动生成
    [Tbl("pub_postalbum")]
    public partial class PostalbumObj : Entity
    {
        #region 构造方法
        PostalbumObj() { }

        public PostalbumObj(
            long PostID,
            long AlbumID)
        {
            AddCell<long>("PostID", PostID);
            AddCell<long>("AlbumID", AlbumID);
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
