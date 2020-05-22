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
#endregion

namespace Dt.Pub
{
    [Tbl("pub_post", "pub")]
    public partial class Post : Entity
    {
        public Post()
        { }

        public Post(
            long ID,
            string Title = default,
            string Cover = default,
            string Content = default,
            bool IsPublish = default,
            bool AllowComment = default,
            bool AddAlbumLink = default,
            bool AddCatLink = default,
            string Url = default,
            int Dispidx = default,
            long CreatorID = default,
            string Creator = default,
            DateTime Ctime = default,
            long? LastEditorID = default,
            string LastEditor = default,
            DateTime? Mtime = default,
            int ReadCount = default,
            int CommentCount = default)
        {
            AddCell<long>("ID", ID);
            AddCell<string>("Title", Title);
            AddCell<string>("Cover", Cover);
            AddCell<string>("Content", Content);
            AddCell<bool>("IsPublish", IsPublish);
            AddCell<bool>("AllowComment", AllowComment);
            AddCell<bool>("AddAlbumLink", AddAlbumLink);
            AddCell<bool>("AddCatLink", AddCatLink);
            AddCell<string>("Url", Url);
            AddCell<int>("Dispidx", Dispidx);
            AddCell<long>("CreatorID", CreatorID);
            AddCell<string>("Creator", Creator);
            AddCell<DateTime>("Ctime", Ctime);
            AddCell<long?>("LastEditorID", LastEditorID);
            AddCell<string>("LastEditor", LastEditor);
            AddCell<DateTime?>("Mtime", Mtime);
            AddCell<int>("ReadCount", ReadCount);
            AddCell<int>("CommentCount", CommentCount);
            IsAdded = true;
        }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get { return (string)_cells["Title"].Val; }
            private set { _cells["Title"].Val = value; }
        }

        /// <summary>
        /// 封面
        /// </summary>
        public string Cover
        {
            get { return (string)_cells["Cover"].Val; }
            private set { _cells["Cover"].Val = value; }
        }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content
        {
            get { return (string)_cells["Content"].Val; }
            private set { _cells["Content"].Val = value; }
        }

        /// <summary>
        /// 是否发布
        /// </summary>
        public bool IsPublish
        {
            get { return (bool)_cells["IsPublish"].Val; }
            private set { _cells["IsPublish"].Val = value; }
        }

        /// <summary>
        /// 是否允许评论
        /// </summary>
        public bool AllowComment
        {
            get { return (bool)_cells["AllowComment"].Val; }
            private set { _cells["AllowComment"].Val = value; }
        }

        /// <summary>
        /// 文章末尾是否添加同专辑链接
        /// </summary>
        public bool AddAlbumLink
        {
            get { return (bool)_cells["AddAlbumLink"].Val; }
            private set { _cells["AddAlbumLink"].Val = value; }
        }

        /// <summary>
        /// 文章末尾是否添加同分类链接
        /// </summary>
        public bool AddCatLink
        {
            get { return (bool)_cells["AddCatLink"].Val; }
            private set { _cells["AddCatLink"].Val = value; }
        }

        /// <summary>
        /// 文章地址
        /// </summary>
        public string Url
        {
            get { return (string)_cells["Url"].Val; }
            private set { _cells["Url"].Val = value; }
        }

        /// <summary>
        /// 显示顺序
        /// </summary>
        public int Dispidx
        {
            get { return (int)_cells["Dispidx"].Val; }
            private set { _cells["Dispidx"].Val = value; }
        }

        /// <summary>
        /// 创建人ID
        /// </summary>
        public long CreatorID
        {
            get { return (long)_cells["CreatorID"].Val; }
            private set { _cells["CreatorID"].Val = value; }
        }

        /// <summary>
        /// 创建人
        /// </summary>
        public string Creator
        {
            get { return (string)_cells["Creator"].Val; }
            private set { _cells["Creator"].Val = value; }
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime Ctime
        {
            get { return (DateTime)_cells["Ctime"].Val; }
            private set { _cells["Ctime"].Val = value; }
        }

        /// <summary>
        /// 最后编辑人ID
        /// </summary>
        public long? LastEditorID
        {
            get { return (long?)_cells["LastEditorID"].Val; }
            private set { _cells["LastEditorID"].Val = value; }
        }

        /// <summary>
        /// 最后编辑人
        /// </summary>
        public string LastEditor
        {
            get { return (string)_cells["LastEditor"].Val; }
            private set { _cells["LastEditor"].Val = value; }
        }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime? Mtime
        {
            get { return (DateTime?)_cells["Mtime"].Val; }
            private set { _cells["Mtime"].Val = value; }
        }

        /// <summary>
        /// 阅读次数
        /// </summary>
        public int ReadCount
        {
            get { return (int)_cells["ReadCount"].Val; }
            private set { _cells["ReadCount"].Val = value; }
        }

        /// <summary>
        /// 评论总数
        /// </summary>
        public int CommentCount
        {
            get { return (int)_cells["CommentCount"].Val; }
            private set { _cells["CommentCount"].Val = value; }
        }
    }
}
