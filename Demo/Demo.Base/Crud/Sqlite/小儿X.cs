#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-05-22 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Demo.Base.Sqlite
{
    public partial class 小儿X
    {
        public static async Task<小儿X> New(
            long GroupID = default,
            string 小儿名 = default)
        {
            return new 小儿X(
                ID: await NewID(),
                GroupID: GroupID,
                小儿名: 小儿名);
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

            //OnChanging(cName, e =>
            //{
                
            //});
        }
    }
}