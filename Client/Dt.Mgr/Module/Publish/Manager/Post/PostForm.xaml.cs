#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-09-17 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Mgr;
using Dt.Base;
using Dt.Base.FormView;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
#endregion

namespace Dt.Mgr.Module
{
    public sealed partial class PostForm : Tab, IHtmlEditHost
    {
        public PostForm()
        {
            InitializeComponent();
        }

        public async void Update(long p_id)
        {
            var d = Data;
            if (d != null && d.ID == p_id)
                return;

            if (!await _fv.DiscardChanges())
                return;

            if (p_id > 0)
            {
                Data = await AtCm.First<PubPostX>("文章-编辑", new { id = p_id });
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

        public PubPostX Data
        {
            get { return _fv.Data.To<PubPostX>(); }
            private set { _fv.Data = value; }
        }

        async void Create()
        {
            _fv.Data = await PubPostX.New();

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
            var d = Data;
            if (d == null)
                return false;

            bool isNew = d.IsAdded;
            if (await PublishDs.SavePost(d))
            {
                _win.List.Update();
                if (isNew)
                {
                    UpdateRelated(d.ID);
                }
                return true;
            }
            return false;
        }

        async void OnDel(object sender, Mi e)
        {
            var d = _fv.Data.To<PubPostX>();
            if (d == null)
                return;

            if (!await Kit.Confirm("确认要删除吗？\r\n删除后文章内容不可恢复，请谨慎删除！"))
            {
                Kit.Msg("已取消删除！");
                return;
            }

            if (d.IsAdded)
            {
                Clear();
                return;
            }

            if (await d.Delete())
            {
                _win.List.Update();
                Clear();
            }
        }

        void OnEditContent(object sender, Mi e)
        {
            var d = _fv.Data.To<PubPostX>();
            if (d != null)
                new HtmlEditDlg().ShowDlg(this);
        }

        void OnExplore(object sender, Mi e)
        {
            var d = _fv.Data.To<PubPostX>();
            if (d != null)
            {
                if (string.IsNullOrEmpty(d.Url))
                    Kit.Warn("文章的标题和内容不可为空！");
                else
                    Kit.OpenWin(typeof(PublishView), d.Title, Icons.公告, d.ID);
            }
        }

        #region IHtmlEditHost
        string IHtmlEditHost.CurrentHtml => _fv.Data.To<PubPostX>().Content;

        Task<bool> IHtmlEditHost.SaveHtml(string p_html)
        {
            ((PubPostX)_fv.Data).Content = p_html;
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

        PostWin _win => (PostWin)OwnWin;
    }
}
