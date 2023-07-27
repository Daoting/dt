#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-07-27 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.MgrDemo.单实体
{
    public partial class ChildViewX
    {
        public static async Task<ChildViewX> New(
            long ParentID = default,
            string ItemName = default,
            string Name = default)
        {
            return new ChildViewX(
                ID: await NewID(),
                ParentID: ParentID,
                ItemName: ItemName,
                Name: Name);
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

            //OnDeleting(() =>
            //{
                
            //    return Task.CompletedTask;
            //});

            //OnDeleted(() =>
            //{
                
            //    return Task.CompletedTask;
            //});

            //OnChanging<string>(nameof(Name), v =>
            //{
                
            //});
        }
    }
}