#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dt.Base;
using Dt.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
#endregion

namespace Dt.App.Model
{
    public sealed partial class EditUserParams : Mv
    {
        public EditUserParams()
        {
            InitializeComponent();
            //_mi.SetBinding(IsEnabledProperty, new Binding { Path = new PropertyPath("IsDirty"), Source = _fv });
            var bind = _mi.GetBindingExpression(IsEnabledProperty);
        }

        public async void LoadData(string p_id)
        {
            if (string.IsNullOrEmpty(p_id))
                OnAdd(null, null);
            else
                _fv.Data = await AtCm.GetByID<Params>(p_id);
        }

        void OnAdd(object sender, Mi e)
        {
            _fv.Data = new Params(ID: "新参数");
        }

        async void OnSave(object sender, Mi e)
        {
            var par = _fv.Data.To<Params>();
            bool delVer = par.IsAdded || par.Cells["ID"].IsChanged || par.Cells["Value"].IsChanged;
            if (await AtCm.Save(par))
            {
                _win.List.Refresh();
                if (delVer)
                    DeleteDataVer();
            }
        }

        async void OnDel(object sender, Mi e)
        {
            var par = _fv.Data.To<Params>();
            if (par == null)
                return;

            if (!await Kit.Confirm("确认要删除吗？"))
            {
                Kit.Msg("已取消删除！");
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
                _win.List.Refresh();
                DeleteDataVer();
            }
        }

        async void DeleteDataVer()
        {
            // 1表任何人，删除所有人的参数版本号
            await AtCm.DeleteDataVer(new List<long> { 1 }, "params");
        }

        void OnUserSetting(object sender, Mi e)
        {
            var par = _fv.Data.To<Params>();
            if (par != null)
                new UserParamsDlg().Show(par.ID);
        }

        UserParamsWin _win => (UserParamsWin)_tab.OwnWin;
    }
}
