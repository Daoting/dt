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
    public partial class PermissionModuleX
    {
        public static async Task<PermissionModuleX> New(
            string Name = default,
            string Note = default)
        {
            return new PermissionModuleX(
                ID: await NewID(),
                Name: Name,
                Note: Note);
        }

        protected override void InitHook()
        {
            //OnSaving(() =>
            //{

            //    return Task.CompletedTask;
            //});

            //OnSaved(() =>
            //{

            //    return Task.CompletedTask;
            //});

            OnDeleting(async () =>
            {
                Throw.If(ID < 1000, "系统目录无法删除！");
                if (await PermissionFuncX.GetCount("where module_id=" + ID) > 0)
                    Throw.Msg($"模块 [{Name}] 下存在功能，禁止删除！");
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