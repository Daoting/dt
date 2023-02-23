#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-02-23 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.MgrDemo.多对多
{
    public partial class 角色X
    {
        public static async Task<角色X> New(
            string 角色名称 = default,
            string 角色描述 = default)
        {
            return new 角色X(
                ID: await NewID(),
                角色名称: 角色名称,
                角色描述: 角色描述);
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