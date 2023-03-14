#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-03-14 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Mgr.Module
{
    public partial class ParamsX
    {
        public static async Task<ParamsX> New(
            string Name = default,
            string Value = default,
            string Note = default,
            DateTime Ctime = default,
            DateTime Mtime = default)
        {
            return new ParamsX(
                ID: await NewID(),
                Name: "新参数",
                Value: Value,
                Note: Note,
                Ctime: Ctime,
                Mtime: Mtime);
        }

        protected override void InitHook()
        {
            OnSaving(async () =>
            {
                Throw.IfEmpty(Name, "参数名称不可为空！");

                if ((IsAdded || Cells["Name"].IsChanged)
                    && await AtCm.GetScalar<int>("参数-重复名称", new { name = Name }) > 0)
                {
                    Throw.Msg("参数名称重复！");
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

            OnDeleting(() =>
            {
                Throw.If(ID < 1000, "系统内置参数无法删除！");
                return Task.CompletedTask;
            });
        }
    }
}