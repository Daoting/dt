#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using Dt.Core.Model;
using Dt.Fz.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
#endregion

namespace Dt.Fz.Mgr
{
    [View("文章管理")]
    public partial class PostMgr : Win
    {
        readonly Repo<Post> _repoPost = new Repo<Post>();

        public PostMgr()
        {
            InitializeComponent();
            LoadAll();
            _fv.DataChanged += OnPostChanged;
        }

        #region 文章列表
        async void LoadAll()
        {
            _lvPost.Data = await _repoPost.Query("文章-管理列表");
        }

        void OnPostClick(object sender, ItemClickArgs e)
        {
            if (e.IsChanged)
            {
                LoadPost(e.Row.ID);
            }
            NaviTo("文章,所属专辑,所属分类");
        }

        async void OnSearch(object sender, string e)
        {
            _lvPost.Data = await _repoPost.Query("文章-模糊查询", new { input = $"%{e}%" });
        }
        #endregion

        #region 文章
        internal Post CurrentPost
        {
            get { return (Post)_fv.Data; }
        }

        async void LoadPost(long p_id)
        {
            _fv.Data = await _repoPost.Get("文章-编辑", new { id = p_id });
        }

        async void OnAddPost(object sender, Mi e)
        {
            var ids = await _repoPost.NewIDAndSeq("sq_post");
            var post = new Post(
                ID: ids[0],
                Title: "新文章",
                Dispidx: (int)ids[1],
                CreatorID: AtUser.ID,
                Creator: AtUser.Name,
                Ctime: AtSys.Now);
            _fv.Data = post;
        }

        internal async Task<bool> SavePost()
        {
            Post post = (Post)_fv.Data;
            if (post == null)
                return false;

            if (_fv.ExistNull("Title"))
                return false;

            if (post.IsPublish && string.IsNullOrEmpty(post.Content))
            {
                AtKit.Warn("发布的文章内容不能为空");
                return false;
            }

            post.Url = await AtPublish.SavePost(_fv.Row);
            _fv.Row.AcceptChanges();
            LoadAll();
            AtKit.Msg("保存成功");
            return true;
        }

        void OnSavePost(object sender, Mi e)
        {
            _ = SavePost();
        }

        void OnEditContent(object sender, Mi e)
        {
            Post post = (Post)_fv.Data;
            if (post != null)
                new EditPostDlg().ShowDlg(this);
        }

        async void OnDelPost(object sender, Mi e)
        {
            Post post = (Post)_fv.Data;
            if (!await AtKit.Confirm($"确认要删除[{post.Title}]吗？"))
            {
                AtKit.Msg("已取消删除！");
                return;
            }

            if (await _repoPost.Delete(post))
            {
                AtKit.Msg("删除成功！");
                LoadAll();
                _fv.Data = null;
            }
            else
            {
                AtKit.Warn("删除失败！");
            }
        }

        void OnExplore(object sender, Mi e)
        {
            Post post = (Post)_fv.Data;
            if (post != null)
            {
                if (string.IsNullOrEmpty(post.Url))
                    AtKit.Warn("文章的标题和内容不可为空！");
                else
                    new ViewPostDlg(this).Show();
            }
        }

        void OnPhotoChanged(object sender, object e)
        {
            _ = SavePost();
        }
        #endregion

        void OnPostChanged(object sender, object e)
        {

        }

    }
}