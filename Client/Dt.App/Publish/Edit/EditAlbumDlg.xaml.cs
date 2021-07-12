#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using System.Threading.Tasks;
#endregion

namespace Dt.App.Publish
{
    public sealed partial class EditAlbumDlg : Dlg
    {
        bool _needRefresh;

        public EditAlbumDlg()
        {
            InitializeComponent();
        }

        public async Task<bool> Show(Album p_keyword)
        {
            if (p_keyword != null)
                _fv.Data = await AtPublish.GetByID<Album>(p_keyword.ID);
            else
                _fv.Data = await Create();
            await ShowAsync();
            return _needRefresh;
        }

        async Task<Album> Create()
        {
            return new Album(
                ID: await AtPublish.NewID(),
                Name: "新专辑",
                Creator: Kit.UserName,
                Ctime: Kit.Now);
        }

        async void OnSave(object sender, Mi e)
        {
            if (await AtPublish.Save(_fv.Data.To<Album>()))
            {
                _needRefresh = true;
                _fv.Data = await Create();
            }
        }

        async void OnAdd(object sender, Mi e)
        {
            _fv.Data = await Create();
        }

        protected override Task<bool> OnClosing(bool p_result)
        {
            if (_fv.Row.IsChanged)
                return Kit.Confirm("数据未保存，要放弃修改吗？");
            return Task.FromResult(true);
        }
    }
}
