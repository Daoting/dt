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
using Dt.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dt.App.Model
{
    [View("参数定义")]
    public partial class UserParamsMgr : Win
    {
        public UserParamsMgr()
        {
            InitializeComponent();
            LoadAll();
        }

        async void OnSearch(object sender, string e)
        {
            if (e == "#全部")
            {
                LoadAll();
            }
            else if (e == "#最近修改")
            {
                LoadLast();
            }
            else if (!string.IsNullOrEmpty(e))
            {
                _lv.Data = await AtCm.Query<Params>("参数-模糊查询", new { ID = $"%{e}%" });
            }
            SelectTab("列表");
        }

        async void LoadAll()
        {
            _lv.Data = await AtCm.GetAll<Params>();
        }

        async void LoadLast()
        {
            _lv.Data = await AtCm.Query<Params>("参数-最近修改");
        }

        async void OnItemClick(object sender, ItemClickArgs e)
        {
            _fv.Data = await AtCm.GetByID<Params>(e.Data.To<Params>().ID);
            SelectTab("定义");
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
                LoadLast();
                if (delVer)
                    DeleteDataVer();
            }
        }

        void OnDel(object sender, Mi e)
        {
            var par = _fv.Data.To<Params>();
            if (par != null)
                DelParams(par);
        }

        void OnListDel(object sender, Mi e)
        {
            DelParams(e.Data.To<Params>());
        }

        async void DelParams(Params p_par)
        {
            if (!await AtKit.Confirm("确认要删除吗？"))
            {
                AtKit.Msg("已取消删除！");
                return;
            }

            
            if (p_par.IsAdded)
            {
                _fv.Data = null;
                return;
            }

            int cnt = await AtCm.GetScalar<int>("参数-用户设置数", new { ParamID = p_par.ID });
            if (cnt > 0)
            {
                if (!await AtKit.Confirm("该参数已存在用户设置，确认要删除吗？"))
                    return;
            }

            if (await AtCm.Delete(p_par))
            {
                LoadLast();
                _fv.Data = null;
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
            new UserParamsDlg().Show(e.Data.To<Params>().ID);
        }
    }
}