#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-01-06 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dt.Mgr.Domain
{
    public partial class RoleX
    {
        public static async Task<RoleX> New()
        {
            long id = await NewID();
            return new RoleX(id, "新角色");
        }

        protected override void InitHook()
        {
            OnSaving(async () =>
            {
                Throw.IfEmpty(Name, "角色名称不可为空！");

                if ((IsAdded || Cells["name"].IsChanged)
                    && await AtCm.GetScalar<int>("角色-名称重复", new { name = Name }) > 0)
                {
                    Throw.Msg("角色名称重复！");
                }
            });
        }
    }
}