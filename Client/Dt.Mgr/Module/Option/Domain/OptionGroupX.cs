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
    public partial class OptionGroupX
    {
        public static async Task<OptionGroupX> New(
            string Name = default)
        {
            return new OptionGroupX(
                ID: await NewID(),
                Name: Name);
        }

        protected override void InitHook()
        {
            OnSaving(async () =>
            {
                Throw.IfEmpty(Name, "分组名称不可为空！");

                if ((IsAdded || Cells["name"].IsChanged)
                    && await AtCm.GetScalar<int>("选项-分组名称重复", new { name = Name }) > 0)
                {
                    Throw.Msg("分组名称重复！");
                }
            });

            OnDeleting(async () =>
            {
                int count = await AtCm.GetScalar<int>("选项-子项个数", new { groupid = ID });
                Throw.If(count > 0, "该分组含选项无法删除！");
            });
        }
    }
}