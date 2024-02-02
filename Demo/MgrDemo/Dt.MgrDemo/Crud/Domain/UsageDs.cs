#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-02-17 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
#endregion

namespace Dt.MgrDemo.Crud
{
    class UsageDs : DomainSvc<UsageDs>
    {
        public static async Task BatchSave()
        {
            var x = await CrudX.New("领域服务");

            var w = At.NewWriter();
            await w.Save(x);

            var tbl = await CrudX.Page(0, 4, null);
            if (tbl.Count > 1)
            {
                tbl.RecordDeleted();
                // 删
                tbl.RemoveAt(0);
                // 更
                tbl[0].Name = "服务更";
            }
            // 增
            tbl.Add(await CrudX.New("服务更"));
            await w.Save(tbl);

            var par = await At.First<ParTblX>("select * from demo_par_tbl");
            par.Name = Kit.NewGuid;
            await w.Save(par);

            bool suc = await w.Commit();

            // 日志源属性有UsageDs，容易识别
            if (suc)
                _log.Debug("领域服务批量保存成功");
            else
                _log.Warning("领域服务保存失败");
        }
    }
}