#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-09-17 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.App;
using Dt.Base;
using Dt.Base.FormView;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
#endregion

namespace Dt.App.Publish
{
    public sealed partial class PostForm : Mv, IHtmlEditHost
    {
        public PostForm()
        {
            InitializeComponent();
            Menu["保存"].Bind(IsEnabledProperty, _fv, "IsDirty");
        }

        public async void Update(long p_id)
        {
            if (!await _fv.DiscardChanges())
                return;

            if (p_id > 0)
            {
                _fv.Data = await AtPublish.First<PostObj>("文章-编辑", new { id = p_id });
                UpdateRelated(p_id);
            }
            else
            {
                Create();
            }
        }

        public void Clear()
        {
            _fv.Data = null;
            ClearRelated();
        }

        async void Create()
        {
            _fv.Data = new PostObj(
                ID: await AtPublish.NewID(),
                Title: "新文章",
                TempType: 0,
                Dispidx: await AtPublish.NewSeq("sq_post"),
                CreatorID: Kit.UserID,
                Creator: Kit.UserName,
                Ctime: Kit.Now);

            ClearRelated();
        }

        void OnSave(object sender, Mi e)
        {
            _ = Save();
        }

        void OnAdd(object sender, Mi e)
        {
            Create();
        }

        async Task<bool> Save()
        {
            var d = _fv.Data.To<PostObj>();
            if (d == null || !d.IsValid())
                return false;

            bool isNew = d.IsAdded;
            d.Url = await AtPublish.SavePost(d);
            _fv.Row.AcceptChanges();
            _win.List.Update();
            if (isNew)
            {
                UpdateRelated(d.ID);
            }
            Kit.Msg("保存成功");
            return true;
        }

        async void OnDel(object sender, Mi e)
        {
            var d = _fv.Data.To<PostObj>();
            if (d == null)
                return;

            if (!await Kit.Confirm("确认要删除吗？"))
            {
                Kit.Msg("已取消删除！");
                return;
            }

            if (d.IsAdded)
            {
                Clear();
                return;
            }

            if (await AtPublish.Delete(d))
            {
                _win.List.Update();
                Clear();
            }
        }

        void OnEditContent(object sender, Mi e)
        {
            var d = _fv.Data.To<PostObj>();
            if (d != null)
                new HtmlEditDlg().ShowDlg(this);
        }

        void OnExplore(object sender, Mi e)
        {
            var d = _fv.Data.To<PostObj>();
            if (d != null)
            {
                if (string.IsNullOrEmpty(d.Url))
                    Kit.Warn("文章的标题和内容不可为空！");
                else
                    Kit.OpenWin(typeof(PublishView), d.Title, Icons.公告, d.ID);
            }
        }

        #region IHtmlEditHost
        string IHtmlEditHost.CurrentHtml => _fv.Data.To<PostObj>().Content;

        Task<bool> IHtmlEditHost.SaveHtml(string p_html)
        {
            ((PostObj)_fv.Data).Content = p_html;
            return Save();
        }
        #endregion

        void UpdateRelated(long p_id)
        {
            _win.KeywordList.Update(p_id);
            _win.AlbumList.Update(p_id);
        }

        void ClearRelated()
        {
            _win.KeywordList.Clear();
            _win.AlbumList.Clear();
        }

        protected override Task<bool> OnClosing()
        {
            return _fv.DiscardChanges();
        }

        PostWin _win => (PostWin)_tab.OwnWin;
    }
}
