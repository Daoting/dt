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
    public partial class 用户X
    {
        public static async Task<用户X> New(
            string 手机号 = default,
            string 姓名 = default,
            string 密码 = default)
        {
            return new 用户X(
                ID: await NewID(),
                手机号: 手机号,
                姓名: 姓名,
                密码: 密码);
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