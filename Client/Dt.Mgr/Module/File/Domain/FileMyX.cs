#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-03-10 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Mgr.Module
{
    public partial class FileMyX
    {
        public static async Task<FileMyX> New(
            long? ParentID = default,
            string Name = default,
            bool IsFolder = default,
            string ExtName = default,
            string Info = default,
            DateTime Ctime = default,
            long UserID = default)
        {
            return new FileMyX(
                ID: await NewID(),
                ParentID: ParentID,
                Name: Name,
                IsFolder: IsFolder,
                ExtName: ExtName,
                Info: Info,
                Ctime: Ctime,
                UserID: UserID);
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