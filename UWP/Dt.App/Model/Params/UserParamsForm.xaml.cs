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
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Dt.App.Model
{
    public sealed partial class UserParamsForm : Mv
    {
        public UserParamsForm()
        {
            InitializeComponent();
            Menu["保存"].Bind(IsEnabledProperty, _fv, "IsDirty");
        }

        public async void Update(string p_id)
        {
            if (!await _fv.DiscardChanges())
                return;

            if (string.IsNullOrEmpty(p_id))
                OnAdd(null, null);
            else
                _fv.Data = await AtCm.GetByID<ParamsObj>(p_id);
        }
        
        void OnAdd(object sender, Mi e)
        {
            _fv.Data = new ParamsObj(ID: "新参数");
        }

        async void OnSave(object sender, Mi e)
        {
            var par = _fv.Data.To<ParamsObj>();
            if (par == null || (!par.IsAdded && !par.IsChanged))
                return;

            bool delVer = par.IsAdded || par.Cells["ID"].IsChanged || par.Cells["Value"].IsChanged;
            if (await AtCm.Save(par))
            {
                _win.List.Update();
                if (delVer)
                    DeleteDataVer();
            }
        }

        async void OnDel(object sender, Mi e)
        {
            var par = _fv.Data.To<ParamsObj>();
            if (par == null)
                return;

            if (!await Kit.Confirm("确认要删除吗？"))
            {
                Kit.Msg("已取消删除！");
                return;
            }

            if (par.IsAdded)
            {
                _fv.Data = null;
                return;
            }

            int cnt = await AtCm.GetScalar<int>("参数-用户设置数", new { ParamID = par.ID });
            if (cnt > 0)
            {
                if (!await Kit.Confirm("该参数已存在用户设置，确认要删除吗？"))
                    return;
            }

            if (await AtCm.Delete(par))
            {
                _fv.Data = null;
                _win.List.Update();
                DeleteDataVer();
            }
        }

        async void DeleteDataVer()
        {
            // 1表任何人，删除所有人的参数版本号
            await AtCm.DeleteDataVer(new List<long> { 1 }, "params");
        }

        protected override Task<bool> OnClosing()
        {
            return _fv.DiscardChanges();
        }

        void OnUserSetting(object sender, Mi e)
        {
            var par = _fv.Data.To<ParamsObj>();
            if (par != null)
                new UserParamsDlg().Show(par.ID);
        }

        UserParamsWin _win => (UserParamsWin)_tab.OwnWin;
    }
}
