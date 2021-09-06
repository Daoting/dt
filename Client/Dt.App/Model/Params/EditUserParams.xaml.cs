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
#endregion

namespace Dt.App.Model
{
    public sealed partial class EditUserParams : Mv
    {
        public EditUserParams(string p_id)
        {
            InitializeComponent();
            LoadData(p_id);
        }

        async void LoadData(string p_id)
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
                //if (delVer)
                //    DeleteDataVer();
            }
        }

        void OnDel(object sender, Mi e)
        {
            var par = _fv.Data.To<Params>();
            //if (par != null)
            //    DelParams(par);
        }

    }
}
