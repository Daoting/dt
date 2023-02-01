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
    public partial class RptX
    {
        public static async Task<RptX> New(
           string Name = default,
           string Define = default,
           string Note = default,
           DateTime Ctime = default,
           DateTime Mtime = default)
        {
            return new RptX(
                ID: await NewID(),
                Name: Name,
                Define: Define,
                Note: Note,
                Ctime: Ctime,
                Mtime: Mtime);
        }

        protected override void InitHook()
        {
            OnSaving(async () =>
            {
                Throw.IfEmpty(Name, "报表名称不可为空！");

                if ((IsAdded || Cells["name"].IsChanged)
                    && await AtCm.GetScalar<int>("报表-重复名称", new { name = Name }) > 0)
                {
                    Throw.Msg("报表名称重复！");
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