#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-06-14 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Demo.Base
{
    [View1("v_部门")]
    public partial class 部门X
    {
        public static async Task<部门X> New(
            long? 上级id = default,
            string 名称 = default,
            string 说明 = default,
            DateTime? 建档时间 = default,
            DateTime? 撤档时间 = default)
        {
            string code = "";
            string sjbm = "";
            if (上级id != null)
            {
                var sj = await 部门X.GetByID(上级id);
                int cnt = await 部门X.GetCount($"where 上级id={上级id}") + 1;
                code = $"{sj.编码}{cnt:D2}";
                sjbm = sj.名称;
            }
            else
            {
                int cnt = await 部门X.GetCount("where 上级id is null") + 1;
                code = $"{cnt:D2}";
            }

            var x = new 部门X(
                ID: await NewID(),
                上级id: 上级id,
                编码: code,
                名称: 名称,
                说明: 说明,
                建档时间: 建档时间,
                撤档时间: 撤档时间);
            
            x.Add("上级部门", sjbm);
            return x;
        }

        protected override void InitHook()
        {
            OnSaving(() =>
            {
                if (名称 == "")
                    Throw.Msg("名称不可为空！", c名称);

                if (IsAdded)
                {
                    建档时间 = Kit.Now;
                }
                return Task.CompletedTask;
            });
            
            OnDeleting(async () =>
            {
                int cnt = await 部门X.GetCount($"where 上级id={ID}");
                if (cnt > 0)
                    Throw.Msg("存在下级部门，无法删除！");
            });
        }

        #region Sql

        #endregion
    }
}