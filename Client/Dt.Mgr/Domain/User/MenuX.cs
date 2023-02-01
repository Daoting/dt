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
    public partial class MenuX
    {
        public static async Task<MenuX> New(
            long? ParentID = default,
            string Name = default,
            bool IsGroup = default,
            string ViewName = default,
            string Params = default,
            string Icon = default,
            string Note = default,
            bool IsLocked = false,
            DateTime Ctime = default,
            DateTime Mtime = default)
        {
            return new MenuX(
                ID: await NewID(),
                ParentID: ParentID,
                Name: Name,
                IsGroup: IsGroup,
                ViewName: ViewName,
                Params: Params,
                Icon: Icon,
                Note: Note,
                Dispidx: await NewSeq("Dispidx"),
                IsLocked: IsLocked,
                Ctime: Ctime,
                Mtime: Mtime);
        }

        protected override void InitHook()
        {
            OnSaving(() =>
            {
                // 调序时无name列
                if (Contains("Name"))
                    Throw.IfEmpty(Name, "菜单名称不可为空！");
                return Task.CompletedTask;
            });

            OnDeleting(async () =>
            {
                if (IsGroup)
                {
                    int count = await AtCm.GetScalar<int>("菜单-是否有子菜单", new { parentid = ID });
                    Throw.If(count > 0, "含子菜单无法删除！");
                }
            });
        }
    }
}