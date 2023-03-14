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
    public partial class OptionX
    {
        public static async Task<OptionX> New(
            string Name = default,
            int Dispidx = default,
            long GroupID = default)
        {
            return new OptionX(
                ID: await NewID(),
                Name: Name,
                Dispidx: await NewSeq("Dispidx"),
                GroupID: GroupID);
        }

        protected override void InitHook()
        {
            OnSaving(() =>
            {
                // 避免上下移动时判断
                if (IsAdded || Cells["Name"].IsChanged)
                    Throw.IfEmpty(Name, "选项名称不可为空！");
                return Task.CompletedTask;
            });
        }
    }
}