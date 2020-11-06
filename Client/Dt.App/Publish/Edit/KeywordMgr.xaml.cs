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
using System.Threading.Tasks;
#endregion

namespace Dt.App.Publish
{
    public partial class KeywordMgr : Win
    {
        public KeywordMgr()
        {
            InitializeComponent();
            LoadAll();
        }

        async void LoadAll()
        {
            _lv.Data = await Repo.Query<Keyword>("发布-所有关键字");
        }

        async void OnSearch(object sender, string e)
        {
            if (e == "#全部")
            {
                LoadAll();
            }
            else if (!string.IsNullOrEmpty(e))
            {
                _lv.Data = await Repo.Query<Keyword>("发布-模糊查询关键字", new { id = $"%{e}%" });
            }
            SelectTab("列表");
        }

        async void OnAdd(object sender, Mi e)
        {
            if (await new EditKeywordDlg().Show(null))
                LoadAll();
        }

        async void OnEdit(object sender, Mi e)
        {
            var key = e.Data.To<Keyword>();
            if (!await IsUsed(key.ID))
            {
                if (await new EditKeywordDlg().Show(e.Data.To<Keyword>()))
                    LoadAll();
            }
        }

        async void OnDel(object sender, Mi e)
        {
            var key = e.Data.To<Keyword>();
            if (await IsUsed(key.ID))
                return;

            if (!await AtKit.Confirm($"确认要删除[{key.ID}]吗？"))
            {
                AtKit.Msg("已取消删除！");
                return;
            }

            if (await Repo.Delete(key))
                LoadAll();
        }

        async Task<bool> IsUsed(string p_keyword)
        {
            int cnt = await AtPublish.GetScalar<int>("发布-关键字引用数", new { keyword = p_keyword });
            if (cnt > 0)
            {
                AtKit.Warn("关键字已被引用，无法修改或删除！");
                return true;
            }
            return false;
        }
    }
}