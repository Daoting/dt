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
    public partial class ParamsX
    {
        protected override void InitHook()
        {
            OnSaving(async () =>
            {
                Throw.IfEmpty(ID, "参数名不可为空！");

                if ((IsAdded || Cells["ID"].IsChanged)
                    && await AtCm.GetScalar<int>("参数-重复名称", new { id = ID }) > 0)
                {
                    Throw.Msg("参数名重复！");
                }

                if (IsAdded)
                {
                    Ctime = Mtime = Kit.Now;
                }
                else
                {
                    Mtime = Kit.Now;
                }
            });
        }
    }
}