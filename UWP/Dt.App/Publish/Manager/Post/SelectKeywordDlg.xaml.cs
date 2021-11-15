#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-06-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dt.App.Publish
{
    public partial class SelectKeywordDlg : Dlg
    {
        long _postID;

        public SelectKeywordDlg()
        {
            InitializeComponent();
        }

        public async Task<bool> Show(long p_postID)
        {
            _postID = p_postID;
            if (!Kit.IsPhoneUI)
            {
                IsPinned = true;
                Height = 400;
                Width = 400;
            }
            LoadData();
            return await ShowAsync();
        }

        async void LoadData()
        {
            _lv.Data = await AtPublish.Query("文章-未选关键字", new { postid = _postID });
        }

        async void OnSelect(object sender, Mi e)
        {
            var selected = _lv.SelectedRows.ToList();
            if (selected.Count == 0)
            {
                Kit.Warn("未选择关键字！");
                return;
            }

            List<PostkeywordObj> ls = new List<PostkeywordObj>();
            foreach (var row in selected)
            {
                ls.Add(new PostkeywordObj(PostID: _postID, Keyword: row.Str("id")));
            }
            if (await AtPublish.BatchSave(ls))
                Close(true);
        }

        async void OnAdd(object sender, Mi e)
        {
            Dlg dlg = new Dlg();
            if (!Kit.IsPhoneUI)
            {
                dlg.Width = 500;
                dlg.Height = 300;
            }
            var kf = new KeywordForm();
            kf.Update(null);
            dlg.LoadMv(kf);

            await dlg.ShowAsync();
            LoadData();
        }
    }
}