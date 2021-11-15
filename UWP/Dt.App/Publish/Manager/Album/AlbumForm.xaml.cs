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
    public sealed partial class AlbumForm : Mv
    {
        public AlbumForm()
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
                _fv.Data = await AtPublish.GetByID<AlbumObj>(p_id);
            }
            else
            {
                Create();
            }
        }

        public void Clear()
        {
            _fv.Data = null;
        }

        async void Create()
        {
            _fv.Data = new AlbumObj(
                ID: await AtPublish.NewID(),
                Name: "新专辑",
                Creator: Kit.UserName,
                Ctime: Kit.Now);
        }

        void OnSave(object sender, Mi e)
        {
            Save();
        }

        void OnAdd(object sender, Mi e)
        {
            Create();
        }

        async void Save()
        {
            var d = _fv.Data.To<AlbumObj>();
            if (d == null)
                return;

            if (!d.IsAdded
                && d.IsChanged
                && await IsUsed(d.GetOriginalVal<long>("id")))
            {
                d.RejectChanges();
                return;
            }

            if (await AtCm.Save(d))
            {
                _win?.List.Update();
            }
        }

        async void OnDel(object sender, Mi e)
        {
            var d = _fv.Data.To<AlbumObj>();
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

            d.RejectChanges();
            if (await IsUsed(d.ID))
                return;

            if (await AtPublish.Delete(d))
            {
                _win?.List.Update();
                Clear();
            }
        }

        async Task<bool> IsUsed(long p_id)
        {
            int cnt = await AtPublish.GetScalar<int>("发布-专辑引用数", new { AlbumID = p_id });
            if (cnt > 0)
            {
                Kit.Warn("专辑已被引用，无法修改或删除！");
                return true;
            }
            return false;
        }

        protected override Task<bool> OnClosing()
        {
            return _fv.DiscardChanges();
        }

        AlbumWin _win => (AlbumWin)_tab.OwnWin;
    }
}
