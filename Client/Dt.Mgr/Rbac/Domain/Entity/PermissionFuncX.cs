#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-10-26 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Mgr.Rbac
{
    public partial class PermissionFuncX
    {
        public static async Task<PermissionFuncX> New(
            long ModuleID = default,
            string Name = default,
            string Note = default)
        {
            return new PermissionFuncX(
                ID: await NewID(),
                ModuleID: ModuleID,
                Name: Name,
                Note: Note);
        }

        protected override void InitHook()
        {
            OnSaving(async () =>
            {
                if (Name == "")
                    Throw.Msg("名称不可为空！");

                if (IsAdded || cName.IsChanged)
                {
                    if (await GetCount($"where name='{Name}' and module_id={ModuleID}") > 0)
                        Throw.Msg("同模块内的功能名称不可重复！", cName);
                }
            });

            //OnSaved(() =>
            //{

            //    return Task.CompletedTask;
            //});

            OnDeleting(async () =>
            {
                Throw.If(ID < 1000, "系统目录无法删除！");
                if (await PermissionX.GetCount("where func_id=" + ID) > 0)
                    Throw.Msg($"功能 [{Name}] 下存在权限，禁止删除！");
            });

            //OnDeleted(() =>
            //{

            //    return Task.CompletedTask;
            //});

            //OnChanging(cName, e =>
            //{

            //});
        }
    }
}