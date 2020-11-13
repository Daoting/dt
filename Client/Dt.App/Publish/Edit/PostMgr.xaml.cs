#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Base.FormView;
using Dt.Core;
using System.Threading.Tasks;
using Windows.UI.Xaml;
#endregion

namespace Dt.App.Publish
{
    public partial class PostMgr : Win, IHtmlEditHost
    {
        public PostMgr()
        {
            InitializeComponent();
            LoadAll();
        }

        #region 文章列表
        async void LoadAll()
        {
            _lvPost.Data = await AtPublish.Query<Post>("文章-管理列表");
        }

        void OnPostClick(object sender, ItemClickArgs e)
        {
            if (e.IsChanged)
            {
                LoadPost(e.Row.ID);
            }
            SelectTab("文章编辑");
        }

        async void OnSearch(object sender, string e)
        {
            _lvPost.Data = await AtPublish.Query<Post>("文章-模糊查询", new { input = $"%{e}%" });
        }
        #endregion

        #region 文章
        internal Post CurrentPost
        {
            get { return (Post)_fv.Data; }
        }

        async void LoadPost(long p_id)
        {
            _fv.Data = await AtPublish.Get<Post>("文章-编辑", new { id = p_id });
            LoadKeyword(p_id);
            LoadAlbum(p_id);
        }

        async void LoadKeyword(long p_id)
        {
            _lvKeyword.Data = await AtPublish.Query("文章-已选关键字", new { postid = p_id });
        }

        async void LoadAlbum(long p_id)
        {
            _lvAlbum.Data = await AtPublish.Query("文章-已选专辑", new { postid = p_id });
        }

        async void OnAddPost(object sender, Mi e)
        {
            var post = new Post(
                ID: await AtPublish.NewID(),
                Title: "新文章",
                TempType: 0,
                Dispidx: await AtPublish.NewSeq("sq_post"),
                CreatorID: AtUser.ID,
                Creator: AtUser.Name,
                Ctime: AtSys.Now);
            _fv.Data = post;
            _lvKeyword.Data = null;
            _lvAlbum.Data = null;
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
                new HtmlEditDlg().ShowDlg(this);
        }

        async void OnDelPost(object sender, Mi e)
        {
            Post post = (Post)_fv.Data;
            if (!await AtKit.Confirm($"确认要删除[{post.Title}]吗？"))
            {
                AtKit.Msg("已取消删除！");
                return;
            }

            if (await AtPublish.Delete(post))
            {
                LoadAll();
                _fv.Data = null;
                _lvKeyword.Data = null;
                _lvAlbum.Data = null;
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
                    AtApp.OpenWin(typeof(PublishView), post.Title, Icons.公告, post.ID);
            }
        }

        async void OnAddKeyword(object sender, Mi e)
        {
            if (await IsValidate())
            {
                if (await new SelectKeywordDlg().Show(_fv.Row.ID))
                    LoadKeyword(_fv.Row.ID);
            }
        }

        async void OnDelPostKeyword(object sender, Mi e)
        {
            Postkeyword pk = new Postkeyword(PostID: _fv.Row.ID, Keyword: e.Row.Str(0));
            if (await AtPublish.Delete(pk))
                LoadKeyword(_fv.Row.ID);
        }

        async void OnAddAlbum(object sender, Mi e)
        {
            if (await IsValidate())
            {
                if (await new SelectAlbumDlg().Show(_fv.Row.ID))
                    LoadAlbum(_fv.Row.ID);
            }
        }

        async void OnDelPostAlbum(object sender, Mi e)
        {
            var pa = new Postalbum(PostID: _fv.Row.ID, AlbumID: e.Row.ID);
            if (await AtPublish.Delete(pa))
                LoadAlbum(_fv.Row.ID);
        }

        Task<bool> IsValidate()
        {
            if (_fv.Data == null)
                return Task.FromResult(false);

            if (_fv.Row.IsAdded || _fv.Row.IsChanged)
            {
                return SavePost();
            }
            return Task.FromResult(true);
        }
        #endregion


        #region IHtmlEditHost
        string IHtmlEditHost.CurrentHtml => CurrentPost.Content;

        Task<bool> IHtmlEditHost.SaveHtml(string p_html)
        {
            return SavePost();
        }
        #endregion
    }
}