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

namespace Dt.App.Pub
{
    public partial class Post
    {
        void SetIsPublish(bool p_value)
        {
            Throw.If(p_value && string.IsNullOrEmpty(Content), "发布的文章内容不能为空");
        }

        void SetContent(string p_value)
        {
            Throw.If(string.IsNullOrEmpty(p_value) && IsPublish, "发布的文章内容不能为空");
        }

        public void OnSaving()
        {
            Throw.IfNullOrEmpty(Title, "标题不可为空");
            Throw.If(IsPublish && string.IsNullOrEmpty(Content), "发布的文章内容不能为空");
        }

        void OnDeleting()
        {
            Throw.If(!IsPublish, "已发布的文章不可删除");
        }
    }

    #region 自动生成
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
            AttachHook();
        }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get { return (string)this["Title"]; }
            set { this["Title"] = value; }
        }

        /// <summary>
        /// 封面
        /// </summary>
        public string Cover
        {
            get { return (string)this["Cover"]; }
            set { this["Cover"] = value; }
        }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content
        {
            get { return (string)this["Content"]; }
            set { this["Content"] = value; }
        }

        /// <summary>
        /// 是否发布
        /// </summary>
        public bool IsPublish
        {
            get { return (bool)this["IsPublish"]; }
            set { this["IsPublish"] = value; }
        }

        /// <summary>
        /// 是否允许评论
        /// </summary>
        public bool AllowComment
        {
            get { return (bool)this["AllowComment"]; }
            set { this["AllowComment"] = value; }
        }

        /// <summary>
        /// 文章末尾是否添加同专辑链接
        /// </summary>
        public bool AddAlbumLink
        {
            get { return (bool)this["AddAlbumLink"]; }
            set { this["AddAlbumLink"] = value; }
        }

        /// <summary>
        /// 文章末尾是否添加同分类链接
        /// </summary>
        public bool AddCatLink
        {
            get { return (bool)this["AddCatLink"]; }
            set { this["AddCatLink"] = value; }
        }

        /// <summary>
        /// 文章地址
        /// </summary>
        public string Url
        {
            get { return (string)this["Url"]; }
            set { this["Url"] = value; }
        }

        /// <summary>
        /// 显示顺序
        /// </summary>
        public int Dispidx
        {
            get { return (int)this["Dispidx"]; }
            set { this["Dispidx"] = value; }
        }

        /// <summary>
        /// 创建人ID
        /// </summary>
        public long CreatorID
        {
            get { return (long)this["CreatorID"]; }
            set { this["CreatorID"] = value; }
        }

        /// <summary>
        /// 创建人
        /// </summary>
        public string Creator
        {
            get { return (string)this["Creator"]; }
            set { this["Creator"] = value; }
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime Ctime
        {
            get { return (DateTime)this["Ctime"]; }
            set { this["Ctime"] = value; }
        }

        /// <summary>
        /// 最后编辑人ID
        /// </summary>
        public long? LastEditorID
        {
            get { return (long?)this["LastEditorID"]; }
            set { this["LastEditorID"] = value; }
        }

        /// <summary>
        /// 最后编辑人
        /// </summary>
        public string LastEditor
        {
            get { return (string)this["LastEditor"]; }
            set { this["LastEditor"] = value; }
        }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime? Mtime
        {
            get { return (DateTime?)this["Mtime"]; }
            set { this["Mtime"] = value; }
        }

        /// <summary>
        /// 阅读次数
        /// </summary>
        public int ReadCount
        {
            get { return (int)this["ReadCount"]; }
            set { this["ReadCount"] = value; }
        }

        /// <summary>
        /// 评论总数
        /// </summary>
        public int CommentCount
        {
            get { return (int)this["CommentCount"]; }
            set { this["CommentCount"] = value; }
        }
    }
    #endregion
}
