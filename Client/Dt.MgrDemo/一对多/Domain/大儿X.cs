#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-02-22 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.MgrDemo.一对多
{
    public partial class 大儿X
    {
        public static async Task<大儿X> New(
            long ParentID = default,
            string 大儿名 = default)
        {
            return new 大儿X(
                ID: await NewID(),
                ParentID: ParentID,
                大儿名: 大儿名);
        }

        protected override void InitHook()
        {
            //OnSaving(() =>
            //{
                
            //    return Task.CompletedTask;
            //});

            //OnDeleting(() =>
            //{
                
            //    return Task.CompletedTask;
            //});

            //OnChanging<string>(nameof(Name), v =>
            //{
                
            //});
        }
    }
}