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
    public partial class AlbumMgr : Win
    {
        public AlbumMgr()
        {
            InitializeComponent();
            LoadAll();
        }

        async void LoadAll()
        {
            _lv.Data = await AtPublish.Query<Album>("发布-所有专辑");
        }

        async void OnSearch(object sender, string e)
        {
            if (e == "#全部")
            {
                LoadAll();
            }
            else if (!string.IsNullOrEmpty(e))
            {
                _lv.Data = await AtPublish.Query<Album>("发布-模糊查询专辑", new { name = $"%{e}%" });
            }
            SelectTab("列表");
        }

        async void OnAdd(object sender, Mi e)
        {
            if (await new EditAlbumDlg().Show(null))
                LoadAll();
        }

        async void OnEdit(object sender, Mi e)
        {
            var album = e.Data.To<Album>();
            if (!await IsUsed(album.ID))
            {
                if (await new EditAlbumDlg().Show(album))
                    LoadAll();
            }
        }

        async void OnDel(object sender, Mi e)
        {
            var album = e.Data.To<Album>();
            if (await IsUsed(album.ID))
                return;

            if (!await AtKit.Confirm($"确认要删除[{album.Name}]吗？"))
            {
                AtKit.Msg("已取消删除！");
                return;
            }

            if (await AtPublish.Delete(album))
                LoadAll();
        }

        async Task<bool> IsUsed(long p_id)
        {
            int cnt = await AtPublish.GetScalar<int>("发布-专辑引用数", new { AlbumID = p_id });
            if (cnt > 0)
            {
                AtKit.Warn("专辑已被引用，无法修改或删除！");
                return true;
            }
            return false;
        }
    }
}