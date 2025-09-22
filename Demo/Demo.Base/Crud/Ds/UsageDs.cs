#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-02-17 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Demo.Base
{
    public class UsageDs : DomainSvc<UsageDs>
    {
        public static async Task BatchSave()
        {
            var x = await 基础X.New("领域服务" + Kit.NewGuid.Substring(0, 6));

            var w = await 基础X.NewWriter();
            await w.Save(x);

            var tbl = await 基础X.Page(0, 4, null);
            if (tbl.Count > 1)
            {
                tbl.LockCollection();
                // 删
                tbl.RemoveAt(0);
                // 更
                tbl[0].名称 = "服务更";
            }
            // 增
            tbl.Add(await 基础X.New("服务更"));
            await w.Save(tbl);
            
            bool suc = await w.Commit();

            // 日志源属性有UsageDs，容易识别
            if (suc)
                _log.Debug("领域服务批量保存成功");
            else
                _log.Warning("领域服务保存失败");
        }
    }
}