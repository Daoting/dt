#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-03-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#endregion

namespace Dt.Mgr.Rbac
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

        public static Task<MenuX> GetWithParentName(long p_id)
        {
            return First($"select a.*,b.name parentname from cm_menu a left join cm_menu b on a.parent_id=b.id where a.id={p_id}");
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
                Throw.If(ID < 1000, "系统菜单无法删除！");
                if (IsGroup)
                {
                    int count = await GetCount($"where parent_id={ID}");
                    Throw.If(count > 0, "含子菜单无法删除！");
                }

                // 清除关联用户的数据版本号，没放在 OnDeleted 处理因为cm_role_menu有级联删除
                var ls = await At.FirstCol<long>($"select id from cm_role a where exists (select role_id from cm_role_menu b where a.id=b.role_id and menu_id={ID})");
                RbacDs.DelRoleDataVer(ls, RbacDs.PrefixMenu);
            });
        }
    }
}