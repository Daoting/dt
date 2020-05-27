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

namespace Dt.App.Pub
{
    public partial class Post
    {
        protected override void SetContent(string p_value)
        {
            Throw.If(string.IsNullOrEmpty(p_value) && IsPublish, "发布的文章内容不能为空");
            base.SetContent(p_value);
        }

        public bool IsValid()
        {
            if (string.IsNullOrEmpty(Title))
            {
                return false;
            }

            if (IsPublish && string.IsNullOrEmpty(Content))
            {
                AtKit.Warn("发布的文章内容不能为空");
                return false;
            }
            return true;
        }
    }

    #region PostEntity
    [Tbl("pub_post", "pub")]
    public partial class Post : PostEntity
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
    }

    public abstract class PostEntity : Entity
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get { return (string)_cells["Title"].Val; }
            set { SetTitle(value); }
        }

        protected virtual void SetTitle(string p_value)
        {
            _cells["Title"].Val = p_value;
        }

        /// <summary>
        /// 封面
        /// </summary>
        public string Cover
        {
            get { return (string)_cells["Cover"].Val; }
            set { SetCover(value); }
        }

        protected virtual void SetCover(string p_value)
        {
            _cells["Cover"].Val = p_value;
        }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content
        {
            get { return (string)_cells["Content"].Val; }
            set { SetContent(value); }
        }

        protected virtual void SetContent(string p_value)
        {
            _cells["Content"].Val = p_value;
        }

        /// <summary>
        /// 是否发布
        /// </summary>
        public bool IsPublish
        {
            get { return (bool)_cells["IsPublish"].Val; }
            set { SetIsPublish(value); }
        }

        protected virtual void SetIsPublish(bool p_value)
        {
            _cells["IsPublish"].Val = p_value;
        }

        /// <summary>
        /// 是否允许评论
        /// </summary>
        public bool AllowComment
        {
            get { return (bool)_cells["AllowComment"].Val; }
            set { SetAllowComment(value); }
        }

        protected virtual void SetAllowComment(bool p_value)
        {
            _cells["AllowComment"].Val = p_value;
        }

        /// <summary>
        /// 文章末尾是否添加同专辑链接
        /// </summary>
        public bool AddAlbumLink
        {
            get { return (bool)_cells["AddAlbumLink"].Val; }
            set { SetAddAlbumLink(value); }
        }

        protected virtual void SetAddAlbumLink(bool p_value)
        {
            _cells["AddAlbumLink"].Val = p_value;
        }

        /// <summary>
        /// 文章末尾是否添加同分类链接
        /// </summary>
        public bool AddCatLink
        {
            get { return (bool)_cells["AddCatLink"].Val; }
            set { SetAddCatLink(value); }
        }

        protected virtual void SetAddCatLink(bool p_value)
        {
            _cells["AddCatLink"].Val = p_value;
        }

        /// <summary>
        /// 文章地址
        /// </summary>
        public string Url
        {
            get { return (string)_cells["Url"].Val; }
            set { SetUrl(value); }
        }

        protected virtual void SetUrl(string p_value)
        {
            _cells["Url"].Val = p_value;
        }

        /// <summary>
        /// 显示顺序
        /// </summary>
        public int Dispidx
        {
            get { return (int)_cells["Dispidx"].Val; }
            set { SetDispidx(value); }
        }

        protected virtual void SetDispidx(int p_value)
        {
            _cells["Dispidx"].Val = p_value;
        }

        /// <summary>
        /// 创建人ID
        /// </summary>
        public long CreatorID
        {
            get { return (long)_cells["CreatorID"].Val; }
            set { SetCreatorID(value); }
        }

        protected virtual void SetCreatorID(long p_value)
        {
            _cells["CreatorID"].Val = p_value;
        }

        /// <summary>
        /// 创建人
        /// </summary>
        public string Creator
        {
            get { return (string)_cells["Creator"].Val; }
            set { SetCreator(value); }
        }

        protected virtual void SetCreator(string p_value)
        {
            _cells["Creator"].Val = p_value;
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime Ctime
        {
            get { return (DateTime)_cells["Ctime"].Val; }
            set { SetCtime(value); }
        }

        protected virtual void SetCtime(DateTime p_value)
        {
            _cells["Ctime"].Val = p_value;
        }

        /// <summary>
        /// 最后编辑人ID
        /// </summary>
        public long? LastEditorID
        {
            get { return (long?)_cells["LastEditorID"].Val; }
            set { SetLastEditorID(value); }
        }

        protected virtual void SetLastEditorID(long? p_value)
        {
            _cells["LastEditorID"].Val = p_value;
        }

        /// <summary>
        /// 最后编辑人
        /// </summary>
        public string LastEditor
        {
            get { return (string)_cells["LastEditor"].Val; }
            set { SetLastEditor(value); }
        }

        protected virtual void SetLastEditor(string p_value)
        {
            _cells["LastEditor"].Val = p_value;
        }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime? Mtime
        {
            get { return (DateTime?)_cells["Mtime"].Val; }
            set { SetMtime(value); }
        }

        protected virtual void SetMtime(DateTime? p_value)
        {
            _cells["Mtime"].Val = p_value;
        }

        /// <summary>
        /// 阅读次数
        /// </summary>
        public int ReadCount
        {
            get { return (int)_cells["ReadCount"].Val; }
            set { SetReadCount(value); }
        }

        protected virtual void SetReadCount(int p_value)
        {
            _cells["ReadCount"].Val = p_value;
        }

        /// <summary>
        /// 评论总数
        /// </summary>
        public int CommentCount
        {
            get { return (int)_cells["CommentCount"].Val; }
            set { SetCommentCount(value); }
        }

        protected virtual void SetCommentCount(int p_value)
        {
            _cells["CommentCount"].Val = p_value;
        }
    }
    #endregion
}
