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
    public partial class UserParamsList : Nav
    {
        public UserParamsList()
        {
            InitializeComponent();
            LoadAll();
        }

        public async void OnSearch(string e)
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
            NaviToSelf();
        }

        async void LoadAll()
        {
            _lv.Data = await AtCm.GetAll<Params>();
        }

        async void LoadLast()
        {
            _lv.Data = await AtCm.Query<Params>("参数-最近修改");
        }

        void OnNaviToSearch(object sender, Mi e)
        {
            NaviTo("查找");
        }

        void OnAdd(object sender, Mi e)
        {
            Forward(new EditUserParams(null));
        }

        void OnEdit(object sender, Mi e)
        {
            Forward(new EditUserParams(e.Data.To<Params>().ID));
        }

        void OnUserSetting(object sender, Mi e)
        {
            new UserParamsDlg().Show(e.Data.To<Params>().ID);
        }

        async void OnListDel(object sender, Mi e)
        {
            if (!await Kit.Confirm("确认要删除吗？"))
            {
                Kit.Msg("已取消删除！");
                return;
            }

            Params p_par = e.Data.To<Params>();
            int cnt = await AtCm.GetScalar<int>("参数-用户设置数", new { ParamID = p_par.ID });
            if (cnt > 0)
            {
                if (!await Kit.Confirm("该参数已存在用户设置，确认要删除吗？"))
                    return;
            }

            if (await AtCm.Delete(p_par))
            {
                LoadLast();

                // 1表任何人，删除所有人的参数版本号
                await AtCm.DeleteDataVer(new List<long> { 1 }, "params");
            }
        }
    }
}