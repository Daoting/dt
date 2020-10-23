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

namespace Dt.App.Model
{
    public sealed partial class EditPrvDlg : Dlg
    {
        readonly Repo<Prv> _repo = new Repo<Prv>();
        bool _needRefresh;

        public EditPrvDlg()
        {
            InitializeComponent();
        }

        public async Task<bool> Show(string p_id)
        {
            if (!string.IsNullOrEmpty(p_id))
                _fv.Data = await _repo.GetByID(p_id);
            else
                CreatePrv();
            await ShowAsync();
            return _needRefresh;
        }

        void CreatePrv()
        {
            _fv.Data = new Prv("");
        }

        async void OnSave(object sender, Mi e)
        {
            if (_fv.ExistNull("id"))
                return;

            var prv = _fv.Data.To<Prv>();
            if ((prv.IsAdded || prv.Cells["id"].IsChanged)
                && await AtCm.GetScalar<int>("权限-名称重复", new { id = prv.ID }) > 0)
            {
                _fv["id"].Warn("权限名称重复！");
                return;
            }

            if (await _repo.Save(prv))
            {
                _needRefresh = true;
                CreatePrv();
                _fv.GotoFirstCell();
            }
        }

        void OnAdd(object sender, Mi e)
        {
            CreatePrv();
        }

        protected override Task<bool> OnClosing()
        {
            if (_fv.Row.IsChanged)
                return AtKit.Confirm("数据未保存，要放弃修改吗？");
            return Task.FromResult(true);
        }
    }
}
